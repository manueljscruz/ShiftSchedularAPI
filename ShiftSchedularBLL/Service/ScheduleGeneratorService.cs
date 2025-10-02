using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.MemberManagement;
using ShiftSchedularRL.Resources.ScheduleManagement;

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
            IGenericRepository<BusinessAspect> businessAspectRepository
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
        }

        #endregion

        #region Methods

        #region Create Entity Schedule

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
                            ruleDTOs = await _entityRuleService.GetEntityRules(createEntityScheduleDTO.EntityId, createEntityScheduleDTO.LanguageCode);
                        else
                            ruleDTOs = await _entityRuleService.GetSpecificRules(createEntityScheduleDTO.EntityId, createEntityScheduleDTO.FilteredRules, createEntityScheduleDTO.LanguageCode);

                        // Get Members
                        if (createEntityScheduleDTO.FilteredMembers.Count() == 0)
                            entityWorkerMemberDTOs = await _entityService.GetEntityMembers(createEntityScheduleDTO.EntityId, new List<string>(), createEntityScheduleDTO.LanguageCode);
                        else
                            entityWorkerMemberDTOs = await _entityService.GetEntityMembers(createEntityScheduleDTO.EntityId, createEntityScheduleDTO.FilteredMembers, createEntityScheduleDTO.LanguageCode);

                        // Get Entity Skills
                        entitySkills = await _entityService.GetEntitySkills(new BaseViewModelRequest
                        {
                            EntityId = createEntityScheduleDTO.EntityId,
                            LanguageCode = createEntityScheduleDTO.LanguageCode
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

        #region Create Shift Entry

        private async Task<ScheduleEntryDTO> CreateShiftEntry(ShiftDTO shift, DateTime cycleDate)
        {
            var entry = await _entityScheduleService.CreateBaseScheduleEntry(shift, cycleDate);

            var dto = _mapper.Map<ScheduleEntryDTO>(entry);
            dto.ShiftDTO = shift;
            dto.ScheduleParticipants = new List<ScheduleEntryParticipantDTO>();

            return dto;
        }

        #endregion

        #region Apply Rotation Cycle

        public async Task<BaseResponse<List<ScheduleEntryDTO>>> ApplyRotationCycle(ApplyRotationCycleDTO rotationCycleDTO)
        {
            // Validate worker exists
            var worker = (await _entityService.GetEntityMembers(
                rotationCycleDTO.EntityId,
                new List<string> { rotationCycleDTO.WorkerId },
                rotationCycleDTO.LanguageCode
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
                LanguageCode = rotationCycleDTO.LanguageCode,
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
                    LanguageCode = dto.LanguageCode,
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
            // List<ScheduleEntryIneligibilityDTOv1> scheduleEntryIneligibilities = new List<ScheduleEntryIneligibilityDTOv1>();
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

                // 2.8 Assign workers
                var assigned = new List<ScheduleEntryParticipantDTO>();

                foreach (var skillRequirement in requiredSkillQuantities)
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

                    foreach (var worker in skillEligibleWorkers)
                    {
                        if (maxPerShiftFinal != 0 && assigned.Count >= maxPerShiftFinal) break;
                        if (scheduleEntry.ScheduleParticipants.Any(p => p.Worker.WorkerId == worker.WorkerId)) continue;

                        if (maxDailyRule != null && !CheckMaxHoursPerDay(scheduleEntry, dailyEntries, worker, maxDailyRule, shifts))
                            continue; // Check maximum amount of hours per day
                        if (maxWeeklyRule != null && !CheckMaxHoursPerWeek(weeklyEntries, worker, maxWeeklyRule, shifts))
                            continue;             // Check maximum amount of hours per week
                        // if(avgHoursWeeklyRule != null && !CheckAverageHoursPerWeek(weeklyEntries, worker, avgHoursWeeklyRule, shifts)) continue;
                        // if (avgHoursMonthlyRule != null && !CheckAverageHoursPerMonth(monthEntries, worker, avgHoursMonthlyRule, shifts)) continue;
                        if (postShiftRestRule != null && !CheckForPostShiftRest(scheduleEntry, previousShiftEntries, worker, postShiftRestRule))
                            continue;  // Check for mandatory rest post shift
                        if (scheduleEntryDTOs.Any(i => i.ScheduleStartDate.Date.Equals(scheduleEntry.ScheduleStartDate.Date)
                            && i.ScheduleEntryId != scheduleEntry.ScheduleEntryId
                            && !worker.MultipleShiftAssignments
                            && i.ScheduleParticipants.Any(j => j.Worker.WorkerId.Equals(worker.WorkerId))))
                            continue;  // Check if worker has already an assignment on the same day and not eligible for multiple

                        if (consecutiveNonRotationTurnsRule != null && !worker.PartOfRotation && !CheckConsecutiveTurns(scheduleEntryDTOs, scheduleEntry, worker, consecutiveNonRotationTurnsRule))  // Check for consecutive turns for non rotationers
                            continue;

                        // If user is to perform a single responsibility and there is a skill identifier
                        if (createEntityScheduleDTO.SingleRoleResponsibility && skillId != -1)
                        {
                            // Assign worker
                            assigned.Add(new ScheduleEntryParticipantDTO
                            {
                                Worker = worker,
                                AssignedSkills = new List<SkillLocalizedDTO> { entitySkills.First(i => i.SkillId.Equals(skillId)) }
                            });
                        }

                        else
                        {
                            assigned.Add(new ScheduleEntryParticipantDTO
                            {
                                Worker = worker,
                                AssignedSkills = worker.SkillSet
                            });
                        }

                        skillAssigned++;
                        eligibleWorkers.Remove(worker); // Remove to prevent duplication

                        scheduleEntryIneligibilityModels = ApplySpecificInegibilities(new List<ScheduleEntryDTO> { scheduleEntry }, worker, scheduleEntryIneligibilityModels, ScheduleGeneratorRelatedMessages.WorkerAssignedToThisEntry);


                        scheduleEntryIneligibilityModels = ValidatePostSelectionIneligibilities(worker, scheduleEntry, scheduleEntryDTOs, scheduleEntryIneligibilityModels, shifts, entityShiftRotationDTOs, entityRules);

                        if (skillId != -1 && quantity != 0 && skillAssigned >= quantity)
                            break;

                    }

                    if (quantity != 0 && assigned.Count >= quantity)
                        break;
                }

                scheduleEntry.ScheduleParticipants = scheduleEntry.ScheduleParticipants.Concat(assigned).ToList();

            }

            await AddOrUpdateScheduleIneligibilities(scheduleEntryIneligibilityModels);

            return scheduleEntryDTOs;
        }

        #endregion

        #region Add Or Update Schedule Ineligibilities

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

        #region Validate Post Selection Ineligibilities

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

        private List<ScheduleEntryIneligibilityDTOv1> _ValidatePostSelectionIneligibilities(EntityWorkerMemberDTO worker,
            ScheduleEntryDTO scheduleEntry,
            List<ScheduleEntryDTO> scheduleEntryDTOs,
            List<ScheduleEntryIneligibilityDTOv1> scheduleEntryIneligibilities,
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
                scheduleEntryIneligibilities = _ApplySpecificInegibilities(currentDayEntries, worker, scheduleEntryIneligibilities);
            }


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

                        bool maxHourAllowed = (maxDailyRule == null || CheckMaxHoursPerDay(nextRotationEntry, dailyEntries, worker, maxDailyRule, shifts));

                        if (canDoSameDay && maxHourAllowed)
                        {
                            nextIneligibleEntries = scheduleEntryDTOs.Where(i => i.ScheduleStartDate.Date.Equals(scheduleEntry.ScheduleStartDate.Date)
                                && !i.ScheduleParticipants.Any(j => j.Worker.WorkerId.Equals(worker.WorkerId))
                                && !i.ShiftId.Equals(nextStepOfRotation.ShiftId)).ToList();
                        }

                        // Else, get all entries of the next day and make all ineligible, except for the next rotation entry 
                        else
                        {
                            nextIneligibleEntries = scheduleEntryDTOs.Where(i => i.ScheduleStartDate.Date.Equals(scheduleEntry.ScheduleStartDate.Date.AddDays(1))
                                && !i.ShiftId.Equals(nextStepOfRotation.ShiftId)).ToList();
                        }
                    }
                    else
                    {
                        // Get Entries of the next day where its not the same shift
                        nextIneligibleEntries = scheduleEntryDTOs.Where(i => i.ScheduleStartDate.Date.Equals(scheduleEntry.ScheduleStartDate.Date.AddDays(1))
                            && !i.ShiftId.Equals(nextStepOfRotation.ShiftId)).ToList();
                    }

                    // scheduleEntryIneligibilities = ApplySpecificInegibilities(nextIneligibleEntries, worker, scheduleEntryIneligibilities);
                }

                // Its a mandatory leave
                else
                {
                    DateTime leaveStart = scheduleEntry.ScheduleEndDate;
                    DateTime leaveEnd = leaveStart.Add(nextStepOfRotation.LeaveDuration);

                    IEnumerable<ScheduleEntryDTO> mandatoryLeaveEntries = scheduleEntryDTOs
                            .Where(i => i.ScheduleStartDate >= scheduleEntry.ScheduleStartDate && i.ScheduleStartDate <= leaveEnd);

                    // scheduleEntryIneligibilities = ApplySpecificInegibilities(mandatoryLeaveEntries, worker, scheduleEntryIneligibilities);
                }
            }

            // If not part of the rotation
            else if (!worker.PartOfRotation)
            {
                // scheduleEntryIneligibilities = CheckAndApplyIneligiblitiesForConsecutiveEntries(entityRules, worker, scheduleEntryDTOs, scheduleEntry, scheduleEntryIneligibilities);
            }

            return scheduleEntryIneligibilities;
        }

        #endregion

        #region Filter Eligible Workers

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

        private List<EntityWorkerMemberDTO> _FilterEligibleWorkers(
            ScheduleEntryDTO scheduleEntryDTO,
            List<EntityRuleDTO> entityRulesDTO,
            List<EntityWorkerMemberDTO> entityWorkerMembers,
            bool isShiftRotation,
            List<ScheduleEntryIneligibilityDTOv1> scheduleEntryIneligibilities,
            bool forceNoSkill)
        {
            IEnumerable<EntityWorkerMemberDTO> filteredWorkers = entityWorkerMembers;

            bool isWeekend = IsWeekend(scheduleEntryDTO.ScheduleStartDate);

            // Apply filters step by step
            filteredWorkers = FilterBySkillRequirements(filteredWorkers, entityRulesDTO, scheduleEntryDTO, forceNoSkill);
            filteredWorkers = FilterByRotation(filteredWorkers, scheduleEntryDTO, isShiftRotation);
            filteredWorkers = FilterByAvailability(filteredWorkers, isWeekend);
            //filteredWorkers = FilterByIneligibility(filteredWorkers, scheduleEntryDTO, scheduleEntryIneligibilities);
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

        private List<ScheduleEntryIneligibilityModel> CheckAndApplyIneligiblitiesForConsecutiveEntries(List<EntityRuleDTO> entityRules, EntityWorkerMemberDTO worker, List<ScheduleEntryDTO> scheduleEntryDTOs, ScheduleEntryDTO scheduleEntry, List<ScheduleEntryIneligibilityModel> scheduleEntryIneligibilities)
        {
            try
            {
                // Check if there is a consecutive turns rule
                EntityRuleDTO consecutiveTurnsRule = entityRules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_CONSECUTIVE_DAYS_NON_ROTATIONERS_ID)).FirstOrDefault();
                if (consecutiveTurnsRule != null)
                {
                    // Get Specified amount
                    int consecutiveDaysRuleAmount = (int)consecutiveTurnsRule.EntityRuleSpecificationDTOs[0].RuleSpecificationValue;

                    // Get Entries where worker is Assigned, order by latest
                    List<ScheduleEntryDTO> workerEntries = scheduleEntryDTOs.Where(i => i.ScheduleParticipants.Any(j => j.Worker.WorkerId.Equals(worker.WorkerId)))
                        .OrderByDescending(i => i.ScheduleStartDate).ToList();

                    workerEntries = workerEntries
                        .GroupBy(e => e.ScheduleStartDate.Date)
                        .Select(g => g.First())
                        .ToList();

                    // Check if worker with this assignment has enough consecutive turns
                    if (consecutiveDaysRuleAmount > 0 && workerEntries.Count != 0)
                    {
                        DateTime nextExpectedDate = scheduleEntry.ScheduleStartDate.AddDays(-1);
                        int currentConsecutive = 1;

                        if (workerEntries.Count > 1)
                        {
                            for (int i = 0; i < consecutiveDaysRuleAmount && i < workerEntries.Count; i++) // workerEntries.Count && currentConsecutive < consecutiveDaysRuleAmount
                            {
                                ScheduleEntryDTO nextExpectedEntry = workerEntries[i];

                                // Check if the entry is on previous day, indicating a consecutive day
                                if (nextExpectedEntry != null && nextExpectedEntry.ScheduleStartDate.Date.Equals(nextExpectedDate.Date))
                                {
                                    currentConsecutive++;

                                    // Check if the amount of consecutive days was reached
                                    if (currentConsecutive == consecutiveDaysRuleAmount)
                                    {
                                        // Check for minimum days of week rule
                                        EntityRuleDTO weekDaysOffRule = entityRules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MIN_DAYS_OFF_WEEK_ID)).FirstOrDefault();

                                        DateTime dateStart, dateEnd;

                                        if (weekDaysOffRule != null)
                                        {
                                            int weekDaysRestAmount = (int)weekDaysOffRule.EntityRuleSpecificationDTOs[0].RuleSpecificationValue;

                                            // Worker can still be assigned to other shifts on the same day
                                            if (worker.MultipleShiftAssignments)
                                            {
                                                dateStart = scheduleEntry.ScheduleStartDate.Date.AddDays(1);
                                                dateEnd = dateStart.AddDays(weekDaysRestAmount).AddSeconds(-1);
                                            }
                                            else
                                            {
                                                // apply inegilibity for next amount of rest days
                                                dateStart = scheduleEntry.ScheduleEndDate;
                                                dateEnd = dateStart.AddDays(weekDaysRestAmount);
                                            }

                                        }
                                        else
                                        {
                                            if (worker.MultipleShiftAssignments)
                                            {
                                                dateStart = scheduleEntry.ScheduleStartDate.Date.AddDays(1);
                                                dateEnd = dateStart.AddDays(1).AddSeconds(-1);
                                            }
                                            else
                                            {
                                                // apply inegilibity for next day
                                                dateStart = scheduleEntry.ScheduleEndDate;
                                                dateEnd = dateStart.AddDays(1);
                                            }
                                        }

                                        IEnumerable<ScheduleEntryDTO> mandatoryLeaveEntries = scheduleEntryDTOs
                                            .Where(i => i.ScheduleStartDate.Date >= dateStart.Date && i.ScheduleStartDate.Date <= dateEnd.Date);

                                        scheduleEntryIneligibilities = ApplySpecificInegibilities(mandatoryLeaveEntries, worker, scheduleEntryIneligibilities, ScheduleGeneratorRelatedMessages.MandatoryRestDaysObs);

                                    }

                                    nextExpectedDate = nextExpectedDate.AddDays(-1);
                                }
                                // Not consecutive, stop search
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string strErr = ex.Message;
            }

            return scheduleEntryIneligibilities;
        }

        private List<ScheduleEntryIneligibilityDTOv1> _CheckAndApplyIneligiblitiesForConsecutiveEntries(List<EntityRuleDTO> entityRules, EntityWorkerMemberDTO worker, List<ScheduleEntryDTO> scheduleEntryDTOs, ScheduleEntryDTO scheduleEntry, List<ScheduleEntryIneligibilityDTOv1> scheduleEntryIneligibilities)
        {
            try
            {
                // Check if there is a consecutive turns rule
                EntityRuleDTO consecutiveTurnsRule = entityRules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_CONSECUTIVE_DAYS_NON_ROTATIONERS_ID)).FirstOrDefault();
                if (consecutiveTurnsRule != null)
                {
                    // Get Specified amount
                    int consecutiveDaysRuleAmount = (int)consecutiveTurnsRule.EntityRuleSpecificationDTOs[0].RuleSpecificationValue;

                    // Get Entries where worker is Assigned, order by latest
                    List<ScheduleEntryDTO> workerEntries = scheduleEntryDTOs.Where(i => i.ScheduleParticipants.Any(j => j.Worker.WorkerId.Equals(worker.WorkerId)))
                        .OrderByDescending(i => i.ScheduleStartDate).ToList();

                    workerEntries = workerEntries
                        .GroupBy(e => e.ScheduleStartDate.Date)
                        .Select(g => g.First())
                        .ToList();

                    // Check if worker with this assignment has enough consecutive turns
                    if (consecutiveDaysRuleAmount > 0 && workerEntries.Count != 0)
                    {
                        DateTime nextExpectedDate = scheduleEntry.ScheduleStartDate.AddDays(-1);
                        int currentConsecutive = 1;

                        if (workerEntries.Count > 1)
                        {
                            for (int i = 0; i < consecutiveDaysRuleAmount && i < workerEntries.Count; i++) // workerEntries.Count && currentConsecutive < consecutiveDaysRuleAmount
                            {
                                ScheduleEntryDTO nextExpectedEntry = workerEntries[i];

                                // Check if the entry is on previous day, indicating a consecutive day
                                if (nextExpectedEntry != null && nextExpectedEntry.ScheduleStartDate.Date.Equals(nextExpectedDate.Date))
                                {
                                    currentConsecutive++;

                                    // Check if the amount of consecutive days was reached
                                    if (currentConsecutive == consecutiveDaysRuleAmount)
                                    {
                                        // Check for minimum days of week rule
                                        EntityRuleDTO weekDaysOffRule = entityRules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MIN_DAYS_OFF_WEEK_ID)).FirstOrDefault();

                                        DateTime dateStart, dateEnd;

                                        if (weekDaysOffRule != null)
                                        {
                                            int weekDaysRestAmount = (int)weekDaysOffRule.EntityRuleSpecificationDTOs[0].RuleSpecificationValue;

                                            // Worker can still be assigned to other shifts on the same day
                                            if (worker.MultipleShiftAssignments)
                                            {
                                                dateStart = scheduleEntry.ScheduleStartDate.Date.AddDays(1);
                                                dateEnd = dateStart.AddDays(weekDaysRestAmount).AddSeconds(-1);
                                            }
                                            else
                                            {
                                                // apply inegilibity for next amount of rest days
                                                dateStart = scheduleEntry.ScheduleEndDate;
                                                dateEnd = dateStart.AddDays(weekDaysRestAmount);
                                            }

                                        }
                                        else
                                        {
                                            if (worker.MultipleShiftAssignments)
                                            {
                                                dateStart = scheduleEntry.ScheduleStartDate.Date.AddDays(1);
                                                dateEnd = dateStart.AddDays(1).AddSeconds(-1);
                                            }
                                            else
                                            {
                                                // apply inegilibity for next day
                                                dateStart = scheduleEntry.ScheduleEndDate;
                                                dateEnd = dateStart.AddDays(1);
                                            }
                                        }

                                        IEnumerable<ScheduleEntryDTO> mandatoryLeaveEntries = scheduleEntryDTOs
                                            .Where(i => i.ScheduleStartDate.Date >= dateStart.Date && i.ScheduleStartDate.Date <= dateEnd.Date);

                                        // scheduleEntryIneligibilities = ApplySpecificInegibilities(mandatoryLeaveEntries, worker, scheduleEntryIneligibilities);

                                    }

                                    nextExpectedDate = nextExpectedDate.AddDays(-1);
                                }
                                // Not consecutive, stop search
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
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

        private List<ScheduleEntryIneligibilityDTOv1> _ApplySpecificInegibilities(IEnumerable<ScheduleEntryDTO> entriesToDenyWorker, EntityWorkerMemberDTO worker, List<ScheduleEntryIneligibilityDTOv1> scheduleEntrysIneligibilities)
        {
            // For each entry
            foreach (ScheduleEntryDTO schedule in entriesToDenyWorker)
            {
                // Check for existing ineligibility for this entry
                ScheduleEntryIneligibilityDTOv1 scheduleEntryIneligibility = scheduleEntrysIneligibilities.Where(i => i.ScheduleEntryID.Equals(schedule.ScheduleEntryId)).FirstOrDefault();

                // If there is no entry yet
                if (scheduleEntryIneligibility == null)
                {
                    // Create a new one and add assigned worker to the list of Ineligible
                    scheduleEntryIneligibility = new ScheduleEntryIneligibilityDTOv1(schedule.ScheduleEntryId, schedule.ScheduleStartDate);
                    scheduleEntryIneligibility.MembersIneligible.Add(worker);
                    scheduleEntrysIneligibilities.Add(scheduleEntryIneligibility);
                }
                else
                {
                    // If there is an entry, check if the worker is already ineligible
                    if (!scheduleEntryIneligibility.MembersIneligible.Contains(worker))
                    {
                        // If not, add it to the list of ineligible
                        scheduleEntryIneligibility.MembersIneligible.Add(worker);
                    }
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

            if (!workerDates.Contains(currentEntry.ScheduleStartDate.Date))
            {
                workerDates.Add(currentEntry.ScheduleStartDate.Date); // Include the current entry's date if not already present
                workerDates = workerDates.OrderByDescending(d => d).ToList(); // Re-sort
            }

            // Step 2: Check for consecutive days starting from currentEntry date
            DateTime expectedDate = currentEntry.ScheduleStartDate.Date;
            int consecutiveCount = 0;

            foreach (var date in workerDates)
            {
                if (date == expectedDate)
                {
                    consecutiveCount++;
                    expectedDate = expectedDate.AddDays(-1);
                    if (consecutiveCount > maxConsecutiveDays)
                        return false; // Violates the rule
                }
                else if (date < expectedDate)
                {
                    break; // Gap found, streak ends
                }
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
                        ScheduleEntryDTO lastPreCurrentScheduleEntry = previousShiftEntries.Where(i => i.ShiftId.ToString().Equals(entityRuleSpecificationDTO.BusinessAspectId)
                    && i.ScheduleStartDate < currentScheduleEntry.ScheduleStartDate
                    && i.ScheduleParticipants.Any(j => j.Worker.WorkerId.Equals(entityWorkerMemberDTO.WorkerId))).OrderByDescending(i => i.ScheduleStartDate).FirstOrDefault();

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

        #region Get Ineligibilities By Absences

        private async Task<List<ScheduleEntryIneligibilityModel>> GetIneligibilitiesByAbsences(CreateEntityScheduleDTO createEntityScheduleDTO, List<ScheduleEntryDTO> scheduleEntryDTOs, List<EntityWorkerMemberDTO> entityWorkerMemberDTOs)
        {
            List<ScheduleEntryIneligibilityModel> scheduleEntryIneligibilities = new List<ScheduleEntryIneligibilityModel>();

            // Get Absences that interfere with current schedule planning
            List<EntityWorkerAbsenceDTO> entityWorkerAbsenceDTOs = await _entityWorkerAbsenceService.GetSpecificWorkerAbsences(createEntityScheduleDTO.EntityId, entityWorkerMemberDTOs.Where(i => i.IsBot == false).Select(i => i.WorkerId).ToList(), createEntityScheduleDTO.LanguageCode, createEntityScheduleDTO.StartDate, createEntityScheduleDTO.EndDate);

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
        // , string.Format(ScheduleGeneratorRelatedMessages.AbsenceObs, entityWorkerAbsence.AbsenceTypeDisplayValue)

        private async Task<List<ScheduleEntryIneligibilityDTOv1>> _GetIneligibilitiesByAbsences(CreateEntityScheduleDTO createEntityScheduleDTO, List<ScheduleEntryDTO> scheduleEntryDTOs, List<EntityWorkerMemberDTO> entityWorkerMemberDTOs)
        {
            List<ScheduleEntryIneligibilityDTOv1> scheduleEntryIneligibilities = new List<ScheduleEntryIneligibilityDTOv1>();

            // Get Absences that interfere with current schedule planning
            List<EntityWorkerAbsenceDTO> entityWorkerAbsenceDTOs = await _entityWorkerAbsenceService.GetSpecificWorkerAbsences(createEntityScheduleDTO.EntityId, entityWorkerMemberDTOs.Where(i => i.IsBot == false).Select(i => i.WorkerId).ToList(), createEntityScheduleDTO.LanguageCode, createEntityScheduleDTO.StartDate, createEntityScheduleDTO.EndDate);

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

                    // scheduleEntryIneligibilities = ApplySpecificInegibilities(new List<ScheduleEntryDTO> { ineligibleEntry }, entityWorkerMember, scheduleEntryIneligibilities);
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
                        LanguageCode = createEntityScheduleDTO.LanguageCode,
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
                    LanguageCode = createEntityScheduleDTO.LanguageCode,
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
                    LanguageCode = createEntityScheduleDTO.LanguageCode,
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

        #region AUX : IsWeekend
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

        #region Refill Dailies WIP

        private async Task<Tuple<List<ScheduleEntryDTO>, List<ScheduleEntryIneligibilityDTOv1>>> _RefillDailies(
            List<ScheduleEntryDTO> scheduleEntries,
            DateTime dayToRefill,
            List<EntityWorkerMemberDTO> entityWorkerMemberDTOs,
            List<ShiftDTO> shifts,
            List<EntityRuleDTO> entityRules,
            List<EntityShiftRotationDTO> shiftRotationDTOs,
            List<ScheduleEntryIneligibilityDTOv1> scheduleEntryIneligibilities,
            CreateEntityScheduleDTO createEntityScheduleDTO)
        {
            // 1. Only consider schedule entries for the target day
            List<ScheduleEntryDTO> dayEntries = scheduleEntries
                .Where(e => e.ScheduleStartDate.Date == dayToRefill.Date)
                .ToList();

            // 2. Find all workers who are NOT already assigned on this day
            var notPickedWorkers = entityWorkerMemberDTOs
                .Where(worker =>
                    !dayEntries.Any(entry =>
                        entry.ScheduleParticipants.Any(p => p.Worker.WorkerId == worker.WorkerId)))
                .ToList();

            // 3. Rules we need repeatedly
            var maxDailyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MAX_HOURS_DAY_ID);
            var maxWeeklyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MAX_HOURS_WEEK_ID);
            var postShiftRestRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.POST_SHIFT_REST_HOURS_ID);
            var consecutiveNonRotationTurnsRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MAX_CONSECUTIVE_DAYS_NON_ROTATIONERS_ID);

            foreach (var worker in notPickedWorkers)
            {
                for (int i = 0; i < dayEntries.Count; i++)
                {
                    ScheduleEntryDTO scheduleEntry = dayEntries[i];

                    var maxPerShiftFinal = ReturnMaxWorkersPerShift(entityRules, scheduleEntry);

                    // Skip if full
                    if (maxPerShiftFinal != 0 && scheduleEntry.ScheduleParticipants.Count >= maxPerShiftFinal)
                        continue;

                    // Skip if already assigned (redundant due to outer check, but safe)
                    if (scheduleEntry.ScheduleParticipants.Any(p => p.Worker.WorkerId == worker.WorkerId))
                        continue;

                    // Check if eligible at all
                    bool isRotation = shiftRotationDTOs.Any(rot => rot.ShiftId == scheduleEntry.ShiftId);
                    // var filtered = FilterEligibleWorkers(scheduleEntry, entityRules, new List<EntityWorkerMemberDTO> { worker }, isRotation, scheduleEntryIneligibilities, createEntityScheduleDTO.ForceNoSkill);
                    //if (!filtered.Any())
                    //    continue;

                    // Skill requirement
                    var requiredSkills = GetMinimumSkilletSetPerShift(entityRules, scheduleEntry)
                        ?? new List<Tuple<int, int>> { Tuple.Create(-1, maxPerShiftFinal) };

                    bool hasRequiredSkill = requiredSkills.Any(req =>
                        req.Item1 == -1 || worker.SkillSet.Any(s => s.SkillId == req.Item1));

                    if (!hasRequiredSkill)
                        continue;

                    // Daily/weekly/monthly entries
                    var dailyEntries = scheduleEntries.Where(e => e.ScheduleStartDate.Date == dayToRefill.Date);
                    var weeklyEntries = await FilterWeeklySessions(scheduleEntry, scheduleEntries, createEntityScheduleDTO);
                    var monthEntries = await FilterMonthlyEntries(scheduleEntry, scheduleEntries, createEntityScheduleDTO);
                    var previousShiftEntries = scheduleEntries
                        .Where(e => e.ScheduleStartDate < scheduleEntry.ScheduleStartDate)
                        .OrderByDescending(e => e.ScheduleStartDate);

                    // Rule checks
                    if (maxDailyRule != null && !CheckMaxHoursPerDay(scheduleEntry, dailyEntries, worker, maxDailyRule, shifts))
                        continue;

                    if (maxWeeklyRule != null && !CheckMaxHoursPerWeek(weeklyEntries, worker, maxWeeklyRule, shifts))
                        continue;

                    if (postShiftRestRule != null && !CheckForPostShiftRest(scheduleEntry, previousShiftEntries, worker, postShiftRestRule))
                        continue;

                    if (consecutiveNonRotationTurnsRule != null && !worker.PartOfRotation && !CheckConsecutiveTurns(scheduleEntries, scheduleEntry, worker, consecutiveNonRotationTurnsRule))  // Check for consecutive turns for non rotationers
                        continue;

                    // Conflict: another shift on the same day
                    bool hasConflictingAssignment = scheduleEntries.Any(i =>
                        i.ScheduleStartDate.Date == scheduleEntry.ScheduleStartDate.Date &&
                        i.ScheduleEntryId != scheduleEntry.ScheduleEntryId &&
                        !worker.MultipleShiftAssignments &&
                        i.ScheduleParticipants.Any(j => j.Worker.WorkerId == worker.WorkerId));

                    if (hasConflictingAssignment)
                        continue;

                    // Passed all checks — assign
                    scheduleEntry.ScheduleParticipants.Add(new ScheduleEntryParticipantDTO
                    {
                        Worker = worker
                    });

                    // Update ineligibility list
                    //scheduleEntryIneligibilityModels = ValidatePostSelectionIneligibilities(
                    //    worker,
                    //    scheduleEntry,
                    //    scheduleEntries,
                    //    scheduleEntryIneligibilityModels,
                    //    shifts,
                    //    shiftRotationDTOs,
                    //    entityRules
                    //);

                    //dayEntries = dayEntries.OrderBy(i => i.ScheduleParticipants.Count)
                    //    .ThenBy(i => i.ScheduleStartDate).ToList();

                    //i = 0;

                    if (!worker.MultipleShiftAssignments)
                        break;
                }
            }

            return Tuple.Create(scheduleEntries, scheduleEntryIneligibilities);
        }


        private async Task<Tuple<List<ScheduleEntryDTO>, List<ScheduleEntryIneligibilityDTOv1>>> __RefillDailies(List<ScheduleEntryDTO> scheduleEntries,
            DateTime dayToRefill,
            List<EntityWorkerMemberDTO> entityWorkerMemberDTOs,
            List<ShiftDTO> shifts,
            List<EntityRuleDTO> entityRules,
            List<EntityShiftRotationDTO> shiftRotationDTOs,
            List<ScheduleEntryIneligibilityDTOv1> scheduleEntryIneligibilities,
            CreateEntityScheduleDTO createEntityScheduleDTO
            )
        {
            List<EntityWorkerMemberDTO> notPickedWorkers = entityWorkerMemberDTOs
                .Where(worker => !scheduleEntries.Any(entry => entry.ScheduleParticipants.Any(p => p.Worker.WorkerId == worker.WorkerId) && entry.ScheduleStartDate.Date == dayToRefill.Date))
                .ToList();

            // For every worker not picked
            foreach (EntityWorkerMemberDTO worker in notPickedWorkers)
            {
                bool dayAssigned = false;
                foreach (var scheduleEntry in scheduleEntries.Where(e => e.ScheduleStartDate.Date == dayToRefill.Date))
                {
                    // Check if the schedule entry is maxed
                    var maxPerShiftFinal = ReturnMaxWorkersPerShift(entityRules, scheduleEntry);

                    if (maxPerShiftFinal != 0 && scheduleEntry.ScheduleParticipants.Count >= maxPerShiftFinal)
                        continue; // Skip if max workers per shift is reached

                    bool isRotation = shiftRotationDTOs.Any(rot => rot.ShiftId == scheduleEntry.ShiftId);

                    // Check if the worker is eligible
                    // bool eligible = FilterEligibleWorkers(scheduleEntry, entityRules, new List<EntityWorkerMemberDTO> { worker }, isRotation, scheduleEntryIneligibilities, createEntityScheduleDTO.ForceNoSkill).Count != 0;

                    var maxDailyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MAX_HOURS_DAY_ID);
                    var maxWeeklyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.MAX_HOURS_WEEK_ID);
                    var postShiftRestRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.POST_SHIFT_REST_HOURS_ID);
                    // var avgHoursWeeklyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.AVG_HOURS_WEEK_ID);
                    // var avgHoursMonthlyRule = entityRules.FirstOrDefault(r => r.RuleTypeId == RuleTypeConstants.AVG_HOURS_MONTH_ID);

                    // 2.1 Determine minimum skillset required
                    List<Tuple<int, int>> requiredSkillQuantities = GetMinimumSkilletSetPerShift(entityRules, scheduleEntry);

                    // Fallback: if no skills are defined, allow any eligible worker
                    if (requiredSkillQuantities == null || !requiredSkillQuantities.Any())
                    {
                        requiredSkillQuantities = new List<Tuple<int, int>> { new Tuple<int, int>(-1, maxPerShiftFinal) };
                    }

                    // 2.5 Get daily and weekly entries
                    var dailyEntries = scheduleEntries.Where(e => e.ScheduleStartDate.Date == scheduleEntry.ScheduleStartDate.Date);
                    var weeklyEntries = await FilterWeeklySessions(scheduleEntry, scheduleEntries, createEntityScheduleDTO);
                    var monthEntries = await FilterMonthlyEntries(scheduleEntry, scheduleEntries, createEntityScheduleDTO);

                    // 2.6 Get previous shift entries for rest check
                    var previousShiftEntries = scheduleEntries
                        .Where(e => e.ScheduleStartDate < scheduleEntry.ScheduleStartDate)
                        .OrderByDescending(e => e.ScheduleStartDate);

                    var assigned = scheduleEntry.ScheduleParticipants;

                    if (maxPerShiftFinal != 0 && assigned.Count >= maxPerShiftFinal) break;
                    if (scheduleEntry.ScheduleParticipants.Any(p => p.Worker.WorkerId == worker.WorkerId)) continue;

                    if (maxDailyRule != null && !CheckMaxHoursPerDay(scheduleEntry, dailyEntries, worker, maxDailyRule, shifts)) break; // Check maximum amount of hours per day
                    if (maxWeeklyRule != null && !CheckMaxHoursPerWeek(weeklyEntries, worker, maxWeeklyRule, shifts)) break;             // Check maximum amount of hours per week
                                                                                                                                         // if(avgHoursWeeklyRule != null && !CheckAverageHoursPerWeek(weeklyEntries, worker, avgHoursWeeklyRule, shifts)) continue;
                                                                                                                                         // if (avgHoursMonthlyRule != null && !CheckAverageHoursPerMonth(monthEntries, worker, avgHoursMonthlyRule, shifts)) continue;
                    if (postShiftRestRule != null && !CheckForPostShiftRest(scheduleEntry, previousShiftEntries, worker, postShiftRestRule)) break;  // Check for mandatory rest post shift
                    if (scheduleEntries.Any(i => i.ScheduleStartDate.Date.Equals(scheduleEntry.ScheduleStartDate.Date)
                        && i.ScheduleEntryId != scheduleEntry.ScheduleEntryId
                        && !worker.MultipleShiftAssignments
                        && i.ScheduleParticipants.Any(j => j.Worker.WorkerId.Equals(worker.WorkerId)))) break;  // Check if worker has already an assignment on the same day and not eligible for multiple

                    // Assign worker
                    scheduleEntry.ScheduleParticipants.Add(new ScheduleEntryParticipantDTO
                    {
                        Worker = worker
                    });
                    dayAssigned = true;

                    // scheduleEntryIneligibilities = ValidatePostSelectionIneligibilities(worker, scheduleEntry, scheduleEntries, scheduleEntryIneligibilities, shifts, shiftRotationDTOs, entityRules);
                }
            }

            // foreach (entityWo)
            // FOR WORKER
            // FOR SCHEDULE
            // if assigned, shuffle order

            return new Tuple<List<ScheduleEntryDTO>, List<ScheduleEntryIneligibilityDTOv1>>(scheduleEntries, scheduleEntryIneligibilities);
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
