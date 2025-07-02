using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.ScheduleManagement;

namespace ShiftSchedularBLL.Service
{
    public class EntityScheduleService : IEntityScheduleService
    {
        #region Properties

        private readonly IMapper _mapper;
        private readonly IGeneralService _generalService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShiftService _shiftService;
        private readonly IEntityRuleService _entityRuleService;
        private readonly IEntityService _entityService;
        private readonly IBaseEntityRuleService _baseEntityRuleService;
        private readonly IScheduleGeneratorService _scheduleGeneratorService;

        #endregion

        #region Constructor

        public EntityScheduleService(IMapper mapper,
            IGeneralService generalService,
            IUnitOfWork unitOfWork,
            IShiftService shiftService,
            IEntityRuleService entityRuleService,
            IEntityService entityService,
            IScheduleGeneratorService scheduleGeneratorService)
        {
            _mapper = mapper;
            _generalService = generalService;
            _unitOfWork = unitOfWork;
            _shiftService = shiftService;
            _entityRuleService = entityRuleService;
            _entityService = entityService;
            _scheduleGeneratorService = scheduleGeneratorService;
        }

        #endregion

        #region Methods

        #region Get Entity Schedule View Model

        public async Task<EntityScheduleViewModel> GetEntityScheduleViewModel(ScheduleViewModelRequestDTO viewModelRequest)
        {
            EntityScheduleViewModel viewModel = new EntityScheduleViewModel();

            if (viewModelRequest != null && viewModelRequest.EntityId != Guid.Empty && !string.IsNullOrEmpty(viewModelRequest.WorkerId) && !string.IsNullOrEmpty(viewModelRequest.LanguageCode))
            {
                Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(viewModelRequest.EntityId);
                EntityWorker entityWorkerInstance = await _unitOfWork.EntityWorkerRepository.GetByWorkerAndEntity(viewModelRequest.WorkerId, viewModelRequest.EntityId);
                viewModel.AllowEdit = entityWorkerInstance.IsOwner;

                // If it can change data
                if (entityWorkerInstance.IsOwner)
                {
                    viewModel.Shifts = await _shiftService.GetEntityShifts(viewModelRequest.EntityId);
                    viewModel.EntityRules = await _entityRuleService.GetEntityRules(viewModelRequest.EntityId, viewModelRequest.LanguageCode);
                    BaseViewModelRequest baseRequest = new BaseViewModelRequest
                    {
                        EntityId = viewModelRequest.EntityId,
                        LanguageCode = viewModelRequest.LanguageCode
                    };
                    EntityMembersViewModel entityMembersViewModel = await _entityService.GetEntitiesMembersViewModel(baseRequest);
                    viewModel.EntityWorkerMembers = entityMembersViewModel.EntityMembers;
                }
            }

            return viewModel;
        }

        #endregion

        #region Add Schedule Entry

        public async Task<BaseResponse<ScheduleEntryDTO>> AddScheduleEntry(AddScheduleEntryDTO addScheduleEntryDTO)
        {
            BaseResponse<ScheduleEntryDTO> response = new BaseResponse<ScheduleEntryDTO>();
            response.Message = ScheduleRelatedMessages.AddNewScheduleEntryUnexpectedError;
            response.Success = false;

            if (addScheduleEntryDTO != null)
            {
                if (string.IsNullOrEmpty(addScheduleEntryDTO.ShiftId))
                {
                    response.Message = ScheduleRelatedMessages.AddNewScheduleEntryShiftEmptyError;
                    return response;
                }
                else if (addScheduleEntryDTO.ScheduleStartDate == new DateTime())
                {
                    response.Message = ScheduleRelatedMessages.AddNewScheduleEntryInvalidStartDateError;
                    return response;
                }

                ShiftDTO shift = await _shiftService.GetShiftById(_generalService.ParseStringToGuid(addScheduleEntryDTO.ShiftId), addScheduleEntryDTO.LanguageCode);
                if (shift == null)
                {
                    response.Message = ScheduleRelatedMessages.ShiftNotFound;
                    return response;
                }

                ScheduleEntry scheduleEntry = _mapper.Map<ScheduleEntry>(addScheduleEntryDTO);
                scheduleEntry.ScheduleEntryWorkers = new List<ScheduleEntryWorkers>();
                scheduleEntry.ScheduleEntryId = new Guid();
                scheduleEntry.ScheduleStartDate = scheduleEntry.ScheduleStartDate.Add(shift.ShiftStartHour);

                TimeSpan totalBreakIncludedDuration = shift.ShiftBreakDTOs
                    .Where(sb => sb.IncludedInShift)
                    .Select(sb => sb.ShiftBreakDuration)
                    .Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));

                scheduleEntry.ScheduleEndDate = scheduleEntry.ScheduleStartDate.Add(shift.ShiftDuration).Add(totalBreakIncludedDuration);

                try
                {
                    await _unitOfWork.BeginTransactionAsync();

                    scheduleEntry = await _unitOfWork.EntityScheduleRepository.Add(scheduleEntry);

                    if (scheduleEntry != null)
                    {
                        List<EntityWorkerMemberDTO> entityWorkerMemberDTOs = new List<EntityWorkerMemberDTO>();

                        if (addScheduleEntryDTO.EntryParticipants != null && addScheduleEntryDTO.EntryParticipants.Any())
                        {
                            foreach (string workerId in addScheduleEntryDTO.EntryParticipants)
                            {
                                ScheduleEntryWorkers scheduleEntryWorkers = new ScheduleEntryWorkers
                                {
                                    ScheduleEntryId = scheduleEntry.ScheduleEntryId,
                                    ApplicationUserId = workerId
                                };
                                await _unitOfWork.EntityScheduleWorkersRepository.Add(scheduleEntryWorkers);
                            }
                        }

                        await _unitOfWork.CommitAsync();

                        response.Success = true;
                        response.Message = ScheduleRelatedMessages.AddNewScheduleEntrySuccess;
                        response.Result = await GetScheduleEntryById(scheduleEntry.ScheduleEntryId.ToString(), addScheduleEntryDTO.LanguageCode);
                    }
                    else
                    {
                        response.Message = ScheduleRelatedMessages.AddNewScheduleEntryUnexpectedError;
                    }
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

            return response;
        }

        #endregion

        #region Add Schedule Participant

        /// <summary>
        /// Adds a Schedule Participant to a current schedule entry
        /// </summary>
        /// <param name="scheduleParticipantOp"></param>
        /// <returns></returns>
        public async Task<BaseResponse<ScheduleEntryDTO>> AddScheduleParticipant(ScheduleParticipantOpDTO scheduleParticipantOp)
        {
            BaseResponse<ScheduleEntryDTO> response = new BaseResponse<ScheduleEntryDTO>();
            response.Message = ScheduleRelatedMessages.AddScheduleParticipantUnexpectedError;

            if (string.IsNullOrEmpty(scheduleParticipantOp.ScheduleEntryId))
            {
                response.Message = ScheduleRelatedMessages.ScheduleIdIsNull;
                return response;
            }

            else if (string.IsNullOrEmpty(scheduleParticipantOp.WorkerId))
            {
                response.Message = ScheduleRelatedMessages.WorkerIdIsNull;
                return response;
            }

            ScheduleEntry scheduleEntry = await _unitOfWork.EntityScheduleRepository.GetById(scheduleParticipantOp.ScheduleEntryId);
            if (scheduleEntry == null)
            {
                response.Message = ScheduleRelatedMessages.ScheduleEntryNotFound;
                return response;
            }

            ScheduleEntryWorkers scheduleEntryWorker = new ScheduleEntryWorkers
            {
                ScheduleEntryId = Guid.Parse(scheduleParticipantOp.ScheduleEntryId),
                ApplicationUserId = scheduleParticipantOp.WorkerId
            };

            scheduleEntryWorker = await _unitOfWork.EntityScheduleWorkersRepository.Add(scheduleEntryWorker);

            if (scheduleEntryWorker != null)
            {
                response.Success = true;
                response.Message = ScheduleRelatedMessages.AddScheduleParticipantSuccess;
                response.Result = await GetScheduleEntryById(scheduleEntry.ScheduleEntryId.ToString(), scheduleParticipantOp.LanguageCode);
            }

            return response;
        }


        #endregion

        #region Get Schedule Entries

        /// <summary>
        /// Gets Schedule Entries of an entity
        /// </summary>
        /// <param name="viewModelRequest"></param>
        /// <returns></returns>
        public async Task<List<ScheduleEntryDTO>> GetScheduleEntries(ScheduleViewModelRequestDTO viewModelRequest)
        {
            List<ScheduleEntryDTO> scheduleEntryDTOs = new List<ScheduleEntryDTO>();

            IEnumerable<ShiftDTO> shifts = await _shiftService.GetEntityShifts(viewModelRequest.EntityId);
            IEnumerable<ScheduleEntry> scheduleEntries = await _unitOfWork.EntityScheduleRepository.GetScheduleEntries(viewModelRequest.EntityId, viewModelRequest.WorkerId, viewModelRequest.StartDateSearch, viewModelRequest.EndDateSearch);

            // For each schedule entry
            foreach (ScheduleEntry entry in scheduleEntries)
            {
                scheduleEntryDTOs.Add(await GetScheduleEntryById(entry.ScheduleEntryId.ToString(), viewModelRequest.LanguageCode));
            }

            return scheduleEntryDTOs;
        }

        #endregion

        #region Get Schedule Entry By Id

        public async Task<ScheduleEntryDTO> GetScheduleEntryById(string scheduleEntryId, string languageCode)
        {
            ScheduleEntryDTO scheduleEntryDTO = new ScheduleEntryDTO();

            ScheduleEntry scheduleEntry = await _unitOfWork.EntityScheduleRepository.GetById(scheduleEntryId);
            ShiftDTO shift = await _shiftService.GetShiftById(_generalService.ParseStringToGuid(scheduleEntry.ShiftId.ToString()), languageCode);

            // Get respective participants of said schedule entry
            IEnumerable<ScheduleEntryWorkers> scheduleEntryWorkers = await _unitOfWork.EntityScheduleWorkersRepository.GetScheduleEntryWorkers(scheduleEntry.ScheduleEntryId);
            
            // Get Entity Skills
            List<SkillLocalizedDTO> entitySkills = await _entityService.GetEntitySkills(new BaseViewModelRequest
            {
                EntityId = shift.EntityId,
                LanguageCode = languageCode
            });

            // Extract participant ids
            List<string> participantsIds = scheduleEntryWorkers.Select(i => i.ApplicationUserId).ToList();

            scheduleEntryDTO = _mapper.Map<ScheduleEntryDTO>(scheduleEntry);
            scheduleEntryDTO.ShiftDTO = shift;

            // Get Entity Worker Member Information
            List<EntityWorkerMemberDTO> entityWorkerMemberDTOs = await _entityService.GetEntityMembersByList(shift.EntityId, participantsIds, languageCode);

            // For each worker assigned to this entry
            foreach(ScheduleEntryWorkers entryWorker in scheduleEntryWorkers)
            {
                // Find the worker in the entity members
                EntityWorkerMemberDTO worker = entityWorkerMemberDTOs.FirstOrDefault(i => i.WorkerId.Equals(entryWorker.ApplicationUserId));
                if (worker != null)
                {
                    // Extract Skill Identifiers
                    string[] skills = entryWorker.SpecificSkillAssignments?.Split(',') ?? Array.Empty<string>();

                    // Get assigned skills instance based on the skills identifiers
                    List<SkillLocalizedDTO> assignedSkills = entitySkills
                        .Where(s => skills.Contains(s.SkillId.ToString()))
                        .ToList();

                    // If no specific skills were assigned, use the worker's skill set
                    if (assignedSkills.Count == 0)
                        assignedSkills = worker.SkillSet;

                    ScheduleEntryParticipantDTO participantDTO = new ScheduleEntryParticipantDTO
                    {
                        Worker = worker,
                        AssignedSkills = assignedSkills
                    };

                    scheduleEntryDTO.ScheduleParticipants.Add(participantDTO);
                }
            }

            return scheduleEntryDTO;
        }

        #endregion

        #region Create Entity Schedule

        public async Task<BaseResponse<List<ScheduleEntryDTO>>> CreateEntitySchedule(CreateEntityScheduleDTO createEntityScheduleDTO)
        {
            BaseResponse<List<ScheduleEntryDTO>> response = new BaseResponse<List<ScheduleEntryDTO>>();
            response.Message = "";
            response.Success = false;

            if (createEntityScheduleDTO != null && createEntityScheduleDTO.EntityId != Guid.Empty && !string.IsNullOrEmpty(createEntityScheduleDTO.WorkerId))
            {
                bool isOwner = await _unitOfWork.EntityWorkerRepository.IsMemberOwner(createEntityScheduleDTO.EntityId, createEntityScheduleDTO.WorkerId);

                if (isOwner)
                {
                    await _unitOfWork.BeginTransactionAsync();

                    List<ScheduleEntryDTO> scheduleEntryDTOs = new List<ScheduleEntryDTO>();
                    try
                    {
                        #region Delete Entries

                        // Get Entity Schedules within a specific time period
                        List<ScheduleEntry> scheduleEntries = await _unitOfWork.EntityScheduleRepository.GetScheduleEntries(createEntityScheduleDTO.EntityId, string.Empty, createEntityScheduleDTO.StartDate, createEntityScheduleDTO.EndDate);

                        // Delete all of them
                        foreach (ScheduleEntry scheduleEntry in scheduleEntries)
                        {
                            BaseResponse<bool> deleteScheduleResponse = await DeleteScheduleEntry(scheduleEntry.ScheduleEntryId);

                            if (!deleteScheduleResponse.Success)
                            {
                                response.Message = deleteScheduleResponse.Message;
                                await _unitOfWork.RollbackAsync();
                                return response;
                            }
                        }

                        #endregion

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

                        #region Get date differential between start and end date V1
                        //TimeSpan dateDifference = createEntityScheduleDTO.EndDate - createEntityScheduleDTO.StartDate;
                        //DateTime cycleDate = createEntityScheduleDTO.StartDate;

                        //// For each day
                        //for (int i = 0; i < dateDifference.Days; i++)
                        //{
                        //    // Check if its the weekend
                        //    bool isWeekend = cycleDate.DayOfWeek == DayOfWeek.Saturday || cycleDate.DayOfWeek == DayOfWeek.Sunday;
                        //    // For each shift
                        //    foreach(ShiftDTO shift in shifts)
                        //    {
                        //        // If there isnt a rule to include this shift on the weekends
                        //        if(isWeekend && !ruleDTOs.Any(i => i.RuleTypeId.Equals(RuleTypeConstants.SHIFT_INCLUDES_WEEKENDS_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(shift.ShiftId) && j.RuleSpecificationValue.Equals(1))))
                        //        {
                        //            continue;
                        //        }

                        //        // Create Schedule Entry
                        //        ScheduleEntry scheduleEntry = new ScheduleEntry
                        //        {
                        //            ScheduleEntryId = new Guid(),
                        //            ShiftId = shift.ShiftId,
                        //            ScheduleStartDate = cycleDate.Add(shift.ShiftStartHour),
                        //        };

                        //        // Calculate and set schedule end date based on shift breaks
                        //        TimeSpan totalBreakIncludedDuration = shift.ShiftBreakDTOs
                        //            .Where(sb => sb.IncludedInShift)
                        //            .Select(sb => sb.ShiftBreakDuration)
                        //            .Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));

                        //        scheduleEntry.ScheduleEndDate = scheduleEntry.ScheduleStartDate.Add(shift.ShiftDuration).Add(totalBreakIncludedDuration);

                        //        // Add entry to the database
                        //        scheduleEntry = await _unitOfWork.EntityScheduleRepository.Add(scheduleEntry);

                        //        // Map it, add shift info and include entry to the list
                        //        ScheduleEntryDTO scheduleEntryDTO = _mapper.Map<ScheduleEntryDTO>(scheduleEntry);
                        //        scheduleEntryDTO.ShiftDTO = shift;
                        //        scheduleEntryDTO.ScheduleParticipants = new List<EntityWorkerMemberDTO>();
                        //        scheduleEntryDTOs.Add(scheduleEntryDTO);
                        //    }

                        //    // Increment to the next day
                        //    cycleDate = cycleDate.AddDays(1);
                        //}

                        #endregion

                        DateTime cycleDate = createEntityScheduleDTO.StartDate;

                        while (cycleDate <= createEntityScheduleDTO.EndDate)
                        {
                            bool isWeekend = cycleDate.DayOfWeek == DayOfWeek.Saturday || cycleDate.DayOfWeek == DayOfWeek.Sunday;

                            foreach (ShiftDTO shift in shifts)
                            {
                                if (isWeekend && !ruleDTOs.Any(i =>
                                    i.RuleTypeId.Equals(RuleTypeConstants.SHIFT_INCLUDES_WEEKENDS_ID) &&
                                    i.EntityRuleSpecificationDTOs.Any(j =>
                                        _generalService.ParseStringToGuid(j.AspectReferenceId).Equals(shift.ShiftId) && j.RuleSpecificationValue.Equals(1))))
                                {
                                    continue;
                                }

                                var scheduleEntry = new ScheduleEntry
                                {
                                    ScheduleEntryId = Guid.NewGuid(),
                                    ShiftId = shift.ShiftId,
                                    ScheduleStartDate = cycleDate.Add(shift.ShiftStartHour),
                                };

                                var totalBreakIncludedDuration = shift.ShiftBreakDTOs
                                    .Where(sb => sb.IncludedInShift)
                                    .Select(sb => sb.ShiftBreakDuration)
                                    .Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));

                                scheduleEntry.ScheduleEndDate = scheduleEntry.ScheduleStartDate
                                    .Add(shift.ShiftDuration)
                                    .Add(totalBreakIncludedDuration);

                                scheduleEntry = await _unitOfWork.EntityScheduleRepository.Add(scheduleEntry);

                                var scheduleEntryDTO = _mapper.Map<ScheduleEntryDTO>(scheduleEntry);
                                scheduleEntryDTO.ShiftDTO = shift;
                                scheduleEntryDTO.ScheduleParticipants = new List<ScheduleEntryParticipantDTO>();
                                scheduleEntryDTOs.Add(scheduleEntryDTO);

                                scheduleEntryDTOs = scheduleEntryDTOs.OrderBy(i => i.ScheduleStartDate).ToList();
                            }

                            // Move to the next day
                            cycleDate = cycleDate.AddDays(1);
                        }

                        #endregion

                        scheduleEntryDTOs = await FillOutSchedule(scheduleEntryDTOs, shifts, ruleDTOs, entityWorkerMemberDTOs, entityShiftRotations, entitySkills, createEntityScheduleDTO);

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

        #region Delete Schedule Entries

        public async Task<BaseResponse<bool>> DeleteScheduleEntry(Guid scheduleEntryId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Message = ScheduleRelatedMessages.DeleteScheduleEntryUnexpectedError;
            response.Success = false;

            if (scheduleEntryId != null)
            {
                IEnumerable<ScheduleEntryWorkers> scheduleEntryWorkers = await _unitOfWork.EntityScheduleWorkersRepository.GetScheduleEntryWorkers(scheduleEntryId);
                IEnumerable<ScheduleEntryBots> scheduleEntryBots = await _unitOfWork.ScheduleEntryBotsRepository.GetScheduleEntryBots(scheduleEntryId);

                if (scheduleEntryWorkers.Count() != 0)
                    await _unitOfWork.EntityScheduleWorkersRepository.DeleteRange(scheduleEntryWorkers);

                await _unitOfWork.EntityScheduleRepository.Delete(scheduleEntryId);
                response.Success = true;
                response.Message = ScheduleRelatedMessages.DeleteScheduleEntrySuccess;
            }

            return response;
        }

        #endregion

        #region Fill Out Schedule V1

        public async Task<List<ScheduleEntryDTO>> FillOutSchedule(
            List<ScheduleEntryDTO> scheduleEntryDTOs,
            List<ShiftDTO> shifts,
            List<EntityRuleDTO> entityRules,
            List<EntityWorkerMemberDTO> entityWorkerMemberDTOs,
            List<EntityShiftRotationDTO> entityShiftRotationDTOs,
            List<SkillLocalizedDTO> entitySkills,
            CreateEntityScheduleDTO createEntityScheduleDTO)
        {
            // 1. Preprocess ineligible workers (e.g., based on weekends off rule)
            List<ScheduleEntryIneligibility> scheduleEntryIneligibilities = new List<ScheduleEntryIneligibility>();

            // scheduleEntryIneligibilities = ApplyMonthlyWeekends(scheduleEntryDTOs, entityWorkerMemberDTOs, entityRules);

            DateTime trackingDay = scheduleEntryDTOs.Count != 0 ? scheduleEntryDTOs[0].ScheduleStartDate.Date : new DateTime();
            bool isFirstDay = true;


            // 2. Iterate over all schedule entries to assign workers
            for (int i = 0; i < scheduleEntryDTOs.Count; i++)
            {
                ScheduleEntryDTO scheduleEntry = scheduleEntryDTOs[i];

                // 2.1 Determine minimum skillset required
                List<Tuple<int, int>> requiredSkillQuantities = GetMinimumSkilletSetPerShift(entityRules, scheduleEntry);

                // Fallback: if no skills are defined, allow any eligible worker
                if (requiredSkillQuantities == null || !requiredSkillQuantities.Any())
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
                    scheduleEntryIneligibilities);

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

                        if(createEntityScheduleDTO.SingleRoleResponsibility && skillId != -1)
                        {
                            // Assign worker
                            assigned.Add(new ScheduleEntryParticipantDTO
                            {
                                Worker = worker,
                                AssignedSkills = new List<SkillLocalizedDTO>()
                            });
                        }

                        else
                        {
                            assigned.Add(new ScheduleEntryParticipantDTO
                            {
                                Worker = worker
                            });
                        }


                        skillAssigned++;
                        eligibleWorkers.Remove(worker); // Remove to prevent duplication

                        scheduleEntryIneligibilities = ValidatePostSelectionIneligibilities(worker, scheduleEntry, scheduleEntryDTOs, scheduleEntryIneligibilities, shifts, entityShiftRotationDTOs, entityRules);

                        if (skillId != -1 && quantity != 0 && skillAssigned >= quantity)
                            break;

                    }

                    if (quantity != 0 && assigned.Count >= quantity)
                        break;
                }

                scheduleEntry.ScheduleParticipants = assigned;

                //Check if there is a next entry and its a different day from the one being tracked // isFirstDay &&
                //if ( i + 1 < scheduleEntryDTOs.Count && scheduleEntryDTOs[i + 1].ScheduleStartDate.Date != trackingDay)
                //{
                //    Tuple<List<ScheduleEntryDTO>, List<ScheduleEntryIneligibility>> refillResults = await RefillDailies(scheduleEntryDTOs, scheduleEntryDTOs[i].ScheduleStartDate.Date, entityWorkerMemberDTOs, shifts, entityRules, entityShiftRotationDTOs, scheduleEntryIneligibilities, createEntityScheduleDTO);
                //    // isFirstDay = false;
                //    trackingDay = scheduleEntryDTOs[i + 1].ScheduleStartDate.Date;
                //    scheduleEntryDTOs = refillResults.Item1;
                //    scheduleEntryIneligibilities = refillResults.Item2;
                //}
            }

            return scheduleEntryDTOs;
        }

        #endregion

        #region Refill Dailies

        private async Task<Tuple<List<ScheduleEntryDTO>, List<ScheduleEntryIneligibility>>> RefillDailies(
            List<ScheduleEntryDTO> scheduleEntries,
            DateTime dayToRefill,
            List<EntityWorkerMemberDTO> entityWorkerMemberDTOs,
            List<ShiftDTO> shifts,
            List<EntityRuleDTO> entityRules,
            List<EntityShiftRotationDTO> shiftRotationDTOs,
            List<ScheduleEntryIneligibility> scheduleEntryIneligibilities,
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
                    var filtered = FilterEligibleWorkers(scheduleEntry, entityRules, new List<EntityWorkerMemberDTO> { worker }, isRotation, scheduleEntryIneligibilities);
                    if (!filtered.Any())
                        continue;

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
                    scheduleEntryIneligibilities = ValidatePostSelectionIneligibilities(
                        worker,
                        scheduleEntry,
                        scheduleEntries,
                        scheduleEntryIneligibilities,
                        shifts,
                        shiftRotationDTOs,
                        entityRules
                    );

                    //dayEntries = dayEntries.OrderBy(i => i.ScheduleParticipants.Count)
                    //    .ThenBy(i => i.ScheduleStartDate).ToList();

                    //i = 0;

                    if (!worker.MultipleShiftAssignments)
                        break;
                }
            }

            return Tuple.Create(scheduleEntries, scheduleEntryIneligibilities);
        }


        private async Task<Tuple<List<ScheduleEntryDTO>, List<ScheduleEntryIneligibility>>> _RefillDailies(List<ScheduleEntryDTO> scheduleEntries,
            DateTime dayToRefill,
            List<EntityWorkerMemberDTO> entityWorkerMemberDTOs,
            List<ShiftDTO> shifts,
            List<EntityRuleDTO> entityRules,
            List<EntityShiftRotationDTO> shiftRotationDTOs,
            List<ScheduleEntryIneligibility> scheduleEntryIneligibilities,
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
                    bool eligible = FilterEligibleWorkers(scheduleEntry, entityRules, new List<EntityWorkerMemberDTO> { worker }, isRotation, scheduleEntryIneligibilities).Count != 0;

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

                    scheduleEntryIneligibilities = ValidatePostSelectionIneligibilities(worker, scheduleEntry, scheduleEntries, scheduleEntryIneligibilities, shifts, shiftRotationDTOs, entityRules);
                }
            }

            // foreach (entityWo)
            // FOR WORKER
            // FOR SCHEDULE
            // if assigned, shuffle order

            return new Tuple<List<ScheduleEntryDTO>, List<ScheduleEntryIneligibility>>(scheduleEntries, scheduleEntryIneligibilities);
        }

        #endregion

        private bool IsWorkerEligible(ScheduleEntryDTO scheduleEntry, EntityWorkerMemberDTO worker)
        {
            bool result = false;



            result = true;

            return result;
        }

        #region Check Consecutive Turns

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

        #region Validate Post Selection Ineligibilities

        private List<ScheduleEntryIneligibility> ValidatePostSelectionIneligibilities(EntityWorkerMemberDTO worker,
            ScheduleEntryDTO scheduleEntry,
            List<ScheduleEntryDTO> scheduleEntryDTOs,
            List<ScheduleEntryIneligibility> scheduleEntryIneligibilities,
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
                scheduleEntryIneligibilities = ApplySpecificInegibilities(currentDayEntries, worker, scheduleEntryIneligibilities);
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

                    scheduleEntryIneligibilities = ApplySpecificInegibilities(nextIneligibleEntries, worker, scheduleEntryIneligibilities);
                }

                // Its a mandatory leave
                else
                {
                    DateTime leaveStart = scheduleEntry.ScheduleEndDate;
                    DateTime leaveEnd = leaveStart.Add(nextStepOfRotation.LeaveDuration);

                    IEnumerable<ScheduleEntryDTO> mandatoryLeaveEntries = scheduleEntryDTOs
                            .Where(i => i.ScheduleStartDate >= scheduleEntry.ScheduleStartDate && i.ScheduleStartDate <= leaveEnd);

                    scheduleEntryIneligibilities = ApplySpecificInegibilities(mandatoryLeaveEntries, worker, scheduleEntryIneligibilities);
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

        #region Check And Apply Ineligiblities For Consecutive Entries

        private List<ScheduleEntryIneligibility> CheckAndApplyIneligiblitiesForConsecutiveEntries(List<EntityRuleDTO> entityRules, EntityWorkerMemberDTO worker, List<ScheduleEntryDTO> scheduleEntryDTOs, ScheduleEntryDTO scheduleEntry, List<ScheduleEntryIneligibility> scheduleEntryIneligibilities)
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

                                        scheduleEntryIneligibilities = ApplySpecificInegibilities(mandatoryLeaveEntries, worker, scheduleEntryIneligibilities);

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

        #region Apply Specific Inegibilities

        private List<ScheduleEntryIneligibility> ApplySpecificInegibilities(IEnumerable<ScheduleEntryDTO> entriesToDenyWorker, EntityWorkerMemberDTO worker, List<ScheduleEntryIneligibility> scheduleEntrysIneligibilities)
        {
            // For each entry
            foreach (ScheduleEntryDTO schedule in entriesToDenyWorker)
            {
                // Check for existing ineligibility for this entry
                ScheduleEntryIneligibility scheduleEntryIneligibility = scheduleEntrysIneligibilities.Where(i => i.ScheduleEntryID.Equals(schedule.ScheduleEntryId)).FirstOrDefault();

                // If there is no entry yet
                if (scheduleEntryIneligibility == null)
                {
                    // Create a new one and add assigned worker to the list of Ineligible
                    scheduleEntryIneligibility = new ScheduleEntryIneligibility(schedule.ScheduleEntryId, schedule.ScheduleStartDate);
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

        #region Apply Monthly Weekends

        private List<ScheduleEntryIneligibility> ApplyMonthlyWeekends(List<ScheduleEntryDTO> scheduleEntryDTOs, List<EntityWorkerMemberDTO> entityWorkerMembers, List<EntityRuleDTO> entityRuleDTOs)
        {
            List<ScheduleEntryIneligibility> scheduleEntryIneligibilities = new List<ScheduleEntryIneligibility>();

            #region Filter Non Rotationers and non weekend workers

            // Check any workers that are not part of rotation and not working weekends
            if (entityWorkerMembers != null && entityWorkerMembers.Any(i => !i.PartOfRotation && !i.WorksWeekends))
            {
                // Get any schedule entry on the weekends
                IEnumerable<ScheduleEntryDTO> weekEndEntries = scheduleEntryDTOs.Where(j => j.ScheduleStartDate.DayOfWeek == DayOfWeek.Saturday || j.ScheduleStartDate.DayOfWeek == DayOfWeek.Sunday);
                foreach (ScheduleEntryDTO weekEndEntry in weekEndEntries)
                {
                    ScheduleEntryIneligibility ineligibility = new ScheduleEntryIneligibility(weekEndEntry.ScheduleEntryId, weekEndEntry.ScheduleStartDate);
                    ineligibility.MembersIneligible = entityWorkerMembers.Where(i => i.PartOfRotation.Equals(false) && i.WorksWeekends.Equals(false)).ToList();

                    scheduleEntryIneligibilities.Add(ineligibility);
                }

                // Remove any workers that are not part of rotation and not working weekends
                entityWorkerMembers.RemoveAll(i => i.PartOfRotation.Equals(false) && i.WorksWeekends.Equals(false));
            }

            #endregion

            // Min Weekends per Month Rule
            EntityRuleDTO entityRuleDTO = entityRuleDTOs.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MIN_WEEKENDS_OFF_MONTH_ID)).FirstOrDefault();
            if (entityRuleDTO != null)
            {
                // Filter weekend entries
                // IEnumerable<ScheduleEntryDTO> weekendEntries = scheduleEntryDTOs.Where(i => i.ScheduleStartDate.DayOfWeek == DayOfWeek.Saturday || i.ScheduleStartDate.DayOfWeek == DayOfWeek.Sunday).ToList();

                // Get number of weekends off a worker must have per month
                int numberOfWorkerWeekendsOff = (int)entityRuleDTO.EntityRuleSpecificationDTOs[0].RuleSpecificationValue;


                if (numberOfWorkerWeekendsOff > 0)
                {
                    var weekends = GetDistinctWeekendsInMonth(scheduleEntryDTOs);
                    var weekendGroups = weekends.ToDictionary(w => w, w =>
                        scheduleEntryDTOs.Where(e =>
                            e.ScheduleStartDate.Date == w.Item1 || e.ScheduleStartDate.Date == w.Item2).ToList());

                    // Track how many weekends each worker is participating in
                    Dictionary<string, int> workerWeekendCount = entityWorkerMembers
                        .ToDictionary(w => w.WorkerId, w => 0);

                    foreach (var weekend in weekendGroups)
                    {
                        var weekendEntries = weekend.Value;

                        // Gather all unique worker IDs participating this weekend
                        var weekendWorkerIds = weekendEntries
                            .SelectMany(e => e.ScheduleParticipants)
                            .Select(p => p.Worker.WorkerId)
                            .Distinct();

                        foreach (var workerId in weekendWorkerIds)
                        {
                            if (workerWeekendCount.ContainsKey(workerId))
                            {
                                workerWeekendCount[workerId]++;
                            }
                        }
                    }

                    foreach (var kvp in workerWeekendCount)
                    {
                        int weekendsWorked = kvp.Value;
                        int weekendsOff = weekends.Count - weekendsWorked;

                        if (weekendsOff < numberOfWorkerWeekendsOff)
                        {
                            // Worker is over-assigned — find their weekend entries
                            var offendingEntries = scheduleEntryDTOs
                                .Where(e =>
                                    (e.ScheduleStartDate.DayOfWeek == DayOfWeek.Saturday || e.ScheduleStartDate.DayOfWeek == DayOfWeek.Sunday) &&
                                    e.ScheduleParticipants.Any(p => p.Worker.WorkerId == kvp.Key))
                                .ToList();

                            foreach (var entry in offendingEntries)
                            {
                                var ineligibility = scheduleEntryIneligibilities.FirstOrDefault(i => i.ScheduleEntryID == entry.ScheduleEntryId);
                                if (ineligibility == null)
                                {
                                    ineligibility = new ScheduleEntryIneligibility(entry.ScheduleEntryId, entry.ScheduleStartDate);
                                    scheduleEntryIneligibilities.Add(ineligibility);
                                }

                                var worker = entry.ScheduleParticipants.FirstOrDefault(p => p.Worker.WorkerId == kvp.Key);
                                if (worker != null && !ineligibility.MembersIneligible.Any(w => w.WorkerId == worker.Worker.WorkerId))
                                {
                                    ineligibility.MembersIneligible.Add(worker.Worker);
                                }
                            }
                        }
                    }

                }
            }

            return scheduleEntryIneligibilities;
        }

        #endregion

        #region Validate Worker Selection

        /// <summary>
        /// Validates the worker selection for a current schedule entry
        /// </summary>
        /// <param name="currentScheduleEntry"></param>
        /// <param name="scheduleEntryDTOs"></param>
        /// <param name="entityWorkerMemberDTO"></param>
        /// <param name="entityRules"></param>
        /// <param name="shiftDTOs"></param>
        /// <param name="createEntityScheduleDTO"></param>
        /// <returns></returns>
        private async Task<bool> ValidateWorkerSelection(ScheduleEntryDTO currentScheduleEntry, List<ScheduleEntryDTO> scheduleEntryDTOs, EntityWorkerMemberDTO entityWorkerMemberDTO, List<EntityRuleDTO> entityRules, List<ShiftDTO> shiftDTOs, CreateEntityScheduleDTO createEntityScheduleDTO)
        {
            // Get Max Daily hours rule
            EntityRuleDTO maxDailyHoursRule = entityRules.FirstOrDefault((i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_HOURS_DAY_ID)));

            IEnumerable<ScheduleEntryDTO> presentDayEntries = scheduleEntryDTOs.Where(i => i.ScheduleEndDate.Date.Equals(currentScheduleEntry.ScheduleEndDate.Date) && i.ScheduleParticipants.Any(j => j.Worker.WorkerId.Equals(entityWorkerMemberDTO.WorkerId)));
            // Checks Max Hours per day
            if (maxDailyHoursRule != null && !CheckMaxHoursPerDay(currentScheduleEntry, presentDayEntries, entityWorkerMemberDTO, maxDailyHoursRule, shiftDTOs))
                return false;

            // Get Max Weekly Hours rule
            EntityRuleDTO maxWeeklyHoursRule = entityRules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_HOURS_WEEK_ID)).FirstOrDefault();

            List<ScheduleEntryDTO> weekEntries = await FilterWeeklySessions(currentScheduleEntry, scheduleEntryDTOs, createEntityScheduleDTO);
            // Check Max Hours per Week
            if (maxWeeklyHoursRule != null && !CheckMaxHoursPerWeek(weekEntries, entityWorkerMemberDTO, maxWeeklyHoursRule, shiftDTOs))
                return false;

            // Check for turns of the same type
            EntityRuleDTO limitOfTurnsRule = entityRules.FirstOrDefault(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_CONSECUTIVE_SHIFTS_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(currentScheduleEntry.ShiftId)));

            if (limitOfTurnsRule != null)
            {
                List<ScheduleEntryDTO> previousShiftEntries = await FilterEntriesForConsecutiveTurns(currentScheduleEntry, scheduleEntryDTOs, createEntityScheduleDTO, limitOfTurnsRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue);
                if (limitOfTurnsRule != null && !CheckForTurnsOfTheSameType(currentScheduleEntry, previousShiftEntries, entityWorkerMemberDTO, limitOfTurnsRule, shiftDTOs))
                    return false;
            }

            // Check if worker has performed a shift and requires rest
            EntityRuleDTO postShiftRestRule = entityRules.FirstOrDefault(i => i.RuleTypeId.Equals(RuleTypeConstants.POST_SHIFT_REST_HOURS_ID));
            if (limitOfTurnsRule != null && !CheckForPostShiftRest(currentScheduleEntry, scheduleEntryDTOs, entityWorkerMemberDTO, postShiftRestRule))
                return false;

            return true;
        }

        #endregion

        #region Check For Turns of the same Type

        /// <summary>
        /// Checks if worker is part of too many turns of the same type
        /// </summary>
        /// <param name="currentScheduleEntry"></param>
        /// <param name="previousShiftEntries"></param>
        /// <param name="entityWorkerMemberDTO"></param>
        /// <param name="limitOfTurnsRule"></param>
        /// <param name="shiftDTOs"></param>
        /// <returns></returns>
        private bool CheckForTurnsOfTheSameType(ScheduleEntryDTO currentScheduleEntry, IEnumerable<ScheduleEntryDTO> previousShiftEntries, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO limitOfTurnsRule, List<ShiftDTO> shiftDTOs)
        {
            if (limitOfTurnsRule != null && limitOfTurnsRule.EntityRuleSpecificationDTOs.Count != 0)
            {
                EntityRuleSpecificationDTO entityRuleSpecificationDTO = limitOfTurnsRule.EntityRuleSpecificationDTOs.FirstOrDefault();

                // If there is a specification 
                if (entityRuleSpecificationDTO.RuleSpecificationValue != 0)
                {
                    // Sort previous shift entries by ScheduleStartDate (latest first)
                    var sortedPreviousShifts = previousShiftEntries
                        .OrderByDescending(i => i.ScheduleStartDate)
                        .ToList();

                    int consecutiveShiftsCount = 1; // Start with 1 since current shift counts

                    // Iterate through the sorted previous shifts
                    for (int i = 0; i < sortedPreviousShifts.Count - 1; i++)
                    {
                        var currentShift = sortedPreviousShifts[i];
                        var nextShift = sortedPreviousShifts[i + 1];

                        // Check if the current shift ends just before or overlaps the next shift
                        if (currentShift.ScheduleEndDate >= nextShift.ScheduleStartDate)
                        {
                            // Increase the consecutive shifts count
                            consecutiveShiftsCount++;
                        }
                        else
                        {
                            // If there's a gap, break the consecutive chain
                            break;
                        }

                        // If consecutive shifts exceed the allowed limit, return false
                        if (consecutiveShiftsCount > entityRuleSpecificationDTO.RuleSpecificationValue)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        #endregion

        #region Is Min Workers Per Shift

        /// <summary>
        /// Checks if schedule entry has the minimum number of workers required for the shift
        /// This only checks if there is a rule for minimum workers per shift
        /// Otherwise will always be true
        /// </summary>
        /// <param name="entityRuleDTOs"></param>
        /// <param name="scheduleEntryDTO"></param>
        /// <returns></returns>
        private bool IsMinWorkersPerShift(List<EntityRuleDTO> entityRuleDTOs, ScheduleEntryDTO scheduleEntryDTO)
        {
            bool validationResult = true;

            EntityRuleDTO entityRuleDTO = entityRuleDTOs.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MIN_WORKERS_SHIFT_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();
            if (entityRuleDTO != null)
            {
                validationResult = scheduleEntryDTO.ScheduleParticipants.Count >= entityRuleDTO.EntityRuleSpecificationDTOs.First().RuleSpecificationValue;
            }

            return validationResult;
        }

        #endregion

        #region Filter Workers For Required Skillset

        /// <summary>
        /// Filters and returns a list of valid workers in case of a required skillset for a shift
        /// </summary>
        /// <param name="scheduleEntryDTO"></param>
        /// <param name="entityRulesDTO"></param>
        /// <param name="entityWorkerMembers"></param>
        /// <param name="isShiftRotation">Flag that indicates to filter by members who are part of the rotation</param>
        /// <returns></returns>
        private List<EntityWorkerMemberDTO> FilterEligibleWorkers(ScheduleEntryDTO scheduleEntryDTO, List<EntityRuleDTO> entityRulesDTO, List<EntityWorkerMemberDTO> entityWorkerMembers, bool isShiftRotation, List<ScheduleEntryIneligibility> scheduleEntryIneligibilities)
        {
            List<EntityWorkerMemberDTO> filteredWorkers = entityWorkerMembers;

            bool isWeekend = scheduleEntryDTO.ScheduleStartDate.DayOfWeek == DayOfWeek.Saturday
                || scheduleEntryDTO.ScheduleStartDate.DayOfWeek == DayOfWeek.Sunday;


            // Filter for rules regarding the quantity of skillset
            List<EntityRuleDTO> entityRuleDTOs = entityRulesDTO
                .Where(i => (i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_ID)
                || i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKDAYS_ID)
                || i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKENDS_ID))
                && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId))).ToList();

            if (entityRuleDTOs.Count != 0)
            {
                // Get all skill identifiers required for the specifications
                List<int> wantedSkillset = (List<int>)entityRuleDTOs.SelectMany(i => i.EntityRuleSpecificationDTOs)
                    .Select(j => j.AspectReferenceId);

                // List<Tuple<int, int>> minSkillsetPerShift = GetMinimumSkilletSetPerShift(entityRulesDTO, scheduleEntryDTO);

                // Filter list of workers valid for this entry
                filteredWorkers = filteredWorkers
                    .Where(worker => worker.SkillSet.Any(skill => wantedSkillset.Contains(skill.SkillId)))
                    .ToList();
            }

            // If this is a shift not part of rotation
            // Filter out workers that are part of the rotation
            if (!isShiftRotation)
            {
                filteredWorkers = filteredWorkers
                    .Where(worker => worker.PartOfRotation.Equals(false))
                    .ToList();
            }
            else
            {
                // If this is a shift part of rotation
                // Filter out workers that are part of the rotation or that are assigned to this shift
                filteredWorkers = filteredWorkers
                    .Where(worker => worker.PartOfRotation.Equals(true) || worker.AssignedShifts.Any(j => j.ShiftId.Equals(scheduleEntryDTO.ShiftId)))
                    .ToList();
            }

            // Keep workers that are part of the rotation that work regardless of the day
            // Filter out workers that are not available on weekdays
            if (!isWeekend)
            {
                // Filter out workers that are not available on weekdays
                filteredWorkers = filteredWorkers
                    .Where(worker => worker.PartOfRotation || worker.WorksWeekDays.Equals(true))
                    .ToList();
            }
            // If this is a weekend
            else
            {
                // Filter out workers that are not available on weekends
                filteredWorkers = filteredWorkers
                    .Where(worker => worker.PartOfRotation || worker.WorksWeekends.Equals(true))
                    .ToList();
            }

            // Check for ineligibilities
            if (scheduleEntryIneligibilities != null && scheduleEntryIneligibilities.Count != 0)
            {
                // Filter out workers that are not eligible for this entry
                filteredWorkers = filteredWorkers
                    .Where(worker => !scheduleEntryIneligibilities.Any(i => i.MembersIneligible.Any(j => j.WorkerId.Equals(worker.WorkerId)) && i.ScheduleEntryID.Equals(scheduleEntryDTO.ScheduleEntryId)))
                    .ToList();
            }

            return filteredWorkers;
        }

        #endregion

        #region Return Max Workers Per Shift

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

        #region Get Minimum Skill Set Per Shift

        private List<Tuple<int, int>> GetMinimumSkilletSetPerShift(List<EntityRuleDTO> entityRuleDTOs, ScheduleEntryDTO scheduleEntryDTO)
        {
            List<Tuple<int, int>> minSkillQuantity = new List<Tuple<int, int>>();

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

        #region Is Skill Set Fullfilled

        private bool IsSkillSetFullfilled(List<EntityWorkerMemberDTO> assignedWorkers, List<Tuple<int, int>> quantityPerSkillset)
        {
            // For each required skillset and quantity
            foreach (Tuple<int, int> tuple in quantityPerSkillset)
            {
                // Get the number of workers assigned with the specific skill
                int assignedSkillCount = assignedWorkers.Count(i => i.SkillSet.Any(j => j.SkillId == tuple.Item1));

                // If the number of assigned workers with this skill is less than required, return false
                if (assignedSkillCount < tuple.Item2)
                    return false;
            }

            // All required skillsets are fulfilled
            return true;
        }

        #endregion

        #region Check Max Hours Per Day

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

        #region Check Max Hours per Week

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

        #region Check Average Hours Per Week 

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

        #region Check Average Hours Per Month

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

        #region Filter Weekly Sessions

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

                    List<ScheduleEntryDTO> previousEntries = await GetScheduleEntries(weekRequest);

                    // Get Schedule entries from Start of Week Date to Start Date
                    filteredWeeklyEntries = filteredWeeklyEntries.Concat(previousEntries).ToList();
                }
            }

            filteredWeeklyEntries = filteredWeeklyEntries.Concat(scheduleEntryDTOs.Where(i => i.ScheduleStartDate > currentScheduleEntry.ScheduleStartDate.AddDays(backtrackDaysQty) && i.ScheduleEndDate < currentScheduleEntry.ScheduleStartDate.AddDays(forwardDaysQty))).ToList();

            return filteredWeeklyEntries;
        }

        #endregion

        #region Filter Monthly Entries

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

                filteredMontlyEntries.AddRange(await GetScheduleEntries(request));
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

                filteredMontlyEntries.AddRange(await GetScheduleEntries(request));
            }

            // Remove duplicates (just in case)
            return filteredMontlyEntries
                .GroupBy(e => e.ScheduleEntryId)
                .Select(g => g.First())
                .ToList();
        }


        #endregion

        #region Check For Post Shift Rest

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

        #region Filter Entries for Consecutive Turns

        /// <summary>
        /// Checks and filters entries both to be generated or existing in the db
        /// for consecutive turns validations
        /// </summary>
        /// <param name="currentScheduleEntry"></param>
        /// <param name="scheduleEntryDTOs"></param>
        /// <param name="createEntityScheduleDTO"></param>
        /// <param name="ruleSpecificationValue"></param>
        /// <returns></returns>
        private async Task<List<ScheduleEntryDTO>> FilterEntriesForConsecutiveTurns(ScheduleEntryDTO currentScheduleEntry, List<ScheduleEntryDTO> scheduleEntryDTOs, CreateEntityScheduleDTO createEntityScheduleDTO, float ruleSpecificationValue)
        {
            List<ScheduleEntryDTO> filteredEntries = new List<ScheduleEntryDTO>();

            // Get the previous shift entries
            IEnumerable<ScheduleEntryDTO> previousShiftEntries = scheduleEntryDTOs.Where(i => i.ScheduleStartDate < currentScheduleEntry.ScheduleStartDate && i.ShiftId.Equals(currentScheduleEntry.ShiftId)).OrderByDescending(i => i.ScheduleStartDate);

            filteredEntries = filteredEntries.Concat(previousShiftEntries).ToList();

            // if there are entries and are greater than the rule specification value
            if (ruleSpecificationValue != 0 && previousShiftEntries.Count() > 0 && previousShiftEntries.Count() < ruleSpecificationValue)
            {
                int daysToGoBack = (int)(ruleSpecificationValue - previousShiftEntries.Count());
                DateTime startDate = previousShiftEntries.Last().ScheduleStartDate.AddDays(-daysToGoBack);
                DateTime endDate = previousShiftEntries.Last().ScheduleStartDate;

                ScheduleViewModelRequestDTO requestDTO = new ScheduleViewModelRequestDTO
                {
                    EntityId = createEntityScheduleDTO.EntityId,
                    WorkerId = createEntityScheduleDTO.WorkerId,
                    LanguageCode = createEntityScheduleDTO.LanguageCode,
                    StartDateSearch = startDate,
                    EndDateSearch = endDate
                };

                filteredEntries = filteredEntries.Concat(await GetScheduleEntries(requestDTO)).ToList();
            }

            return filteredEntries.OrderByDescending(i => i.ScheduleStartDate).ToList();
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

        #region AUX : Get Distinct Weekends In Month

        private List<Tuple<DateTime, DateTime>> GetDistinctWeekendsInMonth(List<ScheduleEntryDTO> entries)
        {
            var weekends = new HashSet<Tuple<DateTime, DateTime>>();

            foreach (var entry in entries)
            {
                if (entry.ScheduleStartDate.DayOfWeek == DayOfWeek.Saturday ||
                    entry.ScheduleStartDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    var date = entry.ScheduleStartDate.Date;
                    var saturday = date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Saturday);
                    var sunday = saturday.AddDays(1);
                    weekends.Add(new Tuple<DateTime, DateTime>(saturday, sunday));
                }
            }

            return weekends.OrderBy(w => w.Item1).ToList();
        }

        #endregion

        #region NOT USED

        private List<ShiftPriorities> CheckShiftPriorities(List<ShiftDTO> shifts, List<EntityRuleDTO> rules)
        {
            List<ShiftPriorities> shiftPriorities = new List<ShiftPriorities>();

            foreach (ShiftDTO shift in shifts)
            {
                ShiftPriorities shiftPriority = new ShiftPriorities(shift.ShiftId);

                // Max Workers
                EntityRuleDTO maxWorkersRule = rules
                    .Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_WORKERS_SHIFT_ID)
                    && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(shift.ShiftId.ToString()))).FirstOrDefault();

                if (maxWorkersRule != null)
                    shiftPriority.MaxWorkers = (int)maxWorkersRule.EntityRuleSpecificationDTOs[0].RuleSpecificationValue;

                // Min Workers
                EntityRuleDTO minWorkersRule = rules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MIN_WORKERS_SHIFT_ID)
                && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(shift.ShiftId.ToString()))).FirstOrDefault();

                if (minWorkersRule != null)
                    shiftPriority.MinWorkers = (int)minWorkersRule.EntityRuleSpecificationDTOs[0].RuleSpecificationValue;

                // List<EntityRuleDTO> skillsetRules = 

                List<EntityRuleDTO> skillSetRules = rules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_SKILLSET_SHIFT_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(shift.ShiftId.ToString()))
                    || i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(shift.ShiftId.ToString()))
                    || i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKDAYS_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(shift.ShiftId.ToString()))
                    || i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKENDS_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(shift.ShiftId.ToString()))).ToList();

                if (skillSetRules.Count > 0)
                    shiftPriority.SkillSetSpecsCount = skillSetRules.Count;

                shiftPriorities.Add(shiftPriority);
            }

            return shiftPriorities;
        }

        // Not Used
        private List<Tuple<DateTime, DateTime>> GetWeekendsInIntervals(IEnumerable<ScheduleEntryDTO> entryDTOs)
        {
            List<Tuple<DateTime, DateTime>> weekendPairs = new List<Tuple<DateTime, DateTime>>();

            DateTime? cachedDateOne = null;
            DateTime? cachedDateTwo = null;

            foreach (ScheduleEntryDTO entry in entryDTOs.OrderBy(e => e.ScheduleStartDate))
            {
                if (weekendPairs.Any(i => i.Item1.Date.Equals(entry.ScheduleStartDate.Date) || i.Item2.Date.Equals(entry.ScheduleStartDate.Date))) { continue; }

                else
                {
                    // First Date 
                    if (cachedDateOne == null)
                    {
                        cachedDateOne = entry.ScheduleStartDate;
                        continue;
                    }
                    // Its the same day
                    else if (entry.ScheduleStartDate.Date.Equals(cachedDateOne?.Date))
                    {
                        continue;
                    }
                    // if this entry is on the next day of previously cached
                    else if (entry.ScheduleStartDate.Date.Equals(cachedDateOne?.Date.AddDays(1)))
                    {
                        cachedDateTwo = entry.ScheduleStartDate;
                        weekendPairs.Add(new Tuple<DateTime, DateTime>((DateTime)cachedDateOne?.Date, entry.ScheduleStartDate.Date));

                        cachedDateOne = new DateTime();
                        cachedDateTwo = new DateTime();
                        continue;
                    }
                    // Its beyond the current weekend
                    else
                    {
                        weekendPairs.Add(new Tuple<DateTime, DateTime>((DateTime)cachedDateOne, (DateTime)cachedDateOne));
                        cachedDateOne = new DateTime();
                        continue;
                    }
                }
            }
            return weekendPairs;
        }



        #endregion

        #endregion
    }
}
