using Microsoft.EntityFrameworkCore.Storage;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Repositories;
using System.Data.Common;

namespace ShiftSchedularDAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        // Transaction Properties
        private IDbContextTransaction _transaction;
        public bool activeTransaction;

        #region Repositories

        private IAbsenceTypeLocalizationRepository _absenceTypeLocalizationRepository;

        public IAbsenceTypeLocalizationRepository AbsenceTypeLocalizationRepository
        {
            get 
            { 
                return _absenceTypeLocalizationRepository = _absenceTypeLocalizationRepository ?? new AbsenceTypeLocalizationRepository(_context, this); 
            }
        }

        private IBaseEntityRuleRepository _baseEntityRuleRepository;

        public IBaseEntityRuleRepository BaseEntityRuleRepository
        {
            get
            {
                return _baseEntityRuleRepository = _baseEntityRuleRepository ?? new BaseEntityRuleRepository(_context, this);
            }
        }

        private IBaseEntityRuleSpecificationRepository _baseEntityRuleSpecificationRepository;
        public IBaseEntityRuleSpecificationRepository BaseEntityRuleSpecificationRepository
        {
            get
            {
                return _baseEntityRuleSpecificationRepository = _baseEntityRuleSpecificationRepository
                    ?? new BaseEntityRuleSpecificationRepository(_context, this);
            }
        }

        private IBusinessAspectLocalizationRepository _businessAspectLocalizationRepository;
        public IBusinessAspectLocalizationRepository BusinessAspectLocalizationRepository
        {
            get
            {
                return _businessAspectLocalizationRepository = _businessAspectLocalizationRepository
                    ?? new BusinessAspectLocalizationRepository(_context, this);
            }
        }

        private IEntityRuleRepository _entityRuleRepository;
        public IEntityRuleRepository EntityRuleRepository
        {
            get
            {
                return _entityRuleRepository = _entityRuleRepository
                    ?? new EntityRuleRepository(_context, this);
            }
        }

        private IEntityRuleSpecificationRepository _entityRuleSpecificationRepository;
        public IEntityRuleSpecificationRepository EntityRuleSpecificationRepository
        {
            get
            {
                return _entityRuleSpecificationRepository = _entityRuleSpecificationRepository
                    ?? new EntityRuleSpecificationRepository(_context, this);
            }
        }

        private IEntityScheduleRepository _entityScheduleRepository;
        public IEntityScheduleRepository EntityScheduleRepository
        {
            get
            {
                return _entityScheduleRepository = _entityScheduleRepository
                    ?? new EntityScheduleRepository(_context, this, SQLRawRepository);
            }
        }

        private IEntityScheduleWorkersRepository _entityScheduleWorkersRepository;
        public IEntityScheduleWorkersRepository EntityScheduleWorkersRepository
        {
            get
            {
                return _entityScheduleWorkersRepository = _entityScheduleWorkersRepository
                    ?? new EntityScheduleWorkersRepository(_context, this);
            }
        }

        private IEntityTypeLocalizationRepository _entityTypeLocalizationRepository;
        public IEntityTypeLocalizationRepository EntityTypeLocalizationRepository
        {
            get
            {
                return _entityTypeLocalizationRepository = _entityTypeLocalizationRepository
                    ?? new EntityTypeLocalizationRepository(_context, this);
            }
        }

        private IEntityWorkerAbsenceRepository _entityWorkerAbsenceRepository;
        public IEntityWorkerAbsenceRepository EntityWorkerAbsenceRepository
        {
            get
            {
                return _entityWorkerAbsenceRepository = _entityWorkerAbsenceRepository
                    ?? new EntityWorkerAbsenceRepository(_context, this);
            }
        }

        private IEntityWorkerInvitationRepository _entityWorkerInvitationRepository;
        public IEntityWorkerInvitationRepository EntityWorkerInvitationRepository
        {
            get
            {
                return _entityWorkerInvitationRepository = _entityWorkerInvitationRepository
                    ?? new EntityWorkerInvitationRepository(_context, this);
            }
        }

        private IEntityWorkerRepository _entityWorkerRepository;
        public IEntityWorkerRepository EntityWorkerRepository
        {
            get
            {
                return _entityWorkerRepository = _entityWorkerRepository
                    ?? new EntityWorkerRepository(_context, this, SQLRawRepository);
            }
        }

        private ILocalizationRepository _localizationRepository;
        public ILocalizationRepository LocalizationRepository
        {
            get
            {
                return _localizationRepository = _localizationRepository
                    ?? new LocalizationRepository(_context, this);
            }
        }

        private IRuleTypeBusinessAspectRepository _ruleTypeBusinessAspectRepository;
        public IRuleTypeBusinessAspectRepository RuleTypeBusinessAspectRepository
        {
            get
            {
                return _ruleTypeBusinessAspectRepository = _ruleTypeBusinessAspectRepository
                    ?? new RuleTypeBusinessAspectRepository(_context, this);
            }
        }

        private IRuleTypeLocalizationRepository _ruleTypeLocalizationRepository;
        public IRuleTypeLocalizationRepository RuleTypeLocalizationRepository
        {
            get
            {
                return _ruleTypeLocalizationRepository = _ruleTypeLocalizationRepository
                    ?? new RuleTypeLocalizationRepository(_context, this);
            }
        }

        private IShiftBreakRepository _shiftBreakRepository;
        public IShiftBreakRepository ShiftBreakRepository
        {
            get
            {
                return _shiftBreakRepository = _shiftBreakRepository
                    ?? new ShiftBreakRepository(_context, this);
            }
        }

        private IShiftBreakTypeLocalizationRepository _shiftBreakTypeLocalizationRepository;
        public IShiftBreakTypeLocalizationRepository ShiftBreakTypeLocalizationRepository
        {
            get
            {
                return _shiftBreakTypeLocalizationRepository = _shiftBreakTypeLocalizationRepository
                    ?? new ShiftBreakTypeLocalizationRepository(_context, this);
            }
        }

        private IShiftRepository _shiftRepository;
        public IShiftRepository ShiftRepository
        {
            get
            {
                return _shiftRepository = _shiftRepository
                    ?? new ShiftRepository(_context, this);
            }
        }

        private IShiftTemplateBreaksRepository _shiftTemplateBreaksRepository;
        public IShiftTemplateBreaksRepository ShiftTemplateBreaksRepository
        {
            get
            {
                return _shiftTemplateBreaksRepository = _shiftTemplateBreaksRepository
                    ?? new ShiftTemplateBreaksRepository(_context, this);
            }
        }

        private ISkillRepository _skillRepository;
        public ISkillRepository SkillRepository
        {
            get
            {
                return _skillRepository = _skillRepository
                    ?? new SkillRepository(_context, this);
            }
        }

        private ISkillLocalizationRepository _skillLocalizationRepository;
        public ISkillLocalizationRepository SkillLocalizationRepository
        {
            get
            {
                return _skillLocalizationRepository = _skillLocalizationRepository
                    ?? new SkillLocalizationRepository(_context, this);
            }
        }

        private IWorkerRepository _workerRepository;
        public IWorkerRepository WorkerRepository
        {
            get
            {
                return _workerRepository = _workerRepository
                    ?? new WorkerRepository(_context, this);
            }
        }

        private IGenderLocalizationRepository _genderLocalizationRepository;
        public IGenderLocalizationRepository GenderLocalizationRepository 
        {
            get
            {
                return _genderLocalizationRepository = _genderLocalizationRepository 
                    ?? new GenderLocalizationRepository(_context, this);
            }
        }

        private ISQLRawRepository<object> _sqlRawRepository;
        public ISQLRawRepository<object> SQLRawRepository
        {
            get
            {
                return _sqlRawRepository = _sqlRawRepository
                    ?? new SqlRawRepository<object>(_context, this);
            }
        }

        private IUserBotRepository _userBotRepository;
        public IUserBotRepository UserBotRepository 
        {
            get
            {
                return _userBotRepository = _userBotRepository
                    ?? new UserBotRepository(_context, this);
            }
        }

        private IEntityUserBotRepository _entityUserBotRepository;
        public IEntityUserBotRepository EntityUserBotRepository 
        {
            get
            {
                return _entityUserBotRepository = _entityUserBotRepository
                    ?? new EntityUserBotRepository(_context, this, SQLRawRepository);
            }
        }

        private IEntityShiftRotationRepository _entityShiftRotationRepository;

        public IEntityShiftRotationRepository EntityShiftRotationRepository 
        {
            get
            {
                return _entityShiftRotationRepository = _entityShiftRotationRepository ??
                    new EntityShiftRotationRepository(_context, this);
            } 
             
        }

        private IEntityUserBotShiftAssignedsRepository _entityUserBotShiftAssignedsRepository;

        public IEntityUserBotShiftAssignedsRepository EntityUserBotShiftAssignedsRepository
        {
            get
            {
                return _entityUserBotShiftAssignedsRepository = _entityUserBotShiftAssignedsRepository ?? new EntityUserBotShiftAssignedsRepository(_context, this);
            }
        }

        private IEntityWorkerShiftAssignedsRepository _entityWorkerShiftAssignedsRepository;

        public IEntityWorkerShiftAssignedsRepository EntityWorkerShiftAssignedsRepository
        {
            get
            {
                return _entityWorkerShiftAssignedsRepository = _entityWorkerShiftAssignedsRepository ?? new EntityWorkerShiftAssignedsRepository(_context, this);
            }
        }

        private IEntityUserBotSkillRepository _entityUserBotSkillRepository;

        public IEntityUserBotSkillRepository EntityUserBotSkillRepository
        {
            get
            {
                return _entityUserBotSkillRepository = _entityUserBotSkillRepository ?? new EntityUserBotSkillRepository(_context, this);
            }
        }

        private IEntityWorkerSkillRepository _entityWorkerSkillRepository;

        public IEntityWorkerSkillRepository EntityWorkerSkillRepository
        {
            get
            {
                return _entityWorkerSkillRepository = _entityWorkerSkillRepository ?? new EntityWorkerSkillRepository(_context, this);
            }
        }

        private IScheduleEntryBotsRepository _scheduleEntryBotsRepository;

        public IScheduleEntryBotsRepository ScheduleEntryBotsRepository
        {
            get
            {
                return _scheduleEntryBotsRepository = _scheduleEntryBotsRepository ?? new ScheduleEntryBotsRepository(_context, this);
            }
        }

        private IScheduleEntryBotIneligibilityRepository _scheduleEntryBotIneligibilityRepository;

        public IScheduleEntryBotIneligibilityRepository ScheduleEntryBotIneligibilityRepository
        {
            get { return _scheduleEntryBotIneligibilityRepository = _scheduleEntryBotIneligibilityRepository ?? new ScheduleEntryBotIneligibilityRepository(_context, this); }
        }

        private IScheduleEntryWorkerIneligibilityRepository _scheduleEntryWorkerIneligibilityRepository;

        public IScheduleEntryWorkerIneligibilityRepository ScheduleEntryWorkerIneligibilityRepository
        {
            get { return _scheduleEntryWorkerIneligibilityRepository = _scheduleEntryWorkerIneligibilityRepository ?? new ScheduleEntryWorkerIneligibilityRepository(_context, this); }
        }

        private IEntityRepository _entityRepository;

        public IEntityRepository EntityRepository
        {
            get { return _entityRepository = _entityRepository ?? new EntityRepository(_context, this); }
        }

        private IHolidayTypeLocalizationRepository _holidayTypeLocalizationRepository;

        /// <summary>
        /// Repository for HolidayTypeLocalization operations.
        /// </summary>
        public IHolidayTypeLocalizationRepository HolidayTypeLocalizationRepository
        {
            get { return _holidayTypeLocalizationRepository = _holidayTypeLocalizationRepository ?? new HolidayTypeLocalizationRepository(_context, this); }
        }

        private IHolidayBehaviourLocalizationRepository _holidayBehaviourLocalizationRepository;

        /// <summary>
        /// Repository for HolidayBehaviourLocalization operations.
        /// </summary>
        public IHolidayBehaviourLocalizationRepository HolidayBehaviourLocalizationRepository
        {
            get { return _holidayBehaviourLocalizationRepository = _holidayBehaviourLocalizationRepository ?? new HolidayBehaviourLocalizationRepository(_context, this); }
        }

        private IHolidayCatalogLocalizationRepository _holidayCatalogLocalizationRepository;

        /// <summary>
        /// Repository for HolidayCatalogLocalization operations.
        /// </summary>
        public IHolidayCatalogLocalizationRepository HolidayCatalogLocalizationRepository
        {
            get { return _holidayCatalogLocalizationRepository = _holidayCatalogLocalizationRepository ?? new HolidayCatalogLocalizationRepository(_context, this); }
        }

        private IEntityHolidayRepository _entityHolidayRepository;

        /// <summary>
        /// Repository for EntityHoliday operations.
        /// </summary>
        public IEntityHolidayRepository EntityHolidayRepository
        {
            get { return _entityHolidayRepository = _entityHolidayRepository ?? new EntityHolidayRepository(_context, this); }
        }

        private IEntityPermissionRepository _entityPermissionRepository;
        public IEntityPermissionRepository EntityPermissionRepository 
        {
            get { return _entityPermissionRepository = _entityPermissionRepository ?? new EntityPermissionRepository(_context, this); }
        }

        private IEntityPermissionRoleRepository _entityPermissionRoleRepository;
        public IEntityPermissionRoleRepository EntityPermissionRoleRepository
        {
            get { return _entityPermissionRoleRepository = _entityPermissionRoleRepository ?? new EntityPermissionRoleRepository(_context, this); }
        }

        private IEntityPermissionRoleLocalizationRepository _entityPermissionRoleLocalizationRepository;
        public IEntityPermissionRoleLocalizationRepository EntityPermissionRoleLocalizationRepository
        {
            get { return _entityPermissionRoleLocalizationRepository = _entityPermissionRoleLocalizationRepository ?? new EntityPermissionRoleLocalizationRepository(_context, this); }
        }

        #endregion

        #region Constructor

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        public bool ReturnTransactionStatus()
        {
            return activeTransaction;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            activeTransaction = true;
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            activeTransaction = false;
        }

        public IGenericRepository<T> GetGenericRepository<T>() where T : class
        {
            return new GenericRepository<T>(_context, this);
        }

        public DbTransaction ReturnCurrentTransaction()
        {
            if (activeTransaction)
                return _transaction.GetDbTransaction();
            else return null;
        }

        #endregion
    }
}
