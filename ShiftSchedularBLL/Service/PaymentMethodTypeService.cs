using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Service
{
    /// <summary>
    /// Service implementation for PaymentMethodType admin management.
    /// </summary>
    public class PaymentMethodTypeService : IPaymentMethodTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentMethodTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AdminPaymentMethodTypeItemDTO>> GetAllAsync()
        {
            var items = await _unitOfWork.PaymentMethodTypeRepository.GetAllWithDetails();
            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminPaymentMethodTypeItemDTO> GetByIdAsync(int id)
        {
            var item = await _unitOfWork.PaymentMethodTypeRepository.GetByIdWithDetails(id);
            return item == null ? null : MapToDTO(item);
        }

        public async Task<BaseResponse<int>> UpsertAsync(AdminUpsertPaymentMethodTypeDTO dto)
        {
            var response = new BaseResponse<int>();

            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            {
                response.Message = "Name is required.";
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                PaymentMethodType item;

                if (dto.Id == 0)
                {
                    item = new PaymentMethodType
                    {
                        PaymentMethodTypeName = dto.Name,
                        IsActive = dto.IsActive
                    };
                    item = await _unitOfWork.PaymentMethodTypeRepository.Add(item);
                }
                else
                {
                    item = await _unitOfWork.PaymentMethodTypeRepository.GetById(dto.Id);
                    if (item == null)
                    {
                        response.NotFound = true;
                        response.Message = "PaymentMethodType not found.";
                        return response;
                    }

                    item.PaymentMethodTypeName = dto.Name;
                    item.IsActive = dto.IsActive;
                    await _unitOfWork.PaymentMethodTypeRepository.Update(item);
                }

                await UpsertLocalizations(item.PaymentMethodTypeId, dto.Localizations);
                await SyncCountries(item.PaymentMethodTypeId, dto.CountryCodes);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Result = item.PaymentMethodTypeId;
                response.Message = "PaymentMethodType saved successfully.";
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = "Failed to save PaymentMethodType.";
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteAsync(int id)
        {
            var response = new BaseResponse<bool>();

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var localizations = await _unitOfWork.PaymentMethodTypeLocalizationRepository.GetByParentId(id);
                await _unitOfWork.PaymentMethodTypeLocalizationRepository.DeleteRange(localizations);

                var countries = await _unitOfWork.PaymentMethodTypeCountryRepository.GetByParentId(id);
                await _unitOfWork.PaymentMethodTypeCountryRepository.DeleteRange(countries);

                await _unitOfWork.PaymentMethodTypeRepository.Delete(id);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Result = true;
                response.Message = "PaymentMethodType deleted successfully.";
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = "Failed to delete PaymentMethodType.";
            }

            return response;
        }

        private async Task UpsertLocalizations(int paymentMethodTypeId, List<AdminUpsertPaymentMethodTypeLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _unitOfWork.PaymentMethodTypeLocalizationRepository.GetByParentAndLocalizationId(paymentMethodTypeId, loc.LocalizationId);

                if (existing == null)
                {
                    await _unitOfWork.PaymentMethodTypeLocalizationRepository.Add(new PaymentMethodTypeLocalization
                    {
                        PaymentMethodTypeId = paymentMethodTypeId,
                        LocalizationId = loc.LocalizationId,
                        PaymentMethodTypeDisplayValue = loc.DisplayValue
                    });
                }
                else
                {
                    existing.PaymentMethodTypeDisplayValue = loc.DisplayValue;
                    await _unitOfWork.PaymentMethodTypeLocalizationRepository.Update(existing);
                }
            }
        }

        private async Task SyncCountries(int paymentMethodTypeId, List<string> countryCodes)
        {
            var existing = (await _unitOfWork.PaymentMethodTypeCountryRepository.GetByParentId(paymentMethodTypeId)).ToList();

            var toRemove = existing.Where(c => !countryCodes.Contains(c.CountryCode)).ToList();
            await _unitOfWork.PaymentMethodTypeCountryRepository.DeleteRange(toRemove);

            var existingCodes = existing.Select(c => c.CountryCode).ToHashSet();
            foreach (var code in countryCodes.Where(code => !existingCodes.Contains(code)))
            {
                await _unitOfWork.PaymentMethodTypeCountryRepository.Add(new PaymentMethodTypeCountry
                {
                    PaymentMethodTypeId = paymentMethodTypeId,
                    CountryCode = code
                });
            }
        }

        private static AdminPaymentMethodTypeItemDTO MapToDTO(PaymentMethodType pmt) => new()
        {
            Id = pmt.PaymentMethodTypeId,
            Name = pmt.PaymentMethodTypeName,
            IsActive = pmt.IsActive,
            CountryCodes = pmt.PaymentMethodTypeCountries?.Select(c => c.CountryCode).ToList() ?? new(),
            Localizations = pmt.PaymentMethodTypeLocalizations?.Select(l => new AdminPaymentMethodTypeLocalizationValueDTO
            {
                LocalizationId = l.LocalizationId,
                LocalizationCode = l.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = l.PaymentMethodTypeDisplayValue
            }).ToList() ?? new()
        };
    }
}
