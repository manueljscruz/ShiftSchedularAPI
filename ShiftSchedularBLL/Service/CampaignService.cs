using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Service
{
    /// <summary>
    /// Service implementation for Campaign admin management.
    /// </summary>
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CampaignService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AdminCampaignItemDTO>> GetAllAsync()
        {
            var items = await _unitOfWork.CampaignRepository.GetAllWithDetails();
            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminCampaignItemDTO> GetByIdAsync(Guid id)
        {
            var item = await _unitOfWork.CampaignRepository.GetByIdWithDetails(id);
            return item == null ? null : MapToDTO(item);
        }

        public async Task<BaseResponse<Guid>> UpsertAsync(AdminUpsertCampaignDTO dto)
        {
            var response = new BaseResponse<Guid>();

            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
            {
                response.Message = "Name is required.";
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                Campaign item;

                if (dto.Id == Guid.Empty)
                {
                    item = new Campaign
                    {
                        CampaignId = Guid.NewGuid(),
                        CampaignName = dto.Name,
                        CampaignDescription = dto.Description,
                        StartDate = dto.StartDate,
                        EndDate = dto.EndDate,
                        PromotionPercent = dto.PromotionPercent,
                        IsActive = dto.IsActive,
                        MaxRedemptions = dto.MaxRedemptions,
                        RedemptionCount = 0,
                        CouponCode = dto.CouponCode
                    };
                    item = await _unitOfWork.CampaignRepository.Add(item);
                }
                else
                {
                    item = await _unitOfWork.CampaignRepository.GetById(dto.Id);
                    if (item == null)
                    {
                        response.NotFound = true;
                        response.Message = "Campaign not found.";
                        return response;
                    }

                    item.CampaignName = dto.Name;
                    item.CampaignDescription = dto.Description;
                    item.StartDate = dto.StartDate;
                    item.EndDate = dto.EndDate;
                    item.PromotionPercent = dto.PromotionPercent;
                    item.IsActive = dto.IsActive;
                    item.MaxRedemptions = dto.MaxRedemptions;
                    item.CouponCode = dto.CouponCode;
                    await _unitOfWork.CampaignRepository.Update(item);
                }

                await UpsertLocalizations(item.CampaignId, dto.Localizations);
                await SyncEligiblePlans(item.CampaignId, dto.EligibleSubscriptionPlanDurationPriceIds);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Result = item.CampaignId;
                response.Message = "Campaign saved successfully.";
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = "Failed to save Campaign.";
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            var response = new BaseResponse<bool>();

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var localizations = await _unitOfWork.CampaignLocalizationRepository.GetByParentId(id);
                await _unitOfWork.CampaignLocalizationRepository.DeleteRange(localizations);

                var eligiblePlans = await _unitOfWork.CampaignSubscriptionPlanRepository.GetByParentId(id);
                await _unitOfWork.CampaignSubscriptionPlanRepository.DeleteRange(eligiblePlans);

                await _unitOfWork.CampaignRepository.Delete(id);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Result = true;
                response.Message = "Campaign deleted successfully.";
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = "Failed to delete Campaign.";
            }

            return response;
        }

        private async Task UpsertLocalizations(Guid campaignId, List<AdminUpsertCampaignLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _unitOfWork.CampaignLocalizationRepository.GetByParentAndLocalizationId(campaignId, loc.LocalizationId);

                if (existing == null)
                {
                    await _unitOfWork.CampaignLocalizationRepository.Add(new CampaignLocalization
                    {
                        CampaignId = campaignId,
                        LocalizationId = loc.LocalizationId,
                        CampaignNameDisplayValue = loc.NameDisplayValue,
                        CampaignDescriptionDisplayValue = loc.DescriptionDisplayValue
                    });
                }
                else
                {
                    existing.CampaignNameDisplayValue = loc.NameDisplayValue;
                    existing.CampaignDescriptionDisplayValue = loc.DescriptionDisplayValue;
                    await _unitOfWork.CampaignLocalizationRepository.Update(existing);
                }
            }
        }

        private async Task SyncEligiblePlans(Guid campaignId, List<Guid> subscriptionPlanDurationPriceIds)
        {
            var existing = (await _unitOfWork.CampaignSubscriptionPlanRepository.GetByParentId(campaignId)).ToList();

            var toRemove = existing.Where(csp => !subscriptionPlanDurationPriceIds.Contains(csp.SubscriptionPlanDurationPriceId)).ToList();
            await _unitOfWork.CampaignSubscriptionPlanRepository.DeleteRange(toRemove);

            var existingIds = existing.Select(csp => csp.SubscriptionPlanDurationPriceId).ToHashSet();
            foreach (var id in subscriptionPlanDurationPriceIds.Where(id => !existingIds.Contains(id)))
            {
                await _unitOfWork.CampaignSubscriptionPlanRepository.Add(new CampaignSubscriptionPlan
                {
                    CampaignId = campaignId,
                    SubscriptionPlanDurationPriceId = id
                });
            }
        }

        private static AdminCampaignItemDTO MapToDTO(Campaign c) => new()
        {
            Id = c.CampaignId,
            Name = c.CampaignName,
            Description = c.CampaignDescription,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            PromotionPercent = c.PromotionPercent,
            IsActive = c.IsActive,
            MaxRedemptions = c.MaxRedemptions,
            RedemptionCount = c.RedemptionCount,
            CouponCode = c.CouponCode,
            EligibleSubscriptionPlanDurationPriceIds = c.CampaignSchedulePlans?.Select(csp => csp.SubscriptionPlanDurationPriceId).ToList() ?? new(),
            Localizations = c.CampaignLocalizations?.Select(l => new AdminCampaignLocalizationValueDTO
            {
                LocalizationId = l.LocalizationId,
                LocalizationCode = l.Localization?.LocalizationCode ?? string.Empty,
                NameDisplayValue = l.CampaignNameDisplayValue,
                DescriptionDisplayValue = l.CampaignDescriptionDisplayValue
            }).ToList() ?? new()
        };
    }
}
