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

            ScheduleEntry scheduleEntry = await  _unitOfWork.EntityScheduleRepository.GetById(scheduleEntryId);
            ShiftDTO shift = await _shiftService.GetShiftById(_generalService.ParseStringToGuid(scheduleEntry.ShiftId.ToString()), languageCode);

            // Get respective participants of said schedule entry
            IEnumerable<ScheduleEntryWorkers> scheduleEntryWorkers = await _unitOfWork.EntityScheduleWorkersRepository.GetScheduleEntryWorkers(scheduleEntry.ScheduleEntryId.ToString());

            // Extract participant ids
            List<string> participantsIds = scheduleEntryWorkers.Select(i => i.ApplicationUserId).ToList();

            scheduleEntryDTO = _mapper.Map<ScheduleEntryDTO>(scheduleEntry);

            scheduleEntryDTO.ShiftDTO = shift;
            scheduleEntryDTO.ScheduleParticipants = await _entityService.GetEntityMembersByList(shift.EntityId, participantsIds, languageCode);

            return scheduleEntryDTO;
        }

        #endregion

        #region Create Entity Schedule

        public async Task<BaseResponse<List<ScheduleEntryDTO>>> CreateEntitySchedule(CreateEntityScheduleDTO createEntityScheduleDTO)
        {
            BaseResponse<List<ScheduleEntryDTO>> response = new BaseResponse<List<ScheduleEntryDTO>>();
            response.Message = "";
            response.Success = false;

            if(createEntityScheduleDTO != null && createEntityScheduleDTO.EntityId != Guid.Empty && !string.IsNullOrEmpty(createEntityScheduleDTO.WorkerId))
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
                        foreach (ScheduleEntry scheduleEntry in scheduleEntries )
                        {
                            BaseResponse<bool> deleteScheduleResponse = await DeleteScheduleEntry(scheduleEntry.ScheduleEntryId.ToString());

                            if(!deleteScheduleResponse.Success) 
                            {
                                response.Message = deleteScheduleResponse.Message;
                                await _unitOfWork.RollbackAsync();
                                return response;
                            }
                        }

                        #endregion

                        #region Get Relevant Data

                        // Get date differential between start and end date
                        TimeSpan dateDifference = createEntityScheduleDTO.EndDate - createEntityScheduleDTO.StartDate;
                        DateTime cycleDate = createEntityScheduleDTO.StartDate;

                        List<ShiftDTO> shifts = new List<ShiftDTO>();
                        List<EntityRuleDTO> ruleDTOs = new List<EntityRuleDTO>();
                        List<EntityWorkerMemberDTO> entityWorkerMemberDTOs = new List<EntityWorkerMemberDTO>();

                        // Get Shifts
                        if (createEntityScheduleDTO.FilteredShifts.Count() == 0)
                            shifts = await _shiftService.GetEntityShifts(createEntityScheduleDTO.EntityId) as List<ShiftDTO>;
                        else
                            shifts = await _shiftService.GetSpecificShifts(createEntityScheduleDTO.FilteredShifts);

                        // Get Base Entity Rules TBD
                        // List<EntityRuleDTO> baseEntityRuleDTOs = await _baseEntityRuleService.GetBaseEntityRulesAsEntityRules(createEntityScheduleDTO.LanguageCode);

                        // Get Rules
                        if(createEntityScheduleDTO.FilteredRules.Count() == 0)
                            ruleDTOs = await _entityRuleService.GetEntityRules(createEntityScheduleDTO.EntityId, createEntityScheduleDTO.LanguageCode);
                        else
                            ruleDTOs = await _entityRuleService.GetSpecificRules(createEntityScheduleDTO.FilteredRules, createEntityScheduleDTO.LanguageCode);

                        // Add missing Base Entity Rules to ruleDTOs
                        //foreach (var baseEntityRule in baseEntityRuleDTOs)
                        //{
                        //    if (!ruleDTOs.Any(r => r.RuleTypeId == baseEntityRule.RuleTypeId))
                        //    {
                        //        ruleDTOs.Add(baseEntityRule);
                        //    }
                        //}

                        #endregion

                        #region Get Shift Rotation

                        List<EntityShiftRotationDTO> entityShiftRotations = await _shiftService.GetEntityShiftRotations(createEntityScheduleDTO.EntityId);

                        #endregion

                        #region Create Shift Entries

                        // For each day
                        for (int i = 0; i < dateDifference.Days; i++)
                        {
                            // Check if its the weekend
                            bool isWeekend = cycleDate.DayOfWeek == DayOfWeek.Saturday || cycleDate.DayOfWeek == DayOfWeek.Sunday;
                            // For each shift
                            foreach(ShiftDTO shift in shifts)
                            {
                                // If there isnt a rule to include this shift on the weekends
                                if(isWeekend && !ruleDTOs.Any(i => i.RuleTypeId.Equals(RuleTypeConstants.SHIFT_INCLUDES_WEEKENDS_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(shift.ShiftId) && j.RuleSpecificationValue.Equals(1))))
                                {
                                    continue;
                                }

                                // Create Schedule Entry
                                ScheduleEntry scheduleEntry = new ScheduleEntry
                                {
                                    ScheduleEntryId = new Guid(),
                                    ShiftId = shift.ShiftId,
                                    ScheduleStartDate = cycleDate.Add(shift.ShiftStartHour),
                                };

                                // Calculate and set schedule end date based on shift breaks
                                TimeSpan totalBreakIncludedDuration = shift.ShiftBreakDTOs
                                    .Where(sb => sb.IncludedInShift)
                                    .Select(sb => sb.ShiftBreakDuration)
                                    .Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));

                                scheduleEntry.ScheduleEndDate = scheduleEntry.ScheduleStartDate.Add(shift.ShiftDuration).Add(totalBreakIncludedDuration);

                                // Add entry to the database
                                scheduleEntry = await _unitOfWork.EntityScheduleRepository.Add(scheduleEntry);

                                // Map it, add shift info and include entry to the list
                                ScheduleEntryDTO scheduleEntryDTO = _mapper.Map<ScheduleEntryDTO>(scheduleEntry);
                                scheduleEntryDTO.ShiftDTO = shift;
                                scheduleEntryDTO.ScheduleParticipants = new List<EntityWorkerMemberDTO>();
                                scheduleEntryDTOs.Add(scheduleEntryDTO);
                            }

                            // Increment to the next day
                            cycleDate.AddDays(1);
                        }

                        #endregion

                        scheduleEntryDTOs = await FillOutSchedule(scheduleEntryDTOs, shifts, ruleDTOs, entityWorkerMemberDTOs, entityShiftRotations, createEntityScheduleDTO);
                    
                        response.Result = scheduleEntryDTOs;
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

        public async Task<BaseResponse<bool>> DeleteScheduleEntry(string scheduleEntryId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Message = ScheduleRelatedMessages.DeleteScheduleEntryUnexpectedError;
            response.Success = false;

            if(scheduleEntryId != null)
            {
                IEnumerable<ScheduleEntryWorkers> scheduleEntryWorkers = await _unitOfWork.EntityScheduleWorkersRepository.GetScheduleEntryWorkers(scheduleEntryId);

                if (scheduleEntryWorkers.Count() != 0)
                    await _unitOfWork.EntityScheduleWorkersRepository.DeleteRange(scheduleEntryWorkers);

                await _unitOfWork.EntityScheduleRepository.Delete(scheduleEntryId);
                response.Success = true;
                response.Message = ScheduleRelatedMessages.DeleteScheduleEntrySuccess;
            }

            return response;
        }

        #endregion

        //public async Task<BaseResponse<bool>> DeleteScheduleEntries(string entityId, DateTime startDate, DateTime endDate)
        //{
        //    BaseResponse<bool> response = new BaseResponse<bool>();

        //    // Get Entity Schedules within a specific time period
        //    List<ScheduleEntry> scheduleEntries = await _entityScheduleRepository.GetScheduleEntries(entityId, string.Empty, startDate, endDate);

        //    _entityScheduleRepository.DeleteRange(sch)

        //    return response;
        //}

        #region Fill Out Schedule

        public async Task<List<ScheduleEntryDTO>> FillOutSchedule(List<ScheduleEntryDTO> scheduleEntryDTOs, List<ShiftDTO> shifts, List<EntityRuleDTO> entityRules, List<EntityWorkerMemberDTO> entityWorkerMemberDTOs, List<EntityShiftRotationDTO> entityShiftRotationDTOs, CreateEntityScheduleDTO createEntityScheduleDTO)
        {
            Random rand = new Random();
            List<ScheduleEntryIneligibility> scheduleEntryIneligibilities = new List<ScheduleEntryIneligibility>();

            try
            {
                if(entityRules.Any(i => i.RuleTypeId.Equals(RuleTypeConstants.MIN_WEEKENDS_OFF_MONTH_ID) || i.RuleTypeId.Equals(RuleTypeConstants.MIN_DAYS_OFF_WEEK_ID)))
                {
                    scheduleEntryIneligibilities = ApplyMonthlyWeekends(scheduleEntryDTOs, entityWorkerMemberDTOs, entityRules);
                }

                foreach (ScheduleEntryDTO scheduleEntryDTO in scheduleEntryDTOs)
                {
                    List<EntityWorkerMemberDTO> assignedWorkers = new List<EntityWorkerMemberDTO>();
                    List<EntityWorkerMemberDTO> notEligible = new List<EntityWorkerMemberDTO>();

                    bool isShiftRotation = entityShiftRotationDTOs.Any(i => i.ShiftId.Equals(scheduleEntryDTO.ShiftId));

                    // Get required quantity per skillset
                    List<Tuple<int, int>> quantityPerSkillset = GetMinimumSkilletSetPerShift(entityRules, scheduleEntryDTO);

                    // Filter valid workers for this entry
                    List<EntityWorkerMemberDTO> filteredWorkers = FilterEligibleWorkers(scheduleEntryDTO, entityRules, entityWorkerMemberDTOs, isShiftRotation, scheduleEntryIneligibilities);

                    // Get the max workers assignable to this shift
                    int maxWorkers = ReturnMaxWorkersPerShift(entityRules, scheduleEntryDTO);

                    // Check for minimum per skill set
                    if (quantityPerSkillset.Count != 0)
                    {
                        bool hasMinSkillSet = false;

                        #region While the skillset is not satisfied or all workers have been assigned V1
                        //while (hasMinSkillSet == false || assignedWorkers.Count + notEligible.Count != filteredWorkers.Count)
                        //{
                        //    EntityWorkerMemberDTO selectedWorker = filteredWorkers[rand.Next(filteredWorkers.Count)];

                        //    // Check if the worker has already been assigned or marked as not eligible
                        //    if (assignedWorkers.Contains(selectedWorker) || notEligible.Contains(selectedWorker))
                        //        continue;

                        //    // Validate worker selection
                        //    bool validateSelection = await ValidateWorkerSelection(scheduleEntryDTO, scheduleEntryDTOs, selectedWorker, entityRules, shifts, createEntityScheduleDTO);

                        //    // Eligible for this entry
                        //    if (validateSelection)
                        //    {
                        //        if (selectedWorker.IsBot)
                        //        {
                        //            ScheduleEntryBots scheduleEntryBot = new ScheduleEntryBots
                        //            {
                        //                ScheduleEntryId = scheduleEntryDTO.ScheduleEntryId,
                        //                UserBotId = _generalService.ParseStringToGuid(selectedWorker.WorkerId)
                        //            };

                        //            await _unitOfWork.ScheduleEntryBotsRepository.Add(scheduleEntryBot);
                        //        }
                        //        else
                        //        {
                        //            ScheduleEntryWorkers scheduleEntryWorker = new ScheduleEntryWorkers
                        //            {
                        //                ScheduleEntryId = scheduleEntryDTO.ScheduleEntryId,
                        //                ApplicationUserId = selectedWorker.WorkerId
                        //            };

                        //            await _unitOfWork.EntityScheduleWorkersRepository.Add(scheduleEntryWorker);
                        //        }

                        //        // Add worker and signal that this worker has been assigned
                        //        scheduleEntryDTO.ScheduleParticipants.Add(selectedWorker);
                        //        assignedWorkers.Add(selectedWorker);
                        //    }
                        //    // Not Eligible for this entry
                        //    else
                        //    {
                        //        notEligible.Add(selectedWorker);
                        //    }

                        //    // If there is a max workers for this shift restriction and its reached
                        //    if (maxWorkers != 0 && assignedWorkers.Count == maxWorkers)
                        //        break;

                        //    hasMinSkillSet = IsSkillSetFullfilled(assignedWorkers, quantityPerSkillset);
                        //}
                        #endregion

                        if (quantityPerSkillset.Count != 0)
                        {
                            // Map: SkillId -> List of eligible workers
                            var skillToWorkersMap = quantityPerSkillset
                                .Select(q => q.Item1) // SkillId
                                .Distinct()
                                .ToDictionary(
                                    skillId => skillId,
                                    skillId => filteredWorkers
                                        .Where(w => w.SkillSet.Any(s => s.SkillId == skillId))
                                        .ToList()
                                );

                            foreach (var (skillId, requiredCount) in quantityPerSkillset)
                            {
                                var availableWorkers = skillToWorkersMap[skillId]
                                    .Where(w => !assignedWorkers.Contains(w) && !notEligible.Contains(w))
                                    .ToList();

                                foreach (var worker in availableWorkers)
                                {
                                    if (assignedWorkers.Count >= maxWorkers)
                                        break;

                                    bool isValid = await ValidateWorkerSelection(scheduleEntryDTO, scheduleEntryDTOs, worker, entityRules, shifts, createEntityScheduleDTO);
                                    if (isValid)
                                    {
                                        assignedWorkers.Add(worker);
                                        scheduleEntryDTO.ScheduleParticipants.Add(worker);
                                    }
                                    else
                                    {
                                        notEligible.Add(worker);
                                    }

                                    // Stop if this skill is fulfilled
                                    if (assignedWorkers.Count(w => w.SkillSet.Any(s => s.SkillId == skillId)) >= requiredCount)
                                        break;
                                }
                            }

                            // Second pass: Fill any remaining slots up to maxWorkers (if needed)
                            var remainingPool = filteredWorkers
                                .Where(w => !assignedWorkers.Contains(w) && !notEligible.Contains(w))
                                .ToList();

                            foreach (var worker in remainingPool)
                            {
                                if (assignedWorkers.Count >= maxWorkers)
                                    break;

                                bool isValid = await ValidateWorkerSelection(scheduleEntryDTO, scheduleEntryDTOs, worker, entityRules, shifts, createEntityScheduleDTO);
                                if (isValid)
                                {
                                    assignedWorkers.Add(worker);
                                    scheduleEntryDTO.ScheduleParticipants.Add(worker);
                                }
                                else
                                {
                                    notEligible.Add(worker);
                                }
                            }
                        }

                    }

                    // Regular assign
                    else
                    {
                        while (assignedWorkers.Count + notEligible.Count != filteredWorkers.Count)
                        {
                            // Select a random worker from the full pool of workers
                            EntityWorkerMemberDTO selectedWorker = filteredWorkers[rand.Next(filteredWorkers.Count)];

                            // Check if the worker has already been assigned or marked as not eligible
                            if (assignedWorkers.Contains(selectedWorker) || notEligible.Contains(selectedWorker))
                                continue;

                            // Validate worker selection
                            bool validateSelection = await ValidateWorkerSelection(scheduleEntryDTO, scheduleEntryDTOs, selectedWorker, entityRules, shifts, createEntityScheduleDTO);

                            // Eligible for this entry
                            if (validateSelection)
                            {
                                // Add worker and signal that this worker has been assigned
                                scheduleEntryDTO.ScheduleParticipants.Add(selectedWorker);
                                assignedWorkers.Add(selectedWorker);
                            }
                            // Not Eligible for this entry
                            else
                            {
                                notEligible.Add(selectedWorker);
                            }

                            // If there is a max workers for this shift restriction and its reached
                            if (maxWorkers != 0 && assignedWorkers.Count == maxWorkers)
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in FillOutSchedule", ex);
            }

            return scheduleEntryDTOs;
        }

        #endregion

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
                    ScheduleEntryIneligibility ineligibility = new ScheduleEntryIneligibility(weekEndEntry.ScheduleEntryId);
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
                            .Select(p => p.WorkerId)
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
                                    e.ScheduleParticipants.Any(p => p.WorkerId == kvp.Key))
                                .ToList();

                            foreach (var entry in offendingEntries)
                            {
                                var ineligibility = scheduleEntryIneligibilities.FirstOrDefault(i => i.ScheduleEntryID == entry.ScheduleEntryId);
                                if (ineligibility == null)
                                {
                                    ineligibility = new ScheduleEntryIneligibility(entry.ScheduleEntryId);
                                    scheduleEntryIneligibilities.Add(ineligibility);
                                }

                                var worker = entry.ScheduleParticipants.FirstOrDefault(p => p.WorkerId == kvp.Key);
                                if (worker != null && !ineligibility.MembersIneligible.Any(w => w.WorkerId == worker.WorkerId))
                                {
                                    ineligibility.MembersIneligible.Add(worker);
                                }
                            }
                        }
                    }

                }
            }

            return scheduleEntryIneligibilities;
        }

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

        // Not Used
        private void GetMonthlyScheduledEntries(List<ScheduleEntryDTO> scheduleEntryDTOs)
        {
            
        }

        /*
        // If month wasnt added yet
                if (!monthlyScheduledEntries.ContainsKey(scheduleEntry.ScheduleStartDate.Month))
                {
                    monthlyScheduledEntries.Add(scheduleEntry.ScheduleStartDate.Month, new List<ScheduleEntryDTO>());
                }
                // Add entry to the month
                monthlyScheduledEntries[scheduleEntry.ScheduleStartDate.Month].Add(scheduleEntry); 
        */

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

            IEnumerable<ScheduleEntryDTO> presentDayEntries = scheduleEntryDTOs.Where(i => i.ScheduleEndDate.Date.Equals(currentScheduleEntry.ScheduleEndDate.Date) && i.ScheduleParticipants.Any(j => j.WorkerId.Equals(entityWorkerMemberDTO.WorkerId)));
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

            // Check assigned weekends
            if(scheduleEntryIneligibilities != null && scheduleEntryIneligibilities.Count != 0)
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
                && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();

            if (entityRuleDTO != null)
                maxPerShift = (int)entityRuleDTO.EntityRuleSpecificationDTOs.First().RuleSpecificationValue;

            return maxPerShift;
        }

        #endregion

        #region Get Minimum SkilletSet Per Shift


        private List<Tuple<int, int>> GetMinimumSkilletSetPerShift(List<EntityRuleDTO> entityRuleDTOs, ScheduleEntryDTO scheduleEntryDTO)
        {
            List<Tuple<int, int>> minSkillQuantity = new List<Tuple<int, int>>();

            bool isWeekend = scheduleEntryDTO.ScheduleStartDate.Equals(DayOfWeek.Saturday) || scheduleEntryDTO.ScheduleStartDate.Equals(DayOfWeek.Sunday);

            EntityRuleDTO entityRuleDTO = null;

            // if there is any rule for the weekdays quantity of skillset regarding this shift
            if (entityRuleDTOs.Any(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKDAYS_ID)
                && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId))
                && !isWeekend))
            {
                entityRuleDTO = entityRuleDTOs
                    .Where(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKDAYS_ID)
                    && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();
            }

            // if there is any rule for the weekdays quantity of skillset regarding this shift
            else if (entityRuleDTOs.Any(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKENDS_ID)
                && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId))
                && isWeekend))
            {
                entityRuleDTO = entityRuleDTOs
                    .Where(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_WEEKENDS_ID)
                    && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();
            }

            // Standard rule for the quantity of skillset regarding this shift
            else
            {
                entityRuleDTO = entityRuleDTOs
                    .Where(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_ID)
                    && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();
            }

            // If there is a rule for the quantity of skillset regarding this shift
            if (entityRuleDTO != null)
            {
                // For each specification of the rule
                foreach (EntityRuleSpecificationDTO entityRuleSpecificationDTO in entityRuleDTO.EntityRuleSpecificationDTOs)
                    // If this is the shift we are looking for
                    if (entityRuleSpecificationDTO.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId))
                        // Add the skill id and quantity to the list
                        minSkillQuantity.Add(new Tuple<int, int>(int.Parse(entityRuleSpecificationDTO.AspectReferenceId), (int)entityRuleSpecificationDTO.RuleSpecificationValue));
            }

            return minSkillQuantity;
        }

        #endregion

        #region Is Skill set fullfilled

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

        private bool CheckMaxHoursPerDay(ScheduleEntryDTO currentScheduleEntry, IEnumerable<ScheduleEntryDTO> presentDayEntries, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO maxDailyHoursRule, List<ShiftDTO> shiftDTOs)
        {
            // Check if current schedule entry is longer than the allowed daily hours
            ShiftDTO shiftDTO = shiftDTOs.Where(i => i.ShiftId.Equals(currentScheduleEntry.ShiftId)).FirstOrDefault();

            // If a shift is found
            if (shiftDTO == null)
                return false;

            // If shift duration is greater than the max allowed value by rule
            if (shiftDTO.ShiftDuration > TimeSpan.FromHours(maxDailyHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue))
                return false;

            // Check possible other entries
            if (presentDayEntries.Count() > 0)
            {
                TimeSpan totalHours = new TimeSpan();
                foreach (ScheduleEntryDTO scheduleEntryDTO in presentDayEntries)
                {
                    if (scheduleEntryDTO.ScheduleEntryId.Equals(currentScheduleEntry.ScheduleEntryId))
                        continue;

                    ShiftDTO shift = shiftDTOs.Where(i => i.ShiftId == scheduleEntryDTO.ShiftId).FirstOrDefault();
                    if (shift != null)
                        totalHours.Add(shift.ShiftDuration);

                }

                // If the total hours of the day plus the current entry is greater than the allowed daily hours
                if (totalHours + shiftDTO.ShiftDuration > TimeSpan.FromHours(maxDailyHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue))
                    return false;
            }

            return true;
        }

        #endregion

        #region Check Max Hours per Week

        private bool CheckMaxHoursPerWeek(IEnumerable<ScheduleEntryDTO> weekEntries, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO maxWeeklyHoursRule, List<ShiftDTO> shiftDTOs)
        {
            TimeSpan totalHours = TimeSpan.Zero;

            // For each week entry
            foreach (ScheduleEntryDTO scheduleEntryDTO in weekEntries)
            {
                if (scheduleEntryDTO.ScheduleParticipants.Any(i => i.WorkerId.Equals(entityWorkerMemberDTO.WorkerId)))
                {
                    ShiftDTO shiftDTO = shiftDTOs.Where(i => i.ShiftId.Equals(scheduleEntryDTO.ShiftId)).FirstOrDefault();
                    if (shiftDTO != null)
                        totalHours.Add(shiftDTO.ShiftDuration);
                }
            }

            if (totalHours > TimeSpan.FromHours(maxWeeklyHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue))
                return false;

            return true;
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

                    // Get Schedule entries from Start of Week Date to Start Date
                    filteredWeeklyEntries = filteredWeeklyEntries.Concat(await GetScheduleEntries(weekRequest)).ToList();
                }
            }

            filteredWeeklyEntries = filteredWeeklyEntries.Concat(scheduleEntryDTOs.Where(i => i.ScheduleStartDate > currentScheduleEntry.ScheduleStartDate.AddDays(backtrackDaysQty) && i.ScheduleEndDate < currentScheduleEntry.ScheduleStartDate.AddDays(forwardDaysQty))).ToList();

            return filteredWeeklyEntries;
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
                        ScheduleEntryDTO lastPreCurrentScheduleEntry = previousShiftEntries.Where(i => i.ShiftId.Equals(entityRuleSpecificationDTO.BusinessAspectId)
                    && i.ScheduleStartDate < currentScheduleEntry.ScheduleStartDate
                    && i.ScheduleParticipants.Any(j => j.WorkerId.Equals(entityWorkerMemberDTO.WorkerId))).OrderByDescending(i => i.ScheduleStartDate).FirstOrDefault();

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

        #endregion


    }
}
