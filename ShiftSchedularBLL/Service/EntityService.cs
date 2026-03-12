using AutoMapper;
using AutoMapper.Execution;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.Queries;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.QueryModels;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.Dashboard;
using ShiftSchedularRL.Resources.Home;
using ShiftSchedularRL.Resources.MemberManagement;
using ShiftSchedularRL.Resources.Shared;
using System.Collections;

namespace ShiftSchedularBLL.Service
{
    public class EntityService : IEntityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICryptographyService _cryptographyService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISkillService _skillService;
        private readonly IShiftService _shiftService;
        private readonly IEntityTypeService _entityTypeService;
        private readonly IGeneralService _generalService;
        private readonly ILanguageAccessor _languageAccessor;

        #region Constructor

        public EntityService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICryptographyService cryptographyService,
            UserManager<ApplicationUser> userManager,
            ISkillService skillService,
            IShiftService shiftService,
            IEntityTypeService entityTypeService,
            IGeneralService generalService,
            ILanguageAccessor languageAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cryptographyService = cryptographyService;
            _userManager = userManager;
            _skillService = skillService;
            _shiftService = shiftService;
            _entityTypeService = entityTypeService;
            _generalService = generalService;
            _languageAccessor = languageAccessor;
        }

        #endregion

        #region Methods

        #region Add Entity

        /// <summary>
        /// Adds a new Entity instance
        /// </summary>
        /// <param name="newEntity"></param>
        /// <returns></returns>
        public async Task<BaseResponse<Entity>> AddEntity(FormEntityDTO newEntity)
        {
            BaseResponse<Entity> response = new BaseResponse<Entity>();

            if (newEntity != null)
            {
                if (string.IsNullOrEmpty(newEntity.EntityName))
                {
                    response.Message = EntitiesRelatedMessages.EntityNameEmptyError;
                    return response;
                }

                if (string.IsNullOrEmpty(newEntity.WorkerId))
                {
                    response.Message = EntitiesRelatedMessages.CreateEntityWorkerOwnerEmptyError;
                    return response;
                }

                EntityType entityTypeInstance = await _unitOfWork.GetGenericRepository<EntityType>().GetById(newEntity.EntityTypeId); // _entityTypeRepository.GetById(newEntity.EntityTypeId);

                if (entityTypeInstance != null && !string.IsNullOrEmpty(newEntity.WorkerId))
                {
                    await _unitOfWork.BeginTransactionAsync();

                    try
                    {
                        // Add Entity
                        Entity entity = _mapper.Map<Entity>(newEntity);
                        entity = await _unitOfWork.GetGenericRepository<Entity>().Add(entity);

                        // If there is no identifier associated to the entity
                        if (entity.EntityId == Guid.Empty)
                        {
                            await _unitOfWork.RollbackAsync();
                            response.Message = EntitiesRelatedMessages.CreateEntityUnexpectedError;
                            return response;
                        }

                        List<Skill> Skills = await _unitOfWork.SkillRepository.GetSkillsByNames(new List<string> { SkillsConstants.GENERAL_WORKER, SkillsConstants.MANAGEMENT });

                        DateTime nowUtcTime = DateTime.UtcNow;

                        EntityWorker entityWorker = new EntityWorker
                        {
                            ApplicationUserId = newEntity.WorkerId,
                            EntityId = entity.EntityId,
                            ActiveWorkerStatus = true,
                            IsOwner = true,
                            CanCreateSchedules = true,
                            DateOfJoin = nowUtcTime,
                            PartOfRotation = true,
                            WorksWeekDays = true,
                            WorksWeekends = true
                        };

                        // Add entity worker and commit
                        await _unitOfWork.EntityWorkerRepository.Add(entityWorker);

                        List<EntityWorkerSkill> entityWorkerSkills = new List<EntityWorkerSkill>();
                        foreach (Skill skill in Skills)
                        {
                            EntityWorkerSkill entityWorkerSkill = new EntityWorkerSkill
                            {
                                ApplicationUserId = newEntity.WorkerId,
                                EntityId = entity.EntityId,
                                SkillId = skill.SkillId
                            };

                            entityWorkerSkills.Add(entityWorkerSkill);
                        }

                        // Add entity worker skills
                        await _unitOfWork.EntityWorkerSkillRepository.AddRange(entityWorkerSkills);

                        await _unitOfWork.CommitAsync();

                        // Set response values
                        response.Result = entity;
                        response.Success = true;
                        response.Message = EntitiesRelatedMessages.CreateEntitySuccess;
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                        response.Message = EntitiesRelatedMessages.CreateEntityUnexpectedError;
                    }
                    finally
                    {
                        _unitOfWork.Dispose();
                    }
                }
                else
                {
                    response.Message = EntitiesRelatedMessages.CreateEntityUnexpectedError;
                    return response;
                }
            }

            return response;
        }

        #endregion

        #region Delete Entity By Id

        /// <summary>
        /// Deletes an entity by its identifier
        /// </summary>
        /// <param name="entityId">Identifier of the entity</param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> DeleteEntityById(Guid entityId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            // if entity identifier is different than null
            if (entityId != Guid.Empty)
            {
                Entity entityInstance = await _unitOfWork.GetGenericRepository<Entity>().GetById(entityId);
                IEnumerable<EntityWorker> entityWorkers = await _unitOfWork.EntityWorkerRepository.GetByEntityId(entityId);
                IEnumerable<EntityWorkerSkill> entityWorkerSkills = await _unitOfWork.EntityWorkerSkillRepository.GetByEntityId(entityId);
                IEnumerable<EntityUserBot> entityUserBots = await _unitOfWork.EntityUserBotRepository.GetUserBotsByEntityId(entityId);
                IEnumerable<EntityUserBotSkill> entityUserBotSkills = await _unitOfWork.EntityUserBotSkillRepository.GetByEntityId(entityId);

                if (entityInstance != null && entityInstance.EntityWorkers.Count != 0)
                {
                    await _unitOfWork.BeginTransactionAsync();

                    try
                    {
                        await _unitOfWork.EntityWorkerRepository.DeleteRange(entityInstance.EntityWorkers);
                        await _unitOfWork.EntityWorkerSkillRepository.DeleteRange(entityWorkerSkills);
                        await _unitOfWork.EntityUserBotRepository.DeleteRange(entityUserBots);
                        await _unitOfWork.EntityUserBotSkillRepository.DeleteRange(entityUserBotSkills);
                        await _unitOfWork.EntityWorkerInvitationRepository.DeleteAllByEntity(entityId);
                        await _unitOfWork.GetGenericRepository<Entity>().Delete(entityInstance.EntityId);
                        await _unitOfWork.CommitAsync();

                        response.Success = true;
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                        response.Message = EntitiesRelatedMessages.DeleteEntityUnexpectedError;
                    }
                    finally
                    {
                        _unitOfWork.Dispose();
                    }
                }
            }
            else
            {
                response.Message = EntitiesRelatedMessages.EntityNoIdentifierError;
            }

            return response;
        }

        #endregion

        #region Get All Entities

        /// <summary>
        /// Get All Entities
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Entity>> GetAllEntities()
        {
            return await _unitOfWork.GetGenericRepository<Entity>().GetAll();
        }

        #endregion

        #region Get Entity By Id

        /// <summary>
        /// Get Entity By Id
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public async Task<EntityDTO> GetEntityById(Guid entityId, string languageCode)
        {
            if (entityId != Guid.Empty)
            {
                Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(entityId);
                EntityType entityType = await _unitOfWork.GetGenericRepository<EntityType>().GetById(entity.EntityTypeId);
                EntityTypeLocalization entityTypeLocalization = await _unitOfWork.EntityTypeLocalizationRepository.GetEntityTypeLocalizationByIds(entityType.EntityTypeId, languageCode);

                int botsCount = await _unitOfWork.EntityUserBotRepository.GetUserBotsByEntityCount(entity.EntityId);
                int workersCount = await _unitOfWork.EntityWorkerRepository.GetTotalCountByEntity(entity.EntityId);

                EntityDTO entityDTO = new EntityDTO(entityId: entity.EntityId,
                                                                    entityName: entity.EntityName,
                                                                    entityDescription: entity.EntityDescription,
                                                                    entityTypeLocalized: entityTypeLocalization.EntityTypeDisplayValue,
                                                                    botsCount + workersCount);

                return entityDTO;
            }
            else
                return null;
        }

        #endregion

        #region Update Entity

        /// <summary>
        /// Updates an entity data
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> UpdateEntity(FormEntityDTO entity)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Message = EntitiesRelatedMessages.UpdateEntityUnexpectedError;

            if (entity == null)
            {
                response.Message = EntitiesRelatedMessages.EntityIsNullError;
                return response;
            }

            else if (string.IsNullOrEmpty(entity.EntityName))
            {
                response.Message = EntitiesRelatedMessages.EntityNameEmptyError;
                return response;
            }

            else if (entity.EntityTypeId == 0)
            {
                response.Message = EntitiesRelatedMessages.EntityTypeInvalidValueError;
                return response;
            }

            Entity entityToUpdate = await _unitOfWork.GetGenericRepository<Entity>().GetById(entity.EntityId);
            if (entityToUpdate == null)
            {
                response.Message = EntitiesRelatedMessages.EntityNotFound;
                return response;
            }

            entityToUpdate.EntityName = entity.EntityName;
            entityToUpdate.EntityTypeId = entity.EntityTypeId;
            entityToUpdate.EntityDescription = entity.EntityDescription;


            // Update the entity and set the message
            await _unitOfWork.GetGenericRepository<Entity>().Update(entityToUpdate);

            response.Success = true;
            response.Message = EntitiesRelatedMessages.UpdateEntitySuccess;

            return response;
        }

        #endregion

        #region Get Entities By Worker Id

        /// <summary>
        /// Get Entities By Worker Id
        /// </summary>
        /// <param name="workerId"></param>
        /// <returns></returns>
        public async Task<List<EntityWorkerDTO>> GetEntitiesByWorkerId(string workerId)
        {
            List<EntityWorkerDTO> entityWorkers = new List<EntityWorkerDTO>();

            if (!string.IsNullOrEmpty(workerId))
            {
                try
                {
                    // Get entity worker instances by worker identifier
                    IEnumerable<EntityWorkerDTO> entityWorkerDTOs = await _unitOfWork.EntityWorkerRepository.GetByWorkerId(workerId);

                    if (entityWorkerDTOs != null)
                        entityWorkers = entityWorkerDTOs.ToList();
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                }
            }

            return entityWorkers;
        }

        #endregion

        #region Get Entities Members View Model

        /// <summary>
        /// Gets the entity members view model
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="lcode">Language code</param>
        /// <returns></returns>
        public async Task<EntityMembersViewModel> GetEntitiesMembersViewModel(PagedModelRequest memberListModelRequest)
        {
            EntityMembersViewModel viewModel = new EntityMembersViewModel();

            viewModel.Skills = await _skillService.GetAllSkillsByLocalization(_languageAccessor.GetLanguageCode());

            viewModel.EntityUsedSkills = await GetEntitySkills(memberListModelRequest);

            viewModel.Shifts = await _shiftService.GetEntityShifts(memberListModelRequest.EntityId);

            viewModel.EntityMembers = await GetEntityMembers(memberListModelRequest.EntityId, new List<string>(), memberListModelRequest.NextPage, memberListModelRequest.ItemsPerPage);

            viewModel.EntityOwnerId = await _unitOfWork.EntityWorkerRepository.GetEntityOwnerId(memberListModelRequest.EntityId);

            return viewModel;
        }

        #endregion

        #region Get Assigned Worker Or Bot Shifts

        private async Task<IEnumerable<ShiftDTO>> GetAssignedWorkerOrBotShifts(bool isBot, Guid entityId, string workerId, string languageCode, IEnumerable<ShiftDTO> shifts = null)
        {
            List<ShiftDTO> entityShifts = new List<ShiftDTO>();

            try
            {
                if (shifts == null)
                {
                    entityShifts = (List<ShiftDTO>)await _shiftService.GetEntityShifts(entityId);
                }
                else
                {
                    entityShifts = (List<ShiftDTO>)shifts;
                }

                Guid workerGuid = _generalService.ParseStringToGuid(workerId);

                if (isBot)
                {
                    // viewModel.Shifts
                    IEnumerable<EntityUserBotShiftAssigned> userBotShiftAssigneds = await _unitOfWork.EntityUserBotShiftAssignedsRepository.GetAllByEntityIdAndUserBotId(entityId, workerGuid);
                    var assignedShiftIds = userBotShiftAssigneds.Select(x => x.ShiftId);

                    return entityShifts
                        .Where(shift => assignedShiftIds.Contains(shift.ShiftId));
                }
                else
                {
                    IEnumerable<EntityWorkerShiftAssigned> entityWorkerShiftAssigneds = await _unitOfWork.EntityWorkerShiftAssignedsRepository.GetAllByEntityIdAndUserId(entityId, workerGuid);
                    var assignedShiftIds = entityWorkerShiftAssigneds.Select(x => x.ShiftId);

                    return entityShifts
                        .Where(shift => assignedShiftIds.Contains(shift.ShiftId));
                }
            }
            catch (Exception ex)
            {
                string strError = ex.Message;
                return null;
            }

            return entityShifts;
        }

        #endregion

        #region Get Entity Members

        public async Task<List<EntityWorkerMemberDTO>> GetEntityMembers(Guid entityId, List<string> workers)
        {
            List<EntityWorkerMemberDTO> entityMembers = new List<EntityWorkerMemberDTO>();

            List<EntityWorkerMemberModel> entityWorkerMembers = await GetAllMembers(entityId, workers);

            entityMembers = await ProcessMemberData(entityId, entityWorkerMembers, _languageAccessor.GetLanguageCode());

            return entityMembers;
        }

        #endregion

        #region Get Entity Members Pagination

        public async Task<PagedList<EntityWorkerMemberDTO>> GetEntityMembers(Guid entityId, List<string> workers, int nextPage = 0, int itemsPerPage = 0)
        {
            List<EntityWorkerMemberModel> entityWorkerMembers = await GetAllMembers(entityId, workers);

            int count = entityWorkerMembers.Count;
            // If pagination is being used
            if (nextPage != 0 && itemsPerPage != 0)
            {
                int skipRows = (nextPage - 1) * itemsPerPage;

                entityWorkerMembers = entityWorkerMembers.Skip(skipRows).Take(itemsPerPage).ToList();
            }

            List<EntityWorkerMemberDTO> processedMembers = await ProcessMemberData(entityId, entityWorkerMembers, _languageAccessor.GetLanguageCode());

            try
            {
                PagedList<EntityWorkerMemberDTO> pagedListMembers = PagedList<EntityWorkerMemberDTO>.Create(processedMembers.AsQueryable(), count, nextPage, itemsPerPage);

                return pagedListMembers;
            }
            catch (Exception exs)
            {
                string str = exs.Message;
                throw;
            }

            return null;
        }

        #endregion

        #region Get All Members

        /// <summary>
        /// Gets all members (workers and bots) for an entity.
        /// </summary>
        /// <param name="entityId">The entity identifier</param>
        /// <param name="workers">Optional list of worker IDs to filter by. If empty, returns all members.</param>
        /// <returns>List of entity members (workers and bots)</returns>
        private async Task<List<EntityWorkerMemberModel>> GetAllMembers(Guid entityId, List<string> workers)
        {
            // Get regular members
            IEnumerable<EntityWorkerMemberModel> entityWorkerMembers = await _unitOfWork.EntityWorkerRepository.GetDistinctMembersByEntityId(entityId);

            // Get user bots
            IEnumerable<EntityWorkerMemberModel> userBots = await _unitOfWork.EntityUserBotRepository.GetDistinctUserBotsByEntityId(entityId);

            List<EntityWorkerMemberModel> members = new();

            // Filter by specific workers/bots if list is provided, otherwise return all
            if (workers.Count > 0)
            {
                foreach (string workerId in workers)
                {
                    EntityWorkerMemberModel member = entityWorkerMembers.FirstOrDefault(m => m.WorkerId == workerId);
                    if (member != null)
                    {
                        members.Add(member);
                    }
                    else
                    {
                        bool isIdBinary = workerId.Length < 25;

                        EntityWorkerMemberModel bot = userBots.FirstOrDefault(b => isIdBinary ? b.WorkerId == workerId
                            : _generalService.ParseStringToGuid(b.WorkerId).ToString() == workerId);

                        if (bot != null)
                        {
                            members.Add(bot);
                        }
                    }
                }
            }
            else
            {
                members = entityWorkerMembers.Concat(userBots).ToList();
            }

            if (members != null)
            {
                // Order by name
                members = members.OrderBy(i => i.WorkerName).ToList();
            }

            return members;
        }

        #endregion

        #region Process Member Data

        private async Task<List<EntityWorkerMemberDTO>> ProcessMemberData(Guid entityId, List<EntityWorkerMemberModel> entityWorkers, string lcode)
        {
            List<EntityWorkerMemberDTO> entityMembers = new List<EntityWorkerMemberDTO>();

            // Load skills and shifts once
            List<SkillLocalizedDTO> skills = await _skillService.GetAllSkillsByLocalization(lcode);
            IEnumerable<ShiftDTO> shifts = await _shiftService.GetEntityShifts(entityId);

            // Batch load ALL shift assignments for workers and bots in TWO queries
            var workerIds = entityWorkers
                .Where(w => !w.IsBot && !w.PartOfRotation)
                .Select(w => w.WorkerId)  // ApplicationUserId is string, not GUID
                .Distinct()
                .ToList();

            var botIds = entityWorkers
                .Where(w => w.IsBot && !w.PartOfRotation)
                .Select(w => _generalService.ParseStringToGuid(w.WorkerId))
                .Distinct()
                .ToList();

            // Batch load worker shift assignments (string-based ApplicationUserId)
            var workerShiftAssignments = new Dictionary<string, List<Guid>>();
            if (workerIds.Any())
            {
                var allWorkerAssignments = await _unitOfWork.EntityWorkerShiftAssignedsRepository
                    .GetAllByEntityId(entityId);

                workerShiftAssignments = allWorkerAssignments
                    .Where(a => workerIds.Contains(a.ApplicationUserId))
                    .GroupBy(a => a.ApplicationUserId)
                    .ToDictionary(g => g.Key, g => g.Select(a => a.ShiftId).ToList());
            }

            // Batch load bot shift assignments
            var botShiftAssignments = new Dictionary<Guid, List<Guid>>();
            if (botIds.Any())
            {
                var allBotAssignments = await _unitOfWork.EntityUserBotShiftAssignedsRepository
                    .GetAllByEntityId(entityId);

                botShiftAssignments = allBotAssignments
                    .Where(a => botIds.Contains(a.UserBotId))
                    .GroupBy(a => a.UserBotId)
                    .ToDictionary(g => g.Key, g => g.Select(a => a.ShiftId).ToList());
            }

            // Process each member using pre-loaded data
            foreach (EntityWorkerMemberModel entityWorkerMember in entityWorkers)
            {
                EntityWorkerMemberDTO entityWorkerMemberDTO = new EntityWorkerMemberDTO();
                entityWorkerMemberDTO = _mapper.Map(entityWorkerMember, entityWorkerMemberDTO);

                int[] skillIds = entityWorkerMember.SkillIds.Split(',').Select(int.Parse).ToArray();

                entityWorkerMemberDTO.SkillSet = skills.Where(i => skillIds.Contains(i.SkillId))
                                            .Select(s => new SkillLocalizedDTO
                                            {
                                                SkillId = s.SkillId,
                                                SkillLocalizedName = s.SkillLocalizedName,
                                                SkillHexBGColor = s.SkillHexBGColor,
                                                SkillHexFontColor = s.SkillHexFontColor
                                            }).ToList();

                if (!entityWorkerMemberDTO.PartOfRotation)
                {
                    List<Guid> assignedShiftIds = null;

                    // Lookup shift assignments from pre-loaded dictionaries
                    if (entityWorkerMemberDTO.IsBot)
                    {
                        Guid workerGuid = _generalService.ParseStringToGuid(entityWorkerMemberDTO.WorkerId);
                        botShiftAssignments.TryGetValue(workerGuid, out assignedShiftIds);
                    }
                    else
                    {
                        // Workers use string-based ApplicationUserId
                        workerShiftAssignments.TryGetValue(entityWorkerMemberDTO.WorkerId, out assignedShiftIds);
                    }

                    // Filter shifts based on assignments
                    if (assignedShiftIds != null && assignedShiftIds.Any())
                    {
                        entityWorkerMemberDTO.AssignedShifts = shifts
                            .Where(shift => assignedShiftIds.Contains(shift.ShiftId))
                            .ToList();
                    }
                    else
                    {
                        entityWorkerMemberDTO.AssignedShifts = new List<ShiftDTO>();
                    }
                }
                else
                {
                    entityWorkerMemberDTO.AssignedShifts = new List<ShiftDTO>();
                }

                entityMembers.Add(entityWorkerMemberDTO);
            }

            return entityMembers;
        }

        #endregion

        #region Get Entity Skills

        /// <summary>
        /// Gets all the entity skills
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<List<SkillLocalizedDTO>> GetEntitySkills(BaseViewModelRequest baseViewModelRequest)
        {
            List<SkillLocalizedDTO> skillLocalizedDTOs = new List<SkillLocalizedDTO>();

            if (baseViewModelRequest.EntityId != Guid.Empty)
            {
                // Gets all skills
                List<SkillLocalizedDTO> allSkills = await _skillService.GetAllSkillsByLocalization(_languageAccessor.GetLanguageCode());

                // Gets all working members
                IEnumerable<int> entityUserSkills = await _unitOfWork.EntityWorkerRepository.GetDistinctSkillsByEntityId(baseViewModelRequest.EntityId);
                IEnumerable<int> entityUserBotsSkills = await _unitOfWork.EntityUserBotRepository.GetDistinctSkillsByEntityId(baseViewModelRequest.EntityId);

                // Add the localized skills into the list, based on what exists in the entity skillset
                skillLocalizedDTOs = allSkills
                    .Where(skill => entityUserSkills.Union(entityUserBotsSkills).Contains(skill.SkillId))
                    .ToList();
            }

            return skillLocalizedDTOs;
        }

        #endregion

        #region Get Entity Profile View Model

        /// <summary>
        /// Gets the entity profile view model
        /// </summary>
        /// <param name="entityProfileViewModelRequest"></param>
        /// <returns></returns>
        public async Task<EntityProfileViewModel> GetEntityProfileViewModel(BaseViewModelRequest entityProfileViewModelRequest)
        {
            EntityProfileViewModel entityProfileViewModel = new EntityProfileViewModel();

            if (entityProfileViewModelRequest != null && entityProfileViewModelRequest.EntityId != Guid.Empty)
            {
                Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(entityProfileViewModelRequest.EntityId);
                EntityWorker entityWorkerInstance = await _unitOfWork.EntityWorkerRepository.GetByWorkerAndEntity(entityProfileViewModelRequest.WorkerId, entityProfileViewModelRequest.EntityId);
                EntityType entityType = await _unitOfWork.GetGenericRepository<EntityType>().GetById(entity.EntityTypeId);
                EntityTypeLocalization entityTypeLocalization = await _unitOfWork.EntityTypeLocalizationRepository.GetEntityTypeLocalizationByIds(entityType.EntityTypeId, _languageAccessor.GetLanguageCode());

                try
                {
                    int botsCount = await _unitOfWork.EntityUserBotRepository.GetUserBotsByEntityCount(entity.EntityId);
                    int workersCount = await _unitOfWork.EntityWorkerRepository.GetTotalCountByEntity(entity.EntityId);

                    entityProfileViewModel.EntityDTO = new EntityDTO(entityId: entity.EntityId,
                                                                    entityName: entity.EntityName,
                                                                    entityDescription: entity.EntityDescription,
                                                                    entityTypeLocalized: entityTypeLocalization.EntityTypeDisplayValue,
                                                                    botsCount + workersCount);
                    entityProfileViewModel.AllowEdit = entityWorkerInstance.IsOwner;

                    if (entityProfileViewModel.AllowEdit)
                    {
                        entityProfileViewModel.EntityTypeLocalizeds = await _entityTypeService.GetAllEntityTypesByLocalization(_languageAccessor.GetLanguageCode());
                    }
                }
                catch (Exception ex)
                {
                    string strErr = ex.Message;
                }

            }

            return entityProfileViewModel;
        }

        #endregion

        #region Add New Entity Member

        /// <summary>
        /// Adds a new Entity member
        /// </summary>
        /// <param name="newMemberDTO"></param>
        /// <returns></returns>
        public async Task<BaseResponse<object>> AddNewEntityMember(AddNewMemberDTO newMemberDTO)
        {
            BaseResponse<object> response = new BaseResponse<object>();
            response.Message = EntityWorkerRelatedMessages.AddNewMemberUnexpectedError;
            response.Success = false;

            if (newMemberDTO != null)
            {
                if (newMemberDTO.DestinationEntityId == Guid.Empty)
                {
                    response.Message = EntityWorkerRelatedMessages.MemberDestinationEntityEmpty;
                    return response;
                }

                // Check if there is a name to the Member
                else if (newMemberDTO.IsBot && string.IsNullOrEmpty(newMemberDTO.MemberName))
                {
                    response.Message = EntityWorkerRelatedMessages.MemberNameEmpty;
                    return response;
                }

                // If not a bot and the email is empty
                else if (!newMemberDTO.IsBot && string.IsNullOrEmpty(newMemberDTO.MemberEmail))
                {
                    response.Message = EntityWorkerRelatedMessages.AddNewMemberEmailEmpty;
                    return response;
                }

                // If not a bot and the email is in invalid format
                else if (!newMemberDTO.IsBot && !_generalService.ValidateRegexEmail(newMemberDTO.MemberEmail))
                {
                    response.Message = EntityWorkerRelatedMessages.AddNewMemberEmailInvalid;
                    return response;
                }

                else if (newMemberDTO.IsBot && !newMemberDTO.PartOfRotation && newMemberDTO.AssignedShifts.Count == 0)
                {
                    response.Message = EntityWorkerRelatedMessages.NotPartOfRotationEmptyShifts;
                    return response;
                }

                Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(newMemberDTO.DestinationEntityId);

                // Destination Entity exists
                if (entity != null)
                {
                    // Save record of time instance
                    DateTime nowUTCTime = DateTime.UtcNow;

                    // Adding a bot member
                    if (newMemberDTO.IsBot)
                        response = await AddUserBotToEntity(entity, newMemberDTO, nowUTCTime);
                    else
                        response = await SendEntityInvitationToUser(newMemberDTO, nowUTCTime);
                }
                // No entity found
                else
                {
                    response.Message = EntityWorkerRelatedMessages.AddNewMemberNoDestinationEntityFound;
                    return response;
                }
            }

            return response;
        }

        #endregion

        #region Add User Bot To Entity

        private async Task<BaseResponse<object>> AddUserBotToEntity(Entity entity, AddNewMemberDTO newMemberDTO, DateTime nowUTCTime)
        {
            BaseResponse<object> response = new BaseResponse<object>();

            UserBot newUserBot = _mapper.Map<UserBot>(newMemberDTO);
            newUserBot.DateOfCreation = nowUTCTime;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Add new bot
                newUserBot = await _unitOfWork.UserBotRepository.Add(newUserBot);

                // Add bot to entity
                EntityUserBot entityUserBot = _mapper.Map<EntityUserBot>(newMemberDTO);
                entityUserBot.UserBotId = newUserBot.UserBotId;

                await _unitOfWork.EntityUserBotRepository.Add(entityUserBot);

                // Add bot skills
                List<EntityUserBotSkill> entityUserBotSkills = new List<EntityUserBotSkill>();

                foreach (SkillLocalizedDTO skill in newMemberDTO.AssignedSkills)
                {
                    EntityUserBotSkill entityUserBotSkill = new EntityUserBotSkill
                    {
                        EntityId = entity.EntityId,
                        UserBotId = newUserBot.UserBotId,
                        SkillId = skill.SkillId
                    };
                    entityUserBotSkills.Add(entityUserBotSkill);
                }

                await _unitOfWork.EntityUserBotSkillRepository.AddRange(entityUserBotSkills);

                // Add bot shifts
                foreach (ShiftDTO shift in newMemberDTO.AssignedShifts)
                {
                    EntityUserBotShiftAssigned userBotShiftAssigned = new EntityUserBotShiftAssigned
                    {
                        EntityId = entity.EntityId,
                        UserBotId = newUserBot.UserBotId,
                        ShiftId = shift.ShiftId,
                    };

                    await _unitOfWork.GetGenericRepository<EntityUserBotShiftAssigned>().Add(userBotShiftAssigned);
                }

                // Commit changes
                await _unitOfWork.CommitAsync();

                response.Message = EntityWorkerRelatedMessages.AddNewMemberBotSuccessful;
                response.Success = true;
                response.Result = new EntityWorkerMemberDTO
                {
                    WorkerId = newUserBot.UserBotId.ToString(),
                    WorkerName = newMemberDTO.MemberName,
                    CanCreateSchedules = false,
                    IsBot = true,
                    IsOwner = false,
                    DateOfJoin = nowUTCTime,
                    SkillSet = newMemberDTO.AssignedSkills,
                    PartOfRotation = newMemberDTO.PartOfRotation,
                    AssignedShifts = newMemberDTO.AssignedShifts,
                    MultipleShiftAssignments = newMemberDTO.MultipleShiftAssignments,
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = EntityWorkerRelatedMessages.AddNewMemberUnexpectedError;
            }

            return response;
        }

        #endregion

        #region Send Entity Invitation to User

        public async Task<BaseResponse<object>> SendEntityInvitationToUser(AddNewMemberDTO newMemberDTO, DateTime nowUTCTime)
        {
            BaseResponse<object> response = new BaseResponse<object>();

            // Check if there is a entity worker with that email already in the entity
            ApplicationUser possibleWorker = await _userManager.FindByEmailAsync(newMemberDTO.MemberEmail);

            if (possibleWorker != null && await _unitOfWork.EntityWorkerRepository.IsWorkerInEntity(newMemberDTO.DestinationEntityId, possibleWorker.Id))
            {
                response.Message = EntityWorkerRelatedMessages.AddNewMemberAlreadyInEntity;
                return response;
            }

            string skillsAggregated = newMemberDTO.AssignedSkills.Select(i => i.SkillId).Aggregate("", (i, j) => i + "," + j);

            EntityWorkerInvitation entityWorkerInvitation = new EntityWorkerInvitation
            {
                EntityId = newMemberDTO.DestinationEntityId,
                Email = newMemberDTO.MemberEmail,
                ApplicationUserId = possibleWorker != null ? possibleWorker.Id : null,
                InviteDate = nowUTCTime,
                SkillsetIds = skillsAggregated,
                PartOfRotation = newMemberDTO.PartOfRotation,
                WorksWeekDays = newMemberDTO.WorksWeekDays,
                WorksWeekends = newMemberDTO.WorksWeekends,
                MultipleShiftAssignments = newMemberDTO.MultipleShiftAssignments
            };

            await _unitOfWork.EntityWorkerInvitationRepository.Add(entityWorkerInvitation);

            response.Result = true;
            response.Message = EntityWorkerRelatedMessages.AddNewMemberInvitationSuccessful;
            response.Success = true;

            return response;
        }

        #endregion

        #region Update Entity Member

        /// <summary>
        /// Updates the Entity Member Skillset
        /// Updates the name of bot if applicable
        /// </summary>
        /// <param name="updateEntityMemberDTO"></param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> UpdateEntityMember(EditMemberDTO updateEntityMemberDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Message = EntityWorkerRelatedMessages.AddNewMemberUnexpectedError;
            response.Success = false;

            if (updateEntityMemberDTO != null)
            {
                if (string.IsNullOrEmpty(updateEntityMemberDTO.WorkerId))
                {
                    response.Message = EntityWorkerRelatedMessages.MemberIdentifierEmpty;
                    return response;
                }

                else if (updateEntityMemberDTO.EntityId == Guid.Empty)
                {
                    response.Message = EntityWorkerRelatedMessages.MemberDestinationEntityEmpty;
                    return response;
                }

                else if (updateEntityMemberDTO.IsBot && string.IsNullOrEmpty(updateEntityMemberDTO.WorkerName))
                {
                    response.Message = EntityWorkerRelatedMessages.MemberNameEmpty;
                    return response;
                }

                else if (updateEntityMemberDTO.AssignedSkills.Count == 0)
                {
                    response.Message = EntityWorkerRelatedMessages.SkillSetRequired;
                    return response;
                }

                else if (!updateEntityMemberDTO.PartOfRotation && updateEntityMemberDTO.AssignedShifts.Count == 0)
                {
                    response.Message = EntityWorkerRelatedMessages.NotPartOfRotationEmptyShifts;
                    return response;
                }

                Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(updateEntityMemberDTO.EntityId);
                if (entity != null)
                {
                    try
                    {
                        await _unitOfWork.BeginTransactionAsync();

                        // If its a bot, update the name, rotation and shift assignments
                        if (updateEntityMemberDTO.IsBot)
                        {
                            response = await UpdateEntityUserBot(updateEntityMemberDTO);
                        }
                        // Not a Bot
                        else
                        {
                            response = await UpdateEntityMemberData(updateEntityMemberDTO);
                        }

                        if (response.Success)
                        {
                            await _unitOfWork.CommitAsync();
                            response.Result = true;
                            response.Message = EntityWorkerRelatedMessages.UpdateMemberSuccess;
                            response.Success = true;
                        }
                        else
                        {
                            await _unitOfWork.RollbackAsync();
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
                else
                {
                    response.Message = EntitiesRelatedMessages.EntityNotFound;
                    return response;
                }
            }

            return response;
        }

        #endregion

        #region Update Entity User Bot

        private async Task<BaseResponse<bool>> UpdateEntityUserBot(EditMemberDTO userBotData)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            Guid userBotId = _generalService.ParseStringToGuid(userBotData.WorkerId);
            UserBot userBot = await _unitOfWork.UserBotRepository.GetById(userBotId);
            if (userBot != null)
            {
                // Update User Bot Instance if name is different
                if (userBotData.WorkerName != userBot.UserDisplayName)
                {
                    userBot.UserDisplayName = userBotData.WorkerName;
                    await _unitOfWork.UserBotRepository.Update(userBot);
                }

                // Get Entity User Bot
                EntityUserBot entityUserBot = await _unitOfWork.EntityUserBotRepository.GetEntityUserBotByEntityAndId(userBotData.EntityId, userBotId);
                if (entityUserBot != null)
                {
                    entityUserBot.WorksWeekDays = userBotData.WorksWeekDays;
                    entityUserBot.WorksWeekends = userBotData.WorksWeekends;
                    entityUserBot.MultipleShiftAssignments = userBotData.MultipleShiftAssignments;

                    // If Part of Rotation flags are different and the user has been assigned as part of rotation
                    if (userBotData.PartOfRotation != entityUserBot.PartOfRotation && userBotData.PartOfRotation)
                    {
                        // Get and clear all specific assignments
                        IEnumerable<EntityUserBotShiftAssigned> userBotShiftAssigneds = await _unitOfWork.EntityUserBotShiftAssignedsRepository.GetAllByEntityIdAndUserBotId(userBotData.EntityId, userBotId);
                        if (userBotShiftAssigneds.Count() != 0)
                        {
                            await _unitOfWork.EntityUserBotShiftAssignedsRepository.DeleteAllByEntityIdAndUserBotId(userBotData.EntityId, userBotId);
                        }
                    }

                    entityUserBot.PartOfRotation = userBotData.PartOfRotation;
                    await _unitOfWork.EntityUserBotRepository.Update(entityUserBot);

                    // Not part of the rotation
                    if (!entityUserBot.PartOfRotation)
                    {
                        // Delete all pre-existing assignments
                        await _unitOfWork.EntityUserBotShiftAssignedsRepository.DeleteAllByEntityIdAndUserBotId(userBotData.EntityId, userBotId);

                        // For each assignment, add it to the Entity assigned shifts
                        List<EntityUserBotShiftAssigned> newBotShiftAssignments = new List<EntityUserBotShiftAssigned>();
                        foreach (ShiftDTO shift in userBotData.AssignedShifts)
                        {
                            newBotShiftAssignments.Add(new EntityUserBotShiftAssigned
                            {
                                EntityId = userBotData.EntityId,
                                UserBotId = userBotId,
                                ShiftId = shift.ShiftId
                            });
                        }

                        await _unitOfWork.EntityUserBotShiftAssignedsRepository.AddRange(newBotShiftAssignments);
                    }

                    // Delete all pre-existing skills
                    await _unitOfWork.EntityUserBotSkillRepository.DeleteAllByEntityIdAndUserBotId(userBotData.EntityId, userBotId);

                    List<EntityUserBotSkill> entityUserBotSkills = new List<EntityUserBotSkill>();
                    foreach (SkillLocalizedDTO skillLocalizedDTO in userBotData.AssignedSkills)
                    {
                        entityUserBotSkills.Add(new EntityUserBotSkill
                        {
                            EntityId = userBotData.EntityId,
                            UserBotId = userBotId,
                            SkillId = skillLocalizedDTO.SkillId
                        });
                    }

                    await _unitOfWork.EntityUserBotSkillRepository.AddRange(entityUserBotSkills);

                    response.Success = true;
                    response.Result = true;
                    response.Message = EntityWorkerRelatedMessages.UpdateMemberSuccess;
                }
                else
                {
                    response.Message = EntityWorkerRelatedMessages.MemberNotFound;
                    return response;
                }
            }
            else
            {
                response.Message = EntityWorkerRelatedMessages.MemberNotFound;
                return response;
            }
            return response;
        }

        #endregion

        #region Update Entity Member Data

        private async Task<BaseResponse<bool>> UpdateEntityMemberData(EditMemberDTO editMemberDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            Guid userId = _generalService.ParseStringToGuid(editMemberDTO.WorkerId);
            EntityWorker entityWorker = await _unitOfWork.EntityWorkerRepository.GetByWorkerAndEntity(editMemberDTO.WorkerId, editMemberDTO.EntityId);
            if (entityWorker != null)
            {
                entityWorker.WorksWeekDays = editMemberDTO.WorksWeekDays;
                entityWorker.WorksWeekends = editMemberDTO.WorksWeekends;
                entityWorker.MultipleShiftAssignments = editMemberDTO.MultipleShiftAssignments;

                // if there is a rotation change
                if (entityWorker.PartOfRotation != editMemberDTO.PartOfRotation && editMemberDTO.PartOfRotation)
                {
                    IEnumerable<EntityWorkerShiftAssigned> entityWorkerShiftAssigneds = await _unitOfWork.EntityWorkerShiftAssignedsRepository.GetAllByEntityIdAndUserId(editMemberDTO.EntityId, userId);
                    if (entityWorkerShiftAssigneds.Count() != 0)
                    {
                        await _unitOfWork.EntityWorkerShiftAssignedsRepository.DeleteAllByEntityIdAndUserId(editMemberDTO.EntityId, userId);
                    }
                }

                entityWorker.PartOfRotation = editMemberDTO.PartOfRotation;
                // Update the entity worker instance
                await _unitOfWork.EntityWorkerRepository.Update(entityWorker);

                if (!entityWorker.PartOfRotation)
                {
                    // Delete all pre-existing assignments
                    await _unitOfWork.EntityWorkerShiftAssignedsRepository.DeleteAllByEntityIdAndUserId(editMemberDTO.EntityId, userId);

                    List<EntityWorkerShiftAssigned> entityWorkerShifts = new List<EntityWorkerShiftAssigned>();

                    // For each assignment, add it to the Entity assigned shifts
                    foreach (ShiftDTO shift in editMemberDTO.AssignedShifts)
                    {
                        entityWorkerShifts.Add(new EntityWorkerShiftAssigned
                        {
                            EntityId = editMemberDTO.EntityId,
                            ApplicationUserId = userId.ToString(),
                            ShiftId = shift.ShiftId
                        });
                    }

                    await _unitOfWork.EntityWorkerShiftAssignedsRepository.AddRange(entityWorkerShifts);
                }

                await _unitOfWork.EntityWorkerSkillRepository.DeleteAllByEntityIdAndUserId(editMemberDTO.EntityId, _generalService.ParseStringToGuid(editMemberDTO.WorkerId));
                List<EntityWorkerSkill> entityUserBotSkills = new List<EntityWorkerSkill>();

                foreach (SkillLocalizedDTO skillLocalizedDTO in editMemberDTO.AssignedSkills)
                {
                    entityUserBotSkills.Add(new EntityWorkerSkill
                    {
                        EntityId = editMemberDTO.EntityId,
                        ApplicationUserId = userId.ToString(),
                        SkillId = skillLocalizedDTO.SkillId
                    });
                }

                await _unitOfWork.EntityWorkerSkillRepository.AddRange(entityUserBotSkills);

                response.Success = true;
                response.Result = true;
            }
            else
            {
                response.Message = EntityWorkerRelatedMessages.MemberNotFound;
                return response;
            }
            return response;
        }

        #endregion

        #region Delete Entity Member

        public async Task<BaseResponse<bool>> DeleteEntityMember(DeleteMemberDTO workerMemberDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = SharedMessages.UnexpectedError;

            if (string.IsNullOrEmpty(workerMemberDTO.WorkerId))
            {
                response.Message = WorkerRelatedMessages.WorkerIdentifierIsEmpty;
                return response;
            }

            else if (workerMemberDTO.EntityId == Guid.Empty)
            {
                response.Message = EntitiesRelatedMessages.EntityNoIdentifierError;
                return response;
            }

            Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(workerMemberDTO.EntityId);
            if (entity == null)
            {
                response.Message = EntitiesRelatedMessages.EntityNotFound;
                return response;
            }
            else
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync();

                    // If bot
                    if (workerMemberDTO.IsBot)
                    {
                        Guid userBotGuid = _generalService.ParseStringToGuid(workerMemberDTO.WorkerId);

                        UserBot userBot = await _unitOfWork.UserBotRepository.GetById(userBotGuid);
                        EntityUserBot entityUserBot = await _unitOfWork.EntityUserBotRepository.GetEntityUserBotByEntityAndId(entity.EntityId, userBotGuid);

                        // Get user bot and entity bot instances
                        if (userBot != null && entityUserBot != null)
                        {
                            // Check if the bot is not part of rotation and remove specifics
                            if (!entityUserBot.PartOfRotation)
                                await _unitOfWork.EntityUserBotShiftAssignedsRepository.DeleteAllByEntityIdAndUserBotId(workerMemberDTO.EntityId, userBotGuid);

                            // Remove user bot skills
                            await _unitOfWork.EntityUserBotSkillRepository.DeleteAllByEntityIdAndUserBotId(workerMemberDTO.EntityId, userBotGuid);

                            // Remove Entity User Bot Instance
                            await _unitOfWork.EntityUserBotRepository.DeleteEntityUserBot(workerMemberDTO.EntityId, userBotGuid);
                            // Remove User Bot instance
                            await _unitOfWork.UserBotRepository.Delete(userBotGuid);
                        }
                        else
                        {
                            response.Message = EntityWorkerRelatedMessages.MemberNotFound;
                            return response;
                        }
                    }
                    else
                    {
                        Guid userId = _generalService.ParseStringToGuid(workerMemberDTO.WorkerId);

                        EntityWorker entityWorker = await _unitOfWork.EntityWorkerRepository.GetByWorkerAndEntity(workerMemberDTO.WorkerId, workerMemberDTO.EntityId);
                        if (entityWorker != null)
                        {
                            // Check if the worker is not part of rotation and remove specifics
                            if (!entityWorker.PartOfRotation)
                                await _unitOfWork.EntityWorkerShiftAssignedsRepository.DeleteAllByEntityIdAndUserId(workerMemberDTO.EntityId, userId);

                            // Remove Entity Worker Skills
                            await _unitOfWork.EntityWorkerSkillRepository.DeleteAllByEntityIdAndUserId(workerMemberDTO.EntityId, userId);

                            // Remove Entity Worker Instance
                            await _unitOfWork.EntityWorkerRepository.Delete(userId);
                        }
                        else
                        {
                            response.Message = EntityWorkerRelatedMessages.MemberNotFound;
                            return response;
                        }
                    }

                    await _unitOfWork.CommitAsync();
                    response.Success = true;
                    response.Message = string.Empty;
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

        #region Convert Bot To User

        public async Task<BaseResponse<EntityWorkerMemberDTO>> ConvertBotToUser(ConvertBotToUserDTO convertBotToUserDTO)
        {
            BaseResponse<EntityWorkerMemberDTO> response = new BaseResponse<EntityWorkerMemberDTO>();
            response.Message = SharedMessages.UnexpectedError;

            EntityDTO entityDTO = await this.GetEntityById(convertBotToUserDTO.EntityId, _languageAccessor.GetLanguageCode());
            if (entityDTO == null)
            {
                response.Message = EntitiesRelatedMessages.EntityNotFound;
                return response;
            }

            EntityUserBot userBot = await _unitOfWork.EntityUserBotRepository.GetEntityUserBotByEntityAndId(convertBotToUserDTO.EntityId, convertBotToUserDTO.UserBotId);
            if (userBot == null)
            {
                response.Message = EntitiesRelatedMessages.EntityWorkerNotFound;
                return response;
            }

            EntityWorker entityWorker = await _unitOfWork.EntityWorkerRepository.GetByWorkerAndEntity(convertBotToUserDTO.ApplicationUserIdTarget, convertBotToUserDTO.EntityId);
            if (entityWorker == null)
            {
                response.Message = EntityWorkerRelatedMessages.MemberNotFound;
                return response;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Entity Worker Skills
                // Delete any current assigned worker skills
                await _unitOfWork.EntityWorkerSkillRepository.DeleteRange(entityWorker.ApplicationUser.EntityWorkerSkills);

                List<EntityWorkerSkill> entityWorkerSkills = new List<EntityWorkerSkill>();

                // set new entity worker skills
                foreach (EntityUserBotSkill entityUserBotSkill in userBot.UserBot.EntityUserBotSkills.Where(s => s.EntityId == convertBotToUserDTO.EntityId))
                {
                    entityWorkerSkills.Add(new EntityWorkerSkill
                    {
                        ApplicationUserId = entityWorker.ApplicationUserId,
                        SkillId = entityUserBotSkill.SkillId,
                        EntityId = entityUserBotSkill.EntityId
                    });
                }

                // Add new entity worker skill entries
                await _unitOfWork.EntityWorkerSkillRepository.AddRange(entityWorkerSkills);

                // Entity Worker Shift Assigned
                await _unitOfWork.EntityWorkerShiftAssignedsRepository.DeleteRange(entityWorker.ApplicationUser.EntityWorkerShiftAssigneds);

                List<EntityWorkerShiftAssigned> entityWorkerShiftAssigneds = new List<EntityWorkerShiftAssigned>();

                foreach (EntityUserBotShiftAssigned entityUserBotShiftAssigneds in userBot.UserBot.EntityUserBotShiftAssigneds.Where(a => a.EntityId == convertBotToUserDTO.EntityId))
                {
                    entityWorkerShiftAssigneds.Add(new EntityWorkerShiftAssigned
                    {
                        ApplicationUserId = entityWorker.ApplicationUserId,
                        EntityId = entityUserBotShiftAssigneds.EntityId,
                        ShiftId = entityUserBotShiftAssigneds.ShiftId
                    });

                }

                // Add new Entity Worker Shift Assignments
                await _unitOfWork.EntityWorkerShiftAssignedsRepository.AddRange(entityWorkerShiftAssigneds);


                // Schedule Entry Workers
                // Delete any current Schedule Entries
                await _unitOfWork.EntityScheduleWorkersRepository.DeleteRange(entityWorker.ApplicationUser.ScheduleEntryWorkers);

                List<ScheduleEntryWorkers> scheduleEntryWorkers = new List<ScheduleEntryWorkers>();

                foreach (ScheduleEntryBots scheduleEntryBots in userBot.UserBot.ScheduleEntryBots)
                {
                    scheduleEntryWorkers.Add(new ScheduleEntryWorkers
                    {
                        ApplicationUserId = entityWorker.ApplicationUserId,
                        ScheduleEntryId = scheduleEntryBots.ScheduleEntryId,
                        SpecificSkillAssignments = scheduleEntryBots.SpecificSkillAssignments
                    });
                }

                // Add new Schedule Entry Workers
                await _unitOfWork.EntityScheduleWorkersRepository.AddRange(scheduleEntryWorkers);

                // Schedule Entry Worker Ineligibilities
                // Delete Existing
                await _unitOfWork.ScheduleEntryWorkerIneligibilityRepository.DeleteRange(entityWorker.ApplicationUser.ScheduleEntryWorkerIneligibilities);

                List<ScheduleEntryWorkerIneligibility> scheduleEntryWorkerIneligibilities = new List<ScheduleEntryWorkerIneligibility>();

                foreach (ScheduleEntryBotIneligibility scheduleEntryBotIneligibility in userBot.UserBot.ScheduleEntryBotIneligibilities)
                {
                    scheduleEntryWorkerIneligibilities.Add(new ScheduleEntryWorkerIneligibility
                    {
                        ScheduleEntryId = scheduleEntryBotIneligibility.ScheduleEntryId,
                        IneligibilityObservations = scheduleEntryBotIneligibility.IneligibilityObservations,
                        DateOfAssessement = scheduleEntryBotIneligibility.DateOfAssessement,
                        ApplicationUserId = entityWorker.ApplicationUserId
                    });
                }

                await _unitOfWork.ScheduleEntryWorkerIneligibilityRepository.AddRange(scheduleEntryWorkerIneligibilities);

                // Remove User bot
                await _unitOfWork.EntityUserBotRepository.DeleteEntityUserBot(userBot.EntityId, userBot.UserBotId);
                await _unitOfWork.UserBotRepository.Delete(userBot.UserBotId);

                await _unitOfWork.CommitAsync();

                List<EntityWorkerMemberDTO> convertedMember = await GetEntityMembers(convertBotToUserDTO.EntityId, new List<string> { convertBotToUserDTO.ApplicationUserIdTarget });
                response.Result = convertedMember.FirstOrDefault();
                response.Message = EntityWorkerRelatedMessages.BotToUserConversionSuccessful;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                await _unitOfWork.RollbackAsync();
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return response;
        }

        #endregion

        #endregion
    }
}
