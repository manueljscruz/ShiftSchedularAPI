using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Enums;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.MemberManagement;
using ShiftSchedularRL.Resources.ScheduleManagement;
using System.Globalization;

namespace ShiftSchedularBLL.Service
{
    public class ScheduleGeneratorService : IScheduleGeneratorService
    {
        private readonly IGenericRepository<BusinessAspect> _businessAspectRepository;
        private readonly IShiftService _shiftService;
        private readonly IEntityWorkerAbsenceService _entityWorkerAbsenceService;
        private readonly IEntityRuleService _entityRuleService;
        private readonly IGeneralService _generalService;
        private readonly IEntityService _entityService;
        private readonly ISkillService _skillService;
        private readonly IEntityScheduleService _entityScheduleService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILanguageAccessor _languageAccessor;

        #region Constructor

        public ScheduleGeneratorService(IUnitOfWork unitOfWork,
            IShiftService shiftService,
            IEntityWorkerAbsenceService entityWorkerAbsenceService,
            IEntityRuleService entityRuleService,
            IGeneralService generalService,
            IEntityService entityService,
            ISkillService skillService,
            IEntityScheduleService entityScheduleService,
            IMapper mapper,
            IGenericRepository<BusinessAspect> businessAspectRepository,
            ILanguageAccessor languageAccessor
            )
        {
            _unitOfWork = unitOfWork;
            _shiftService = shiftService;
            _entityWorkerAbsenceService = entityWorkerAbsenceService;
            _entityRuleService = entityRuleService;
            _entityService = entityService;
            _businessAspectRepository = businessAspectRepository;
            _skillService = skillService;
            _entityScheduleService = entityScheduleService;
            _mapper = mapper;
            _generalService = generalService;
            _languageAccessor = languageAccessor;
        }

        #endregion

        #region Methods

        #region CORE: Create Entity Schedule

        public async Task<BaseResponse<List<ScheduleEntryDTO>>> CreateEntitySchedule(CreateEntityScheduleDTO createEntityScheduleDTO, bool isOpRotation = false)
        {
            BaseResponse<List<ScheduleEntryDTO>> response = new BaseResponse<List<ScheduleEntryDTO>>();
            response.Message = "";
            response.Success = false;

            if (createEntityScheduleDTO != null && createEntityScheduleDTO.EntityId != Guid.Empty && !string.IsNullOrEmpty(createEntityScheduleDTO.WorkerId))
            {
                bool isOwner = await _unitOfWork.EntityWorkerRepository.IsMemberOwner(createEntityScheduleDTO.EntityId, createEntityScheduleDTO.WorkerId);

                if (isOwner || isOpRotation)
                {
                    await _unitOfWork.BeginTransactionAsync();

                    List<ScheduleEntryDTO> scheduleEntryDTOs = new List<ScheduleEntryDTO>();

                    try
                    {
                        scheduleEntryDTOs = await InitializeScheduleEntries(createEntityScheduleDTO);

                        #region Get Relevant Data

                        List<ShiftDTO> shifts = new List<ShiftDTO>();
                        List<EntityRuleDTO> ruleDTOs = new List<EntityRuleDTO>();
                        List<EntityWorkerMemberDTO> entityWorkerMemberDTOs = new List<EntityWorkerMemberDTO>();
                        List<SkillLocalizedDTO> entitySkills = new List<SkillLocalizedDTO>();

                        // Get Shifts
                        if (createEntityScheduleDTO.FilteredShifts.Count() == 0)
                            shifts = await _shiftService.GetEntityShifts(createEntityScheduleDTO.EntityId) as List<ShiftDTO>;
                        else
                            shifts = await _shiftService.GetSpecificShifts(createEntityScheduleDTO.FilteredShifts);

                        // Get Rules
                        if (createEntityScheduleDTO.FilteredRules.Count() == 0)
                            ruleDTOs = await _entityRuleService.GetEntityRules(createEntityScheduleDTO.EntityId, _languageAccessor.GetLanguageCode());
                        else
                            ruleDTOs = await _entityRuleService.GetSpecificRules(createEntityScheduleDTO.EntityId, createEntityScheduleDTO.FilteredRules, _languageAccessor.GetLanguageCode());

                        // Get Members
                        if (createEntityScheduleDTO.FilteredMembers.Count() == 0)
                            entityWorkerMemberDTOs = await _entityService.GetEntityMembers(createEntityScheduleDTO.EntityId, new List<string>());
                        else
                            entityWorkerMemberDTOs = await _entityService.GetEntityMembers(createEntityScheduleDTO.EntityId, createEntityScheduleDTO.FilteredMembers);

                        // Get Entity Skills
                        entitySkills = await _entityService.GetEntitySkills(new BaseViewModelRequest
                        {
                            EntityId = createEntityScheduleDTO.EntityId
                        });

                        #endregion

                        #region Get Shift Rotation

                        List<EntityShiftRotationDTO> entityShiftRotations = await _shiftService.GetEntityShiftRotations(createEntityScheduleDTO.EntityId);

                        #endregion

                        #region Create Shift Entries

                        DateTime cycleDate = createEntityScheduleDTO.StartDate;

                        while (cycleDate <= createEntityScheduleDTO.EndDate)
                        {
                            bool isWeekend = cycleDate.DayOfWeek == DayOfWeek.Saturday || cycleDate.DayOfWeek == DayOfWeek.Sunday;

                            foreach (ShiftDTO shift in shifts)
                            {
                                // Check if Shift is to be applied on the weekends
                                if (isWeekend && !ruleDTOs.Any(i =>
                                        i.RuleTypeId.Equals(RuleTypeConstants.SHIFT_INCLUDES_WEEKENDS_ID) &&
                                        i.EntityRuleSpecificationDTOs.Any(j =>
                                            _generalService.ParseStringToGuid(j.AspectReferenceId).Equals(shift.ShiftId) &&
                                            j.RuleSpecificationValue.Equals(1))))
                                {
                                    continue;
                                }

                                // Check if there is an entry for this shift at this date already
                                ScheduleEntryDTO existingEntry = scheduleEntryDTOs
                                        .FirstOrDefault(i => i.ScheduleStartDate.Date.Equals(cycleDate.Date) && i.ShiftId.Equals(shift.ShiftId));

                                // If it exists continue
                                if (existingEntry != null)
                                    continue;

                                // Create new one
                                ScheduleEntryDTO scheduleEntryDTO = await CreateShiftEntry(shift, cycleDate);

                                // Add it
                                scheduleEntryDTOs.Add(scheduleEntryDTO);
                            }

                            // Move to the next day
                            cycleDate = cycleDate.AddDays(1);
                        }

                        scheduleEntryDTOs = scheduleEntryDTOs.OrderBy(i => i.ScheduleStartDate).ToList();

                        #endregion

                        #region Fill Out Schedule

                        scheduleEntryDTOs = await FillOutSchedule(scheduleEntryDTOs, shifts, ruleDTOs, entityWorkerMemberDTOs, entityShiftRotations, entitySkills, createEntityScheduleDTO);

                        #endregion

                        #region Add To Db

                        foreach (ScheduleEntryDTO entry in scheduleEntryDTOs)
                        {
                            await PersistParticipants(entry);
                        }

                        #endregion

                        await _unitOfWork.CommitAsync();

                        response.Result = scheduleEntryDTOs;
                        response.Success = true;
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                    }
                    finally
                    {
                        _unitOfWork.Dispose();
                    }
                }
            }


            return response;
        }

        #endregion

        #region AUX: Create Shift Entry

        private async Task<ScheduleEntryDTO> CreateShiftEntry(ShiftDTO shift, DateTime cycleDate)
        {
            var entry = await _entityScheduleService.CreateBaseScheduleEntry(shift, cycleDate);

            var dto = _mapper.Map<ScheduleEntryDTO>(entry);
            dto.ShiftDTO = shift;
            dto.ScheduleParticipants = new List<ScheduleEntryParticipantDTO>();

            return dto;
        }

        #endregion

        #region AUX: Apply Rotation Cycle

        public async Task<BaseResponse<List<ScheduleEntryDTO>>> ApplyRotationCycle(ApplyRotationCycleDTO rotationCycleDTO)
        {
            // Validate worker exists
            var worker = (await _entityService.GetEntityMembers(
                rotationCycleDTO.EntityId,
                new List<string> { rotationCycleDTO.WorkerId }
            )).FirstOrDefault();

            if (worker == null)
            {
                return new BaseResponse<List<ScheduleEntryDTO>>
                {
                    Message = EntityWorkerRelatedMessages.MemberNotFound,
                    Success = false
                };
            }

            if (rotationCycleDTO.CycleStartDate > rotationCycleDTO.CycleEndDate)
            {
                return new BaseResponse<List<ScheduleEntryDTO>>
                {
                    Message = ScheduleRelatedMessages.InvalidDateInputOrder,
                    Success = false
                };
            }

            // Reuse CreateEntitySchedule
            var createDto = new CreateEntityScheduleDTO
            {
                EntityId = rotationCycleDTO.EntityId,
                WorkerId = rotationCycleDTO.WorkerId,
                StartDate = rotationCycleDTO.CycleStartDate,
                EndDate = rotationCycleDTO.CycleEndDate,
                FilteredMembers = new List<string> { rotationCycleDTO.WorkerId },
                SingleRoleResponsibility = false,
                ClearExistingSchedule = false,
                ForceNoSkill = true
            };

            var response = await CreateEntitySchedule(createDto, true);

            // Filter down to only entries involving this worker
            if (response.Success)
            {
                response.Result = response.Result
                    .Where(e => e.ScheduleParticipants.Any(p => p.Worker.WorkerId == rotationCycleDTO.WorkerId))
                    .ToList();
            }

            return response;
        }

        #endregion

        #region Initialize Schedule Entries

        private async Task<List<ScheduleEntryDTO>> InitializeScheduleEntries(CreateEntityScheduleDTO dto)
        {
            if (dto.ClearExistingSchedule)
            {
                List<ScheduleEntry> entries = await _unitOfWork.EntityScheduleRepository.GetEFScheduleEntries(dto.EntityId, dto.StartDate, dto.EndDate);

                foreach (ScheduleEntry entry in entries)
                {
                    await _unitOfWork.ScheduleEntryBotsRepository.DeleteRange(entry.ScheduleEntryBots);
                    await _unitOfWork.EntityScheduleWorkersRepository.DeleteRange(entry.ScheduleEntryWorkers);
                    await _unitOfWork.GetGenericRepository<ScheduleEntryBotIneligibility>().DeleteRange(entry.ScheduleEntryBotIneligibilities);
                    await _unitOfWork.GetGenericRepository<ScheduleEntryWorkerIneligibility>().DeleteRange(entry.ScheduleEntryWorkerIneligibilities);
                }

                await _unitOfWork.EntityScheduleRepository.DeleteRange(entries);

                return new List<ScheduleEntryDTO>();
            }
            else
            {
                var request = new ScheduleViewModelRequestDTO
                {
                    WorkerId = string.Empty,
                    EntityId = dto.EntityId,
                    StartDateSearch = dto.StartDate,
                    EndDateSearch = dto.EndDate
                };

                return await _entityScheduleService.GetScheduleEntries(request);
            }
        }

        #endregion

        #region Persist Participants

        private async Task PersistParticipants(ScheduleEntryDTO entry)
        {
            foreach (var participant in entry.ScheduleParticipants)
            {
                if (participant.Worker.IsBot)
                {
                    Guid botGuid = _generalService.ParseStringToGuid(participant.Worker.WorkerId);

                    // If the participant 
                    if (await _unitOfWork.ScheduleEntryBotsRepository.ParticipantExists(entry.ScheduleEntryId, botGuid))
                        continue;

                    await _unitOfWork.ScheduleEntryBotsRepository.Add(new ScheduleEntryBots
                    {
                        ScheduleEntryId = entry.ScheduleEntryId,
                        UserBotId = botGuid,
                        SpecificSkillAssignments = string.Join(",", participant.AssignedSkills.Select(s => s.SkillId))
                    });
                }
                else
                {
                    if (await _unitOfWork.EntityScheduleWorkersRepository.ParticipantExist(entry.ScheduleEntryId, participant.Worker.WorkerId))
                        continue;

                    await _unitOfWork.EntityScheduleWorkersRepository.Add(new ScheduleEntryWorkers
                    {
                        ScheduleEntryId = entry.ScheduleEntryId,
                        ApplicationUserId = participant.Worker.WorkerId,
                        SpecificSkillAssignments = string.Join(",", participant.AssignedSkills.Select(s => s.SkillId))
                    });
                }
            }
        }

        #endregion

        #region Fill Out Schedule

        public async Task<List<ScheduleEntryDTO>> FillOutSchedule(
            List<ScheduleEntryDTO> scheduleEntryDTOs,
            List<ShiftDTO> shifts,
            List<EntityRuleDTO> entityRules,
            List<EntityWorkerMemberDTO> entityWorkerMemberDTOs,
            List<EntityShiftRotationDTO> entityShiftRotationDTOs,
            List<SkillLocalizedDTO> entitySkills,
            CreateEntityScheduleDTO createEntityScheduleDTO)
        {
            // Instantiate List of Schedule Entry Ineligibilities
            List<ScheduleEntryIneligibilityModel> scheduleEntryIneligibilityModels = new List<ScheduleEntryIneligibilityModel>();

            // Track the day at the start date
            DateTime trackingDay = scheduleEntryDTOs.Count != 0 ? scheduleEntryDTOs[0].ScheduleStartDate.Date : new DateTime();
            bool isFirstDay = true;

            // Get Workers Absences to apply Entry Ineligibilities
            scheduleEntryIneligibilityModels = await GetIneligibilitiesByAbsences(createEntityScheduleDTO, scheduleEntryDTOs, entityWorkerMemberDTOs);

            IEnumerable<EntityWorkerMemberDTO> botMembers = entityWorkerMemberDTOs.Where(i => i.IsBot);
            IEnumerable<EntityWorkerMemberDTO> workerMembers = entityWorkerMemberDTOs.Where(i => !i.IsBot);

            if (botMembers.Count() != 0)
            {
                List<ScheduleEntryBotIneligibility> scheduleEntryBotIneligibilities = await _unitOfWork.ScheduleEntryBotIneligibilityRepository.GetScheduleEntryBotIneligibilitiesByBotIdentifiers(createEntityScheduleDTO.EntityId, createEntityScheduleDTO.StartDate, createEntityScheduleDTO.EndDate, botMembers.Select(i => _generalService.ParseStringToGuid(i.WorkerId)).ToList());
                scheduleEntryIneligibilityModels.AddRange(_mapper.Map<List<ScheduleEntryIneligibilityModel>>(scheduleEntryBotIneligibilities));
            }

            if (workerMembers.Count() != 0)
            {
                List<ScheduleEntryWorkerIneligibility> scheduleEntryWorkersIneligibilities = await _unitOfWork.ScheduleEntryWorkerIneligibilityRepository.GetScheduleEntryWorkerIneligibilitiesByWorkerIdentifiers(createEntityScheduleDTO.EntityId, createEntityScheduleDTO.StartDate, createEntityScheduleDTO.EndDate, workerMembers.Select(i => i.WorkerId).ToList());
                scheduleEntryIneligibilityModels.AddRange(_mapper.Map<List<ScheduleEntryIneligibilityModel>>(scheduleEntryWorkersIneligibilities));
            }

            //Iterate over all schedule entries to assign workers
            for (int i = 0; i < scheduleEntryDTOs.Count; i++)
            {
                ScheduleEntryDTO scheduleEntry = scheduleEntryDTOs[i];

                if (scheduleEntry.ScheduleParticipants.Count != 0)
                {
                    foreach (ScheduleEntryParticipantDTO scheduleEntryParticipant in scheduleEntry.ScheduleParticipants)
                    {
                        scheduleEntryIneligibilityModels = ValidatePostSelectionIneligibilities(scheduleEntryParticipant.Worker, scheduleEntry, scheduleEntryDTOs, scheduleEntryIneligibilityModels, shifts, entityShiftRotationDTOs, entityRules);
                    }
                }

                // 2.1 Determine minimum skillset required
                List<Tuple<int, int>> requiredSkillQuantities = GetMinimumSkilletSetPerShift(entityRules, scheduleEntry);

                // Fallback: if no skills are defined, allow any eligible worker
                if (requiredSkillQuantities == null || !requiredSkillQuantities.Any() || createEntityScheduleDTO.ForceNoSkill)
                {
                    var maxPerShift = ReturnMaxWorkersPerShift(entityRules, scheduleEntry);
                    requiredSkillQuantities = new List<Tuple<int, int>> { new Tuple<int, int>(-1, maxPerShift) };
                }

                // 2.2 Check if this shift is in rotation
                bool isRotation = entityShiftRotationDTOs.Any(rot => rot.ShiftId == scheduleEntry.ShiftId);

                // 2.3 Filter eligible workers based on rules and rotation
                var eligibleWorkers = FilterEligibleWorkers(
                    scheduleEntry,
                    entityRules,
                    entityWorkerMemberDTOs,
                    isRotation,
                    scheduleEntryIneligibilityModels,
                    createEntityScheduleDTO.ForceNoSkill);

                // 2.4 Get related max rules
                var maxPerShiftFinal = ReturnMaxWorkersPerShift(entityRules, scheduleEntry);
                var maxDailyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MAX_HOURS_DAY_ID);
                var maxWeeklyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MAX_HOURS_WEEK_ID);
                var postShiftRestRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.POST_SHIFT_REST_HOURS_ID);
                // var avgHoursWeeklyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.AVG_HOURS_WEEK_ID);
                // var avgHoursMonthlyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.AVG_HOURS_MONTH_ID); 
                var consecutiveNonRotationTurnsRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MAX_CONSECUTIVE_DAYS_NON_ROTATIONERS_ID);

                // 2.5 Get daily and weekly entries
                var dailyEntries = scheduleEntryDTOs.Where(e => e.ScheduleStartDate.Date == scheduleEntry.ScheduleStartDate.Date);
                var weeklyEntries = await FilterWeeklySessions(scheduleEntry, scheduleEntryDTOs, createEntityScheduleDTO);
                var monthEntries = await FilterMonthlyEntries(scheduleEntry, scheduleEntryDTOs, createEntityScheduleDTO);

                // 2.6 Get previous shift entries for rest check
                var previousShiftEntries = scheduleEntryDTOs
                    .Where(e => e.ScheduleStartDate < scheduleEntry.ScheduleStartDate)
                    .OrderByDescending(e => e.ScheduleStartDate);

                
                // Instantiate Participants
                var assigned = new List<ScheduleEntryParticipantDTO>();

                // If it already exist participants, store them in the participant list
                if (scheduleEntry.ScheduleParticipants.Count != 0)
                    assigned = scheduleEntry.ScheduleParticipants;

                // Order requirements: specific skills first (scarce to abundant), generic (-1) last
                var orderedRequirements = requiredSkillQuantities
                    .Where(r => r.Item1 != -1)
                    .OrderBy(r => eligibleWorkers.Count(w => w.SkillSet.Any(s => s.SkillId == r.Item1)))
                    .Concat(requiredSkillQuantities.Where(r => r.Item1 == -1))
                    .ToList();

                foreach (var skillRequirement in orderedRequirements)
                {
                    int skillId = skillRequirement.Item1;
                    int quantity = skillRequirement.Item2;

                    // If skillId == -1, accept any eligible worker
                    var skillEligibleWorkers = (skillId == -1)
                        ? eligibleWorkers.ToList()
                        : eligibleWorkers.Where(w => w.SkillSet.Any(s => s.SkillId == skillId)).ToList();

                    int skillAssigned = 0;

                    // If there are already participants for this entry
                    if (scheduleEntry.ScheduleParticipants.Count != 0)
                    {
                        // Check current count of skills assigned
                        skillAssigned = (skillId == -1)
                             ? scheduleEntry.ScheduleParticipants.Count
                             : scheduleEntry.ScheduleParticipants.Count(p => p.AssignedSkills.Any(s => s.SkillId == skillId));

                    }

                    // If the number of skill assignments, reached its required quantity, skip to the next skill
                    if(quantity != 0 && skillAssigned >= quantity)
                        continue;

                    foreach (var worker in skillEligibleWorkers)
                    {
                        bool passCheck = PassesAllChecks(worker, assigned.Count, scheduleEntry, dailyEntries.ToList(), weeklyEntries, monthEntries, previousShiftEntries.ToList(), scheduleEntryDTOs, shifts, maxPerShiftFinal, maxDailyRule, maxWeeklyRule, postShiftRestRule, consecutiveNonRotationTurnsRule);

                        if (!passCheck)
                            continue;

                        assigned.Add(CreateScheduleEntryParticipation(createEntityScheduleDTO, worker, entitySkills, skillId));
                        
                        skillAssigned++;
                        eligibleWorkers.Remove(worker); // Remove to prevent duplication

                        scheduleEntryIneligibilityModels = ApplySpecificInegibilities(new List<ScheduleEntryDTO> { scheduleEntry }, worker, scheduleEntryIneligibilityModels, ScheduleGeneratorRelatedMessages.WorkerAssignedToThisEntry);


                        scheduleEntryIneligibilityModels = ValidatePostSelectionIneligibilities(worker, scheduleEntry, scheduleEntryDTOs, scheduleEntryIneligibilityModels, shifts, entityShiftRotationDTOs, entityRules);

                        if (skillId != -1 && quantity != 0 && skillAssigned >= quantity)
                            break;

                    }

                    if (quantity != 0 && skillAssigned >= quantity)
                        continue;
                }

                scheduleEntry.ScheduleParticipants = scheduleEntry.ScheduleParticipants.Concat(assigned).ToList();

            }

            await AddOrUpdateScheduleIneligibilities(scheduleEntryIneligibilityModels);

            var minWeeklyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MIN_HOURS_WEEK_ID);
            var minMonthlyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MIN_HOURS_MONTH_ID);

            if (minWeeklyRule != null)
            {
                List<WorkerUnderhoursSchedule> workerUnderhoursSchedules = await ValidateMinimumHours(scheduleEntryDTOs, entityWorkerMemberDTOs, minWeeklyRule, ScheduleInterval.Weekly);
                if (workerUnderhoursSchedules.Count != 0)
                {
                    Tuple<List<ScheduleEntryDTO>, List<ScheduleEntryIneligibilityModel>> minimumResults = await FillOutForMinimumRequirements(workerUnderhoursSchedules, scheduleEntryDTOs, entitySkills, shifts, entityRules, entityShiftRotationDTOs, scheduleEntryIneligibilityModels, createEntityScheduleDTO, minWeeklyRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue);
                    scheduleEntryDTOs = minimumResults.Item1;
                    scheduleEntryIneligibilityModels = minimumResults.Item2;
                }
            }

            if(minMonthlyRule != null)
            {
                List<WorkerUnderhoursSchedule> workerUnderhoursSchedules = await ValidateMinimumHours(scheduleEntryDTOs, entityWorkerMemberDTOs, minMonthlyRule, ScheduleInterval.Monthly);
                if (workerUnderhoursSchedules.Count != 0)
                {
                    Tuple<List<ScheduleEntryDTO>, List<ScheduleEntryIneligibilityModel>> minimumResults = await FillOutForMinimumRequirements(workerUnderhoursSchedules, scheduleEntryDTOs, entitySkills, shifts, entityRules, entityShiftRotationDTOs, scheduleEntryIneligibilityModels, createEntityScheduleDTO, minWeeklyRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue);
                    scheduleEntryDTOs = minimumResults.Item1;
                    scheduleEntryIneligibilityModels = minimumResults.Item2;
                }
            }

            return scheduleEntryDTOs;
        }

        #endregion

        #region Fill Out For Minimum Requirements

        private async Task<Tuple<List<ScheduleEntryDTO>, List<ScheduleEntryIneligibilityModel>>> FillOutForMinimumRequirements(
            List<WorkerUnderhoursSchedule> workerUnderhoursSchedules,
            List<ScheduleEntryDTO> scheduleEntryDTOs,
            List<SkillLocalizedDTO> entitySkills,
            List<ShiftDTO> shifts,
            List<EntityRuleDTO> entityRules,
            List<EntityShiftRotationDTO> entityShiftRotationDTOs,
            List<ScheduleEntryIneligibilityModel> scheduleEntryIneligibilities,
            CreateEntityScheduleDTO createEntityScheduleDTO,
            double minHours)
        {
            foreach (var underhours in workerUnderhoursSchedules)
            {
                var worker = underhours.Worker;
                double hoursAssigned = 0;

                // Order entries by desirability (best shifts first)
                var entries = underhours.ToBeAssignedEntries
                    .Select(e => new
                    {
                        Entry = e,
                        Score = ComputeShiftScore(e, scheduleEntryDTOs, entityRules)
                    })
                    .OrderByDescending(x => x.Score)
                    .Select(x => x.Entry)
                    .ToList();

                foreach (var entry in entries)
                {
                    if (hoursAssigned >= minHours)
                        break;

                    int maxPerShift = ReturnMaxWorkersPerShift(entityRules, entry);
                    bool isRotation = entityShiftRotationDTOs.Any(rot => rot.ShiftId == entry.ShiftId);

                    // Eligibility check
                    var eligible = FilterEligibleWorkers(
                        entry,
                        entityRules,
                        new List<EntityWorkerMemberDTO> { worker },
                        isRotation,
                        scheduleEntryIneligibilities,
                        createEntityScheduleDTO.ForceNoSkill);

                    if (eligible.Count != 1)
                        continue;

                    // Required skills for this shift (local ratio)
                    List<Tuple<int, int>> requiredSkillQuantities =
                        GetMinimumSkilletSetPerShift(entityRules, entry);

                    bool hasSkillRules = requiredSkillQuantities != null && requiredSkillQuantities.Any();

                    // If no required skills: use global balancing
                    int skillToAssign = hasSkillRules
                        ? DetermineSkillByRatioForEntry(entry, requiredSkillQuantities)
                        : GetGloballyBalancedSkill(worker, scheduleEntryDTOs);

                    // Worker must have chosen skill (if required)
                    if (skillToAssign != -1 &&
                        !worker.SkillSet.Any(s => s.SkillId == skillToAssign))
                        continue;

                    // Hard rule checks (rest, max day, max week, etc.)
                    if (!PassesAllChecksForMinimumHours(worker, entry, scheduleEntryDTOs, shifts, entityRules))
                        continue;

                    // Assign
                    entry.ScheduleParticipants.Add(
                        CreateScheduleEntryParticipation(createEntityScheduleDTO, worker, entitySkills, skillToAssign));

                    // Track hours
                    double shiftHours = CalculateShiftHours(entry, shifts);
                    hoursAssigned += shiftHours;

                    scheduleEntryIneligibilities = ApplySpecificInegibilities(new List<ScheduleEntryDTO> { entry }, worker, scheduleEntryIneligibilities, ScheduleGeneratorRelatedMessages.WorkerAssignedToThisEntry);


                    scheduleEntryIneligibilities = ValidatePostSelectionIneligibilities(worker, entry, scheduleEntryDTOs, scheduleEntryIneligibilities, shifts, entityShiftRotationDTOs, entityRules);

                }
            }

            return new Tuple<List<ScheduleEntryDTO>, List<ScheduleEntryIneligibilityModel>>( scheduleEntryDTOs, scheduleEntryIneligibilities);
        }



        #endregion

        #region AUX: Calculate Shift Hours

        private double CalculateShiftHours(ScheduleEntryDTO entry, List<ShiftDTO> shifts)
        {
            ShiftDTO entryShift = shifts.Where(i => i.ShiftId.Equals(entry.ShiftId)).FirstOrDefault();
            if (entryShift != null)
                return entryShift.ShiftDuration.TotalHours;
            else
                return 0;
        }

        #endregion

        #region AUX: Compute Shift Score

        private int ComputeShiftScore(
            ScheduleEntryDTO entry,
            List<ScheduleEntryDTO> allEntries,
            List<EntityRuleDTO> rules)
        {
            int score = 0;

            // 1. Fewer assigned workers → higher priority
            score += (20 - entry.ScheduleParticipants.Count);

            // 2. Shift urgency: how far it is from being full
            int maxPerShift = ReturnMaxWorkersPerShift(rules, entry);
            score += (maxPerShift - entry.ScheduleParticipants.Count) * 3;

            // 3. Busy shifts get higher priority (mornings > evenings)
            if (entry.ScheduleStartDate.Hour < 12) score += 3;

            // 4. Shortage of required skills increases priority
            var requiredSkills = GetMinimumSkilletSetPerShift(rules, entry);
            if (requiredSkills != null)
            {
                foreach (var req in requiredSkills)
                {
                    int skillId = req.Item1;
                    int required = req.Item2;
                    int current = entry.ScheduleParticipants
                        .Count(p => p.AssignedSkills.Any(s => s.SkillId == skillId));

                    if (current < required)
                        score += 5;
                }
            }

            return score;
        }

        #endregion

        #region AUX: Determine Skill By Ratio For Entry

        private int DetermineSkillByRatioForEntry(
            ScheduleEntryDTO entry,
            List<Tuple<int, int>> requiredSkillQuantities)
        {
            // Count existing assignments per skill
            var current = entry.ScheduleParticipants
                .SelectMany(p => p.AssignedSkills)
                .GroupBy(s => s.SkillId)
                .ToDictionary(g => g.Key, g => g.Count());

            // Choose skill where (current / required) is smallest
            double bestRatio = double.MaxValue;
            int chosenSkill = -1;

            foreach (var req in requiredSkillQuantities)
            {
                int skillId = req.Item1;
                int required = req.Item2;

                current.TryGetValue(skillId, out int assigned);

                double ratio = (double)(assigned + 1) / required;

                if (ratio < bestRatio)
                {
                    bestRatio = ratio;
                    chosenSkill = skillId;
                }
            }

            return chosenSkill;
        }

        #endregion

        #region AUX: Get Globally Balanced Skill

        private int GetGloballyBalancedSkill(
            EntityWorkerMemberDTO worker,
            List<ScheduleEntryDTO> allEntries)
        {
            // Count total assignments per skill
            var totals = allEntries
                .SelectMany(e => e.ScheduleParticipants)
                .SelectMany(p => p.AssignedSkills)
                .GroupBy(s => s.SkillId)
                .ToDictionary(g => g.Key, g => g.Count());

            // Choose the skill the worker has that is LEAST assigned globally
            int bestSkill = -1;
            int minAssigned = int.MaxValue;

            foreach (var ws in worker.SkillSet)
            {
                int assigned = totals.ContainsKey(ws.SkillId) ? totals[ws.SkillId] : 0;

                if (assigned < minAssigned)
                {
                    minAssigned = assigned;
                    bestSkill = ws.SkillId;
                }
            }

            return bestSkill;
        }

        #endregion

        #region CORE: Passes All Checks

        private bool PassesAllChecks(EntityWorkerMemberDTO worker, 
            int assignedCount,
            ScheduleEntryDTO scheduleEntry,
            List<ScheduleEntryDTO> dailyEntries,
            List<ScheduleEntryDTO> weeklyEntries,
            List<ScheduleEntryDTO> monthEntries,
            List<ScheduleEntryDTO> previousShiftEntries,
            List<ScheduleEntryDTO> scheduleEntryDTOs,
            List<ShiftDTO> shifts,
            int maxPerShiftFinal,
            EntityRuleDTO maxDailyRule,
            EntityRuleDTO maxWeeklyRule,
            EntityRuleDTO postShiftRestRule,
            // EntityRuleDTO avgHoursWeeklyRule,
            // EntityRuleDTO avgHoursMonthlyRule,
            EntityRuleDTO consecutiveNonRotationTurnsRule
            )
        {
            bool result = false;

            if (maxPerShiftFinal != 0 && assignedCount >= maxPerShiftFinal) return result;
            if (scheduleEntry.ScheduleParticipants.Any(p => p.Worker.WorkerId == worker.WorkerId)) return result; 
            if (maxDailyRule != null && !CheckMaxHoursPerDay(scheduleEntry, dailyEntries, worker, maxDailyRule, shifts)) return result; // Check maximum amount of hours per day
            if (maxWeeklyRule != null && !CheckMaxHoursPerWeek(weeklyEntries, worker, maxWeeklyRule, shifts)) return result;
            // if (avgHoursWeeklyRule != null && !CheckAverageHoursPerWeek(weeklyEntries, worker, avgHoursWeeklyRule, shifts)) return result ;
            //if (avgHoursMonthlyRule != null && !CheckAverageHoursPerMonth(monthEntries, worker, avgHoursMonthlyRule, shifts)) return result;
            if (postShiftRestRule != null && !CheckForPostShiftRest(scheduleEntry, previousShiftEntries, worker, postShiftRestRule)) return result;
            if (scheduleEntryDTOs.Any(i => i.ScheduleStartDate.Date.Equals(scheduleEntry.ScheduleStartDate.Date)
                && i.ScheduleEntryId != scheduleEntry.ScheduleEntryId
                && !worker.MultipleShiftAssignments
                && i.ScheduleParticipants.Any(j => j.Worker.WorkerId.Equals(worker.WorkerId)))) return result;

            if (consecutiveNonRotationTurnsRule != null && !worker.PartOfRotation && !CheckConsecutiveTurns(scheduleEntryDTOs, scheduleEntry, worker, consecutiveNonRotationTurnsRule))  // Check for consecutive turns for non rotationers
                return result;

            result = true;

            return result;
        }

        #endregion

        #region CORE: Passes All Checks For Minimum Hours

        private bool PassesAllChecksForMinimumHours(
            EntityWorkerMemberDTO worker,
            ScheduleEntryDTO entry,
            List<ScheduleEntryDTO> allEntries,
            List<ShiftDTO> shifts,
            List<EntityRuleDTO> rules)
        {
            var maxDailyRule = rules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MAX_HOURS_DAY_ID);
            var maxWeeklyRule = rules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MAX_HOURS_WEEK_ID);
            var postShiftRestRule = rules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.POST_SHIFT_REST_HOURS_ID);
            var consecutiveNonRotationRule = rules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MAX_CONSECUTIVE_DAYS_NON_ROTATIONERS_ID);

            var dailyEntries = allEntries.Where(e => e.ScheduleStartDate.Date == entry.ScheduleStartDate.Date);
            var weeklyEntries = allEntries.Where(e =>
                e.ScheduleStartDate >= entry.ScheduleStartDate.AddDays(-6) &&
                e.ScheduleStartDate <= entry.ScheduleStartDate);

            var monthEntries = allEntries.Where(e =>
                e.ScheduleStartDate.Month == entry.ScheduleStartDate.Month);

            var previousEntries = allEntries
                .Where(e => e.ScheduleStartDate < entry.ScheduleStartDate)
                .OrderByDescending(e => e.ScheduleStartDate);

            return PassesAllChecks(
                worker,
                entry.ScheduleParticipants.Count,
                entry,
                dailyEntries.ToList(),
                weeklyEntries.ToList(),
                monthEntries.ToList(),
                previousEntries.ToList(),
                allEntries,
                shifts,
                ReturnMaxWorkersPerShift(rules, entry),
                maxDailyRule,
                maxWeeklyRule,
                postShiftRestRule,
                consecutiveNonRotationRule);
        }

        #endregion

        #region AUX : Create Schedule Entry Participation

        private ScheduleEntryParticipantDTO CreateScheduleEntryParticipation(CreateEntityScheduleDTO createEntityScheduleDTO, EntityWorkerMemberDTO worker, List<SkillLocalizedDTO> entitySkills, int skillId)
        {
            // If user is to perform a single responsibility and there is a skill identifier
            if (createEntityScheduleDTO.SingleRoleResponsibility && skillId != -1)
            {
                // Assign worker
                return new ScheduleEntryParticipantDTO
                {
                    Worker = worker,
                    AssignedSkills = new List<SkillLocalizedDTO> { entitySkills.First(i => i.SkillId.Equals(skillId)) }
                };
            }

            else
            {
                return new ScheduleEntryParticipantDTO
                {
                    Worker = worker,
                    AssignedSkills = worker.SkillSet
                };
            }
        }

        #endregion

        #region CORE: Add Or Update Schedule Ineligibilities

        private async Task AddOrUpdateScheduleIneligibilities(List<ScheduleEntryIneligibilityModel> scheduleEntryIneligibilities)
        {
            List<ScheduleEntryBotIneligibility> scheduleEntryBotIneligibilities = new List<ScheduleEntryBotIneligibility>();
            List<ScheduleEntryWorkerIneligibility> scheduleEntryWorkerIneligibilities = new List<ScheduleEntryWorkerIneligibility>();

            try
            {
                // For each entry, 
                foreach (ScheduleEntryIneligibilityModel scheduleEntryIneligibility in scheduleEntryIneligibilities)
                {
                    // Check and skip those that already exist
                    bool alreadyExists = false;
                    if (scheduleEntryIneligibility.IsBot)
                    {
                        alreadyExists = await _unitOfWork.ScheduleEntryBotIneligibilityRepository.DoesBotScheduleIneligibilityExist(scheduleEntryIneligibility.ScheduleEntryId, _generalService.ParseStringToGuid(scheduleEntryIneligibility.WorkerId));

                        if (alreadyExists)
                            continue;

                        ScheduleEntryBotIneligibility scheduleEntryBotIneligibility = _mapper.Map<ScheduleEntryBotIneligibility>(scheduleEntryIneligibility);
                        scheduleEntryBotIneligibility.UserBotId = _generalService.ParseStringToGuid(scheduleEntryIneligibility.WorkerId);
                        scheduleEntryBotIneligibilities.Add(scheduleEntryBotIneligibility);
                    }
                    else
                    {
                        alreadyExists = await _unitOfWork.ScheduleEntryWorkerIneligibilityRepository.DoesWorkerScheduleIneligibilityExist(scheduleEntryIneligibility.ScheduleEntryId, scheduleEntryIneligibility.WorkerId);

                        if (alreadyExists)
                            continue;

                        ScheduleEntryWorkerIneligibility scheduleEntryWorkerIneligibility = _mapper.Map<ScheduleEntryWorkerIneligibility>(scheduleEntryIneligibility);
                        scheduleEntryWorkerIneligibilities.Add(scheduleEntryWorkerIneligibility);
                    }
                }

                scheduleEntryBotIneligibilities = scheduleEntryBotIneligibilities
                    .GroupBy(x => new { x.ScheduleEntryId, x.UserBotId })
                    .Select(g => g.First())
                    .ToList();

                scheduleEntryWorkerIneligibilities = scheduleEntryWorkerIneligibilities
                    .GroupBy(x => new { x.ScheduleEntryId, x.ApplicationUserId })
                    .Select(g => g.First())
                    .ToList();


                // Add the new entries
                await _unitOfWork.ScheduleEntryBotIneligibilityRepository.AddRange(scheduleEntryBotIneligibilities);
                await _unitOfWork.ScheduleEntryWorkerIneligibilityRepository.AddRange(scheduleEntryWorkerIneligibilities);
            }
            catch (Exception ex)
            {
                string strErr = ex.Message;
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region CORE: Validate Post Selection Ineligibilities

        private List<ScheduleEntryIneligibilityModel> ValidatePostSelectionIneligibilities(EntityWorkerMemberDTO worker,
            ScheduleEntryDTO scheduleEntry,
            List<ScheduleEntryDTO> scheduleEntryDTOs,
            List<ScheduleEntryIneligibilityModel> scheduleEntryIneligibilities,
            List<ShiftDTO> shifts,
            List<EntityShiftRotationDTO> entityShiftRotationDTOs,
            List<EntityRuleDTO> entityRules
            )
        {
            bool isRotation = entityShiftRotationDTOs.Any(i => i.ShiftId.Equals(scheduleEntry.ShiftId));
            EntityRuleDTO maxDailyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MAX_HOURS_DAY_ID);
            List<ScheduleEntryDTO> dailyEntries = scheduleEntryDTOs.Where(i => i.ScheduleStartDate.Date == scheduleEntry.ScheduleStartDate.Date).ToList();

            // If a worker can only be assigned one shift a day
            if (!worker.MultipleShiftAssignments)
            {
                // Get Other Entries of the day
                IEnumerable<ScheduleEntryDTO> currentDayEntries = scheduleEntryDTOs.Where(i => i.ScheduleStartDate.Date.Equals(scheduleEntry.ScheduleStartDate.Date) && i.ScheduleEntryId != scheduleEntry.ScheduleEntryId);

                // Apply specific ineligibilities to the current day entries
                scheduleEntryIneligibilities = ApplySpecificInegibilities(currentDayEntries, worker, scheduleEntryIneligibilities, ScheduleGeneratorRelatedMessages.OneShiftPerDayObs);
            }

            string observations = string.Empty;

            // Check if the worker is part of a rotation
            if (isRotation && worker.PartOfRotation)
            {
                EntityShiftRotationDTO currentShiftRotationStage = entityShiftRotationDTOs.Where(i => i.ShiftId.Equals(scheduleEntry.ShiftId)).FirstOrDefault();

                EntityShiftRotationDTO nextStepOfRotation = entityShiftRotationDTOs.Where(i => i.OrderNo == currentShiftRotationStage.OrderNo + 1).FirstOrDefault();

                // If there isnt a next rotation, go back to 1
                if (nextStepOfRotation == null)
                {
                    // Get the first step of rotation
                    nextStepOfRotation = entityShiftRotationDTOs.Where(i => i.OrderNo == 1).FirstOrDefault();
                }


                // Check if the next step of rotation is not a leave
                if (!nextStepOfRotation.IsLeave)
                {
                    List<ScheduleEntryDTO> nextIneligibleEntries = new List<ScheduleEntryDTO>();

                    if (worker.MultipleShiftAssignments)
                    {
                        // Check if the next step of rotation can still be done on the same day
                        ShiftDTO nextRotationShift = shifts.Where(i => i.ShiftId.Equals(nextStepOfRotation.ShiftId)).FirstOrDefault();
                        ScheduleEntryDTO nextRotationEntry = scheduleEntryDTOs.Where(i => i.ScheduleStartDate.Date.Equals(scheduleEntry.ScheduleStartDate.Date) && i.ShiftId.Equals(nextRotationShift.ShiftId)).FirstOrDefault();


                        // 1. Check if current schedule start date is earlier than the next rotation entry of the same day
                        bool canDoSameDay = (nextRotationEntry != null && scheduleEntry.ScheduleStartDate < nextRotationEntry.ScheduleStartDate);

                        if (!canDoSameDay)
                            observations = ScheduleGeneratorRelatedMessages.CannotDoNextRotationObs;

                        bool maxHourAllowed = (maxDailyRule == null || CheckMaxHoursPerDay(nextRotationEntry, dailyEntries, worker, maxDailyRule, shifts));

                        if (!maxHourAllowed)
                            observations = ScheduleGeneratorRelatedMessages.MaxDailyHoursObs;

                        if (canDoSameDay && maxHourAllowed)
                        {
                            nextIneligibleEntries = scheduleEntryDTOs.Where(i => i.ScheduleStartDate.Date.Equals(scheduleEntry.ScheduleStartDate.Date)
                                && !i.ScheduleParticipants.Any(j => j.Worker.WorkerId.Equals(worker.WorkerId))
                                && !i.ShiftId.Equals(nextStepOfRotation.ShiftId)).ToList();

                            observations = ScheduleGeneratorRelatedMessages.ExcludedForNotMatchingNextRotationObs;
                        }

                        // Else, get all entries of the next day and make all ineligible, except for the next rotation entry 
                        else
                        {
                            nextIneligibleEntries = scheduleEntryDTOs.Where(i => i.ScheduleStartDate.Date.Equals(scheduleEntry.ScheduleStartDate.Date.AddDays(1))
                                && !i.ShiftId.Equals(nextStepOfRotation.ShiftId)).ToList();

                            observations = ScheduleGeneratorRelatedMessages.ExcludedForNotMatchingNextRotationObs;
                        }
                    }
                    else
                    {
                        // Get Entries of the next day where its not the same shift
                        nextIneligibleEntries = scheduleEntryDTOs.Where(i => i.ScheduleStartDate.Date.Equals(scheduleEntry.ScheduleStartDate.Date.AddDays(1))
                            && !i.ShiftId.Equals(nextStepOfRotation.ShiftId)).ToList();

                        observations = ScheduleGeneratorRelatedMessages.OneShiftPerDayObs;
                    }

                    scheduleEntryIneligibilities = ApplySpecificInegibilities(nextIneligibleEntries, worker, scheduleEntryIneligibilities, observations);
                }

                // Its a mandatory leave
                else
                {
                    DateTime leaveStart = scheduleEntry.ScheduleEndDate;
                    DateTime leaveEnd = leaveStart.Add(nextStepOfRotation.LeaveDuration);

                    IEnumerable<ScheduleEntryDTO> mandatoryLeaveEntries = scheduleEntryDTOs
                            .Where(i => i.ScheduleStartDate >= scheduleEntry.ScheduleStartDate && i.ScheduleStartDate <= leaveEnd);

                    observations = string.Format(ScheduleGeneratorRelatedMessages.MandatoryRotationLeaveObs, leaveStart.ToString("dd/MM/yyyy"), leaveEnd.ToString("dd/MM/yyyy"));

                    scheduleEntryIneligibilities = ApplySpecificInegibilities(mandatoryLeaveEntries, worker, scheduleEntryIneligibilities, observations);
                }
            }

            // If not part of the rotation
            else if (!worker.PartOfRotation)
            {
                scheduleEntryIneligibilities = CheckAndApplyIneligiblitiesForConsecutiveEntries(entityRules, worker, scheduleEntryDTOs, scheduleEntry, scheduleEntryIneligibilities);
            }

            return scheduleEntryIneligibilities;
        }

        #endregion

        #region AUX: Filter Eligible Workers

        /// <summary>
        /// Filters and returns a list of valid workers in case of a required skillset for a shift
        /// </summary>
        /// <param name="scheduleEntryDTO"></param>
        /// <param name="entityRulesDTO"></param>
        /// <param name="entityWorkerMembers"></param>
        /// <param name="isShiftRotation">Flag that indicates to filter by members who are part of the rotation</param>
        /// <returns></returns>
        /// 
        private List<EntityWorkerMemberDTO> FilterEligibleWorkers(
            ScheduleEntryDTO scheduleEntryDTO,
            List<EntityRuleDTO> entityRulesDTO,
            List<EntityWorkerMemberDTO> entityWorkerMembers,
            bool isShiftRotation,
            List<ScheduleEntryIneligibilityModel> scheduleEntryIneligibilities,
            bool forceNoSkill)
        {
            IEnumerable<EntityWorkerMemberDTO> filteredWorkers = entityWorkerMembers;

            bool isWeekend = IsWeekend(scheduleEntryDTO.ScheduleStartDate);

            // Apply filters step by step
            filteredWorkers = FilterBySkillRequirements(filteredWorkers, entityRulesDTO, scheduleEntryDTO, forceNoSkill);
            filteredWorkers = FilterByRotation(filteredWorkers, scheduleEntryDTO, isShiftRotation);
            filteredWorkers = FilterByAvailability(filteredWorkers, isWeekend);
            filteredWorkers = FilterByIneligibility(filteredWorkers, scheduleEntryDTO, scheduleEntryIneligibilities);
            filteredWorkers = FilterOutAlreadyParticipating(filteredWorkers, scheduleEntryDTO);

            return filteredWorkers.ToList();
        }

        #endregion

        #region AUX : Filter By Skill Requirements

        private IEnumerable<EntityWorkerMemberDTO> FilterBySkillRequirements(
            IEnumerable<EntityWorkerMemberDTO> workers,
            List<EntityRuleDTO> entityRules,
            ScheduleEntryDTO scheduleEntry,
            bool forceNoSkill)
        {
            if (forceNoSkill) return workers;

            var skillRequirements = entityRules
                .Where(r =>
                    (r.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_ID)
                     || r.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKDAYS_ID)
                     || r.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKENDS_ID))
                    && r.EntityRuleSpecificationDTOs.Any(s => s.AspectReferenceId2.Equals(scheduleEntry.ShiftId)))
                .SelectMany(r => r.EntityRuleSpecificationDTOs)
                .Select(s => s.AspectReferenceId)
                .Distinct()
                .ToList();

            if (!skillRequirements.Any())
                return workers;

            // Require workers to have at least one of the required skills
            return workers.Where(w => w.SkillSet.Any(s => skillRequirements.Contains(s.SkillId.ToString())));
        }

        #endregion

        #region AUX : Filter By Rotation

        private IEnumerable<EntityWorkerMemberDTO> FilterByRotation(
            IEnumerable<EntityWorkerMemberDTO> workers,
            ScheduleEntryDTO scheduleEntry,
            bool isShiftRotation)
        {
            if (!isShiftRotation)
            {
                // Exclude workers that are part of rotation
                return workers.Where(w => !w.PartOfRotation);
            }

            // Include only rotation workers or explicitly assigned to this shift
            return workers.Where(w => w.PartOfRotation || w.AssignedShifts.Any(s => s.ShiftId.Equals(scheduleEntry.ShiftId)));
        }

        #endregion

        #region Aux : Filter By Availability

        private IEnumerable<EntityWorkerMemberDTO> FilterByAvailability(
            IEnumerable<EntityWorkerMemberDTO> workers,
            bool isWeekend)
        {
            if (isWeekend)
            {
                return workers.Where(w => w.PartOfRotation || w.WorksWeekends);
            }
            else
            {
                return workers.Where(w => w.PartOfRotation || w.WorksWeekDays);
            }
        }

        #endregion

        #region AUX : Filter By Ineligibility

        private IEnumerable<EntityWorkerMemberDTO> FilterByIneligibility(
            IEnumerable<EntityWorkerMemberDTO> workers,
            ScheduleEntryDTO scheduleEntry,
            List<ScheduleEntryIneligibilityModel> ineligibilities)
        {
            if (ineligibilities == null || !ineligibilities.Any())
                return workers;

            List<EntityWorkerMemberDTO> eligibileWorkers = new List<EntityWorkerMemberDTO>();

            foreach (EntityWorkerMemberDTO workerMemberDTO in workers)
            {
                if (!workerMemberDTO.IsBot)
                {
                    if (!ineligibilities.Any(i => i.ScheduleEntryId.Equals(scheduleEntry.ScheduleEntryId) && i.WorkerId.Equals(workerMemberDTO.WorkerId)))
                        eligibileWorkers.Add(workerMemberDTO);
                }
                else if (workerMemberDTO.IsBot)
                {
                    if (!ineligibilities.Any(i => i.ScheduleEntryId.Equals(scheduleEntry.ScheduleEntryId) && 
                        _generalService.ParseStringToGuid(workerMemberDTO.WorkerId).ToString().Equals(i.WorkerId) ||
                        i.ScheduleEntryId.Equals(scheduleEntry.ScheduleEntryId) && 
                        i.WorkerId.Equals(workerMemberDTO.WorkerId)))
                        eligibileWorkers.Add(workerMemberDTO);
                }
            }

            return eligibileWorkers;

            //var results = workers
            //    .Where(worker => !ineligibilities.Any(i => i.WorkerId.Equals(worker.WorkerId) && i.ScheduleEntryId.Equals(scheduleEntry.ScheduleEntryId)))
            //    .ToList();


            /*
            var entryIneligibilities = ineligibilities
                .Where(i => i.ScheduleEntryId.Equals(scheduleEntry.ScheduleEntryId))
                .ToList();

            var results = workers.Where(worker =>
            {
                if (worker.IsBot)
                {
                    var workerGuid = _generalService.ParseStringToGuid(worker.WorkerId);
                    return !entryIneligibilities.Any(ine => ine.IsBot && ine.WorkerId.Equals(workerGuid.ToString()));
                }
                else
                {
                    return !entryIneligibilities.Any(ine => !ine.IsBot && ine.WorkerId.Equals(worker.WorkerId));
                }
            });

            */
            //return results;
        }

        private IEnumerable<EntityWorkerMemberDTO> _FilterByIneligibility(
            IEnumerable<EntityWorkerMemberDTO> workers,
            ScheduleEntryDTO scheduleEntry,
            List<ScheduleEntryIneligibilityDTOv1> ineligibilities)
        {
            if (ineligibilities == null || !ineligibilities.Any())
                return workers;

            return workers.Where(worker =>
                !ineligibilities.Any(ine =>
                    ine.ScheduleEntryID.Equals(scheduleEntry.ScheduleEntryId) &&
                    ine.MembersIneligible.Any(m => m.WorkerId.Equals(worker.WorkerId))));
        }

        #endregion

        #region AUX : Filter Out Already Participating

        private IEnumerable<EntityWorkerMemberDTO> FilterOutAlreadyParticipating(
            IEnumerable<EntityWorkerMemberDTO> workers,
            ScheduleEntryDTO scheduleEntry)
        {
            if (scheduleEntry.ScheduleParticipants == null || !scheduleEntry.ScheduleParticipants.Any())
                return workers;

            var alreadyParticipatingIds = scheduleEntry.ScheduleParticipants
                .Select(p => p.Worker.WorkerId)
                .ToHashSet();

            return workers.Where(w => !alreadyParticipatingIds.Contains(w.WorkerId));
        }

        #endregion

        #region AUX: Check And Apply Ineligiblities For Consecutive Entries

        private List<ScheduleEntryIneligibilityModel> CheckAndApplyIneligiblitiesForConsecutiveEntries(
            List<EntityRuleDTO> entityRules,
            EntityWorkerMemberDTO worker,
            List<ScheduleEntryDTO> scheduleEntryDTOs,
            ScheduleEntryDTO scheduleEntry,
            List<ScheduleEntryIneligibilityModel> scheduleEntryIneligibilities)
        {
            try
            {
                // Check if there is a consecutive turns rule
                EntityRuleDTO consecutiveTurnsRule = entityRules
                    .FirstOrDefault(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_CONSECUTIVE_DAYS_NON_ROTATIONERS_ID));

                if (consecutiveTurnsRule == null)
                    return scheduleEntryIneligibilities;

                int maxConsecutiveDays = (int)consecutiveTurnsRule.EntityRuleSpecificationDTOs[0].RuleSpecificationValue;

                // Get worker's scheduled entries (unique by date)
                var workerDates = scheduleEntryDTOs
                    .Where(e => e.ScheduleParticipants.Any(p => p.Worker.WorkerId == worker.WorkerId))
                    .Select(e => e.ScheduleStartDate.Date)
                    .Distinct()
                    .ToList();

                DateTime currentDate = scheduleEntry.ScheduleStartDate.Date;

                if (!workerDates.Contains(currentDate))
                    workerDates.Add(currentDate);

                // Count consecutive backwards
                int consecutiveCount = 1; // include current day
                DateTime checkDate = currentDate.AddDays(-1);

                while (workerDates.Contains(checkDate))
                {
                    consecutiveCount++;
                    if (consecutiveCount >= maxConsecutiveDays)
                        break;
                    checkDate = checkDate.AddDays(-1);
                }

                // Count consecutive forwards
                checkDate = currentDate.AddDays(1);

                while (workerDates.Contains(checkDate))
                {
                    consecutiveCount++;
                    if (consecutiveCount >= maxConsecutiveDays)
                        break;
                    checkDate = checkDate.AddDays(1);
                }

                // If we reached/exceeded the limit, apply rest-day ineligibilities
                if (consecutiveCount >= maxConsecutiveDays)
                {
                    EntityRuleDTO weekDaysOffRule = entityRules
                        .FirstOrDefault(i => i.RuleTypeId.Equals(RuleTypeConstants.MIN_DAYS_OFF_WEEK_ID));

                    DateTime dateStart, dateEnd;

                    if (weekDaysOffRule != null)
                    {
                        int weekDaysRestAmount = (int)weekDaysOffRule.EntityRuleSpecificationDTOs[0].RuleSpecificationValue;

                        if (worker.MultipleShiftAssignments)
                        {
                            dateStart = currentDate.AddDays(1);
                            dateEnd = dateStart.AddDays(weekDaysRestAmount).AddSeconds(-1);
                        }
                        else
                        {
                            dateStart = scheduleEntry.ScheduleEndDate;
                            dateEnd = dateStart.AddDays(weekDaysRestAmount);
                        }
                    }
                    else
                    {
                        if (worker.MultipleShiftAssignments)
                        {
                            dateStart = currentDate.AddDays(1);
                            dateEnd = dateStart.AddDays(1).AddSeconds(-1);
                        }
                        else
                        {
                            dateStart = scheduleEntry.ScheduleEndDate;
                            dateEnd = dateStart.AddDays(1);
                        }
                    }

                    // Apply ineligibilities for mandatory rest period
                    var mandatoryLeaveEntries = scheduleEntryDTOs
                        .Where(i => i.ScheduleStartDate.Date >= dateStart.Date && i.ScheduleStartDate.Date <= dateEnd.Date);

                    scheduleEntryIneligibilities = ApplySpecificInegibilities(
                        mandatoryLeaveEntries,
                        worker,
                        scheduleEntryIneligibilities,
                        ScheduleGeneratorRelatedMessages.MandatoryRestDaysObs
                    );
                }
            }
            catch (Exception ex)
            {
                string strErr = ex.Message;
            }

            return scheduleEntryIneligibilities;
        }

        #endregion

        #region AUX: Apply Specific Inegibilities

        private List<ScheduleEntryIneligibilityModel> ApplySpecificInegibilities(IEnumerable<ScheduleEntryDTO> entriesToDenyWorker, EntityWorkerMemberDTO worker, List<ScheduleEntryIneligibilityModel> scheduleEntrysIneligibilities, string observations)
        {
            foreach (ScheduleEntryDTO scheduleEntry in entriesToDenyWorker)
            {
                // Check for an existing entry
                ScheduleEntryIneligibilityModel existingIneligibility =
                    scheduleEntrysIneligibilities.Where(i => i.ScheduleEntryId.Equals(scheduleEntry.ScheduleEntryId) &&
                    i.WorkerId.Equals(worker.WorkerId) &&
                    i.IsBot.Equals(worker.IsBot)).FirstOrDefault();

                if (existingIneligibility != null)
                    continue;

                else
                {
                    ScheduleEntryIneligibilityModel newScheduleEntryIneligibility = new ScheduleEntryIneligibilityModel
                    {
                        ScheduleEntryId = scheduleEntry.ScheduleEntryId,
                        DateOfAssessment = DateTime.UtcNow,
                        IneligibilityObservations = observations,
                        WorkerId = worker.WorkerId,
                        IsBot = worker.IsBot
                    };

                    scheduleEntrysIneligibilities.Add(newScheduleEntryIneligibility);
                }
            }

            return scheduleEntrysIneligibilities;
        }

        #endregion

        #region RULE CHECKER : Check Max Hours Per Day

        /// <summary>
        /// Checks if a worker is not allocated beyond the amount of hours
        /// allowed by the defined rules
        /// </summary>
        /// <param name="currentScheduleEntry">Current Schedule Entry</param>
        /// <param name="presentDayEntries">Entries of the day being analyzed</param>
        /// <param name="entityWorkerMemberDTO">Worker Information</param>
        /// <param name="maxDailyHoursRule">Max</param>
        /// <param name="shiftDTOs"></param>
        /// <returns>True if the worker is eligible</returns>
        private bool CheckMaxHoursPerDay(
            ScheduleEntryDTO currentScheduleEntry,
            IEnumerable<ScheduleEntryDTO> presentDayEntries,
            EntityWorkerMemberDTO entityWorkerMemberDTO,
            EntityRuleDTO maxDailyHoursRule,
            List<ShiftDTO> shiftDTOs)
        {
            double maxAllowedHours = maxDailyHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue;
            var timeRanges = new List<(DateTime Start, DateTime End)>();

            // Include present day entries (excluding current)
            foreach (var entry in presentDayEntries)
            {
                if (entry.ScheduleEntryId == currentScheduleEntry.ScheduleEntryId)
                    continue;

                if (entry.ScheduleParticipants.Any(p => p.Worker.WorkerId == entityWorkerMemberDTO.WorkerId))
                {
                    var shift = shiftDTOs.FirstOrDefault(s => s.ShiftId == entry.ShiftId);
                    if (shift != null)
                    {
                        timeRanges.Add((entry.ScheduleStartDate, entry.ScheduleEndDate));
                    }
                }
            }

            // Include current entry
            var currentShift = shiftDTOs.FirstOrDefault(s => s.ShiftId == currentScheduleEntry.ShiftId);
            if (currentShift == null)
                return false;

            // Check if the single shift itself exceeds max
            if ((currentScheduleEntry.ScheduleEndDate - currentScheduleEntry.ScheduleStartDate).TotalHours > maxAllowedHours)
                return false;

            timeRanges.Add((currentScheduleEntry.ScheduleStartDate, currentScheduleEntry.ScheduleEndDate));

            // Merge overlapping intervals
            var merged = MergeTimeRanges(timeRanges);

            TimeSpan totalWorked = TimeSpan.Zero;
            foreach (var range in merged)
            {
                totalWorked += range.End - range.Start;
            }

            return totalWorked.TotalHours <= maxAllowedHours;
        }

        #endregion

        #region RULE CHECKER : Check Max Hours per Week

        private bool CheckMaxHoursPerWeek(IEnumerable<ScheduleEntryDTO> weekEntries, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO maxWeeklyHoursRule, List<ShiftDTO> shiftDTOs)
        {
            TimeSpan totalHours = TimeSpan.Zero;

            // For each week entry
            foreach (ScheduleEntryDTO scheduleEntryDTO in weekEntries)
            {
                if (scheduleEntryDTO.ScheduleParticipants.Any(i => i.Worker.WorkerId.Equals(entityWorkerMemberDTO.WorkerId)))
                {
                    ShiftDTO shiftDTO = shiftDTOs.Where(i => i.ShiftId.Equals(scheduleEntryDTO.ShiftId)).FirstOrDefault();
                    if (shiftDTO != null)
                        totalHours += shiftDTO.ShiftDuration;
                }
            }

            if (totalHours > TimeSpan.FromHours(maxWeeklyHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue))
                return false;

            return true;
        }

        #endregion

        #region RULE CHECKER : Check Consecutive Turns

        private bool CheckConsecutiveTurns(List<ScheduleEntryDTO> scheduleEntries, ScheduleEntryDTO currentEntry, EntityWorkerMemberDTO worker, EntityRuleDTO consecutiveShiftsRule)
        {
            if (consecutiveShiftsRule == null || consecutiveShiftsRule.EntityRuleSpecificationDTOs.Count == 0)
                return true;

            int maxConsecutiveDays = (int)consecutiveShiftsRule.EntityRuleSpecificationDTOs[0].RuleSpecificationValue;

            // Step 1: Get all unique dates the worker is scheduled
            var workerDates = scheduleEntries
                .Where(e => e.ScheduleParticipants.Any(p => p.Worker.WorkerId == worker.WorkerId))
                .Select(e => e.ScheduleStartDate.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            // Ensure current date is considered
            if (!workerDates.Contains(currentEntry.ScheduleStartDate.Date))
            {
                workerDates.Add(currentEntry.ScheduleStartDate.Date);
                workerDates = workerDates.OrderBy(d => d).ToList();
            }

            DateTime currentDate = currentEntry.ScheduleStartDate.Date;

            // Step 2: Count backwards
            int consecutiveCount = 1; // start with current date
            DateTime prevDate = currentDate.AddDays(-1);

            while (workerDates.Contains(prevDate))
            {
                consecutiveCount++;
                if (consecutiveCount > maxConsecutiveDays)
                    return false;
                prevDate = prevDate.AddDays(-1);
            }

            // Step 3: Count forwards
            DateTime nextDate = currentDate.AddDays(1);

            while (workerDates.Contains(nextDate))
            {
                consecutiveCount++;
                if (consecutiveCount > maxConsecutiveDays)
                    return false;
                nextDate = nextDate.AddDays(1);
            }

            return true;
        }

        #endregion

        #region RULE CHECKER: Check For Post Shift Rest

        /// <summary>
        /// Checks if worker is valid for this entry in case of post shift rest
        /// </summary>
        /// <param name="currentScheduleEntry"></param>
        /// <param name="previousShiftEntries"></param>
        /// <param name="entityWorkerMemberDTO"></param>
        /// <param name="postShiftRestRule"></param>
        /// <returns></returns>
        private bool CheckForPostShiftRest(ScheduleEntryDTO currentScheduleEntry, IEnumerable<ScheduleEntryDTO> previousShiftEntries, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO? postShiftRestRule)
        {
            if (postShiftRestRule != null)
            {
                // For each shift post rest rule specification 
                foreach (EntityRuleSpecificationDTO entityRuleSpecificationDTO in postShiftRestRule.EntityRuleSpecificationDTOs)
                {
                    if (entityRuleSpecificationDTO.BusinessAspectId != null && entityRuleSpecificationDTO.RuleSpecificationValue != null)
                    {
                        // Check entries where the worker is part of shifts before the current entry
                        ScheduleEntryDTO lastPreCurrentScheduleEntry = previousShiftEntries.Where(i => i.ShiftId.ToString().Equals(entityRuleSpecificationDTO.AspectReferenceId) && 
                        i.ScheduleStartDate < currentScheduleEntry.ScheduleStartDate && 
                        i.ScheduleParticipants.Any(j => j.Worker.WorkerId.Equals(entityWorkerMemberDTO.WorkerId))).OrderByDescending(i => i.ScheduleStartDate).FirstOrDefault();

                        if (lastPreCurrentScheduleEntry != null)
                        {
                            // Check if from the end of the schedule entry, plus the time of rest, if it overlaps the current entry, if so it cannot happen
                            if (lastPreCurrentScheduleEntry.ScheduleEndDate + TimeSpan.FromHours(entityRuleSpecificationDTO.RuleSpecificationValue) >= currentScheduleEntry.ScheduleStartDate)
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        #endregion

        #region RULE CHECKER: Get Minimum Skill Set Per Shift

        private List<Tuple<int, int>> GetMinimumSkilletSetPerShift(List<EntityRuleDTO> entityRuleDTOs, ScheduleEntryDTO scheduleEntryDTO)
        {
            List<Tuple<int, int>> minSkillQuantity = new List<Tuple<int, int>>();

            // Check if its a weekend entry
            bool isWeekend = scheduleEntryDTO.ScheduleStartDate.Equals(DayOfWeek.Saturday) || scheduleEntryDTO.ScheduleStartDate.Equals(DayOfWeek.Sunday);

            EntityRuleDTO entityRuleDTO = null;

            // if there is any rule for the weekdays quantity of skillset regarding this shift
            if (entityRuleDTOs.Any(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKDAYS_ID)
                && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId.ToString()))
                && !isWeekend))
            {
                entityRuleDTO = entityRuleDTOs
                    .Where(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKDAYS_ID)
                    && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId.ToString()))).FirstOrDefault();
            }

            // if there is any rule for the weekdays quantity of skillset regarding this shift
            else if (entityRuleDTOs.Any(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKENDS_ID)
                && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId.ToString()))
                && isWeekend))
            {
                entityRuleDTO = entityRuleDTOs
                    .Where(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKENDS_ID)
                    && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId.ToString()))).FirstOrDefault();
            }

            // Standard rule for the quantity of skillset regarding this shift
            else
            {
                entityRuleDTO = entityRuleDTOs
                    .Where(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_ID)
                    && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId.ToString()))).FirstOrDefault();
            }

            // If there is a rule for the quantity of skillset regarding this shift
            if (entityRuleDTO != null)
            {
                // For each specification of the rule
                foreach (EntityRuleSpecificationDTO entityRuleSpecificationDTO in entityRuleDTO.EntityRuleSpecificationDTOs)
                    // If this is the shift we are looking for
                    if (entityRuleSpecificationDTO.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId.ToString()))
                        // Add the skill id and quantity to the list
                        minSkillQuantity.Add(new Tuple<int, int>(int.Parse(entityRuleSpecificationDTO.AspectReferenceId), (int)entityRuleSpecificationDTO.RuleSpecificationValue));
            }

            return minSkillQuantity;
        }

        #endregion

        #region RULE CHECKER: Return Max Workers Per Shift

        /// <summary>
        /// Returns the maximum amount of workers can work on a specific shift
        /// </summary>
        /// <param name="entityRuleDTOs"></param>
        /// <param name="scheduleEntryDTO"></param>
        /// <returns></returns>
        private int ReturnMaxWorkersPerShift(List<EntityRuleDTO> entityRuleDTOs, ScheduleEntryDTO scheduleEntryDTO)
        {
            int maxPerShift = 0;

            EntityRuleDTO entityRuleDTO = entityRuleDTOs
                .Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_WORKERS_SHIFT_ID)
                && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(scheduleEntryDTO.ShiftId.ToString()))).FirstOrDefault();

            if (entityRuleDTO != null)
                maxPerShift = (int)entityRuleDTO.EntityRuleSpecificationDTOs.First().RuleSpecificationValue;

            return maxPerShift;
        }

        #endregion

        #region RULE CHECKER: Validate Minimum Hours

        private async Task<List<WorkerUnderhoursSchedule>> ValidateMinimumHours(List<ScheduleEntryDTO> allEntries,
            List<EntityWorkerMemberDTO> workers,
            EntityRuleDTO rule,
            ScheduleInterval interval)
        {
            List<WorkerUnderhoursSchedule> workersUnderhourSchedules = new List<WorkerUnderhoursSchedule>();
            // Store Minimum hours per week value
            float minRequiredHours = rule.EntityRuleSpecificationDTOs.FirstOrDefault().RuleSpecificationValue;

            // Group entries by week or month
            var grouped = GroupEntriesByInterval(allEntries, interval);

            foreach (var group in grouped)
            {
                foreach (var worker in workers)
                {
                    var timeRanges = new List<(DateTime Start, DateTime End)>();

                    // Get Entries for this interval where this worker participates
                    IEnumerable<ScheduleEntryDTO> workerEntries = group
                        .Where(e => e.ScheduleParticipants.Any(p => p.Worker.WorkerId == worker.WorkerId));

                    if(workerEntries.Count() == 0)
                    {
                        workersUnderhourSchedules.Add(new WorkerUnderhoursSchedule
                        {
                            Worker = worker,
                            ToBeAssignedEntries = group // all entries in period
                        });
                        continue;
                    }

                    // For each entry found, store time ranges
                    foreach (ScheduleEntryDTO entry in workerEntries)
                        timeRanges.Add((entry.ScheduleStartDate, entry.ScheduleEndDate));

                    // Merge overlapping intervals
                    var merged = MergeTimeRanges(timeRanges);

                    // Calculate amount of hours
                    TimeSpan totalWorked = TimeSpan.Zero;
                    foreach (var range in merged)
                    {
                        totalWorked += range.End - range.Start;
                    }

                    // If worker has not filled enough hours
                    if (totalWorked.TotalHours < minRequiredHours)
                    {
                        workersUnderhourSchedules.Add(new WorkerUnderhoursSchedule
                        {
                            ToBeAssignedEntries = group,
                            Worker = worker
                        });
                    }
                }
            }

            return workersUnderhourSchedules;
        }

        #endregion

        #region AUX: Group Entries By Interval

        private IEnumerable<IGrouping<string, ScheduleEntryDTO>> GroupEntriesByInterval(
            List<ScheduleEntryDTO> entries,
            ScheduleInterval interval)
        {
            switch (interval)
            {
                case ScheduleInterval.Weekly:
                    return entries
                        .GroupBy(e => $"{e.ScheduleStartDate.Year}-W{ISOWeek.GetWeekOfYear(e.ScheduleStartDate)}");

                case ScheduleInterval.Monthly:
                    return entries
                        .GroupBy(e => $"{e.ScheduleStartDate.Year}-{e.ScheduleStartDate.Month}");

                default:
                    throw new NotImplementedException($"Unsupported interval: {interval}");
            }
        }

        #endregion

        #region CORE: Get Ineligibilities By Absences

        private async Task<List<ScheduleEntryIneligibilityModel>> GetIneligibilitiesByAbsences(CreateEntityScheduleDTO createEntityScheduleDTO, List<ScheduleEntryDTO> scheduleEntryDTOs, List<EntityWorkerMemberDTO> entityWorkerMemberDTOs)
        {
            List<ScheduleEntryIneligibilityModel> scheduleEntryIneligibilities = new List<ScheduleEntryIneligibilityModel>();

            // Get Absences that interfere with current schedule planning
            List<EntityWorkerAbsenceDTO> entityWorkerAbsenceDTOs = await _entityWorkerAbsenceService.GetSpecificWorkerAbsences(createEntityScheduleDTO.EntityId, entityWorkerMemberDTOs.Where(i => i.IsBot == false).Select(i => i.WorkerId).ToList(), _languageAccessor.GetLanguageCode(), createEntityScheduleDTO.StartDate, createEntityScheduleDTO.EndDate);

            // For each absence
            foreach (EntityWorkerAbsenceDTO entityWorkerAbsence in entityWorkerAbsenceDTOs)
            {
                // Get Ineligible Entries where the dates match
                var ineligibleEntries = scheduleEntryDTOs
                    .Where(i =>
                        i.ScheduleStartDate < entityWorkerAbsence.AbsenceEndDate &&
                        i.ScheduleEndDate > entityWorkerAbsence.AbsenceStartDate);

                // For each entry, get the member associated to the absence and apply inegibility
                foreach (ScheduleEntryDTO ineligibleEntry in ineligibleEntries)
                {
                    EntityWorkerMemberDTO entityWorkerMember = entityWorkerMemberDTOs.Where(i => i.WorkerId.Equals(entityWorkerAbsence.WorkerId)).FirstOrDefault();

                    scheduleEntryIneligibilities = ApplySpecificInegibilities(new List<ScheduleEntryDTO> { ineligibleEntry }, entityWorkerMember, scheduleEntryIneligibilities, string.Format(ScheduleGeneratorRelatedMessages.AbsenceObs, entityWorkerAbsence.AbsenceTypeDisplayValue));
                }

            }

            return scheduleEntryIneligibilities;
        }
        
        #endregion

        #region AUX: Filter Weekly Sessions

        private async Task<List<ScheduleEntryDTO>> FilterWeeklySessions(ScheduleEntryDTO currentScheduleEntry, List<ScheduleEntryDTO> scheduleEntryDTOs, CreateEntityScheduleDTO createEntityScheduleDTO)
        {
            List<ScheduleEntryDTO> filteredWeeklyEntries = new List<ScheduleEntryDTO>();
            int backtrackDaysQty = 0;
            int forwardDaysQty = 0;

            // Calculate how many days back and forward we need to go
            switch (currentScheduleEntry.ScheduleStartDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    backtrackDaysQty = 0;
                    forwardDaysQty = 6;
                    break;

                case DayOfWeek.Tuesday:
                    backtrackDaysQty = 1;
                    forwardDaysQty = 5;
                    break;

                case DayOfWeek.Wednesday:
                    backtrackDaysQty = 2;
                    forwardDaysQty = 4;
                    break;

                case DayOfWeek.Thursday:
                    backtrackDaysQty = 3;
                    forwardDaysQty = 3;
                    break;

                case DayOfWeek.Friday:
                    backtrackDaysQty = 4;
                    forwardDaysQty = 2;
                    break;

                case DayOfWeek.Saturday:
                    backtrackDaysQty = 5;
                    forwardDaysQty = 1;
                    break;

                case DayOfWeek.Sunday:
                    backtrackDaysQty = 6;
                    forwardDaysQty = 0;
                    break;
            }

            // Set Start Dates
            DateTime StartOfWeekDate = currentScheduleEntry.ScheduleStartDate.AddDays(-backtrackDaysQty);
            DateTime EndOfWeekDate = currentScheduleEntry.ScheduleStartDate.AddDays(forwardDaysQty);

            // Check if this week is part of the first day of the search
            if (StartOfWeekDate >= createEntityScheduleDTO.StartDate && createEntityScheduleDTO.StartDate <= EndOfWeekDate)
            {
                // If Start Date is not the first day of the week, need to get previous entries
                if (createEntityScheduleDTO.StartDate.DayOfWeek != DayOfWeek.Monday)
                {

                    ScheduleViewModelRequestDTO weekRequest = new ScheduleViewModelRequestDTO
                    {
                        EntityId = createEntityScheduleDTO.EntityId,
                        WorkerId = createEntityScheduleDTO.WorkerId,
                        StartDateSearch = StartOfWeekDate,
                        EndDateSearch = createEntityScheduleDTO.StartDate
                    };

                    List<ScheduleEntryDTO> previousEntries = await _entityScheduleService.GetScheduleEntries(weekRequest);

                    // Get Schedule entries from Start of Week Date to Start Date
                    filteredWeeklyEntries = filteredWeeklyEntries.Concat(previousEntries).ToList();
                }
            }

            filteredWeeklyEntries = filteredWeeklyEntries.Concat(scheduleEntryDTOs.Where(i => i.ScheduleStartDate > currentScheduleEntry.ScheduleStartDate.AddDays(backtrackDaysQty) && i.ScheduleEndDate < currentScheduleEntry.ScheduleStartDate.AddDays(forwardDaysQty))).ToList();

            return filteredWeeklyEntries;
        }

        #endregion

        #region AUX: Filter Monthly Entries

        private async Task<List<ScheduleEntryDTO>> FilterMonthlyEntries(
            ScheduleEntryDTO currentScheduleEntry,
            List<ScheduleEntryDTO> scheduleEntryDTOs,
            CreateEntityScheduleDTO createEntityScheduleDTO)
        {
            List<ScheduleEntryDTO> filteredMontlyEntries = new List<ScheduleEntryDTO>();

            DateTime firstDayOfMonth = new DateTime(currentScheduleEntry.ScheduleStartDate.Year, currentScheduleEntry.ScheduleStartDate.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Check if we need to fetch the first part of the month
            DateTime firstDayOfCurrentPool = scheduleEntryDTOs.OrderBy(i => i.ScheduleStartDate).First().ScheduleStartDate;
            if (firstDayOfCurrentPool > firstDayOfMonth)
            {
                var request = new ScheduleViewModelRequestDTO
                {
                    EntityId = createEntityScheduleDTO.EntityId,
                    WorkerId = createEntityScheduleDTO.WorkerId,
                    StartDateSearch = firstDayOfMonth,
                    EndDateSearch = firstDayOfCurrentPool.AddDays(-1)
                };

                filteredMontlyEntries.AddRange(await _entityScheduleService.GetScheduleEntries(request));
            }

            // Add current entries in the month
            filteredMontlyEntries.AddRange(scheduleEntryDTOs
                .Where(i => i.ScheduleStartDate >= firstDayOfMonth && i.ScheduleStartDate <= lastDayOfMonth));

            // Check if we need to fetch the last part of the month
            DateTime lastDayOfCurrentPool = scheduleEntryDTOs.OrderByDescending(i => i.ScheduleStartDate).First().ScheduleStartDate;
            if (lastDayOfCurrentPool < lastDayOfMonth)
            {
                var request = new ScheduleViewModelRequestDTO
                {
                    EntityId = createEntityScheduleDTO.EntityId,
                    WorkerId = createEntityScheduleDTO.WorkerId,
                    StartDateSearch = lastDayOfCurrentPool.AddDays(1),
                    EndDateSearch = lastDayOfMonth
                };

                filteredMontlyEntries.AddRange(await _entityScheduleService.GetScheduleEntries(request));
            }

            // Remove duplicates (just in case)
            return filteredMontlyEntries
                .GroupBy(e => e.ScheduleEntryId)
                .Select(g => g.First())
                .ToList();
        }


        #endregion

        #region AUX : Is Weekend
        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        #endregion

        #region AUX : Merge Time Ranges

        private List<(DateTime Start, DateTime End)> MergeTimeRanges(List<(DateTime Start, DateTime End)> timeRanges)
        {
            if (!timeRanges.Any()) return new List<(DateTime, DateTime)>();

            var sorted = timeRanges.OrderBy(r => r.Start).ToList();
            var merged = new List<(DateTime Start, DateTime End)>();

            var current = sorted[0];

            for (int i = 1; i < sorted.Count; i++)
            {
                var next = sorted[i];
                if (current.End >= next.Start)
                {
                    // Merge
                    current = (current.Start, current.End > next.End ? current.End : next.End);
                }
                else
                {
                    merged.Add(current);
                    current = next;
                }
            }

            merged.Add(current);
            return merged;
        }

        #endregion

        #region Check Average Hours Per Week NOT USED

        private bool CheckAverageHoursPerWeek(
            IEnumerable<ScheduleEntryDTO> weekEntries,
            EntityWorkerMemberDTO entityWorkerMemberDTO,
            EntityRuleDTO avgWeekHoursRule,
            List<ShiftDTO> shiftDTOs)
        {
            TimeSpan totalHours = TimeSpan.Zero;

            foreach (ScheduleEntryDTO scheduleEntryDTO in weekEntries)
            {
                if (scheduleEntryDTO.ScheduleParticipants.Any(i => i.Worker.WorkerId.Equals(entityWorkerMemberDTO.WorkerId)))
                {
                    ShiftDTO shiftDTO = shiftDTOs.FirstOrDefault(i => i.ShiftId.Equals(scheduleEntryDTO.ShiftId));
                    if (shiftDTO != null)
                    {
                        totalHours += shiftDTO.ShiftDuration;
                    }
                }
            }

            // Expected weekly average from the rule
            double expectedHours = avgWeekHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue;
            double actualHours = totalHours.TotalHours;

            // Allow, for example, ±10% variance
            double tolerance = 0.1;
            //double minAllowed = expectedHours * (1 - tolerance);
            double maxAllowed = expectedHours * (1 + tolerance);

            return actualHours <= maxAllowed; // actualHours >= minAllowed &&
        }

        #endregion

        #region Check Average Hours Per Month NOT USED

        private bool CheckAverageHoursPerMonth(
            IEnumerable<ScheduleEntryDTO> weekEntries,
            EntityWorkerMemberDTO entityWorkerMemberDTO,
            EntityRuleDTO avgMonthHoursRule,
            List<ShiftDTO> shiftDTOs)
        {
            TimeSpan totalHours = TimeSpan.Zero;

            foreach (ScheduleEntryDTO scheduleEntryDTO in weekEntries)
            {
                if (scheduleEntryDTO.ScheduleParticipants.Any(i => i.Worker.WorkerId.Equals(entityWorkerMemberDTO.WorkerId)))
                {
                    ShiftDTO shiftDTO = shiftDTOs.FirstOrDefault(i => i.ShiftId.Equals(scheduleEntryDTO.ShiftId));
                    if (shiftDTO != null)
                    {
                        totalHours += shiftDTO.ShiftDuration;
                    }
                }
            }

            // Expected weekly average from the rule
            double expectedHours = avgMonthHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue;
            double actualHours = totalHours.TotalHours;

            // Allow, for example, ±10% variance
            double tolerance = 0.1;
            // double minAllowed = expectedHours * (1 - tolerance);
            double maxAllowed = expectedHours * (1 + tolerance);

            return actualHours <= maxAllowed; // actualHours >= minAllowed &&
        }


        #endregion

        #endregion
    }
}
