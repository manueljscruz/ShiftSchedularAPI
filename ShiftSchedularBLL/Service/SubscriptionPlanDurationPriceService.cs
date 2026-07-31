using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Service
{
    /// <summary>
    /// Service implementation for SubscriptionPlanDurationPrice admin management.
    /// </summary>
    public class SubscriptionPlanDurationPriceService : ISubscriptionPlanDurationPriceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionPlanDurationPriceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AdminSubscriptionPlanDurationPriceItemDTO>> GetAllAsync()
        {
            var items = await _unitOfWork.SubscriptionPlanDurationPriceRepository.GetAllWithDetails();
            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminSubscriptionPlanDurationPriceItemDTO> GetByIdAsync(Guid id)
        {
            var item = await _unitOfWork.SubscriptionPlanDurationPriceRepository.GetByIdWithDetails(id);
            return item == null ? null : MapToDTO(item);
        }

        public async Task<BaseResponse<Guid>> UpsertAsync(AdminUpsertSubscriptionPlanDurationPriceDTO dto)
        {
            var response = new BaseResponse<Guid>();

            if (dto == null || dto.SubscriptionPlanTypeId == 0 || dto.SubscriptionDurationTypeId == 0)
            {
                response.Message = "SubscriptionPlanTypeId and SubscriptionDurationTypeId are required.";
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                SubscriptionPlanDurationPrice item;

                if (dto.Id == Guid.Empty)
                {
                    item = new SubscriptionPlanDurationPrice
                    {
                        SubscriptionPlanDurationPriceId = Guid.NewGuid(),
                        SubscriptionPlanTypeId = dto.SubscriptionPlanTypeId,
                        SubscriptionDurationTypeId = dto.SubscriptionDurationTypeId,
                        BasePrice = dto.BasePrice,
                        ToScale = dto.ToScale,
                        ScaleRequirement = dto.ScaleRequirement,
                        PricePerExtraMember = dto.PricePerExtraMember,
                        IncludedGenerations = dto.IncludedGenerations,
                        PricePerExtraGeneration = dto.PricePerExtraGeneration,
                        IsPublicPlan = dto.IsPublicPlan,
                        IsActive = dto.IsActive
                    };
                    item = await _unitOfWork.SubscriptionPlanDurationPriceRepository.Add(item);
                }
                else
                {
                    item = await _unitOfWork.SubscriptionPlanDurationPriceRepository.GetById(dto.Id);
                    if (item == null)
                    {
                        response.NotFound = true;
                        response.Message = "SubscriptionPlanDurationPrice not found.";
                        return response;
                    }

                    item.SubscriptionPlanTypeId = dto.SubscriptionPlanTypeId;
                    item.SubscriptionDurationTypeId = dto.SubscriptionDurationTypeId;
                    item.BasePrice = dto.BasePrice;
                    item.ToScale = dto.ToScale;
                    item.ScaleRequirement = dto.ScaleRequirement;
                    item.PricePerExtraMember = dto.PricePerExtraMember;
                    item.IncludedGenerations = dto.IncludedGenerations;
                    item.PricePerExtraGeneration = dto.PricePerExtraGeneration;
                    item.IsPublicPlan = dto.IsPublicPlan;
                    item.IsActive = dto.IsActive;
                    await _unitOfWork.SubscriptionPlanDurationPriceRepository.Update(item);
                }

                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Result = item.SubscriptionPlanDurationPriceId;
                response.Message = "SubscriptionPlanDurationPrice saved successfully.";
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = "Failed to save SubscriptionPlanDurationPrice.";
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            var response = new BaseResponse<bool>();

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                await _unitOfWork.SubscriptionPlanDurationPriceRepository.Delete(id);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Result = true;
                response.Message = "SubscriptionPlanDurationPrice deleted successfully.";
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = "Failed to delete SubscriptionPlanDurationPrice.";
            }

            return response;
        }

        private static AdminSubscriptionPlanDurationPriceItemDTO MapToDTO(SubscriptionPlanDurationPrice spdp) => new()
        {
            Id = spdp.SubscriptionPlanDurationPriceId,
            SubscriptionPlanTypeId = spdp.SubscriptionPlanTypeId,
            SubscriptionPlanTypeName = spdp.SubscriptionPlanType?.SubscriptionPlanTypeName ?? string.Empty,
            SubscriptionDurationTypeId = spdp.SubscriptionDurationTypeId,
            SubscriptionDurationTypeName = spdp.SubscriptionDurationType?.SubscriptionDurationTypeName ?? string.Empty,
            BasePrice = spdp.BasePrice,
            ToScale = spdp.ToScale,
            ScaleRequirement = spdp.ScaleRequirement,
            PricePerExtraMember = spdp.PricePerExtraMember,
            IncludedGenerations = spdp.IncludedGenerations,
            PricePerExtraGeneration = spdp.PricePerExtraGeneration,
            IsPublicPlan = spdp.IsPublicPlan,
            IsActive = spdp.IsActive
        };
    }
}
