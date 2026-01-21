using ShiftSchedularBLL.IService;
using ShiftSchedularBLL.Service;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Repositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularIL.IServices;
using ShiftSchedularIL.Mappers;
using ShiftSchedularIL.Services;
using System.Reflection;

namespace ShiftSchedularAPI.Configurations
{
    public static class ServicesInjections
    {
        public static void AddServicesInjections(
            this IServiceCollection services,
            string logDirectory,
            EmailSettings emailSettings)
        {
            #region Repositories

            services.AddScoped<IGenericRepository<Gender>, GenericRepository<Gender>>();
            services.AddScoped<IGenderLocalizationRepository, GenderLocalizationRepository>();
            services.AddScoped<ILocalizationRepository, LocalizationRepository>();
            services.AddScoped<IWorkerRepository, WorkerRepository>();
            services.AddScoped<IGenericRepository<EntityType>, GenericRepository<EntityType>>();
            services.AddScoped<IGenericRepository<EntityTypeLocalization>, GenericRepository<EntityTypeLocalization>>();
            services.AddScoped<IGenericRepository<Entity>, GenericRepository<Entity>>();
            services.AddScoped<IEntityWorkerRepository, EntityWorkerRepository>();
            services.AddScoped<IEntityUserBotRepository, EntityUserBotRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<ISkillLocalizationRepository, SkillLocalizationRepository>();
            services.AddScoped<IEntityTypeLocalizationRepository, EntityTypeLocalizationRepository>();
            services.AddScoped<ISQLRawRepository<object>, SqlRawRepository<object>>();
            services.AddScoped<IEntityWorkerInvitationRepository, EntityWorkerInvitationRepository>();
            // Repositories - Shifts
            services.AddScoped<IShiftRepository, ShiftRepository>();
            services.AddScoped<IShiftBreakRepository, ShiftBreakRepository>();
            services.AddScoped<IGenericRepository<ShiftBreakType>, GenericRepository<ShiftBreakType>>();
            services.AddScoped<IShiftBreakTypeLocalizationRepository, ShiftBreakTypeLocalizationRepository>();
            services.AddScoped<IGenericRepository<ShiftTemplate>, GenericRepository<ShiftTemplate>>();
            services.AddScoped<IGenericRepository<ShiftBreakTemplate>, GenericRepository<ShiftBreakTemplate>>();
            services.AddScoped<IShiftTemplateBreaksRepository, ShiftTemplateBreaksRepository>();
            services.AddScoped<IEntityWorkerShiftAssignedsRepository, EntityWorkerShiftAssignedsRepository>();
            services.AddScoped<IEntityUserBotShiftAssignedsRepository, EntityUserBotShiftAssignedsRepository>();
            // Repositories - Rules
            services.AddScoped<IBusinessAspectLocalizationRepository, BusinessAspectLocalizationRepository>();
            services.AddScoped<IGenericRepository<BusinessAspect>, GenericRepository<BusinessAspect>>();
            services.AddScoped<IRuleTypeLocalizationRepository, RuleTypeLocalizationRepository>();
            services.AddScoped<IGenericRepository<RuleType>, GenericRepository<RuleType>>();
            services.AddScoped<IEntityRuleRepository, EntityRuleRepository>();
            services.AddScoped<IEntityRuleSpecificationRepository, EntityRuleSpecificationRepository>();
            services.AddScoped<IRuleTypeBusinessAspectRepository, RuleTypeBusinessAspectRepository>();
            // Repositories - Absences
            services.AddScoped<IAbsenceTypeLocalizationRepository, AbsenceTypeLocalizationRepository>();
            services.AddScoped<IGenericRepository<AbsenceType>, GenericRepository<AbsenceType>>();
            services.AddScoped<IEntityWorkerAbsenceRepository, EntityWorkerAbsenceRepository>();
            // Repositories - Schedule
            services.AddScoped<IEntityScheduleRepository, EntityScheduleRepository>();
            services.AddScoped<IEntityScheduleWorkersRepository, EntityScheduleWorkersRepository>();
            services.AddScoped<IScheduleEntryBotIneligibilityRepository, ScheduleEntryBotIneligibilityRepository>();
            services.AddScoped<IScheduleEntryWorkerIneligibilityRepository, ScheduleEntryWorkerIneligibilityRepository>();
            // Repositories - Base Entity Rule
            services.AddScoped<IBaseEntityRuleRepository, BaseEntityRuleRepository>();
            services.AddScoped<IBaseEntityRuleSpecificationRepository, BaseEntityRuleSpecificationRepository>();

            #endregion

            #region Services

            services.AddLogging();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IGenderService, GenderService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEntityTypeService, EntityTypeService>();
            services.AddScoped<IEntityService, EntityService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IShiftService, ShiftService>();
            services.AddScoped<IShiftBreakTypeService, ShiftBreakTypeService>();
            services.AddScoped<IShiftTemplateService, ShiftTemplateService>();
            services.AddScoped<IBusinessAspectService, BusinessAspectService>();
            services.AddScoped<IEntityRuleService, EntityRuleService>();
            services.AddScoped<IRuleTypeService, RuleTypeService>();
            services.AddScoped<IEntityWorkerAbsenceService, EntityWorkerAbsenceService>();
            services.AddScoped<IAbsenceTypeService, AbsenceTypeService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEntityScheduleService, EntityScheduleService>();
            services.AddScoped<IBaseEntityRuleService, BaseEntityRuleService>();
            services.AddScoped<IScheduleGeneratorService, ScheduleGeneratorService>();
            services.AddScoped<IEntityDashboardService, EntityDashboardService>();

            // Infrastructure Services
            services.AddAutoMapper(Assembly.GetAssembly(typeof(ApplicationMapper)));
            services.AddScoped<IGeneralService, GeneralService>();
            services.AddScoped<ICryptographyService, CryptographyService>();
            services.AddScoped<ILoggerService>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<LoggerService>>();
                return new LoggerService(logger, logDirectory);
            });
            services.AddScoped<IEmailService>(provider =>
            {
                return new EmailService(emailSettings);
            });


            #endregion
        }
    }
}
