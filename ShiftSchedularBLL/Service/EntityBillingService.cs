using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Billing;

namespace ShiftSchedularBLL.Service
{
    public class EntityBillingService : IEntityBillingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EntityBillingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Get Billing Summary

        public async Task<BaseResponse<BillingSummaryDTO>> GetBillingSummaryAsync(Guid entityId, string requesterId)
        {
            BaseResponse<BillingSummaryDTO> response = new BaseResponse<BillingSummaryDTO>();

            if (entityId == Guid.Empty || string.IsNullOrEmpty(requesterId))
            {
                response.Message = "Invalid request.";
                return response;
            }

            bool allowed = await IsAllowedAsync(entityId, requesterId);
            if (!allowed)
            {
                response.Forbidden = true;
                response.Message = "You do not have permission to view billing information for this entity.";
                return response;
            }

            EntitySubscriptionPlan activePlan = await _unitOfWork.EntitySubscriptionPlanRepository.GetActiveByEntityId(entityId);
            if (activePlan == null)
            {
                response.NotFound = true;
                response.Message = "No active subscription plan found for this entity.";
                return response;
            }

            IEnumerable<EntityWorker> members = await _unitOfWork.EntityWorkerRepository.GetByEntityId(entityId);
            int membersUsed = members?.Count() ?? 0;

            IEnumerable<ScheduleGeneration> allGenerations = await _unitOfWork.GetGenericRepository<ScheduleGeneration>().GetAll();
            int generationsUsed = allGenerations
                .Count(g => g.EntitySubscriptionPlanId == activePlan.EntitySubscriptionPlanId && g.Status == "Completed");

            SubscriptionPlanDurationPrice planPrice = activePlan.SubscriptionPlanDurationPrice;

            BillingSummaryDTO summary = new BillingSummaryDTO
            {
                CurrentPlan = MapToCurrentPlanDTO(activePlan),
                Usage = new BillingUsageDTO
                {
                    MembersUsed = membersUsed,
                    ToScale = planPrice.ToScale,
                    MembersIncluded = planPrice.ToScale ? planPrice.ScaleRequirement : (int?)null,
                    GenerationsUsed = generationsUsed,
                    GenerationsIncluded = planPrice.IncludedGenerations
                },
                EstimatedAmount = CalculateEstimatedAmount(planPrice, membersUsed, generationsUsed)
            };

            IEnumerable<PaymentMethod> paymentMethods = await _unitOfWork.PaymentMethodRepository.GetByEntityId(entityId);
            summary.PaymentMethods = paymentMethods
                .Where(pm => pm.IsActive)
                .Select(pm => new PaymentMethodSummaryDTO
                {
                    PaymentMethodId = pm.PaymentMethodId,
                    PaymentMethodTypeId = pm.PaymentMethodTypeId,
                    PaymentMethodTypeName = pm.PaymentMethodType?.PaymentMethodTypeName,
                    CardBrand = pm.CardBrand,
                    LastFourDigits = pm.LastFourDigits,
                    IsDefault = pm.IsDefault
                })
                .ToList();

            response.Result = summary;
            response.Success = true;
            return response;
        }

        #endregion

        #region Get Subscription History

        public async Task<BaseResponse<List<SubscriptionHistoryItemDTO>>> GetSubscriptionHistoryAsync(Guid entityId, string requesterId)
        {
            BaseResponse<List<SubscriptionHistoryItemDTO>> response = new BaseResponse<List<SubscriptionHistoryItemDTO>>();

            if (entityId == Guid.Empty || string.IsNullOrEmpty(requesterId))
            {
                response.Message = "Invalid request.";
                return response;
            }

            bool allowed = await IsAllowedAsync(entityId, requesterId);
            if (!allowed)
            {
                response.Forbidden = true;
                response.Message = "You do not have permission to view billing information for this entity.";
                return response;
            }

            IEnumerable<EntitySubscriptionPlan> history = await _unitOfWork.EntitySubscriptionPlanRepository.GetHistoryByEntityId(entityId);

            response.Result = history.Select(p => new SubscriptionHistoryItemDTO
            {
                EntitySubscriptionPlanId = p.EntitySubscriptionPlanId,
                SubscriptionPlanTypeName = p.SubscriptionPlanDurationPrice?.SubscriptionPlanType?.SubscriptionPlanTypeName,
                SubscriptionDurationTypeName = p.SubscriptionPlanDurationPrice?.SubscriptionDurationType?.SubscriptionDurationTypeName,
                BasePrice = p.SubscriptionPlanDurationPrice?.BasePrice ?? 0,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status
            }).ToList();
            response.Success = true;
            return response;
        }

        #endregion

        #region Get Available Plans

        public async Task<BaseResponse<List<AvailablePlanDTO>>> GetAvailablePlansAsync()
        {
            BaseResponse<List<AvailablePlanDTO>> response = new BaseResponse<List<AvailablePlanDTO>>();

            IEnumerable<SubscriptionPlanDurationPrice> plans = await _unitOfWork.SubscriptionPlanDurationPriceRepository.GetAllWithDetails();

            response.Result = plans
                .Where(p => p.IsPublicPlan && p.IsActive)
                .Select(p => new AvailablePlanDTO
                {
                    SubscriptionPlanDurationPriceId = p.SubscriptionPlanDurationPriceId,
                    SubscriptionPlanTypeId = p.SubscriptionPlanTypeId,
                    SubscriptionPlanTypeName = p.SubscriptionPlanType?.SubscriptionPlanTypeName,
                    SubscriptionPlanTypeDescription = p.SubscriptionPlanType?.SubscriptionPlanTypeDescription,
                    SubscriptionDurationTypeId = p.SubscriptionDurationTypeId,
                    SubscriptionDurationTypeName = p.SubscriptionDurationType?.SubscriptionDurationTypeName,
                    DurationInDays = p.SubscriptionDurationType?.DurationInDays ?? 0,
                    BasePrice = p.BasePrice,
                    ToScale = p.ToScale,
                    ScaleRequirement = p.ScaleRequirement,
                    PricePerExtraMember = p.PricePerExtraMember,
                    IncludedGenerations = p.IncludedGenerations,
                    PricePerExtraGeneration = p.PricePerExtraGeneration
                })
                .ToList();
            response.Success = true;
            return response;
        }

        #endregion

        #region Helpers

        private async Task<bool> IsAllowedAsync(Guid entityId, string requesterId)
        {
            bool isOwner = await _unitOfWork.EntityWorkerRepository.IsMemberOwner(entityId, requesterId);
            if (isOwner)
                return true;

            return await _unitOfWork.EntityPermissionRepository.CanUserEditEntity(entityId, requesterId);
        }

        private static CurrentSubscriptionPlanDTO MapToCurrentPlanDTO(EntitySubscriptionPlan plan)
        {
            SubscriptionPlanDurationPrice planPrice = plan.SubscriptionPlanDurationPrice;

            return new CurrentSubscriptionPlanDTO
            {
                EntitySubscriptionPlanId = plan.EntitySubscriptionPlanId,
                SubscriptionPlanTypeId = planPrice.SubscriptionPlanTypeId,
                SubscriptionPlanTypeName = planPrice.SubscriptionPlanType?.SubscriptionPlanTypeName,
                SubscriptionDurationTypeId = planPrice.SubscriptionDurationTypeId,
                SubscriptionDurationTypeName = planPrice.SubscriptionDurationType?.SubscriptionDurationTypeName,
                BasePrice = planPrice.BasePrice,
                StartDate = plan.StartDate,
                EndDate = plan.EndDate,
                Status = plan.Status
            };
        }

        private static decimal CalculateEstimatedAmount(SubscriptionPlanDurationPrice planPrice, int membersUsed, int generationsUsed)
        {
            decimal amount = planPrice.BasePrice;

            if (planPrice.ToScale && membersUsed > planPrice.ScaleRequirement)
            {
                amount += (membersUsed - planPrice.ScaleRequirement) * planPrice.PricePerExtraMember;
            }

            if (generationsUsed > planPrice.IncludedGenerations)
            {
                amount += (generationsUsed - planPrice.IncludedGenerations) * planPrice.PricePerExtraGeneration;
            }

            return amount;
        }

        #endregion
    }
}
