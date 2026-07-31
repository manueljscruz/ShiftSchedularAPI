using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Service
{
    /// <summary>
    /// Service implementation for SubscriptionPlanType admin management.
    /// </summary>
    public class SubscriptionPlanTypeService : ISubscriptionPlanTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionPlanTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AdminSubscriptionPlanTypeItemDTO>> GetAllAsync()
        {
            var items = await _unitOfWork.SubscriptionPlanTypeRepository.GetAllWithLocalizations();
            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminSubscriptionPlanTypeItemDTO> GetByIdAsync(int id)
        {
            var item = await _unitOfWork.SubscriptionPlanTypeRepository.GetByIdWithLocalizations(id);
            return item == null ? null : MapToDTO(item);
        }

        public async Task<BaseResponse<int>> UpsertAsync(AdminUpsertSubscriptionPlanTypeDTO dto)
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

                SubscriptionPlanType item;

                if (dto.Id == 0)
                {
                    item = new SubscriptionPlanType
                    {
                        SubscriptionPlanTypeName = dto.Name,
                        SubscriptionPlanTypeDescription = dto.Description
                    };
                    item = await _unitOfWork.SubscriptionPlanTypeRepository.Add(item);
                }
                else
                {
                    item = await _unitOfWork.SubscriptionPlanTypeRepository.GetById(dto.Id);
                    if (item == null)
                    {
                        response.NotFound = true;
                        response.Message = "SubscriptionPlanType not found.";
                        return response;
                    }

                    item.SubscriptionPlanTypeName = dto.Name;
                    item.SubscriptionPlanTypeDescription = dto.Description;
                    await _unitOfWork.SubscriptionPlanTypeRepository.Update(item);
                }

                await UpsertLocalizations(item.SubscriptionPlanTypeId, dto.Localizations);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Result = item.SubscriptionPlanTypeId;
                response.Message = "SubscriptionPlanType saved successfully.";
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = "Failed to save SubscriptionPlanType.";
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteAsync(int id)
        {
            var response = new BaseResponse<bool>();

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var localizations = await _unitOfWork.SubscriptionPlanTypeLocalizationRepository.GetByParentId(id);
                await _unitOfWork.SubscriptionPlanTypeLocalizationRepository.DeleteRange(localizations);

                await _unitOfWork.SubscriptionPlanTypeRepository.Delete(id);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Result = true;
                response.Message = "SubscriptionPlanType deleted successfully.";
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = "Failed to delete SubscriptionPlanType.";
            }

            return response;
        }

        private async Task UpsertLocalizations(int subscriptionPlanTypeId, List<AdminUpsertSubscriptionPlanTypeLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _unitOfWork.SubscriptionPlanTypeLocalizationRepository.GetByParentAndLocalizationId(subscriptionPlanTypeId, loc.LocalizationId);

                if (existing == null)
                {
                    await _unitOfWork.SubscriptionPlanTypeLocalizationRepository.Add(new SubscriptionPlanTypeLocalization
                    {
                        SubscriptionPlanTypeId = subscriptionPlanTypeId,
                        LocalizationId = loc.LocalizationId,
                        SubscriptionPlanTypeNameDisplayValue = loc.NameDisplayValue,
                        SubscriptionPlanTypeDescriptionDisplayValue = loc.DescriptionDisplayValue
                    });
                }
                else
                {
                    existing.SubscriptionPlanTypeNameDisplayValue = loc.NameDisplayValue;
                    existing.SubscriptionPlanTypeDescriptionDisplayValue = loc.DescriptionDisplayValue;
                    await _unitOfWork.SubscriptionPlanTypeLocalizationRepository.Update(existing);
                }
            }
        }

        private static AdminSubscriptionPlanTypeItemDTO MapToDTO(SubscriptionPlanType spt) => new()
        {
            Id = spt.SubscriptionPlanTypeId,
            Name = spt.SubscriptionPlanTypeName,
            Description = spt.SubscriptionPlanTypeDescription,
            Localizations = spt.SubscriptionPlanTypeLocalizations?.Select(l => new AdminSubscriptionPlanTypeLocalizationValueDTO
            {
                LocalizationId = l.LocalizationId,
                LocalizationCode = l.Localization?.LocalizationCode ?? string.Empty,
                NameDisplayValue = l.SubscriptionPlanTypeNameDisplayValue,
                DescriptionDisplayValue = l.SubscriptionPlanTypeDescriptionDisplayValue
            }).ToList() ?? new()
        };
    }
}
