using AutoMapper;
using Azure;
using Microsoft.IdentityModel.Tokens;
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
using ShiftSchedularRL.Resources.Dashboard;
using ShiftSchedularRL.Resources.MemberManagement;
using ShiftSchedularRL.Resources.ScheduleManagement;
using ShiftSchedularRL.Resources.Shared;
using ShiftSchedularRL.Resources.ShiftManagement;
using System.Linq;

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
        private readonly ILanguageAccessor _languageAccessor;

        #endregion

        #region Constructor

        public EntityScheduleService(IMapper mapper,
            IGeneralService generalService,
            IUnitOfWork unitOfWork,
            IShiftService shiftService,
            IEntityRuleService entityRuleService,
            IEntityService entityService,
            IEntityWorkerAbsenceService entityWorkerAbsenceService,
            ILanguageAccessor languageAccessor)
        {
            _mapper = mapper;
            _generalService = generalService;
            _unitOfWork = unitOfWork;
            _shiftService = shiftService;
            _entityRuleService = entityRuleService;
            _entityService = entityService;
            _languageAccessor = languageAccessor;
        }

        #endregion

        #region Methods

        #region Get Entity Schedule View Model

        public async Task<EntityScheduleViewModel> GetEntityScheduleViewModel(ScheduleViewModelRequestDTO viewModelRequest)
        {
            EntityScheduleViewModel viewModel = new EntityScheduleViewModel();

            if (viewModelRequest != null && viewModelRequest.EntityId != Guid.Empty && !string.IsNullOrEmpty(viewModelRequest.WorkerId))
            {
                Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(viewModelRequest.EntityId);
                EntityWorker entityWorkerInstance = await _unitOfWork.EntityWorkerRepository.GetByWorkerAndEntity(viewModelRequest.WorkerId, viewModelRequest.EntityId);
                viewModel.AllowEdit = await _unitOfWork.EntityPermissionRepository.CanUserEditEntity(viewModelRequest.EntityId, viewModelRequest.WorkerId);

                // If it can change data
                if (viewModel.AllowEdit)
                {
                    viewModel.Shifts = await _shiftService.GetEntityShifts(viewModelRequest.EntityId);
                    viewModel.EntityRules = await _entityRuleService.GetEntityRules(viewModelRequest.EntityId, _languageAccessor.GetLanguageCode());
                    MemberPagedModelRequestDTO baseRequest = new MemberPagedModelRequestDTO
                    {
                        EntityId = viewModelRequest.EntityId
                    };
                    EntityMembersViewModel entityMembersViewModel = await _entityService.GetEntitiesMembersViewModel(baseRequest);
                    viewModel.EntityWorkerMembers = entityMembersViewModel.EntityMembers.Data.Where(m => m.IsBot || m.PartOfRoster);
                }


                // Get all Schedules from the date selection
                viewModel.ScheduleEntries = await GetScheduleEntries(viewModelRequest);
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

                ShiftDTO shift = await _shiftService.GetShiftById(_generalService.ParseStringToGuid(addScheduleEntryDTO.ShiftId));
                if (shift == null)
                {
                    response.Message = ScheduleRelatedMessages.ShiftNotFound;
                    return response;
                }

                // Check if there is already an entry
                ScheduleEntry scheduleEntry= await _unitOfWork.EntityScheduleRepository.GetByShiftAndDateEntry(shift.ShiftId, addScheduleEntryDTO.ScheduleStartDate);
                if(scheduleEntry != null)
                {
                    response.Message = ScheduleRelatedMessages.ScheduleEntryAlreadyExistsError;
                    return response;
                }

                scheduleEntry = await CreateBaseScheduleEntry(shift, addScheduleEntryDTO.ScheduleStartDate);

                response.Success = true;
                response.Message = ScheduleRelatedMessages.AddNewScheduleEntrySuccess;
                ScheduleEntryDTO scheduleEntryDTO = _mapper.Map<ScheduleEntryDTO>(scheduleEntry);
                scheduleEntryDTO.ShiftDTO = shift;

                response.Result = scheduleEntryDTO;
            }

            return response;
        }

        #endregion

        #region Assign Entry

        public async Task<BaseResponse<ScheduleEntryDTO>> AssignEntry(AssignEntryDTO assignEntryDTO)
        {
            BaseResponse<ScheduleEntryDTO> response = new BaseResponse<ScheduleEntryDTO>();
            response.Message = SharedMessages.UnexpectedError;

            // Check if entity exists
            Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(assignEntryDTO.EntityId);

            // Entity Not Found
            if (entity == null)
            {
                response.Message = EntitiesRelatedMessages.EntityNoIdentifierError;
                return response;
            }

            ShiftDTO shiftDTO = await _shiftService.GetShiftById(assignEntryDTO.ShiftId);

            // Shift Not Found
            if (shiftDTO == null)
            {
                response.Message = ShiftRelatedMessages.ShiftIdIsNull;
                return response;
            }

            // Get Worker data related to the entity
            IEnumerable<EntityWorkerMemberDTO> entityWorkerMembers = await _entityService.GetEntityMembers(assignEntryDTO.EntityId, new List<string> { assignEntryDTO.WorkerId });

            // No records or more than 1
            if (entityWorkerMembers.Count() == 0 || entityWorkerMembers.Count() > 1)
            {
                response.Message = SharedMessages.UnexpectedError;
                return response;
            }

            ScheduleEntry scheduleEntry = null;

            bool isEntryToBeCreated = false;

            // Check if schedule Id is null
            if (string.IsNullOrEmpty(assignEntryDTO.ScheduleId))
            {
                // With the shift and date, check if there is a schedule entry for that shift and date
                scheduleEntry = await _unitOfWork.EntityScheduleRepository.GetByShiftAndDateEntry(assignEntryDTO.ShiftId, assignEntryDTO.Date);

                // If no date, create entry regardless of rules
                if (scheduleEntry == null)
                {
                    scheduleEntry = new ScheduleEntry
                    {
                        ScheduleEntryId = Guid.NewGuid(),
                        ShiftId = assignEntryDTO.ShiftId,
                        ScheduleStartDate = assignEntryDTO.Date.Add(shiftDTO.ShiftStartHour)
                    };

                    var totalBreakIncludedDuration = shiftDTO.ShiftBreakDTOs
                                    .Where(sb => sb.IncludedInShift)
                                    .Select(sb => sb.ShiftBreakDuration)
                                    .Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));

                    scheduleEntry.ScheduleEndDate = scheduleEntry.ScheduleStartDate
                        .Add(shiftDTO.ShiftDuration)
                        .Add(totalBreakIncludedDuration);

                    scheduleEntry = await _unitOfWork.EntityScheduleRepository.Add(scheduleEntry);
                    isEntryToBeCreated = true;
                }
            }

            // If not null, get schedule entry
            else
            {
                Guid scheduleEntryId = _generalService.ParseStringToGuid(assignEntryDTO.ScheduleId);

                scheduleEntry = await _unitOfWork.EntityScheduleRepository.GetById(scheduleEntryId);
                // If not found, return error
                if (scheduleEntry == null)
                {
                    response.Message = ScheduleRelatedMessages.ScheduleIdIsNull;
                    return response;
                }
            }

            EntityWorkerMemberDTO workerToBeAssigned = entityWorkerMembers.First();

            ScheduleEntryDTO scheduleEntryDTO = _mapper.Map<ScheduleEntryDTO>(scheduleEntry);
            scheduleEntryDTO.ShiftDTO = shiftDTO;

            // Add Participant
            if (assignEntryDTO.IsBot)
            {
                ScheduleEntryBots scheduleEntryBot = new ScheduleEntryBots
                {
                    ScheduleEntryId = scheduleEntry.ScheduleEntryId,
                    UserBotId = _generalService.ParseStringToGuid(workerToBeAssigned.WorkerId),
                    SpecificSkillAssignments = string.Join(",", workerToBeAssigned.SkillSet.Select(i => i.SkillId))
                };

                await _unitOfWork.ScheduleEntryBotsRepository.Add(scheduleEntryBot);
            }
            else
            {
                ScheduleEntryWorkers scheduleEntryWorker = new ScheduleEntryWorkers
                {
                    ScheduleEntryId = scheduleEntry.ScheduleEntryId,
                    ApplicationUserId = workerToBeAssigned.WorkerId,
                    SpecificSkillAssignments = string.Join(",", workerToBeAssigned.SkillSet.Select(i => i.SkillId))
                };

                await _unitOfWork.EntityScheduleWorkersRepository.Add(scheduleEntryWorker);
            }

            // Return Schedule entry in DTO format

            // If this entry was created now
            if (isEntryToBeCreated)
            {
                scheduleEntryDTO.ScheduleParticipants.Add(new ScheduleEntryParticipantDTO
                {
                    Worker = workerToBeAssigned,
                    AssignedSkills = workerToBeAssigned.SkillSet
                });
            }
            else
            {
                scheduleEntryDTO = await GetScheduleEntryById(scheduleEntryDTO.ScheduleEntryId, _languageAccessor.GetLanguageCode());
            }

            response.Success = true;
            response.Result = scheduleEntryDTO;

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

            // Load schedule entries with all related data (Shift, Workers, Bots) in ONE query
            IEnumerable<ScheduleEntry> scheduleEntries = await _unitOfWork.EntityScheduleRepository.GetEFScheduleEntries(
                viewModelRequest.EntityId,
                viewModelRequest.StartDateSearch,
                viewModelRequest.EndDateSearch);

            if (!scheduleEntries.Any())
                return scheduleEntryDTOs;

            // Batch load entity skills ONCE for all entries
            List<SkillLocalizedDTO> entitySkills = await _entityService.GetEntitySkills(new BaseViewModelRequest
            {
                EntityId = viewModelRequest.EntityId
            });

            // Extract ALL unique participant IDs across all schedule entries
            var allParticipantIds = scheduleEntries
                .SelectMany(entry => entry.ScheduleEntryWorkers.Select(w => w.ApplicationUserId))
                .Concat(scheduleEntries.SelectMany(entry => entry.ScheduleEntryBots.Select(b => b.UserBotId.ToString())))
                .Distinct()
                .ToList();

            // Batch load ALL participants in ONE query
            List<EntityWorkerMemberDTO> allParticipants = new List<EntityWorkerMemberDTO>();
            if (allParticipantIds.Any())
            {
                allParticipants = await _entityService.GetEntityMembers(
                    viewModelRequest.EntityId,
                    allParticipantIds);
            }

            // Create a lookup dictionary for fast participant access
            var participantLookup = allParticipants.ToDictionary(p => p.WorkerId, p => p);

            // Process each schedule entry using pre-loaded data
            foreach (ScheduleEntry entry in scheduleEntries)
            {
                ScheduleEntryDTO scheduleEntryDTO = _mapper.Map<ScheduleEntryDTO>(entry);

                // Map shift data (already loaded via Include)
                if (entry.Shift != null)
                {
                    scheduleEntryDTO.ShiftDTO = _mapper.Map<ShiftDTO>(entry.Shift);
                }

                // Process workers
                foreach (ScheduleEntryWorkers entryWorker in entry.ScheduleEntryWorkers)
                {
                    if (participantLookup.TryGetValue(entryWorker.ApplicationUserId, out var worker))
                    {
                        string[] skills = entryWorker.SpecificSkillAssignments?.Split(',') ?? Array.Empty<string>();

                        List<SkillLocalizedDTO> assignedSkills = entitySkills
                            .Where(s => skills.Contains(s.SkillId.ToString()))
                            .ToList();

                        if (assignedSkills.Count == 0)
                            assignedSkills = worker.SkillSet;

                        scheduleEntryDTO.ScheduleParticipants.Add(new ScheduleEntryParticipantDTO
                        {
                            Worker = worker,
                            AssignedSkills = assignedSkills
                        });
                    }
                }

                // Process bots
                foreach (ScheduleEntryBots entryBot in entry.ScheduleEntryBots)
                {
                    // Find bot by parsing WorkerId to Guid and comparing with UserBotId (binary comparison)
                    EntityWorkerMemberDTO worker = allParticipants.FirstOrDefault(p =>
                        _generalService.ParseStringToGuid(p.WorkerId).Equals(entryBot.UserBotId));

                    if (worker != null)
                    {
                        string[] skills = entryBot.SpecificSkillAssignments?.Split(',') ?? Array.Empty<string>();

                        List<SkillLocalizedDTO> assignedSkills = entitySkills
                            .Where(s => skills.Contains(s.SkillId.ToString()))
                            .ToList();

                        if (assignedSkills.Count == 0)
                            assignedSkills = worker.SkillSet;

                        scheduleEntryDTO.ScheduleParticipants.Add(new ScheduleEntryParticipantDTO
                        {
                            Worker = worker,
                            AssignedSkills = assignedSkills
                        });
                    }
                }

                scheduleEntryDTOs.Add(scheduleEntryDTO);
            }

            return scheduleEntryDTOs;
        }

        #endregion

        #region Get Schedule Entry By Id

        public async Task<ScheduleEntryDTO> GetScheduleEntryById(Guid scheduleEntryId, string languageCode)
        {
            ScheduleEntryDTO scheduleEntryDTO = new ScheduleEntryDTO();

            ScheduleEntry scheduleEntry = await _unitOfWork.EntityScheduleRepository.GetById(scheduleEntryId);
            ShiftDTO shift = await _shiftService.GetShiftById(_generalService.ParseStringToGuid(scheduleEntry.ShiftId.ToString()));

            // Get Entity Skills
            List<SkillLocalizedDTO> entitySkills = await _entityService.GetEntitySkills(new BaseViewModelRequest
            {
                EntityId = shift.EntityId
            });

            // Extract participant ids
            List<string> participantsIds = scheduleEntry.ScheduleEntryWorkers.Select(i => i.ApplicationUserId).ToList();
            if (scheduleEntry.ScheduleEntryBots.Any())
            {
                participantsIds = participantsIds.Concat(
                    scheduleEntry.ScheduleEntryBots.Select(i => i.UserBotId.ToString())
                ).ToList();
            }


            scheduleEntryDTO = _mapper.Map<ScheduleEntryDTO>(scheduleEntry);
            scheduleEntryDTO.ShiftDTO = shift;

            // Get Entity Worker Member Information
            List<EntityWorkerMemberDTO> entityWorkerMemberDTOs = await _entityService.GetEntityMembers(shift.EntityId, participantsIds);

            // For each worker assigned to this entry
            foreach (ScheduleEntryWorkers entryWorker in scheduleEntry.ScheduleEntryWorkers)
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

            foreach (ScheduleEntryBots entryBot in scheduleEntry.ScheduleEntryBots)
            {
                // Find the worker in the entity members
                EntityWorkerMemberDTO worker = entityWorkerMemberDTOs.FirstOrDefault(i => _generalService.ParseStringToGuid(i.WorkerId).Equals(entryBot.UserBotId));
                if (worker != null)
                {
                    // Extract Skill Identifiers
                    string[] skills = entryBot.SpecificSkillAssignments?.Split(',') ?? Array.Empty<string>();

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

        #region Save Schedule Entries

        public async Task<BaseResponse<bool>> SaveScheduleEntries(ScheduleEntryDTO[] scheduleEntryDTOs)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Message = SharedMessages.UnexpectedError;

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // For each entry
                foreach(ScheduleEntryDTO scheduleEntryDTO in scheduleEntryDTOs)
                {
                    // Check if entry exists
                    ScheduleEntry scheduleEntry = await _unitOfWork.EntityScheduleRepository.GetById(scheduleEntryDTO.ScheduleEntryId);
                    if(scheduleEntry == null)
                    {
                        response.Message = ScheduleRelatedMessages.ScheduleEntryNotFound;
                        throw new InvalidOperationException();
                    }

                    // Delete existing participations
                    await _unitOfWork.ScheduleEntryBotsRepository.DeleteRange(scheduleEntry.ScheduleEntryBots);
                    await _unitOfWork.EntityScheduleWorkersRepository.DeleteRange(scheduleEntry.ScheduleEntryWorkers);

                    // For each participation received
                    foreach(ScheduleEntryParticipantDTO scheduleEntryParticipant in scheduleEntryDTO.ScheduleParticipants)
                    {
                        // If its bot, add it to the ScheduleEntryBots
                        if (scheduleEntryParticipant.Worker.IsBot)
                        {
                            ScheduleEntryBots scheduleEntryBot = new ScheduleEntryBots
                            {
                                ScheduleEntryId = scheduleEntryDTO.ScheduleEntryId,
                                UserBotId = _generalService.ParseStringToGuid(scheduleEntryParticipant.Worker.WorkerId),
                                SpecificSkillAssignments = string.Join(",", scheduleEntryParticipant.AssignedSkills.Where(i => i.SkillId != 0).Select(i => i.SkillId))
                            };

                            await _unitOfWork.ScheduleEntryBotsRepository.Add(scheduleEntryBot);
                        }
                        // Add it to the ScheduleEntryWorkers
                        else
                        {
                            ScheduleEntryWorkers scheduleEntryWorker = new ScheduleEntryWorkers
                            {
                                ScheduleEntryId = scheduleEntryDTO.ScheduleEntryId,
                                ApplicationUserId = scheduleEntryParticipant.Worker.WorkerId,
                                SpecificSkillAssignments = string.Join(",", scheduleEntryParticipant.AssignedSkills.Where(i => i.SkillId != 0).Select(i => i.SkillId))
                            };

                            await _unitOfWork.EntityScheduleWorkersRepository.Add(scheduleEntryWorker);
                        }
                    }
                }

                await _unitOfWork.CommitAsync();
                response.Success = true;
                response.Message = ScheduleRelatedMessages.ScheduleEntriesUpdatedSuccessfuly;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return response;
        }

        #endregion

        #region Delete Worker Schedule Entries

        public async Task<BaseResponse<bool>> DeleteWorkerScheduleEntries(DeleteIntervalWorkerScheduleEntriesDTO intervalWorkerScheduleEntriesDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Message = SharedMessages.UnexpectedError;

            if(intervalWorkerScheduleEntriesDTO.StartDate > intervalWorkerScheduleEntriesDTO.EndDate)
            {
                response.Message = ScheduleRelatedMessages.StartDateGreaterThanEndDateError;
                return response;
            }

            // Check if entity exists
            Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(intervalWorkerScheduleEntriesDTO.EntityId);

            // Entity Not Found
            if (entity == null)
            {
                response.Message = EntitiesRelatedMessages.EntityNoIdentifierError;
                return response;
            }

            // Get Worker data related to the entity
            IEnumerable<EntityWorkerMemberDTO> entityWorkerMembers = await _entityService.GetEntityMembers(intervalWorkerScheduleEntriesDTO.EntityId, new List<string> { intervalWorkerScheduleEntriesDTO.WorkerId });

            // No records or more than 1
            if (entityWorkerMembers.Count() == 0 || entityWorkerMembers.Count() > 1)
            {
                response.Message = SharedMessages.UnexpectedError;
                return response;
            }

            EntityWorkerMemberDTO entityWorkerMemberDTO = entityWorkerMembers.First();

            response = await this.DeleteParticipations(intervalWorkerScheduleEntriesDTO.EntityId, entityWorkerMemberDTO.WorkerId, entityWorkerMemberDTO.IsBot, intervalWorkerScheduleEntriesDTO.StartDate, intervalWorkerScheduleEntriesDTO.EndDate);

            return response;
        }

        #endregion

        #region Delete Schedule Entries

        public async Task<BaseResponse<bool>> DeleteScheduleEntries(DeleteIntervalWorkerScheduleEntriesDTO intervalWorkerScheduleEntriesDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Message = SharedMessages.UnexpectedError;

            if (intervalWorkerScheduleEntriesDTO.StartDate > intervalWorkerScheduleEntriesDTO.EndDate)
            {
                response.Message = ScheduleRelatedMessages.StartDateGreaterThanEndDateError;
                return response;
            }

            // Check if entity exists
            Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(intervalWorkerScheduleEntriesDTO.EntityId);

            // Entity Not Found
            if (entity == null)
            {
                response.Message = EntitiesRelatedMessages.EntityNoIdentifierError;
                return response;
            }

            List<ScheduleEntry> entries = await _unitOfWork.EntityScheduleRepository.GetEFScheduleEntries(intervalWorkerScheduleEntriesDTO.EntityId, intervalWorkerScheduleEntriesDTO.StartDate, intervalWorkerScheduleEntriesDTO.EndDate);

            if (entries.Count == 0)
            {
                response.Message = ScheduleRelatedMessages.ScheduleEntriesNotFound;
                return response;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach(ScheduleEntry entry in entries)
                {
                    await _unitOfWork.ScheduleEntryBotsRepository.DeleteRange(entry.ScheduleEntryBots);
                    await _unitOfWork.EntityScheduleWorkersRepository.DeleteRange(entry.ScheduleEntryWorkers);
                    await _unitOfWork.GetGenericRepository<ScheduleEntryBotIneligibility>().DeleteRange(entry.ScheduleEntryBotIneligibilities);
                    await _unitOfWork.GetGenericRepository<ScheduleEntryWorkerIneligibility>().DeleteRange(entry.ScheduleEntryWorkerIneligibilities);
                }

                await _unitOfWork.EntityScheduleRepository.DeleteRange(entries);

                await _unitOfWork.CommitAsync();
                response.Success = true;
                response.Message = ScheduleRelatedMessages.ScheduleEntriesRemovedSuccess;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return response;
        }

        #endregion

        #region Delete Participations

        private async Task<BaseResponse<bool>> DeleteParticipations(Guid entityId, string workerId, bool isBot, DateTime startDate, DateTime endDate)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            Guid memberGuid = _generalService.ParseStringToGuid(workerId);

            List<ScheduleEntry> dayEntries = await _unitOfWork.EntityScheduleRepository.GetWorkerScheduleEntries(entityId, startDate, endDate, memberGuid, isBot);

            if (dayEntries.Count == 0)
            {
                response.Message = ScheduleRelatedMessages.ScheduleEntriesNotFound;
                return response;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (ScheduleEntry scheduleEntry in dayEntries)
                {
                    if (isBot)
                    {
                        ScheduleEntryBots scheduleEntryBot = scheduleEntry.ScheduleEntryBots.First();

                        // Delete entry
                        bool result = await _unitOfWork.ScheduleEntryBotsRepository.DeleteScheduleEntryBot(scheduleEntryBot.ScheduleEntryId, scheduleEntryBot.UserBotId);
                        if (result)
                            continue;

                        // If error occurs, rollback and return
                        else
                        {
                            await _unitOfWork.RollbackAsync();
                            _unitOfWork.Dispose();
                            return response;
                        }
                    }

                    else
                    {
                        ScheduleEntryWorkers scheduleEntryWorker = scheduleEntry.ScheduleEntryWorkers.First();

                        bool result = await _unitOfWork.EntityScheduleWorkersRepository.DeleteScheduleEntryWorker(scheduleEntryWorker.ScheduleEntryId, scheduleEntryWorker.ApplicationUserId);
                        if (result)
                            continue;

                        // If error occurs, rollback and return
                        else
                        {
                            await _unitOfWork.RollbackAsync();
                            _unitOfWork.Dispose();
                            return response;
                        }
                    }
                }

                await _unitOfWork.CommitAsync();
                response.Message = ScheduleRelatedMessages.EntryParticipationRemoved;
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

            return response;
        } 

        #endregion

        #region Create Base Schedule Entry

        public async Task<ScheduleEntry> CreateBaseScheduleEntry(ShiftDTO shift, DateTime date, EntityHolidayDTO entityHolidayDTO = null)
        {
            bool isOperationalConstricted = false;
            if (entityHolidayDTO != null && entityHolidayDTO.OperatingStartTime.HasValue && entityHolidayDTO.OperatingEndTime.HasValue)
                isOperationalConstricted = true;

            var breakDuration = shift.ShiftBreakDTOs
                .Where(sb => sb.IncludedInShift)
                .Select(sb => sb.ShiftBreakDuration)
                .Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));

            var shiftStart = date.Add(shift.ShiftStartHour);
            var shiftEnd = shiftStart.Add(shift.ShiftDuration)
                            .Add(breakDuration);

            // If there is a holiday with operational constrictions
            if (isOperationalConstricted)
            {
                var opStart = date.Add(entityHolidayDTO.OperatingStartTime.Value);
                var opEnd = date.Add(entityHolidayDTO.OperatingEndTime.Value);

                // Doesn't overlap, meaning its completely outside the operating times
                if (shiftStart >= opEnd || shiftEnd <= opStart)
                    return null;

                // Check overlaps and adjust timers
                shiftStart = shiftStart < opStart ? opStart : shiftStart;
                shiftEnd = shiftEnd > opEnd ? opEnd : shiftEnd;
            }

            var entry = new ScheduleEntry
            {
                ScheduleEntryId = Guid.NewGuid(),
                ShiftId = shift.ShiftId,
                ScheduleStartDate = shiftStart,
                ScheduleEndDate = shiftEnd
            };

            entry = await _unitOfWork.EntityScheduleRepository.Add(entry);

            return entry;
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

                if (scheduleEntryBots.Count() != 0)
                    await _unitOfWork.ScheduleEntryBotsRepository.DeleteRange(scheduleEntryBots);

                await _unitOfWork.EntityScheduleRepository.Delete(scheduleEntryId);
                response.Success = true;
                response.Message = ScheduleRelatedMessages.DeleteScheduleEntrySuccess;
            }

            return response;
        }

        #endregion

        #region Apply Monthly Weekends NOT USED

        private List<ScheduleEntryIneligibilityDTOv1> ApplyMonthlyWeekends(List<ScheduleEntryDTO> scheduleEntryDTOs, List<EntityWorkerMemberDTO> entityWorkerMembers, List<EntityRuleDTO> entityRuleDTOs)
        {
            List<ScheduleEntryIneligibilityDTOv1> scheduleEntryIneligibilities = new List<ScheduleEntryIneligibilityDTOv1>();

            #region Filter Non Rotationers and non weekend workers

            // Check any workers that are not part of rotation and not working weekends
            if (entityWorkerMembers != null && entityWorkerMembers.Any(i => !i.PartOfRotation && !i.WorksWeekends))
            {
                // Get any schedule entry on the weekends
                IEnumerable<ScheduleEntryDTO> weekEndEntries = scheduleEntryDTOs.Where(j => j.ScheduleStartDate.DayOfWeek == DayOfWeek.Saturday || j.ScheduleStartDate.DayOfWeek == DayOfWeek.Sunday);
                foreach (ScheduleEntryDTO weekEndEntry in weekEndEntries)
                {
                    ScheduleEntryIneligibilityDTOv1 ineligibility = new ScheduleEntryIneligibilityDTOv1(weekEndEntry.ScheduleEntryId, weekEndEntry.ScheduleStartDate);
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
                                    ineligibility = new ScheduleEntryIneligibilityDTOv1(entry.ScheduleEntryId, entry.ScheduleStartDate);
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

        #region Validate Worker Selection NOT USED

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
            //// Get Max Daily hours rule
            //EntityRuleDTO maxDailyHoursRule = entityRules.FirstOrDefault((i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_HOURS_DAY_ID)));

            //IEnumerable<ScheduleEntryDTO> presentDayEntries = scheduleEntryDTOs.Where(i => i.ScheduleEndDate.Date.Equals(currentScheduleEntry.ScheduleEndDate.Date) && i.ScheduleParticipants.Any(j => j.Worker.WorkerId.Equals(entityWorkerMemberDTO.WorkerId)));
            //// Checks Max Hours per day
            //if (maxDailyHoursRule != null && !CheckMaxHoursPerDay(currentScheduleEntry, presentDayEntries, entityWorkerMemberDTO, maxDailyHoursRule, shiftDTOs))
            //    return false;

            //// Get Max Weekly Hours rule
            //EntityRuleDTO maxWeeklyHoursRule = entityRules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_HOURS_WEEK_ID)).FirstOrDefault();

            //List<ScheduleEntryDTO> weekEntries = await FilterWeeklySessions(currentScheduleEntry, scheduleEntryDTOs, createEntityScheduleDTO);
            //// Check Max Hours per Week
            //if (maxWeeklyHoursRule != null && !CheckMaxHoursPerWeek(weekEntries, entityWorkerMemberDTO, maxWeeklyHoursRule, shiftDTOs))
            //    return false;

            //// Check for turns of the same type
            //EntityRuleDTO limitOfTurnsRule = entityRules.FirstOrDefault(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_CONSECUTIVE_SHIFTS_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(currentScheduleEntry.ShiftId)));

            //if (limitOfTurnsRule != null)
            //{
            //    List<ScheduleEntryDTO> previousShiftEntries = await FilterEntriesForConsecutiveTurns(currentScheduleEntry, scheduleEntryDTOs, createEntityScheduleDTO, limitOfTurnsRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue);
            //    if (limitOfTurnsRule != null && !CheckForTurnsOfTheSameType(currentScheduleEntry, previousShiftEntries, entityWorkerMemberDTO, limitOfTurnsRule, shiftDTOs))
            //        return false;
            //}

            //// Check if worker has performed a shift and requires rest
            //EntityRuleDTO postShiftRestRule = entityRules.FirstOrDefault(i => i.RuleTypeId.Equals(RuleTypeConstants.POST_SHIFT_REST_HOURS_ID));
            //if (limitOfTurnsRule != null && !CheckForPostShiftRest(currentScheduleEntry, scheduleEntryDTOs, entityWorkerMemberDTO, postShiftRestRule))
            //    return false;

            return true;
        }

        #endregion

        #region RULE CHECKER: Check For Turns of the same Type NOT USED

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

        #region Is Min Workers Per Shift NOT USED

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

        #region Is Skill Set Fullfilled NOT USED

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

        #region AUX : Filter Entries for Consecutive Turns

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
                    StartDateSearch = startDate,
                    EndDateSearch = endDate
                };

                filteredEntries = filteredEntries.Concat(await GetScheduleEntries(requestDTO)).ToList();
            }

            return filteredEntries.OrderByDescending(i => i.ScheduleStartDate).ToList();
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

        #region Add Schedule Participant NOT USED

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
                response.Result = await GetScheduleEntryById(scheduleEntry.ScheduleEntryId, _languageAccessor.GetLanguageCode());
            }

            return response;
        }

        #endregion


        #endregion

        #endregion
    }
}
