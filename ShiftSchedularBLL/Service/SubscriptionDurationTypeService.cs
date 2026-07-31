using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Service
{
    /// <summary>
    /// Service implementation for SubscriptionDurationType admin management.
    /// </summary>
    public class SubscriptionDurationTypeService : ISubscriptionDurationTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionDurationTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AdminSubscriptionDurationTypeItemDTO>> GetAllAsync()
        {
            var items = await _unitOfWork.SubscriptionDurationTypeRepository.GetAllWithLocalizations();
            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminSubscriptionDurationTypeItemDTO> GetByIdAsync(int id)
        {
            var item = await _unitOfWork.SubscriptionDurationTypeRepository.GetByIdWithLocalizations(id);
            return item == null ? null : MapToDTO(item);
        }

        public async Task<BaseResponse<int>> UpsertAsync(AdminUpsertSubscriptionDurationTypeDTO dto)
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

                SubscriptionDurationType item;

                if (dto.Id == 0)
                {
                    item = new SubscriptionDurationType
                    {
                        SubscriptionDurationTypeName = dto.Name,
                        DurationInDays = dto.DurationInDays,
                        AppliesPromo = dto.AppliesPromo,
                        SubscriptionDurationTypePromoPercentage = dto.PromoPercentage
                    };
                    item = await _unitOfWork.SubscriptionDurationTypeRepository.Add(item);
                }
                else
                {
                    item = await _unitOfWork.SubscriptionDurationTypeRepository.GetById(dto.Id);
                    if (item == null)
                    {
                        response.NotFound = true;
                        response.Message = "SubscriptionDurationType not found.";
                        return response;
                    }

                    item.SubscriptionDurationTypeName = dto.Name;
                    item.DurationInDays = dto.DurationInDays;
                    item.AppliesPromo = dto.AppliesPromo;
                    item.SubscriptionDurationTypePromoPercentage = dto.PromoPercentage;
                    await _unitOfWork.SubscriptionDurationTypeRepository.Update(item);
                }

                await UpsertLocalizations(item.SubscriptionDurationTypeId, dto.Localizations);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Result = item.SubscriptionDurationTypeId;
                response.Message = "SubscriptionDurationType saved successfully.";
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = "Failed to save SubscriptionDurationType.";
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteAsync(int id)
        {
            var response = new BaseResponse<bool>();

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var localizations = await _unitOfWork.SubscriptionDurationTypeLocalizationRepository.GetByParentId(id);
                await _unitOfWork.SubscriptionDurationTypeLocalizationRepository.DeleteRange(localizations);

                await _unitOfWork.SubscriptionDurationTypeRepository.Delete(id);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Result = true;
                response.Message = "SubscriptionDurationType deleted successfully.";
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = "Failed to delete SubscriptionDurationType.";
            }

            return response;
        }

        private async Task UpsertLocalizations(int subscriptionDurationTypeId, List<AdminUpsertSubscriptionDurationTypeLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _unitOfWork.SubscriptionDurationTypeLocalizationRepository.GetByParentAndLocalizationId(subscriptionDurationTypeId, loc.LocalizationId);

                if (existing == null)
                {
                    await _unitOfWork.SubscriptionDurationTypeLocalizationRepository.Add(new SubscriptionDurationTypeLocalization
                    {
                        SubscriptionDurationTypeId = subscriptionDurationTypeId,
                        LocalizationId = loc.LocalizationId,
                        SubscriptionDurationTypeDisplayValue = loc.DisplayValue
                    });
                }
                else
                {
                    existing.SubscriptionDurationTypeDisplayValue = loc.DisplayValue;
                    await _unitOfWork.SubscriptionDurationTypeLocalizationRepository.Update(existing);
                }
            }
        }

        private static AdminSubscriptionDurationTypeItemDTO MapToDTO(SubscriptionDurationType sdt) => new()
        {
            Id = sdt.SubscriptionDurationTypeId,
            Name = sdt.SubscriptionDurationTypeName,
            DurationInDays = sdt.DurationInDays,
            AppliesPromo = sdt.AppliesPromo,
            PromoPercentage = sdt.SubscriptionDurationTypePromoPercentage,
            Localizations = sdt.SubscriptionDurationTypeLocalizations?.Select(l => new AdminSubscriptionDurationTypeLocalizationValueDTO
            {
                LocalizationId = l.LocalizationId,
                LocalizationCode = l.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = l.SubscriptionDurationTypeDisplayValue
            }).ToList() ?? new()
        };
    }
}
