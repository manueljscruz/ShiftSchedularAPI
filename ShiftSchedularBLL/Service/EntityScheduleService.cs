using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Repositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.ScheduleManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.Service
{
    public class EntityScheduleService : IEntityScheduleService
    {
        #region Properties

        private readonly IMapper _mapper;
        private readonly IGeneralService _generalService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityScheduleRepository _entityScheduleRepository;
        private readonly IEntityScheduleWorkersRepository _scheduleEntryWorkersRepository;
        private readonly IGenericRepository<Entity> _entityRepository;
        private readonly IEntityWorkerRepository _entityWorkerRepository;
        private readonly IShiftService _shiftService;
        private readonly IEntityRuleService _entityRuleService;
        private readonly IEntityService _entityService;
        private readonly IBaseEntityRuleService _baseEntityRuleService;
        private readonly IScheduleGeneratorService _scheduleGeneratorService;

        #endregion

        #region Constructor

        public EntityScheduleService(IMapper mapper, IGeneralService generalService, IUnitOfWork unitOfWork, IEntityScheduleRepository entityScheduleRepository,
            IEntityScheduleWorkersRepository scheduleEntryWorkersRepository, IGenericRepository<Entity> entityRepository, IEntityWorkerRepository entityWorkerRepository, 
            IShiftService shiftService, IEntityRuleService entityRuleService, IEntityService entityService, IScheduleGeneratorService scheduleGeneratorService)
        {
            _mapper = mapper;
            _generalService = generalService;
            _unitOfWork = unitOfWork;
            _entityScheduleRepository = entityScheduleRepository;
            _scheduleEntryWorkersRepository = scheduleEntryWorkersRepository;
            _entityRepository = entityRepository;
            _entityWorkerRepository = entityWorkerRepository;
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

            if (viewModelRequest != null && !string.IsNullOrEmpty(viewModelRequest.EntityId) && !string.IsNullOrEmpty(viewModelRequest.WorkerId) && !string.IsNullOrEmpty(viewModelRequest.LanguageCode))
            {
                Entity entity = await _entityRepository.GetById(viewModelRequest.EntityId);
                EntityWorker entityWorker = await _entityWorkerRepository.GetByWorkerAndEntity(viewModelRequest.WorkerId, viewModelRequest.EntityId);
                viewModel.AllowEdit = entityWorker.IsOwner;

                // If it can change data
                if (entityWorker.IsOwner)
                {
                    viewModel.Shifts = await _shiftService.GetEntityShifts(viewModelRequest.EntityId);
                    viewModel.EntityRules = await _entityRuleService.GetEntityRules(viewModelRequest.EntityId, viewModelRequest.LanguageCode);
                    EntityMembersViewModel entityMembersViewModel = await _entityService.GetEntitiesMembersViewModel(viewModelRequest.EntityId, viewModelRequest.LanguageCode);
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

                ShiftDTO shift = await _shiftService.GetShiftById(addScheduleEntryDTO.ShiftId, addScheduleEntryDTO.LanguageCode);
                if (shift == null)
                {
                    response.Message = ScheduleRelatedMessages.ShiftNotFound;
                    return response;
                }

                ScheduleEntry scheduleEntry = _mapper.Map<ScheduleEntry>(addScheduleEntryDTO);
                scheduleEntry.ScheduleEntryWorkers = new List<ScheduleEntryWorkers>();
                scheduleEntry.ScheduleEntryId = _generalService.GenerateGuid();
                scheduleEntry.ScheduleStartDate = scheduleEntry.ScheduleStartDate.Add(shift.ShiftStartHour);

                TimeSpan totalBreakIncludedDuration = shift.ShiftBreakDTOs
                    .Where(sb => sb.IncludedInShift)
                    .Select(sb => sb.ShiftBreakDuration)
                    .Aggregate(TimeSpan.Zero, (sum, next) => sum.Add(next));

                scheduleEntry.ScheduleEndDate = scheduleEntry.ScheduleStartDate.Add(shift.ShiftDuration).Add(totalBreakIncludedDuration);

                try
                {
                    await _unitOfWork.BeginTransactionAsync();

                    scheduleEntry = await _entityScheduleRepository.Add(scheduleEntry);

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
                                    WorkerId = workerId
                                };
                                await _scheduleEntryWorkersRepository.Add(scheduleEntryWorkers);
                            }
                        }

                        await _unitOfWork.CommitAsync();

                        response.Success = true;
                        response.Message = ScheduleRelatedMessages.AddNewScheduleEntrySuccess;
                        response.Result = await GetScheduleEntryById(scheduleEntry.ScheduleEntryId, addScheduleEntryDTO.LanguageCode);
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

            ScheduleEntry scheduleEntry = await _entityScheduleRepository.GetById(scheduleParticipantOp.ScheduleEntryId);
            if (scheduleEntry == null)
            {
                response.Message = ScheduleRelatedMessages.ScheduleEntryNotFound;
                return response;
            }

            ScheduleEntryWorkers scheduleEntryWorker = new ScheduleEntryWorkers
            {
                ScheduleEntryId = scheduleParticipantOp.ScheduleEntryId,
                WorkerId = scheduleParticipantOp.WorkerId
            };

            scheduleEntryWorker = await _scheduleEntryWorkersRepository.Add(scheduleEntryWorker);

            if (scheduleEntryWorker != null)
            {
                response.Success = true;
                response.Message = ScheduleRelatedMessages.AddScheduleParticipantSuccess;
                response.Result = await GetScheduleEntryById(scheduleEntry.ScheduleEntryId, scheduleParticipantOp.LanguageCode);
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
            IEnumerable<ScheduleEntry> scheduleEntries = await _entityScheduleRepository.GetScheduleEntries(viewModelRequest.EntityId, viewModelRequest.WorkerId, viewModelRequest.StartDateSearch, viewModelRequest.EndDateSearch);

            // For each schedule entry
            foreach (ScheduleEntry entry in scheduleEntries)
            {
                scheduleEntryDTOs.Add(await GetScheduleEntryById(entry.ScheduleEntryId, viewModelRequest.LanguageCode));
            }

            return scheduleEntryDTOs;
        }

        #endregion

        #region Get Schedule Entry By Id

        public async Task<ScheduleEntryDTO> GetScheduleEntryById(string scheduleEntryId, string languageCode)
        {
            ScheduleEntryDTO scheduleEntryDTO = new ScheduleEntryDTO();

            ScheduleEntry scheduleEntry = await _entityScheduleRepository.GetById(scheduleEntryId);
            ShiftDTO shift = await _shiftService.GetShiftById(scheduleEntry.ShiftId, languageCode);

            // Get respective participants of said schedule entry
            IEnumerable<ScheduleEntryWorkers> scheduleEntryWorkers = await _scheduleEntryWorkersRepository.GetScheduleEntryWorkers(scheduleEntry.ScheduleEntryId);

            // Extract participant ids
            List<string> participantsIds = scheduleEntryWorkers.Select(i => i.WorkerId).ToList();

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

            if(createEntityScheduleDTO != null && !string.IsNullOrEmpty(createEntityScheduleDTO.EntityId) && !string.IsNullOrEmpty(createEntityScheduleDTO.WorkerId))
            {
                bool isOwner = await _entityWorkerRepository.IsMemberOwner(createEntityScheduleDTO.EntityId, createEntityScheduleDTO.WorkerId);

                if (isOwner)
                {
                    await _unitOfWork.BeginTransactionAsync();

                    List<ScheduleEntryDTO> scheduleEntryDTOs = new List<ScheduleEntryDTO>();
                    try
                    {
                        #region Delete Entries

                        // Get Entity Schedules within a specific time period
                        List<ScheduleEntry> scheduleEntries = await _entityScheduleRepository.GetScheduleEntries(createEntityScheduleDTO.EntityId, string.Empty, createEntityScheduleDTO.StartDate, createEntityScheduleDTO.EndDate);

                        // Delete all of them
                        foreach (ScheduleEntry scheduleEntry in scheduleEntries )
                        {
                            BaseResponse<bool> deleteScheduleResponse = await DeleteScheduleEntry(scheduleEntry.ScheduleEntryId);

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

                        // Get Base Entity Rules
                        List<EntityRuleDTO> baseEntityRuleDTOs = await _baseEntityRuleService.GetBaseEntityRulesAsEntityRules(createEntityScheduleDTO.LanguageCode);

                        // Get Rules
                        if(createEntityScheduleDTO.FilteredRules.Count() == 0)
                            ruleDTOs = await _entityRuleService.GetEntityRules(createEntityScheduleDTO.EntityId, createEntityScheduleDTO.LanguageCode);
                        else
                            ruleDTOs = await _entityRuleService.GetSpecificRules(createEntityScheduleDTO.FilteredRules, createEntityScheduleDTO.LanguageCode);

                        // Add missing Base Entity Rules to ruleDTOs
                        foreach (var baseEntityRule in baseEntityRuleDTOs)
                        {
                            if (!ruleDTOs.Any(r => r.RuleTypeId == baseEntityRule.RuleTypeId))
                            {
                                ruleDTOs.Add(baseEntityRule);
                            }
                        }

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
                                    ScheduleEntryId = _generalService.GenerateGuid(),
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
                                scheduleEntry = await _entityScheduleRepository.Add(scheduleEntry);

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

                        scheduleEntryDTOs = await _scheduleGeneratorService.FillOutSchedule(scheduleEntryDTOs, shifts, ruleDTOs, entityWorkerMemberDTOs, createEntityScheduleDTO);
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
                IEnumerable<ScheduleEntryWorkers> scheduleEntryWorkers = await _scheduleEntryWorkersRepository.GetScheduleEntryWorkers(scheduleEntryId);

                if (scheduleEntryWorkers.Count() != 0)
                    await _scheduleEntryWorkersRepository.DeleteRange(scheduleEntryWorkers);

                await _entityScheduleRepository.Delete(scheduleEntryId);
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

        #region Rule Validation - Methods


        #endregion

        #endregion


    }
}
