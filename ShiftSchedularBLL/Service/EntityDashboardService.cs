using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularIL.IServices;

namespace ShiftSchedularBLL.Service
{
    public class EntityDashboardService : IEntityDashboardService
    {
        private readonly IGeneralService _generalService;
        private readonly IEntityService _entityService;
        private readonly IEntityScheduleService _entityScheduleService;
        private readonly IEntityWorkerAbsenceService _entityWorkerAbsenceService;
        private readonly IShiftService _shiftService;
        private readonly IEntityRuleService _entityRuleService;
        private readonly ILanguageAccessor _languageAccessor;

        #region Constructor

        public EntityDashboardService(IGeneralService generalService,
            IEntityService entityService,
            IEntityScheduleService entityScheduleService,
            IEntityWorkerAbsenceService entityWorkerAbsenceService,
            IShiftService shiftService,
            IEntityRuleService entityRuleService,
            ILanguageAccessor languageAccessor)
        {
            _generalService = generalService;
            _entityService = entityService;
            _entityScheduleService = entityScheduleService;
            _entityWorkerAbsenceService = entityWorkerAbsenceService;
            _shiftService = shiftService;
            _entityRuleService = entityRuleService;
            _languageAccessor = languageAccessor;
        }

        #endregion

        #region Get Entity Dashboard View Model

        public async Task<DashboardEntityViewModel> GetEntityDashboardViewModel(BaseViewModelRequest request)
        {
            DashboardEntityViewModel dashboardEntityViewModel = new DashboardEntityViewModel();

            Guid entityId = _generalService.ParseStringToGuid(request.EntityId.ToString());

            // Get entity data
            dashboardEntityViewModel.EntityDTO = await _entityService.GetEntityById(entityId, _languageAccessor.GetLanguageCode());

            // Get entity member data that is requesting data
            List<EntityWorkerMemberDTO> entityWorkerMemberDTOLst = await _entityService.GetEntityMembers(entityId, new List<string> { request.WorkerId });
            EntityWorkerMemberDTO entityWorkerMemberDTO = entityWorkerMemberDTOLst.First();

            // Set skillset
            dashboardEntityViewModel.AssignedEntitySkills = entityWorkerMemberDTO.SkillSet;

            // Get schedule entries
            ScheduleViewModelRequestDTO scheduleViewModelRequest = new ScheduleViewModelRequestDTO(entityId, request.WorkerId, DateTime.UtcNow, DateTime.UtcNow.AddDays(7));
            List<ScheduleEntryDTO> scheduleEntryDTOs = await _entityScheduleService.GetScheduleEntries(scheduleViewModelRequest);

            // TODO: Filter schedule entries based on permission role once permission system is wired
            dashboardEntityViewModel.ScheduleEntries = scheduleEntryDTOs;

            // Get Absences
            PagedModelRequest absencesRequest = new PagedModelRequest(entityId, entityWorkerMemberDTO.WorkerId, 0, 1, 20, false);
            dashboardEntityViewModel.EntityWorkerAbsenceEntries = await _entityWorkerAbsenceService.GetEntityWorkerAbsences(absencesRequest);

            // Get Statistics — TODO: scope based on permission role once permission system is wired
            int totalShifts = await _shiftService.GetTotalEntityShifts(entityId);
            int totalRules = await _entityRuleService.GetTotalEntityRules(entityId);
            dashboardEntityViewModel.EntityStatistics = new EntityStatisticsDTO(dashboardEntityViewModel.EntityDTO.EntityWorkersCount, totalRules, totalShifts);

            return dashboardEntityViewModel;
        }

        #endregion
    }
}
