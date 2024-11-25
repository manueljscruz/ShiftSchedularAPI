using ShiftSchedularDAL.IRepositories;

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
        ISQLRawRepository<object> SQLRawRepository { get; }
        IGenericRepository<T> GetGenericRepository<T>() where T : class;

        // Methods
        bool ReturnTransactionStatus();
        Task BeginTransactionAsync();
        Task SaveChangesAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
