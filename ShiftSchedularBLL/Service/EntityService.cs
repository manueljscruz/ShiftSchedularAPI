using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.DbConstants;
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
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        #region Constructor

        public EntityService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICryptographyService cryptographyService,
            UserManager<ApplicationUser> userManager,
            ISkillService skillService,
            IShiftService shiftService,
            IEntityTypeService entityTypeService,
            IGeneralService generalService,
            ILanguageAccessor languageAccessor,
            IEmailService emailService,
            IConfiguration configuration)
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
            _emailService = emailService;
            _configuration = configuration;
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

                // Check if its a child entity being created
                if (newEntity.ParentEntityId != Guid.Empty)
                {
                    // Check if parent exists
                    Entity parentEntity = await _unitOfWork.EntityRepository.GetEntityById(newEntity.ParentEntityId.Value, _languageAccessor.GetLanguageCode());
                    if (parentEntity == null)
                    {
                        response.Message = EntitiesRelatedMessages.ParentEntityNotFound;
                        return response;
                    }
                    else
                    {
                        bool valid = await _unitOfWork.EntityPermissionRepository.CanUserCreateEntities(parentEntity.EntityId, newEntity.WorkerId);
                        if (!valid)
                        {
                            response.Message = EntitiesRelatedMessages.UserCannotCreateEntityPermission;
                            return response;
                        }
                    }
                }

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

                        // Add Entity Permission - Creator becomes General Manager
                        EntityPermission entityPermission = new EntityPermission
                        {
                            ApplicationUserId = newEntity.WorkerId,
                            EntityId = entity.EntityId,
                            EntityPermissionRoleId = EntityPermisisonRoleConstants.GENERAL_MANAGER_ID
                        };

                        await _unitOfWork.EntityPermissionRepository.Add(entityPermission);

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
        public async Task<BaseResponse<bool>> DeleteEntityById(Guid entityId, string workerId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            // if entity identifier is different than null
            if (entityId != Guid.Empty)
            {
                bool isGM = await _unitOfWork.EntityPermissionRepository.IsGeneralManager(entityId, workerId);
                if (!isGM)
                {
                    response.Message = "You do not have permission to delete this entity.";
                    return response;
                }

                Entity entityInstance = await _unitOfWork.EntityRepository.GetEntityById(entityId, _languageAccessor.GetLanguageCode());

                if (entityInstance == null)
                {
                    response.Message = EntitiesRelatedMessages.EntityNotFound;
                    return response;
                }

                if (entityInstance.ChildrenEntities.Count != 0)
                {
                    response.Message = EntitiesRelatedMessages.DeleteEntityHasChildrenError;
                    return response;
                }

                IEnumerable<EntityWorker> entityWorkers = await _unitOfWork.EntityWorkerRepository.GetByEntityId(entityId);
                IEnumerable<EntityWorkerSkill> entityWorkerSkills = await _unitOfWork.EntityWorkerSkillRepository.GetByEntityId(entityId);
                IEnumerable<EntityUserBot> entityUserBots = await _unitOfWork.EntityUserBotRepository.GetUserBotsByEntityId(entityId);
                IEnumerable<EntityUserBotSkill> entityUserBotSkills = await _unitOfWork.EntityUserBotSkillRepository.GetByEntityId(entityId);
                IEnumerable<EntityPermission> entityPermissions = await _unitOfWork.EntityPermissionRepository.GetByEntityId(entityId);
                IEnumerable<EntityWorkerShiftAssigned> entityWorkerShiftAssigneds = await _unitOfWork.EntityWorkerShiftAssignedsRepository.GetAllByEntityId(entityId);
                IEnumerable<EntityUserBotShiftAssigned> entityUserBotShiftAssigneds = await _unitOfWork.EntityUserBotShiftAssignedsRepository.GetAllByEntityId(entityId);
                IEnumerable<EntityWorkerAbsence> entityWorkerAbsences = await _unitOfWork.EntityWorkerAbsenceRepository.GetEntityWorkerAbsences(entityId, "", true);
                IEnumerable<EntityRule> entityRules = await _unitOfWork.EntityRuleRepository.GetEntityRules(entityId);
                IEnumerable<EntityHoliday> entityHolidays = await _unitOfWork.EntityHolidayRepository.GetByEntityId(entityId);
                List<EntityShiftRotation> entityShiftRotations = await _unitOfWork.EntityShiftRotationRepository.GetEntityShiftsRotation(entityId);
                IEnumerable<Shift> entityShifts = await _unitOfWork.ShiftRepository.GetEntityShifts(entityId);
                IEnumerable<ShiftBreak> shiftBreaks = await _unitOfWork.ShiftBreakRepository.GetByEntityId(entityId);
                List<ScheduleEntry> scheduleEntries = await _unitOfWork.EntityScheduleRepository.GetByEntityId(entityId);


                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    await _unitOfWork.EntityWorkerRepository.DeleteRange(entityWorkers);
                    await _unitOfWork.EntityWorkerSkillRepository.DeleteRange(entityWorkerSkills);
                    await _unitOfWork.EntityUserBotRepository.DeleteRange(entityUserBots);
                    await _unitOfWork.EntityUserBotSkillRepository.DeleteRange(entityUserBotSkills);
                    await _unitOfWork.EntityWorkerInvitationRepository.DeleteAllByEntity(entityId);
                    await _unitOfWork.EntityPermissionRepository.DeleteRange(entityPermissions);
                    await _unitOfWork.EntityHolidayRepository.DeleteRange(entityHolidays);
                    await _unitOfWork.EntityRuleSpecificationRepository.DeleteRange(entityRules.SelectMany(i => i.EntityRuleSpecifications));
                    await _unitOfWork.EntityWorkerShiftAssignedsRepository.DeleteRange(entityWorkerShiftAssigneds);
                    await _unitOfWork.EntityUserBotShiftAssignedsRepository.DeleteRange(entityUserBotShiftAssigneds);
                    await _unitOfWork.EntityWorkerAbsenceRepository.DeleteRange(entityWorkerAbsences);
                    await _unitOfWork.EntityRuleRepository.DeleteRange(entityRules);
                    await _unitOfWork.EntityShiftRotationRepository.DeleteRange(entityShiftRotations);
                    await _unitOfWork.ScheduleEntryBotsRepository.DeleteRange(scheduleEntries.SelectMany(i => i.ScheduleEntryBots));
                    await _unitOfWork.ScheduleEntryBotIneligibilityRepository.DeleteRange(scheduleEntries.SelectMany(i => i.ScheduleEntryBotIneligibilities));
                    await _unitOfWork.EntityScheduleWorkersRepository.DeleteRange(scheduleEntries.SelectMany(i => i.ScheduleEntryWorkers));
                    await _unitOfWork.ScheduleEntryWorkerIneligibilityRepository.DeleteRange(scheduleEntries.SelectMany(i => i.ScheduleEntryWorkerIneligibilities));
                    await _unitOfWork.EntityScheduleRepository.DeleteRange(scheduleEntries);
                    await _unitOfWork.ShiftBreakRepository.DeleteRange(shiftBreaks);
                    await _unitOfWork.ShiftRepository.DeleteRange(entityShifts);
                    await _unitOfWork.EntityRepository.Delete(entityInstance.EntityId);
                    await _unitOfWork.CommitAsync();

                    response.Success = true;
                    response.Message = EntitiesRelatedMessages.EntityDeletedSuccessfuly;
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
                                                                    totalCount: botsCount + workersCount,
                                                                    parentEntityId: entity.ParentEntityId);

                return entityDTO;
            }
            else
                return null;
        }

        #endregion

        #region Get Child Entities

        /// <summary>
        /// Gets all direct child entities of a given parent entity
        /// </summary>
        /// <param name="parentEntityId">Identifier of the parent entity</param>
        /// <returns>List of child entities as EntityDTOs</returns>
        public async Task<List<EntityDTO>> GetChildEntities(Guid parentEntityId)
        {
            List<EntityDTO> childEntities = new List<EntityDTO>();

            if (parentEntityId == Guid.Empty)
                return childEntities;

            List<Entity> children = await _unitOfWork.EntityRepository.GetChildEntities(parentEntityId, _languageAccessor.GetLanguageCode());

            foreach (Entity child in children)
            {
                int botsCount = await _unitOfWork.EntityUserBotRepository.GetUserBotsByEntityCount(child.EntityId);
                int workersCount = await _unitOfWork.EntityWorkerRepository.GetTotalCountByEntity(child.EntityId);

                string entityTypeLocalized = child.EntityType?.EntityTypeLocalizations?.FirstOrDefault()?.EntityTypeDisplayValue ?? string.Empty;

                childEntities.Add(new EntityDTO(
                    entityId: child.EntityId,
                    entityName: child.EntityName,
                    entityDescription: child.EntityDescription,
                    entityTypeLocalized: entityTypeLocalized,
                    totalCount: botsCount + workersCount,
                    parentEntityId: child.ParentEntityId
                ));
            }

            return childEntities;
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
        /// Returns a flat list of entities for breadcrumb tree rendering.
        /// Entities where the worker has an explicit EntityPermission are included with their role.
        /// All ancestors of those entities are also included with EntityPermissionRoleId = null
        /// so Angular can reconstruct the full path from root to the worker's entity.
        /// </summary>
        public async Task<List<EntityWorkerDTO>> GetEntitiesByWorkerId(string workerId)
        {
            if (string.IsNullOrEmpty(workerId))
                return new List<EntityWorkerDTO>();

            try
            {
                // Step 1: Load all explicit permissions for this worker
                IEnumerable<EntityPermission> permissions = await _unitOfWork.EntityPermissionRepository.GetByWorkerId(workerId);

                if (permissions == null || !permissions.Any())
                    return new List<EntityWorkerDTO>();

                // Step 2: Build a map of EntityId -> role (null = ancestor only)
                // Role wins over null if the same entity appears in multiple paths
                Dictionary<Guid, int?> entityRoleMap = new Dictionary<Guid, int?>();

                foreach (EntityPermission permission in permissions)
                {
                    // Register the directly-permitted entity with its role
                    if (!entityRoleMap.ContainsKey(permission.EntityId) || entityRoleMap[permission.EntityId] == null)
                        entityRoleMap[permission.EntityId] = permission.EntityPermissionRoleId;

                    // Traverse ancestors and register them with null role if not already present
                    List<Entity> ancestors = await _unitOfWork.EntityRepository.GetAncestorChain(permission.EntityId);
                    foreach (Entity ancestor in ancestors)
                    {
                        if (!entityRoleMap.ContainsKey(ancestor.EntityId))
                            entityRoleMap[ancestor.EntityId] = null;
                    }
                }

                // Step 3: Batch fetch all entity names and ParentEntityIds in one query
                List<Entity> allEntities = await _unitOfWork.EntityRepository.GetEntitiesByIds(entityRoleMap.Keys.ToList());
                Dictionary<Guid, Entity> entityLookup = allEntities.ToDictionary(e => e.EntityId);

                // Step 4: Project to DTOs
                List<EntityWorkerDTO> result = new List<EntityWorkerDTO>();
                foreach (KeyValuePair<Guid, int?> entry in entityRoleMap)
                {
                    if (entityLookup.TryGetValue(entry.Key, out Entity entity))
                    {
                        result.Add(new EntityWorkerDTO
                        {
                            EntityId = entity.EntityId,
                            EntityName = entity.EntityName,
                            ParentEntityId = entity.ParentEntityId,
                            EntityPermissionRoleId = entry.Value
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return new List<EntityWorkerDTO>();
            }
        }

        #endregion

        #region Get Entities Members View Model

        /// <summary>
        /// Gets the entity members view model
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="lcode">Language code</param>
        /// <returns></returns>
        public async Task<EntityMembersViewModel> GetEntitiesMembersViewModel(MemberPagedModelRequestDTO memberListModelRequest)
        {
            EntityMembersViewModel viewModel = new EntityMembersViewModel();

            string lcode = _languageAccessor.GetLanguageCode();
            if (lcode.Contains("-")) lcode = lcode.Split('-')[0];

            viewModel.Skills = await _skillService.GetAllSkillsByLocalization(lcode);

            viewModel.EntityUsedSkills = await GetEntitySkills(memberListModelRequest);

            viewModel.Shifts = await _shiftService.GetEntityShifts(memberListModelRequest.EntityId);

            viewModel.EntityMembers = await GetEntityMembers(memberListModelRequest.EntityId, new List<string>(), memberListModelRequest.MemberFilters ?? new MemberListFilterDTO(), memberListModelRequest.NextPage, memberListModelRequest.ItemsPerPage);

            viewModel.EntityOwnerId = await _unitOfWork.EntityWorkerRepository.GetEntityOwnerId(memberListModelRequest.EntityId);

            var roleLocalizations = await _unitOfWork.EntityPermissionRoleLocalizationRepository.GetAllByLanguageCode(lcode);
            viewModel.EntityPermissionRoles = roleLocalizations.Select(r => new EntityPermissionRoleDTO
            {
                EntityPermissionRoleId = r.EntityPermissionRoleId,
                EntityPermissionRoleDisplayValue = r.EntityPermissionRoleDisplayValue
            }).ToList();

            var currentUserPermission = await _unitOfWork.EntityPermissionRepository
                .GetByEntityAndWorker(memberListModelRequest.EntityId, memberListModelRequest.WorkerId);

            viewModel.CurrentUserPermissionRoleId = currentUserPermission?.EntityPermissionRoleId ?? 0;
            viewModel.CurrentUserCanManageChildren = currentUserPermission?.CanManageChildren ?? false;

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

        public async Task<PagedList<EntityWorkerMemberDTO>> GetEntityMembers(Guid entityId, List<string> workers, MemberListFilterDTO memberListFilterDTO, int nextPage = 0, int itemsPerPage = 0)
        {
            List<EntityWorkerMemberModel> entityWorkerMembers = await GetAllMembers(entityId, workers, memberListFilterDTO);

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
        /// Gets all members (workers and bots) for an entity, with optional filtering.
        /// </summary>
        /// <param name="entityId">The entity identifier</param>
        /// <param name="workers">Optional list of worker IDs to filter by. If empty, returns all members.</param>
        /// <param name="filters">Optional member list filters to apply.</param>
        /// <returns>Filtered and ordered list of entity members (workers and/or bots)</returns>
        private async Task<List<EntityWorkerMemberModel>> GetAllMembers(Guid entityId, List<string> workers, MemberListFilterDTO filters = null)
        {
            // Determine which member types to fetch — skip the unused repo call entirely
            bool fetchWorkers = filters == null || !filters.ApplyMemberTypeFilter || !filters.IsBot;
            bool fetchBots = filters == null || !filters.ApplyMemberTypeFilter || filters.IsBot;

            IEnumerable<EntityWorkerMemberModel> entityWorkerMembers = fetchWorkers
                ? await _unitOfWork.EntityWorkerRepository.GetDistinctMembersByEntityId(entityId)
                : Enumerable.Empty<EntityWorkerMemberModel>();

            IEnumerable<EntityWorkerMemberModel> userBots = fetchBots
                ? await _unitOfWork.EntityUserBotRepository.GetDistinctUserBotsByEntityId(entityId)
                : Enumerable.Empty<EntityWorkerMemberModel>();

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

            // Apply in-memory filters after combining both sets
            if (filters != null)
            {
                if (!string.IsNullOrWhiteSpace(filters.NameFilter))
                    members = members
                        .Where(m => m.WorkerName.Contains(filters.NameFilter, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                if (filters.PartOfRotation)
                    members = members.Where(m => m.PartOfRotation).ToList();

                if (filters.WorkWeekDays)
                    members = members.Where(m => m.WorksWeekDays).ToList();

                if (filters.WorkWeekEnds)
                    members = members.Where(m => m.WorksWeekends).ToList();

                if (filters.SelectedSkills is { Count: > 0 })
                {
                    var selectedSkillIds = filters.SelectedSkills
                        .Select(s => s.SkillId.ToString())
                        .ToHashSet();

                    members = members
                        .Where(m =>
                        {
                            if (string.IsNullOrEmpty(m.SkillIds)) return false;
                            var memberSkillIds = m.SkillIds
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .ToHashSet();
                            return selectedSkillIds.All(id => memberSkillIds.Contains(id));
                        })
                        .ToList();
                }

                // Note: SelectedShits (shift assignment filter) is not applied here because shift
                // assignments are not part of EntityWorkerMemberModel. Apply after ProcessMemberData if needed.
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
                                                                    totalCount: botsCount + workersCount,
                                                                    parentEntityId: entity.ParentEntityId);
                    entityProfileViewModel.AllowEdit = await _unitOfWork.EntityPermissionRepository.CanUserEditEntity(entityProfileViewModelRequest.EntityId, entityProfileViewModelRequest.WorkerId);
                    entityProfileViewModel.AllowDelete = await _unitOfWork.EntityPermissionRepository.IsGeneralManager(entityProfileViewModelRequest.EntityId, entityProfileViewModelRequest.WorkerId);

                    if (entityProfileViewModel.AllowEdit)
                    {
                        entityProfileViewModel.EntityTypeLocalizeds = await _entityTypeService.GetAllEntityTypesByLocalization(_languageAccessor.GetLanguageCode());
                    }

                    // Load parent entity if applicable
                    if (entity.ParentEntityId.HasValue)
                    {
                        Entity parentEntity = await _unitOfWork.EntityRepository.GetEntityById(entity.ParentEntityId.Value, _languageAccessor.GetLanguageCode());
                        if (parentEntity != null)
                        {
                            EntityTypeLocalization parentTypeLocalization = parentEntity.EntityType?.EntityTypeLocalizations?.FirstOrDefault();
                            entityProfileViewModel.ParentEntity = new EntityDTO(entityId: parentEntity.EntityId,
                                                                                entityName: parentEntity.EntityName,
                                                                                entityDescription: parentEntity.EntityDescription,
                                                                                entityTypeLocalized: parentTypeLocalization?.EntityTypeDisplayValue ?? string.Empty,
                                                                                totalCount: 0);
                        }
                    }

                    // Load direct children
                    List<Entity> children = await _unitOfWork.EntityRepository.GetChildEntities(entity.EntityId, _languageAccessor.GetLanguageCode());
                    entityProfileViewModel.ChildrenEntities = children.Select(c =>
                    {
                        EntityTypeLocalization childTypeLocalization = c.EntityType?.EntityTypeLocalizations?.FirstOrDefault();
                        return new EntityDTO(entityId: c.EntityId,
                                            entityName: c.EntityName,
                                            entityDescription: c.EntityDescription,
                                            entityTypeLocalized: childTypeLocalization?.EntityTypeDisplayValue ?? string.Empty,
                                            totalCount: 0);
                    }).ToList();
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
                    IsBot = true,
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

            // Check for any existing invitation (including soft-deleted) to avoid PK violation
            EntityWorkerInvitation existingInvitation = await _unitOfWork.EntityWorkerInvitationRepository
                .FindByEntityAndEmail(newMemberDTO.DestinationEntityId, newMemberDTO.MemberEmail);

            if (existingInvitation != null)
            {
                if (!existingInvitation.IsDeleted)
                {
                    response.Message = "An invitation has already been sent to this email for this entity.";
                    return response;
                }

                // Reactivate the soft-deleted invitation with updated fields
                existingInvitation.IsDeleted = false;
                existingInvitation.DeletedAt = null;
                existingInvitation.DeletedById = null;
                existingInvitation.ApplicationUserId = possibleWorker != null ? possibleWorker.Id : null;
                existingInvitation.InviteDate = nowUTCTime;
                existingInvitation.SkillsetIds = skillsAggregated;
                existingInvitation.PartOfRotation = newMemberDTO.PartOfRotation;
                existingInvitation.WorksWeekDays = newMemberDTO.WorksWeekDays;
                existingInvitation.WorksWeekends = newMemberDTO.WorksWeekends;
                existingInvitation.MultipleShiftAssignments = newMemberDTO.MultipleShiftAssignments;
                existingInvitation.EntityPermissionRoleId = newMemberDTO.EntityPermissionRoleId;
                existingInvitation.PartOfRoster = newMemberDTO.PartOfRoster;
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
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
                    MultipleShiftAssignments = newMemberDTO.MultipleShiftAssignments,
                    EntityPermissionRoleId = newMemberDTO.EntityPermissionRoleId,
                    PartOfRoster = newMemberDTO.PartOfRoster
                };
                await _unitOfWork.EntityWorkerInvitationRepository.Add(entityWorkerInvitation);
            }

            // Send notification email — fire and forget (errors are swallowed in EmailService)
            string frontendUrl = _configuration.GetValue<string>("FrontendUrl") ?? string.Empty;
            if (!string.IsNullOrEmpty(frontendUrl))
            {
                string invitationsLink = $"{frontendUrl}/dashboard/my-invitations";
                string displayName = possibleWorker?.DisplayName ?? newMemberDTO.MemberEmail;
                _ = _emailService.SendInvitationEmail(newMemberDTO.MemberEmail, displayName, invitationsLink);
            }

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

                        if (response.Success && updateEntityMemberDTO.DateToExit.HasValue)
                        {
                            await ApplyExitLogicAsync(new MemberExitDTO
                            {
                                WorkerId = updateEntityMemberDTO.WorkerId,
                                EntityId = updateEntityMemberDTO.EntityId,
                                IsBot = updateEntityMemberDTO.IsBot,
                                DateToExit = updateEntityMemberDTO.DateToExit.Value,
                                ActingUserId = updateEntityMemberDTO.ActingUserId ?? string.Empty
                            });
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
                    foreach (SkillLocalizedDTO skillLocalizedDTO in userBotData.AssignedSkills.DistinctBy(s => s.SkillId))
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
            EntityWorker entityWorker = await _unitOfWork.EntityWorkerRepository.GetSimpleByWorkerAndEntity(editMemberDTO.WorkerId, editMemberDTO.EntityId);
            if (entityWorker != null)
            {
                try
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

                foreach (SkillLocalizedDTO skillLocalizedDTO in editMemberDTO.AssignedSkills.DistinctBy(s => s.SkillId))
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
                catch (Exception ex)
                {
                    string strErr = ex.Message;
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

                        // Use the simple lookup (no navigation-property includes) to avoid
                        // silent null returns caused by unmapped/failing ThenInclude chains
                        EntityWorker entityWorker = await _unitOfWork.EntityWorkerRepository
                            .GetSimpleByWorkerAndEntity(workerMemberDTO.WorkerId, workerMemberDTO.EntityId);

                        if (entityWorker != null)
                        {
                            // Check if the worker is not part of rotation and remove specifics
                            if (!entityWorker.PartOfRotation)
                                await _unitOfWork.EntityWorkerShiftAssignedsRepository.DeleteAllByEntityIdAndUserId(workerMemberDTO.EntityId, userId);

                            // Remove Entity Worker Skills
                            await _unitOfWork.EntityWorkerSkillRepository.DeleteAllByEntityIdAndUserId(workerMemberDTO.EntityId, userId);

                            // Remove Entity Worker Instance (composite key — must use both EntityId and WorkerId)
                            await _unitOfWork.EntityWorkerRepository.DeleteByEntityAndWorker(workerMemberDTO.EntityId, workerMemberDTO.WorkerId);

                            // Remove Entity Permission record
                            await _unitOfWork.EntityPermissionRepository.DeleteByEntityAndWorker(workerMemberDTO.EntityId, workerMemberDTO.WorkerId);
                        }
                        else
                        {
                            response.NotFound = true;
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

        #region Apply Exit Logic (no own transaction)

        private async Task ApplyExitLogicAsync(MemberExitDTO dto)
        {
            DateTime exitDate = dto.DateToExit!.Value;
            bool isImmediate = exitDate.Date <= DateTime.UtcNow.Date;

            if (dto.IsBot)
            {
                Guid botId = _generalService.ParseStringToGuid(dto.WorkerId);
                EntityUserBot entityUserBot = await _unitOfWork.EntityUserBotRepository.GetEntityUserBotByEntityAndId(dto.EntityId, botId);
                if (entityUserBot != null)
                {
                    entityUserBot.DateOfExit = exitDate;
                    await _unitOfWork.EntityUserBotRepository.Update(entityUserBot);

                    await _unitOfWork.ScheduleEntryBotsRepository.DeleteFutureBotParticipations(dto.EntityId, botId, exitDate);

                    if (isImmediate)
                    {
                        entityUserBot.IsDeleted = true;
                        entityUserBot.DeletedAt = DateTime.UtcNow;
                        entityUserBot.DeletedById = dto.ActingUserId;
                        await _unitOfWork.EntityUserBotRepository.Update(entityUserBot);
                    }
                }
            }
            else
            {
                EntityWorker entityWorker = await _unitOfWork.EntityWorkerRepository.GetSimpleByWorkerAndEntity(dto.WorkerId, dto.EntityId);
                if (entityWorker != null)
                {
                    entityWorker.DateToExit = exitDate;
                    await _unitOfWork.EntityWorkerRepository.Update(entityWorker);

                    await _unitOfWork.EntityScheduleWorkersRepository.DeleteFutureWorkerParticipations(dto.EntityId, dto.WorkerId, exitDate);

                    if (isImmediate)
                    {
                        entityWorker.IsDeleted = true;
                        entityWorker.DeletedAt = DateTime.UtcNow;
                        entityWorker.DeletedById = dto.ActingUserId;
                        await _unitOfWork.EntityWorkerRepository.Update(entityWorker);

                        EntityPermission entityPermission = await _unitOfWork.EntityPermissionRepository.GetByEntityAndWorker(dto.EntityId, dto.WorkerId);
                        if (entityPermission != null)
                        {
                            entityPermission.IsDeleted = true;
                            entityPermission.DeletedAt = DateTime.UtcNow;
                            entityPermission.DeletedById = dto.ActingUserId;
                            await _unitOfWork.EntityPermissionRepository.Update(entityPermission);
                        }
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        #endregion

        #region Set Member Date To Exit

        public async Task<BaseResponse<bool>> SetMemberDateToExit(MemberExitDTO dto)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = SharedMessages.UnexpectedError;

            if (string.IsNullOrEmpty(dto.WorkerId) || dto.EntityId == Guid.Empty)
            {
                response.Message = EntityWorkerRelatedMessages.MemberIdentifierEmpty;
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await ApplyExitLogicAsync(dto);
                await _unitOfWork.CommitAsync();
                response.Success = true;
                response.Result = true;
                response.Message = string.Empty;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = ex.Message;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return response;
        }

        #endregion

        #region Cancel Member Exit

        public async Task<BaseResponse<bool>> CancelMemberExit(MemberExitDTO dto)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = SharedMessages.UnexpectedError;

            if (string.IsNullOrEmpty(dto.WorkerId) || dto.EntityId == Guid.Empty)
            {
                response.Message = EntityWorkerRelatedMessages.MemberIdentifierEmpty;
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                if (dto.IsBot)
                {
                    Guid botId = _generalService.ParseStringToGuid(dto.WorkerId);
                    EntityUserBot entityUserBot = await _unitOfWork.EntityUserBotRepository.GetEntityUserBotByEntityAndId(dto.EntityId, botId);
                    if (entityUserBot != null)
                    {
                        entityUserBot.DateOfExit = DateTime.MinValue;
                        await _unitOfWork.EntityUserBotRepository.Update(entityUserBot);
                    }
                }
                else
                {
                    EntityWorker entityWorker = await _unitOfWork.EntityWorkerRepository.GetSimpleByWorkerAndEntity(dto.WorkerId, dto.EntityId);
                    if (entityWorker != null)
                    {
                        entityWorker.DateToExit = DateTime.MinValue;
                        await _unitOfWork.EntityWorkerRepository.Update(entityWorker);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                response.Success = true;
                response.Result = true;
                response.Message = string.Empty;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = ex.Message;
            }
            finally
            {
                _unitOfWork.Dispose();
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

        #region Update Member Permission

        public async Task<BaseResponse<bool>> UpdateMemberPermission(UpdateMemberPermissionDTO dto)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            try
            {
                EntityPermission existing = await _unitOfWork.EntityPermissionRepository
                    .GetByEntityAndWorker(dto.EntityId, dto.WorkerId);

                if (existing == null)
                {
                    // No active record — check for a soft-deleted record with the target role
                    EntityPermission softDeleted = await _unitOfWork.EntityPermissionRepository
                        .FindByEntityWorkerAndRole(dto.EntityId, dto.WorkerId, dto.EntityPermissionRoleId);

                    if (softDeleted != null)
                    {
                        softDeleted.IsDeleted = false;
                        softDeleted.DeletedAt = null;
                        softDeleted.DeletedById = null;
                        softDeleted.CanManageChildren = dto.CanManageChildren;
                        softDeleted.PartOfRoster = dto.PartOfRoster;
                    }
                    else
                    {
                        await _unitOfWork.EntityPermissionRepository.Add(new EntityPermission
                        {
                            EntityId = dto.EntityId,
                            ApplicationUserId = dto.WorkerId,
                            EntityPermissionRoleId = dto.EntityPermissionRoleId,
                            CanManageChildren = dto.CanManageChildren,
                            PartOfRoster = dto.PartOfRoster,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
                else if (existing.EntityPermissionRoleId == dto.EntityPermissionRoleId)
                {
                    // Same role — update non-PK fields in place
                    existing.CanManageChildren = dto.CanManageChildren;
                    existing.PartOfRoster = dto.PartOfRoster;
                }
                else
                {
                    // Role changed — EntityPermissionRoleId is part of the PK so it cannot be updated in-place.
                    // Soft-delete the old record, then reactivate or insert the new role record.
                    existing.IsDeleted = true;
                    existing.DeletedAt = DateTime.UtcNow;

                    EntityPermission targetRole = await _unitOfWork.EntityPermissionRepository
                        .FindByEntityWorkerAndRole(dto.EntityId, dto.WorkerId, dto.EntityPermissionRoleId);

                    if (targetRole != null)
                    {
                        targetRole.IsDeleted = false;
                        targetRole.DeletedAt = null;
                        targetRole.DeletedById = null;
                        targetRole.CanManageChildren = dto.CanManageChildren;
                        targetRole.PartOfRoster = dto.PartOfRoster;
                    }
                    else
                    {
                        await _unitOfWork.EntityPermissionRepository.Add(new EntityPermission
                        {
                            EntityId = dto.EntityId,
                            ApplicationUserId = dto.WorkerId,
                            EntityPermissionRoleId = dto.EntityPermissionRoleId,
                            CanManageChildren = dto.CanManageChildren,
                            PartOfRoster = dto.PartOfRoster,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                response.Success = true;
                response.Result = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return response;
            }
        }

        #endregion

        #region Get Pending Invitations

        public async Task<List<PendingInvitationDTO>> GetPendingInvitations(string workerId)
        {
            List<PendingInvitationDTO> result = new List<PendingInvitationDTO>();

            if (string.IsNullOrEmpty(workerId))
                return result;

            IEnumerable<EntityWorkerInvitation> invitations = await _unitOfWork.EntityWorkerInvitationRepository.GetAllByWorker(workerId);

            foreach (EntityWorkerInvitation inv in invitations)
            {
                Entity entity = await _unitOfWork.EntityRepository.GetById(inv.EntityId);
                if (entity != null)
                {
                    result.Add(new PendingInvitationDTO
                    {
                        EntityId = inv.EntityId,
                        EntityName = entity.EntityName,
                        InviteDate = inv.InviteDate,
                        EntityPermissionRoleId = inv.EntityPermissionRoleId
                    });
                }
            }

            return result;
        }

        #endregion

        #region Accept Invitation

        public async Task<BaseResponse<EntityWorkerDTO>> AcceptInvitation(AcceptDeclineInvitationDTO dto)
        {
            BaseResponse<EntityWorkerDTO> response = new BaseResponse<EntityWorkerDTO>();

            EntityWorkerInvitation invitation = await _unitOfWork.EntityWorkerInvitationRepository
                .GetByEntityAndWorker(dto.EntityId, dto.WorkerId);

            if (invitation == null)
            {
                response.Message = "Invitation not found.";
                return response;
            }

            bool alreadyMember = await _unitOfWork.EntityWorkerRepository.IsWorkerInEntity(dto.EntityId, dto.WorkerId);
            if (alreadyMember)
            {
                response.Message = EntityWorkerRelatedMessages.AddNewMemberAlreadyInEntity;
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // 1. Create or reactivate EntityWorker
                EntityWorker entityWorker = await _unitOfWork.EntityWorkerRepository
                    .FindByWorkerAndEntity(dto.WorkerId, dto.EntityId);

                if (entityWorker != null)
                {
                    entityWorker.IsDeleted = false;
                    entityWorker.DeletedAt = null;
                    entityWorker.DeletedById = null;
                    entityWorker.DateOfJoin = DateTime.UtcNow;
                    entityWorker.PartOfRotation = invitation.PartOfRotation;
                    entityWorker.WorksWeekDays = invitation.WorksWeekDays;
                    entityWorker.WorksWeekends = invitation.WorksWeekends;
                    entityWorker.MultipleShiftAssignments = invitation.MultipleShiftAssignments;
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    entityWorker = new EntityWorker
                    {
                        EntityId = dto.EntityId,
                        ApplicationUserId = dto.WorkerId,
                        DateOfJoin = DateTime.UtcNow,
                        PartOfRotation = invitation.PartOfRotation,
                        WorksWeekDays = invitation.WorksWeekDays,
                        WorksWeekends = invitation.WorksWeekends,
                        MultipleShiftAssignments = invitation.MultipleShiftAssignments
                    };
                    await _unitOfWork.GetGenericRepository<EntityWorker>().Add(entityWorker);
                }

                // 2. Create or reactivate EntityWorkerSkill records
                if (!string.IsNullOrEmpty(invitation.SkillsetIds))
                {
                    foreach (string skillIdStr in invitation.SkillsetIds.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (int.TryParse(skillIdStr.Trim(), out int skillId))
                        {
                            EntityWorkerSkill existingSkill = await _unitOfWork.EntityWorkerSkillRepository
                                .FindByWorkerEntityAndSkill(dto.WorkerId, dto.EntityId, skillId);

                            if (existingSkill != null)
                            {
                                existingSkill.IsDeleted = false;
                                existingSkill.DeletedAt = null;
                                existingSkill.DeletedById = null;
                                await _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                await _unitOfWork.EntityWorkerSkillRepository.Add(new EntityWorkerSkill
                                {
                                    ApplicationUserId = dto.WorkerId,
                                    EntityId = dto.EntityId,
                                    SkillId = skillId
                                });
                            }
                        }
                    }
                }

                // 3. Create or reactivate EntityPermission
                EntityPermission permission = await _unitOfWork.EntityPermissionRepository
                    .FindByEntityAndWorker(dto.EntityId, dto.WorkerId);

                if (permission != null)
                {
                    permission.IsDeleted = false;
                    permission.DeletedAt = null;
                    permission.DeletedById = null;
                    permission.EntityPermissionRoleId = invitation.EntityPermissionRoleId;
                    permission.CanManageChildren = false;
                    permission.PartOfRoster = invitation.PartOfRoster;
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    permission = new EntityPermission
                    {
                        EntityId = dto.EntityId,
                        ApplicationUserId = dto.WorkerId,
                        EntityPermissionRoleId = invitation.EntityPermissionRoleId,
                        CanManageChildren = false,
                        PartOfRoster = invitation.PartOfRoster
                    };
                    await _unitOfWork.EntityPermissionRepository.Add(permission);
                }

                // 4. Delete invitation
                await _unitOfWork.EntityWorkerInvitationRepository.DeleteByCompositeKey(invitation.EntityId, invitation.Email);

                await _unitOfWork.CommitAsync();

                // 5. Fetch entity to build the return DTO for immediate sidebar access
                Entity entity = await _unitOfWork.EntityRepository.GetById(dto.EntityId);
                response.Success = true;
                response.Result = new EntityWorkerDTO
                {
                    EntityId = dto.EntityId,
                    EntityName = entity?.EntityName ?? string.Empty,
                    ParentEntityId = entity?.ParentEntityId,
                    EntityPermissionRoleId = invitation.EntityPermissionRoleId
                };
                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = ex.Message;
                return response;
            }
        }

        #endregion

        #region Decline Invitation

        public async Task<BaseResponse<bool>> DeclineInvitation(AcceptDeclineInvitationDTO dto)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            EntityWorkerInvitation invitation = await _unitOfWork.EntityWorkerInvitationRepository
                .GetByEntityAndWorker(dto.EntityId, dto.WorkerId);

            if (invitation == null)
            {
                response.Message = "Invitation not found.";
                return response;
            }

            try
            {
                await _unitOfWork.EntityWorkerInvitationRepository.DeleteByCompositeKey(invitation.EntityId, invitation.Email);
                response.Success = true;
                response.Result = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                return response;
            }
        }

        #endregion

        #region Get Umbrella Entities

        public async Task<List<EntityDTO>> GetUmbrellaEntities(Guid entityId)
        {
            if (entityId == Guid.Empty)
                return new List<EntityDTO>();

            Guid rootId = await _unitOfWork.EntityRepository.GetRootEntityId(entityId);

            // BFS from root down the full subtree
            List<Entity> result = new List<Entity>();
            Queue<Guid> queue = new Queue<Guid>();
            queue.Enqueue(rootId);

            while (queue.Count > 0)
            {
                Guid current = queue.Dequeue();
                Entity node = await _unitOfWork.EntityRepository.GetById(current);
                if (node != null)
                    result.Add(node);

                List<Entity> children = await _unitOfWork.EntityRepository.GetChildEntities(current, "en");
                foreach (Entity child in children)
                    queue.Enqueue(child.EntityId);
            }

            // Exclude the requesting entity itself (you can't transfer to your own entity)
            return result
                .Where(e => e.EntityId != entityId)
                .Select(e => new EntityDTO(e.EntityId, e.EntityName, e.EntityDescription, string.Empty, 0, e.ParentEntityId))
                .ToList();
        }

        #endregion

        #region Transfer / Copy Members

        public async Task<BaseResponse<bool>> TransferCopyMembers(TransferMembersDTO dto, string requesterId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (dto == null || dto.Members == null || dto.Members.Count == 0)
            {
                response.Message = "No members provided.";
                return response;
            }

            if (dto.SourceEntityId == Guid.Empty || dto.DestinationEntityId == Guid.Empty)
            {
                response.Message = EntitiesRelatedMessages.EntityNoIdentifierError;
                return response;
            }

            // Umbrella check: both entities must share the same root
            Guid sourceRoot = await _unitOfWork.EntityRepository.GetRootEntityId(dto.SourceEntityId);
            Guid destRoot = await _unitOfWork.EntityRepository.GetRootEntityId(dto.DestinationEntityId);

            if (sourceRoot != destRoot)
            {
                response.Message = "Transfer is only allowed between entities within the same umbrella.";
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                foreach (MemberTransferItemDTO member in dto.Members)
                {
                    if (member.IsBot)
                    {
                        Guid userBotId = _generalService.ParseStringToGuid(member.WorkerId);
                        if (userBotId == Guid.Empty) continue;

                        // Check if bot already active in destination
                        EntityUserBot existingBot = await _unitOfWork.EntityUserBotRepository
                            .GetEntityUserBotByEntityAndId(dto.DestinationEntityId, userBotId);
                        if (existingBot != null) continue;  // already there, skip

                        // Get source bot details
                        UserBot sourceBot = await _unitOfWork.UserBotRepository.GetById(userBotId);
                        if (sourceBot == null) continue;

                        if (dto.IsTransfer)
                        {
                            // Move: reassign the existing EntityUserBot to the destination entity
                            EntityUserBot sourceEntityBot = await _unitOfWork.EntityUserBotRepository
                                .GetEntityUserBotByEntityAndId(dto.SourceEntityId, userBotId);
                            if (sourceEntityBot == null) continue;

                            // Delete source entity-bot link + skills
                            await _unitOfWork.EntityUserBotSkillRepository.DeleteAllByEntityIdAndUserBotId(dto.SourceEntityId, userBotId);
                            await _unitOfWork.EntityUserBotRepository.DeleteEntityUserBot(dto.SourceEntityId, userBotId);

                            // Add to destination
                            EntityUserBot destEntityBot = new EntityUserBot
                            {
                                EntityId = dto.DestinationEntityId,
                                UserBotId = userBotId,
                                DateOfJoin = DateTime.UtcNow,
                                PartOfRotation = member.PartOfRotation,
                                WorksWeekDays = member.WorksWeekDays,
                                WorksWeekends = member.WorksWeekends,
                                MultipleShiftAssignments = member.MultipleShiftAssignments,
                                ActiveWorkerStatus = sourceEntityBot.ActiveWorkerStatus
                            };
                            await _unitOfWork.EntityUserBotRepository.Add(destEntityBot);
                        }
                        else
                        {
                            // Copy: create a new UserBot record (bots are entity-specific)
                            UserBot newBot = new UserBot
                            {
                                UserDisplayName = !string.IsNullOrEmpty(member.WorkerName)
                                    ? member.WorkerName
                                    : sourceBot.UserDisplayName
                            };
                            newBot = await _unitOfWork.UserBotRepository.Add(newBot);

                            EntityUserBot destEntityBot = new EntityUserBot
                            {
                                EntityId = dto.DestinationEntityId,
                                UserBotId = newBot.UserBotId,
                                DateOfJoin = DateTime.UtcNow,
                                PartOfRotation = member.PartOfRotation,
                                WorksWeekDays = member.WorksWeekDays,
                                WorksWeekends = member.WorksWeekends,
                                MultipleShiftAssignments = member.MultipleShiftAssignments,
                                ActiveWorkerStatus = true
                            };
                            await _unitOfWork.EntityUserBotRepository.Add(destEntityBot);

                            foreach (SkillLocalizedDTO skill in member.AssignedSkills)
                            {
                                await _unitOfWork.EntityUserBotSkillRepository.Add(new EntityUserBotSkill
                                {
                                    EntityId = dto.DestinationEntityId,
                                    UserBotId = newBot.UserBotId,
                                    SkillId = skill.SkillId
                                });
                            }
                        }
                    }
                    else
                    {
                        // Human member
                        bool alreadyInDest = await _unitOfWork.EntityWorkerRepository
                            .IsWorkerInEntity(dto.DestinationEntityId, member.WorkerId);
                        if (alreadyInDest) continue;  // already active there, skip

                        // 1. EntityWorker — reactivate or insert
                        EntityWorker entityWorker = await _unitOfWork.EntityWorkerRepository
                            .FindByWorkerAndEntity(member.WorkerId, dto.DestinationEntityId);

                        if (entityWorker != null)
                        {
                            entityWorker.IsDeleted = false;
                            entityWorker.DeletedAt = null;
                            entityWorker.DeletedById = null;
                            entityWorker.DateOfJoin = DateTime.UtcNow;
                            entityWorker.PartOfRotation = member.PartOfRotation;
                            entityWorker.WorksWeekDays = member.WorksWeekDays;
                            entityWorker.WorksWeekends = member.WorksWeekends;
                            entityWorker.MultipleShiftAssignments = member.MultipleShiftAssignments;
                            await _unitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            await _unitOfWork.GetGenericRepository<EntityWorker>().Add(new EntityWorker
                            {
                                EntityId = dto.DestinationEntityId,
                                ApplicationUserId = member.WorkerId,
                                DateOfJoin = DateTime.UtcNow,
                                PartOfRotation = member.PartOfRotation,
                                WorksWeekDays = member.WorksWeekDays,
                                WorksWeekends = member.WorksWeekends,
                                MultipleShiftAssignments = member.MultipleShiftAssignments
                            });
                        }

                        // 2. EntityWorkerSkill — reactivate or insert
                        foreach (SkillLocalizedDTO skill in member.AssignedSkills)
                        {
                            EntityWorkerSkill existingSkill = await _unitOfWork.EntityWorkerSkillRepository
                                .FindByWorkerEntityAndSkill(member.WorkerId, dto.DestinationEntityId, skill.SkillId);

                            if (existingSkill != null)
                            {
                                existingSkill.IsDeleted = false;
                                existingSkill.DeletedAt = null;
                                existingSkill.DeletedById = null;
                                await _unitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                await _unitOfWork.EntityWorkerSkillRepository.Add(new EntityWorkerSkill
                                {
                                    ApplicationUserId = member.WorkerId,
                                    EntityId = dto.DestinationEntityId,
                                    SkillId = skill.SkillId
                                });
                            }
                        }

                        // 3. EntityPermission — reactivate or insert
                        EntityPermission permission = await _unitOfWork.EntityPermissionRepository
                            .FindByEntityAndWorker(dto.DestinationEntityId, member.WorkerId);

                        if (permission != null)
                        {
                            permission.IsDeleted = false;
                            permission.DeletedAt = null;
                            permission.DeletedById = null;
                            permission.EntityPermissionRoleId = member.EntityPermissionRoleId;
                            permission.CanManageChildren = member.CanManageChildren;
                            permission.PartOfRoster = member.PartOfRoster;
                            await _unitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            await _unitOfWork.EntityPermissionRepository.Add(new EntityPermission
                            {
                                EntityId = dto.DestinationEntityId,
                                ApplicationUserId = member.WorkerId,
                                EntityPermissionRoleId = member.EntityPermissionRoleId,
                                CanManageChildren = member.CanManageChildren,
                                PartOfRoster = member.PartOfRoster
                            });
                        }

                        // 4. If transfer (move): remove from source entity
                        if (dto.IsTransfer)
                        {
                            Guid workerGuid = _generalService.ParseStringToGuid(member.WorkerId);
                            EntityWorker sourceWorker = await _unitOfWork.EntityWorkerRepository
                                .GetSimpleByWorkerAndEntity(member.WorkerId, dto.SourceEntityId);

                            if (sourceWorker != null)
                            {
                                if (!sourceWorker.PartOfRotation)
                                    await _unitOfWork.EntityWorkerShiftAssignedsRepository
                                        .DeleteAllByEntityIdAndUserId(dto.SourceEntityId, workerGuid);

                                await _unitOfWork.EntityWorkerSkillRepository
                                    .DeleteAllByEntityIdAndUserId(dto.SourceEntityId, workerGuid);
                                await _unitOfWork.EntityWorkerRepository
                                    .DeleteByEntityAndWorker(dto.SourceEntityId, member.WorkerId);
                                await _unitOfWork.EntityPermissionRepository
                                    .DeleteByEntityAndWorker(dto.SourceEntityId, member.WorkerId);
                            }
                        }
                    }
                }

                await _unitOfWork.CommitAsync();
                response.Success = true;
                response.Result = true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = ex.Message;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return response;
        }

        #endregion

        #region Config Import

        public async Task<BaseResponse<ImportCandidatesDTO>> GetImportCandidates(Guid entityId, string requesterId)
        {
            BaseResponse<ImportCandidatesDTO> response = new BaseResponse<ImportCandidatesDTO>();

            if (entityId == Guid.Empty)
            {
                response.Message = EntitiesRelatedMessages.EntityNoIdentifierError;
                return response;
            }

            Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(entityId);
            if (entity == null || entity.ParentEntityId == null || entity.ParentEntityId == Guid.Empty)
            {
                response.Message = "Entity has no parent to import from.";
                return response;
            }

            Guid parentId = entity.ParentEntityId.Value;
            ImportCandidatesDTO candidates = new ImportCandidatesDTO();

            // Load parent's shifts
            IEnumerable<Shift> parentShifts = await _unitOfWork.ShiftRepository.GetEntityShifts(parentId);
            foreach (Shift shift in parentShifts)
                candidates.Shifts.Add(new ShiftSimpleDTO { ShiftId = shift.ShiftId, ShiftName = shift.ShiftName });

            // Load parent's rules with localized names
            IEnumerable<EntityRule> parentRules = await _unitOfWork.EntityRuleRepository.GetEntityRules(parentId);
            IEnumerable<RuleTypeLocalization> ruleTypeLocalizations = await _unitOfWork.RuleTypeLocalizationRepository.GetRuleTypesByLocalization(_languageAccessor.GetLanguageCode());
            foreach (EntityRule rule in parentRules)
            {
                string ruleTypeName = ruleTypeLocalizations
                    .FirstOrDefault(r => r.RuleTypeId == rule.RuleTypeId)?.RuleTypeDisplayValue ?? rule.RuleTypeId.ToString();
                candidates.Rules.Add(new EntityRuleSimpleDTO
                {
                    EntityRuleId = rule.EntityRuleId,
                    RuleTypeId = rule.RuleTypeId,
                    RuleTypeName = ruleTypeName
                });
            }

            // Load parent's holidays with localized catalog names
            IEnumerable<EntityHoliday> parentHolidays = await _unitOfWork.EntityHolidayRepository.GetByEntityId(parentId);
            IEnumerable<HolidayCatalogLocalization> catalogLocalizations = await _unitOfWork.HolidayCatalogLocalizationRepository.GetHolidayCatalogsByLocalization(_languageAccessor.GetLanguageCode());
            foreach (EntityHoliday holiday in parentHolidays)
            {
                string displayName = holiday.HolidayCatalogId.HasValue
                    ? catalogLocalizations.FirstOrDefault(c => c.HolidayCatalogId == holiday.HolidayCatalogId.Value)?.LocalizedName ?? holiday.CustomHolidayName
                    : holiday.CustomHolidayName;
                candidates.Holidays.Add(new EntityHolidaySimpleDTO
                {
                    EntityHolidayId = holiday.EntityHolidayId,
                    HolidayDisplayName = displayName,
                    HolidayCatalogId = holiday.HolidayCatalogId,
                    CustomDay = holiday.CustomDay,
                    CustomMonth = holiday.CustomMonth
                });
            }

            response.Result = candidates;
            response.Success = true;
            return response;
        }

        public async Task<BaseResponse<int>> ImportConfigFromParent(ImportConfigDTO dto, string requesterId)
        {
            BaseResponse<int> response = new BaseResponse<int>();

            if (dto == null || dto.DestinationEntityId == Guid.Empty)
            {
                response.Message = EntitiesRelatedMessages.EntityNoIdentifierError;
                return response;
            }

            if (!dto.ShiftIds.Any() && !dto.RuleIds.Any() && !dto.HolidayIds.Any())
            {
                response.Message = "No items selected for import.";
                return response;
            }

            Entity destEntity = await _unitOfWork.GetGenericRepository<Entity>().GetById(dto.DestinationEntityId);
            if (destEntity == null || destEntity.ParentEntityId == null || destEntity.ParentEntityId == Guid.Empty)
            {
                response.Message = "Destination entity has no parent.";
                return response;
            }

            Guid parentId = destEntity.ParentEntityId.Value;
            int count = 0;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // ── SHIFTS ───────────────────────────────────────────────
                if (dto.ShiftIds.Any())
                {
                    IEnumerable<Shift> parentShifts = await _unitOfWork.ShiftRepository.GetEntityShifts(parentId);
                    IEnumerable<Shift> destShifts = await _unitOfWork.ShiftRepository.GetEntityShifts(dto.DestinationEntityId);
                    HashSet<string> existingShiftNames = new HashSet<string>(
                        destShifts.Select(s => s.ShiftName.ToLowerInvariant()));

                    foreach (Guid shiftId in dto.ShiftIds)
                    {
                        Shift source = parentShifts.FirstOrDefault(s => s.ShiftId == shiftId);
                        if (source == null) continue; // security: must belong to parent

                        if (existingShiftNames.Contains(source.ShiftName.ToLowerInvariant()))
                            continue; // conflict — skip

                        Guid newShiftId = Guid.NewGuid();
                        Shift newShift = new Shift
                        {
                            ShiftId = newShiftId,
                            EntityId = dto.DestinationEntityId,
                            ShiftName = source.ShiftName,
                            ShiftAlias = source.ShiftAlias,
                            ShiftDescription = source.ShiftDescription,
                            ShiftStartHour = source.ShiftStartHour,
                            ShiftDuration = source.ShiftDuration,
                            ShiftColorHex = source.ShiftColorHex
                        };
                        await _unitOfWork.GetGenericRepository<Shift>().Add(newShift);

                        // Copy breaks (already loaded via GetEntityShifts eager load)
                        if (source.ShiftBreaks != null)
                        {
                            foreach (ShiftBreak brk in source.ShiftBreaks)
                            {
                                await _unitOfWork.GetGenericRepository<ShiftBreak>().Add(new ShiftBreak
                                {
                                    ShiftBreakId = Guid.NewGuid(),
                                    ShiftId = newShiftId,
                                    ShiftBreakTypeId = brk.ShiftBreakTypeId,
                                    ShiftBreakStartTime = brk.ShiftBreakStartTime,
                                    ShiftBreakDuration = brk.ShiftBreakDuration,
                                    IncludedInShift = brk.IncludedInShift,
                                    IsTimeFlexible = brk.IsTimeFlexible
                                });
                            }
                        }

                        existingShiftNames.Add(source.ShiftName.ToLowerInvariant());
                        count++;
                    }
                }

                // ── RULES ────────────────────────────────────────────────
                if (dto.RuleIds.Any())
                {
                    IEnumerable<EntityRule> parentRules = await _unitOfWork.EntityRuleRepository.GetEntityRules(parentId);
                    IEnumerable<EntityRule> destRules = await _unitOfWork.EntityRuleRepository.GetEntityRules(dto.DestinationEntityId);
                    HashSet<int> existingRuleTypeIds = new HashSet<int>(destRules.Select(r => r.RuleTypeId));

                    foreach (Guid ruleId in dto.RuleIds)
                    {
                        EntityRule source = parentRules.FirstOrDefault(r => r.EntityRuleId == ruleId);
                        if (source == null) continue; // security: must belong to parent

                        if (existingRuleTypeIds.Contains(source.RuleTypeId))
                            continue; // conflict — skip

                        Guid newRuleId = Guid.NewGuid();
                        await _unitOfWork.GetGenericRepository<EntityRule>().Add(new EntityRule
                        {
                            EntityRuleId = newRuleId,
                            EntityId = dto.DestinationEntityId,
                            RuleTypeId = source.RuleTypeId,
                            RuleTypeDescription = source.RuleTypeDescription
                        });

                        // Copy specifications
                        IEnumerable<EntityRuleSpecification> specs = await _unitOfWork.EntityRuleSpecificationRepository.GetEntityRuleSpecifications(ruleId);
                        foreach (EntityRuleSpecification spec in specs)
                        {
                            await _unitOfWork.GetGenericRepository<EntityRuleSpecification>().Add(new EntityRuleSpecification
                            {
                                EntityRuleId = newRuleId,
                                SpecificationId = spec.SpecificationId,
                                SpecificationValue = spec.SpecificationValue,
                                AspectReferenceId = spec.AspectReferenceId,
                                BusinessAspectId = spec.BusinessAspectId,
                                AspectReferenceId2 = spec.AspectReferenceId2,
                                BusinessAspectId2 = spec.BusinessAspectId2
                            });
                        }

                        existingRuleTypeIds.Add(source.RuleTypeId);
                        count++;
                    }
                }

                // ── HOLIDAYS ─────────────────────────────────────────────
                if (dto.HolidayIds.Any())
                {
                    IEnumerable<EntityHoliday> parentHolidays = await _unitOfWork.EntityHolidayRepository.GetByEntityId(parentId);
                    IEnumerable<EntityHoliday> destHolidays = await _unitOfWork.EntityHolidayRepository.GetByEntityId(dto.DestinationEntityId);

                    foreach (Guid holidayId in dto.HolidayIds)
                    {
                        EntityHoliday source = parentHolidays.FirstOrDefault(h => h.EntityHolidayId == holidayId);
                        if (source == null) continue; // security: must belong to parent

                        // Conflict check
                        bool conflict = source.HolidayCatalogId.HasValue
                            ? destHolidays.Any(h => h.HolidayCatalogId.HasValue && h.HolidayCatalogId == source.HolidayCatalogId)
                            : destHolidays.Any(h => !h.HolidayCatalogId.HasValue && h.CustomDay == source.CustomDay && h.CustomMonth == source.CustomMonth);

                        if (conflict) continue;

                        await _unitOfWork.GetGenericRepository<EntityHoliday>().Add(new EntityHoliday
                        {
                            EntityHolidayId = Guid.NewGuid(),
                            EntityId = dto.DestinationEntityId,
                            HolidayCatalogId = source.HolidayCatalogId,
                            HolidayBehaviourId = source.HolidayBehaviourId,
                            CustomHolidayName = source.CustomHolidayName,
                            CustomDay = source.CustomDay,
                            CustomMonth = source.CustomMonth,
                            OperatingStartTime = source.OperatingStartTime,
                            OperatingEndTime = source.OperatingEndTime,
                            IsActive = source.IsActive,
                            Notes = source.Notes
                        });
                        count++;
                    }
                }

                await _unitOfWork.CommitAsync();
                response.Result = count;
                response.Success = true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = ex.Message;
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
