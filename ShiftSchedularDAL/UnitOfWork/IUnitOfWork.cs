using ShiftSchedularDAL.IRepositories;
using System.Data.Common;

namespace ShiftSchedularDAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IAbsenceTypeLocalizationRepository AbsenceTypeLocalizationRepository { get; }
        IBaseEntityRuleRepository BaseEntityRuleRepository { get; }
        IBaseEntityRuleSpecificationRepository BaseEntityRuleSpecificationRepository { get; }
        IBusinessAspectLocalizationRepository BusinessAspectLocalizationRepository { get; }
        IEntityRuleRepository EntityRuleRepository { get; }
        IEntityRuleSpecificationRepository EntityRuleSpecificationRepository { get; }
        IEntityScheduleRepository EntityScheduleRepository { get; }
        IEntityScheduleWorkersRepository EntityScheduleWorkersRepository { get; }
        IEntityTypeLocalizationRepository EntityTypeLocalizationRepository { get; }
        IEntityWorkerAbsenceRepository EntityWorkerAbsenceRepository { get; }
        IEntityWorkerInvitationRepository EntityWorkerInvitationRepository { get; }
        IEntityWorkerRepository EntityWorkerRepository { get; }
        IEntityWorkerSkillRepository EntityWorkerSkillRepository { get; }
        ILocalizationRepository LocalizationRepository { get; }
        IRuleTypeBusinessAspectRepository RuleTypeBusinessAspectRepository { get; }
        IRuleTypeLocalizationRepository RuleTypeLocalizationRepository { get; }
        IShiftBreakRepository ShiftBreakRepository { get; }
        IShiftBreakTypeLocalizationRepository ShiftBreakTypeLocalizationRepository { get; }
        IShiftRepository ShiftRepository { get; }
        IShiftTemplateBreaksRepository ShiftTemplateBreaksRepository { get; }
        ISkillRepository SkillRepository { get;}
        ISkillLocalizationRepository SkillLocalizationRepository { get; }
        IWorkerRepository WorkerRepository { get; }
        IGenderLocalizationRepository GenderLocalizationRepository { get; }
        IUserBotRepository UserBotRepository { get; }
        IEntityUserBotRepository EntityUserBotRepository { get; }
        IEntityUserBotSkillRepository EntityUserBotSkillRepository { get; }
        IEntityShiftRotationRepository EntityShiftRotationRepository { get; }
        IEntityUserBotShiftAssignedsRepository EntityUserBotShiftAssignedsRepository { get; }
        IEntityWorkerShiftAssignedsRepository EntityWorkerShiftAssignedsRepository { get; }
        IScheduleEntryBotsRepository ScheduleEntryBotsRepository { get; }
        IScheduleEntryBotIneligibilityRepository ScheduleEntryBotIneligibilityRepository { get; }
        IScheduleEntryWorkerIneligibilityRepository ScheduleEntryWorkerIneligibilityRepository { get; }
        IEntityRepository EntityRepository { get; }
        ISQLRawRepository<object> SQLRawRepository { get; }
        IHolidayTypeLocalizationRepository HolidayTypeLocalizationRepository { get; }
        IHolidayBehaviourLocalizationRepository HolidayBehaviourLocalizationRepository { get; }
        IHolidayCatalogLocalizationRepository HolidayCatalogLocalizationRepository { get; }
        IEntityHolidayRepository EntityHolidayRepository { get; }
        IEntityPermissionRepository EntityPermissionRepository { get; }
        IEntityPermissionRoleRepository EntityPermissionRoleRepository { get; }
        IEntityPermissionRoleLocalizationRepository EntityPermissionRoleLocalizationRepository { get; }
        IUserNotificationRepository UserNotificationRepository { get; }
        ISubscriptionPlanTypeRepository SubscriptionPlanTypeRepository { get; }
        ISubscriptionPlanTypeLocalizationRepository SubscriptionPlanTypeLocalizationRepository { get; }
        ISubscriptionDurationTypeRepository SubscriptionDurationTypeRepository { get; }
        ISubscriptionDurationTypeLocalizationRepository SubscriptionDurationTypeLocalizationRepository { get; }
        IPaymentMethodTypeRepository PaymentMethodTypeRepository { get; }
        IPaymentMethodTypeLocalizationRepository PaymentMethodTypeLocalizationRepository { get; }
        IPaymentMethodTypeCountryRepository PaymentMethodTypeCountryRepository { get; }
        ISubscriptionPlanDurationPriceRepository SubscriptionPlanDurationPriceRepository { get; }
        ICampaignRepository CampaignRepository { get; }
        ICampaignLocalizationRepository CampaignLocalizationRepository { get; }
        ICampaignSubscriptionPlanRepository CampaignSubscriptionPlanRepository { get; }
        IEntitySubscriptionPlanRepository EntitySubscriptionPlanRepository { get; }
        IPaymentMethodRepository PaymentMethodRepository { get; }

        IGenericRepository<T> GetGenericRepository<T>() where T : class;

        // Methods
        bool ReturnTransactionStatus();
        Task BeginTransactionAsync();
        DbTransaction ReturnCurrentTransaction();
        Task SaveChangesAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
