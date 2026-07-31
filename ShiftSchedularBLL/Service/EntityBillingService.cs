using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Billing;
using Stripe;
using PaymentMethod = ShiftSchedularEntity.Entities.PaymentMethod;

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
                .Select(MapToPaymentMethodSummaryDTO)
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

        #region Create Setup Intent

        public async Task<BaseResponse<SetupIntentDTO>> CreateSetupIntentAsync(Guid entityId, string requesterId)
        {
            BaseResponse<SetupIntentDTO> response = new BaseResponse<SetupIntentDTO>();

            if (entityId == Guid.Empty || string.IsNullOrEmpty(requesterId))
            {
                response.Message = "Invalid request.";
                return response;
            }

            bool allowed = await IsAllowedAsync(entityId, requesterId);
            if (!allowed)
            {
                response.Forbidden = true;
                response.Message = "You do not have permission to manage payment methods for this entity.";
                return response;
            }

            try
            {
                string gatewayCustomerId = await GetOrCreateGatewayCustomerIdAsync(entityId);

                SetupIntentService setupIntentService = new SetupIntentService();
                SetupIntent setupIntent = await setupIntentService.CreateAsync(new SetupIntentCreateOptions
                {
                    Customer = gatewayCustomerId,
                    PaymentMethodTypes = new List<string> { "card" }
                });

                response.Result = new SetupIntentDTO { ClientSecret = setupIntent.ClientSecret };
                response.Success = true;
            }
            catch (StripeException ex)
            {
                response.Message = $"Failed to create setup intent: {ex.Message}";
            }

            return response;
        }

        #endregion

        #region Confirm Payment Method

        public async Task<BaseResponse<PaymentMethodSummaryDTO>> ConfirmPaymentMethodAsync(Guid entityId, string requesterId, ConfirmPaymentMethodDTO dto)
        {
            BaseResponse<PaymentMethodSummaryDTO> response = new BaseResponse<PaymentMethodSummaryDTO>();

            if (entityId == Guid.Empty || string.IsNullOrEmpty(requesterId) || dto == null || string.IsNullOrWhiteSpace(dto.StripePaymentMethodId))
            {
                response.Message = "Invalid request.";
                return response;
            }

            bool allowed = await IsAllowedAsync(entityId, requesterId);
            if (!allowed)
            {
                response.Forbidden = true;
                response.Message = "You do not have permission to manage payment methods for this entity.";
                return response;
            }

            IEnumerable<PaymentMethodType> paymentMethodTypes = await _unitOfWork.PaymentMethodTypeRepository.GetAllWithDetails();
            PaymentMethodType cardType = paymentMethodTypes.FirstOrDefault(t => string.Equals(t.PaymentMethodTypeName, "Card", StringComparison.OrdinalIgnoreCase));
            if (cardType == null)
            {
                response.Message = "No 'Card' payment method type configured in the catalog.";
                return response;
            }

            try
            {
                PaymentMethodService stripePaymentMethodService = new PaymentMethodService();
                Stripe.PaymentMethod stripePaymentMethod = await stripePaymentMethodService.GetAsync(dto.StripePaymentMethodId);

                if (stripePaymentMethod?.Card == null)
                {
                    response.Message = "The provided payment method is not a card.";
                    return response;
                }

                IEnumerable<PaymentMethod> existingMethods = await _unitOfWork.PaymentMethodRepository.GetByEntityId(entityId);
                bool isFirstPaymentMethod = !existingMethods.Any(pm => pm.IsActive);

                PaymentMethod paymentMethod = new PaymentMethod
                {
                    PaymentMethodId = Guid.NewGuid(),
                    EntityId = entityId,
                    GatewayCustomerId = stripePaymentMethod.CustomerId,
                    PaymentMethodTypeId = cardType.PaymentMethodTypeId,
                    LastFourDigits = stripePaymentMethod.Card.Last4,
                    CardBrand = stripePaymentMethod.Card.Brand,
                    ExpiryMonth = (int)stripePaymentMethod.Card.ExpMonth,
                    ExpiryYear = (int)stripePaymentMethod.Card.ExpYear,
                    Token = stripePaymentMethod.Id,
                    IsDefault = isFirstPaymentMethod,
                    IsActive = true
                };

                await _unitOfWork.GetGenericRepository<PaymentMethod>().Add(paymentMethod);

                response.Result = MapToPaymentMethodSummaryDTO(paymentMethod);
                response.Result.PaymentMethodTypeName = cardType.PaymentMethodTypeName;
                response.Success = true;
            }
            catch (StripeException ex)
            {
                response.Message = $"Failed to confirm payment method: {ex.Message}";
            }

            return response;
        }

        #endregion

        #region Set Default Payment Method

        public async Task<BaseResponse<bool>> SetDefaultPaymentMethodAsync(Guid entityId, string requesterId, Guid paymentMethodId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (entityId == Guid.Empty || string.IsNullOrEmpty(requesterId) || paymentMethodId == Guid.Empty)
            {
                response.Message = "Invalid request.";
                return response;
            }

            bool allowed = await IsAllowedAsync(entityId, requesterId);
            if (!allowed)
            {
                response.Forbidden = true;
                response.Message = "You do not have permission to manage payment methods for this entity.";
                return response;
            }

            IEnumerable<PaymentMethod> paymentMethods = await _unitOfWork.PaymentMethodRepository.GetByEntityId(entityId);
            List<PaymentMethod> activeMethods = paymentMethods.Where(pm => pm.IsActive).ToList();
            PaymentMethod target = activeMethods.FirstOrDefault(pm => pm.PaymentMethodId == paymentMethodId);

            if (target == null)
            {
                response.NotFound = true;
                response.Message = "Payment method not found for this entity.";
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                foreach (PaymentMethod method in activeMethods)
                {
                    bool shouldBeDefault = method.PaymentMethodId == paymentMethodId;
                    if (method.IsDefault != shouldBeDefault)
                    {
                        method.IsDefault = shouldBeDefault;
                        await _unitOfWork.GetGenericRepository<PaymentMethod>().Update(method);
                    }
                }

                await _unitOfWork.CommitAsync();
                response.Result = true;
                response.Success = true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = "Failed to set default payment method.";
            }

            return response;
        }

        #endregion

        #region Remove Payment Method

        public async Task<BaseResponse<bool>> RemovePaymentMethodAsync(Guid entityId, string requesterId, Guid paymentMethodId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (entityId == Guid.Empty || string.IsNullOrEmpty(requesterId) || paymentMethodId == Guid.Empty)
            {
                response.Message = "Invalid request.";
                return response;
            }

            bool allowed = await IsAllowedAsync(entityId, requesterId);
            if (!allowed)
            {
                response.Forbidden = true;
                response.Message = "You do not have permission to manage payment methods for this entity.";
                return response;
            }

            IEnumerable<PaymentMethod> paymentMethods = await _unitOfWork.PaymentMethodRepository.GetByEntityId(entityId);
            PaymentMethod target = paymentMethods.FirstOrDefault(pm => pm.PaymentMethodId == paymentMethodId && pm.IsActive);

            if (target == null)
            {
                response.NotFound = true;
                response.Message = "Payment method not found for this entity.";
                return response;
            }

            try
            {
                PaymentMethodService stripePaymentMethodService = new PaymentMethodService();
                await stripePaymentMethodService.DetachAsync(target.Token);
            }
            catch (StripeException)
            {
                // Detach failures on the gateway side should not block deactivating it locally.
            }

            target.IsActive = false;
            target.IsDefault = false;
            await _unitOfWork.GetGenericRepository<PaymentMethod>().Update(target);

            response.Result = true;
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

        private async Task<string> GetOrCreateGatewayCustomerIdAsync(Guid entityId)
        {
            IEnumerable<PaymentMethod> existingMethods = await _unitOfWork.PaymentMethodRepository.GetByEntityId(entityId);
            string existingCustomerId = existingMethods.FirstOrDefault(pm => !string.IsNullOrEmpty(pm.GatewayCustomerId))?.GatewayCustomerId;
            if (!string.IsNullOrEmpty(existingCustomerId))
                return existingCustomerId;

            CustomerService customerService = new CustomerService();
            Customer customer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Metadata = new Dictionary<string, string> { { "EntityId", entityId.ToString() } }
            });

            return customer.Id;
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

        private static PaymentMethodSummaryDTO MapToPaymentMethodSummaryDTO(PaymentMethod pm)
        {
            return new PaymentMethodSummaryDTO
            {
                PaymentMethodId = pm.PaymentMethodId,
                PaymentMethodTypeId = pm.PaymentMethodTypeId,
                PaymentMethodTypeName = pm.PaymentMethodType?.PaymentMethodTypeName,
                CardBrand = pm.CardBrand,
                LastFourDigits = pm.LastFourDigits,
                IsDefault = pm.IsDefault
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
