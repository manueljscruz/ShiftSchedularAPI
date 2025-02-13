using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.QueryModels;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.Dashboard;
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
        private readonly IEntityTypeService _entityTypeService;
        private readonly IGeneralService _generalService;

        #region Constructor

        public EntityService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICryptographyService cryptographyService,
            UserManager<ApplicationUser> userManager,
            ISkillService skillService,
            IEntityTypeService entityTypeService,
            IGeneralService generalService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cryptographyService = cryptographyService;
            _userManager = userManager;
            _skillService = skillService;
            _entityTypeService = entityTypeService;
            _generalService = generalService;
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
                        List<EntityWorker> entityWorkers = new List<EntityWorker>();

                        DateTime nowUtcTime = DateTime.UtcNow;

                        foreach (Skill skill in Skills)
                        {
                            EntityWorker entityWorker = new EntityWorker
                            {
                                ApplicationUserId = newEntity.WorkerId,
                                EntityId = entity.EntityId,
                                SkillId = skill.SkillId,
                                ActiveWorkerStatus = true,
                                IsOwner = true,
                                CanCreateSchedules = true,
                                DateOfJoin = nowUtcTime
                            };

                            entityWorkers.Add(entityWorker);
                        }

                        // Add entity worker and commit
                        await _unitOfWork.EntityWorkerRepository.AddRange(entityWorkers);
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
        public async Task<BaseResponse<bool>> DeleteEntityById(string entityId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            // if entity identifier is different than null
            if (!string.IsNullOrEmpty(entityId))
            {
                byte[] entityIdBytes = Convert.FromBase64String(entityId);

                // Convert the byte array to a Guid
                Guid entityIdGuid = new Guid(entityIdBytes);

                Entity entityInstance = await _unitOfWork.GetGenericRepository<Entity>().GetById(entityIdGuid);
                IEnumerable<EntityWorker> entityWorkers = await _unitOfWork.EntityWorkerRepository.GetByEntityId(entityIdGuid);

                if (entityInstance != null && entityInstance.EntityWorkers.Count != 0)
                {
                    await _unitOfWork.BeginTransactionAsync();

                    try
                    {
                        await _unitOfWork.EntityWorkerRepository.DeleteRange(entityInstance.EntityWorkers);
                        await _unitOfWork.EntityWorkerInvitationRepository.DeleteAllByEntity(entityIdGuid);
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
                response.Message = EntitiesRelatedMessages.DeleteEntityNoIdentifierError;
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
        public async Task<Entity> GetEntityById(string entityId)
        {
            if (!string.IsNullOrEmpty(entityId))
            {
                byte[] entityIdBytes = Convert.FromBase64String(entityId);

                // Convert the byte array to a Guid
                Guid entityIdGuid = new Guid(entityIdBytes);
                return await _unitOfWork.GetGenericRepository<Entity>().GetById(entityIdGuid);
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

            byte[] entityIdBytes = Convert.FromBase64String(entity.EntityId);

            // Convert the byte array to a Guid
            Guid entityIdGuid = new Guid(entityIdBytes);

            Entity entityToUpdate = await _unitOfWork.GetGenericRepository<Entity>().GetById(entityIdGuid);
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
        public async Task<EntityMembersViewModel> GetEntitiesMembersViewModel(string entityId, string lcode)
        {
            EntityMembersViewModel viewModel = new EntityMembersViewModel();

            viewModel.Skills = await _skillService.GetAllSkillsByLocalization(lcode);

            byte[] entityIdBytes = Convert.FromBase64String(entityId);

            // Convert the byte array to a Guid
            Guid entityIdGuid = new Guid(entityIdBytes);
            
            // Get regular members
            IEnumerable<EntityWorkerMemberModel> entityWorkerMembers = await _unitOfWork.EntityWorkerRepository.GetDistinctMembersByEntityId(entityIdGuid);

            // Get user bots
            IEnumerable<EntityWorkerMemberModel> userBots = await _unitOfWork.EntityUserBotRepository.GetDistinctUserBotsByEntityId(entityIdGuid);

            // Merge all members if there is a bot instance
            if (userBots != null)
                entityWorkerMembers = entityWorkerMembers.Concat(userBots);

            if (entityWorkerMembers != null)
            {
                foreach (EntityWorkerMemberModel entityWorkerMember in entityWorkerMembers)
                {
                    EntityWorkerMemberDTO entityWorkerMemberDTO = new EntityWorkerMemberDTO();
                    entityWorkerMemberDTO = _mapper.Map(entityWorkerMember, entityWorkerMemberDTO);

                    int[] skillIds = entityWorkerMember.SkillIds.Split(',').Select(int.Parse).ToArray();

                    entityWorkerMemberDTO.SkillSet = viewModel.Skills.Where(i => skillIds.Contains(i.SkillId))
                                                .Select(s => new SkillLocalizedDTO
                                                {
                                                    SkillId = s.SkillId,
                                                    SkillLocalizedName = s.SkillLocalizedName,
                                                    SkillHexBGColor = s.SkillHexBGColor,
                                                    SkillHexFontColor = s.SkillHexFontColor
                                                }).ToList();

                    viewModel.EntityMembers.Add(entityWorkerMemberDTO);

                }
            }
            
            viewModel.EntityOwnerId = await _unitOfWork.EntityWorkerRepository.GetEntityOwnerId(entityIdGuid);

            return viewModel;
        }

        #endregion

        #region Get Entity Member By Id

        /// <summary>
        /// Get Entity Member By Id
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="workers">List of worker ids</param>
        /// <param name="lcode">Language Code</param>
        /// <returns></returns>
        public async Task<List<EntityWorkerMemberDTO>> GetEntityMembersByList(string entityId, List<string> workers, string lcode)
        {
            List<EntityWorkerMemberDTO> entityWorkerMembers = new List<EntityWorkerMemberDTO>();
            if (!string.IsNullOrEmpty(entityId) && workers.Count != 0 && !string.IsNullOrEmpty(lcode))
            {
                byte[] entityIdBytes = Convert.FromBase64String(entityId);

                // Convert the byte array to a Guid
                Guid entityIdGuid = new Guid(entityIdBytes);

                List<SkillLocalizedDTO> skillLocalizeds = await _skillService.GetAllSkillsByLocalization(lcode);
                IEnumerable<EntityWorkerMemberModel> entityWorkerMemberModels = await _unitOfWork.EntityWorkerRepository.GetDistinctMembersByEntityId(entityIdGuid, workers);

                foreach (EntityWorkerMemberModel entityWorkerMember in entityWorkerMemberModels)
                {
                    EntityWorkerMemberDTO entityWorkerMemberDTO = new EntityWorkerMemberDTO();
                    entityWorkerMemberDTO = _mapper.Map(entityWorkerMember, entityWorkerMemberDTO);

                    int[] skillIds = entityWorkerMember.SkillIds.Split(',').Select(int.Parse).ToArray();

                    entityWorkerMemberDTO.SkillSet = skillLocalizeds.Where(i => skillIds.Contains(i.SkillId))
                                                .Select(s => new SkillLocalizedDTO
                                                {
                                                    SkillId = s.SkillId,
                                                    SkillLocalizedName = s.SkillLocalizedName,
                                                    SkillHexBGColor = s.SkillHexBGColor,
                                                    SkillHexFontColor = s.SkillHexFontColor
                                                }).ToList();

                    entityWorkerMembers.Add(entityWorkerMemberDTO);

                }
            }

            return entityWorkerMembers;
        }

        #endregion

        #region Get Entity Skills

        /// <summary>
        /// Gets all the entity skills
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<List<SkillLocalizedDTO>> GetEntitySkills(string entityId, string lcode)
        {
            List<SkillLocalizedDTO> skillLocalizedDTOs = new List<SkillLocalizedDTO>();
            
            if(!string.IsNullOrEmpty(entityId) && !string.IsNullOrEmpty(lcode))
            {
                byte[] entityIdBytes = Convert.FromBase64String(entityId);

                // Convert the byte array to a Guid
                Guid entityIdGuid = new Guid(entityIdBytes);

                // Gets all skills
                List<SkillLocalizedDTO> allSkills = await _skillService.GetAllSkillsByLocalization(lcode);

                // Gets all working members
                IEnumerable<int> entitySkills = await _unitOfWork.EntityWorkerRepository.GetDistinctSkillsByEntityId(entityIdGuid);

                // Add the localized skills into the list, based on what exists in the entity skillset
                skillLocalizedDTOs = allSkills
                    .Where(skill => entitySkills.Contains(skill.SkillId))
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
        public async Task<EntityProfileViewModel> GetEntityProfileViewModel(EntityProfileViewModelRequestDTO entityProfileViewModelRequest)
        {
            EntityProfileViewModel entityProfileViewModel = new EntityProfileViewModel();

            if (entityProfileViewModelRequest != null && !string.IsNullOrEmpty(entityProfileViewModelRequest.EntityId))
            {
                byte[] entityIdBytes = Convert.FromBase64String(entityProfileViewModelRequest.EntityId);

                // Convert the byte array to a Guid
                Guid entityIdGuid = new Guid(entityIdBytes);

                Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(entityIdGuid);
                List<EntityWorker> entityWorkerInstances = await _unitOfWork.EntityWorkerRepository.GetByWorkerAndEntity(entityProfileViewModelRequest.WorkerId, entityIdGuid);
                EntityType entityType = await _unitOfWork.GetGenericRepository<EntityType>().GetById(entity.EntityTypeId);
                EntityTypeLocalization entityTypeLocalization = await _unitOfWork.EntityTypeLocalizationRepository.GetEntityTypeLocalizationByIds(entityType.EntityTypeId, entityProfileViewModelRequest.LanguageCode);

                entityProfileViewModel.EntityDTO = new EntityDTO(entityId: entity.EntityId, entityName: entity.EntityName, entityDescription: entity.EntityDescription, entityTypeLocalized: entityTypeLocalization.EntityTypeDisplayValue, await _unitOfWork.EntityWorkerRepository.GetTotalCountByEntity(entity.EntityId));
                entityProfileViewModel.AllowEdit = entityWorkerInstances.Any(i => i.IsOwner);

                if (entityProfileViewModel.AllowEdit)
                {
                    entityProfileViewModel.EntityTypeLocalizeds = await _entityTypeService.GetAllEntityTypesByLocalization(entityProfileViewModelRequest.LanguageCode);
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
                // Checks if there is an destination entity
                //if (string.IsNullOrEmpty(newMemberDTO.DestinationEntityId))
                if(newMemberDTO.EntityIdGuid == Guid.Empty)
                {
                    response.Message = EntityWorkerRelatedMessages.AddNewMemberDestinationEntityEmpty;
                    return response;
                }

                // Check if there is a name to the Member
                else if (newMemberDTO.IsBot && string.IsNullOrEmpty(newMemberDTO.MemberName))
                {
                    response.Message = EntityWorkerRelatedMessages.AddNewMemberBotNameEmpty;
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

                Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(newMemberDTO.EntityIdGuid);

                // Destination Entity exists
                if(entity != null)
                {
                    // Save record of time instance
                    DateTime nowUTCTime = DateTime.UtcNow;

                    // Adding a bot member
                    if (newMemberDTO.IsBot)
                    {
                        UserBot newUserBot = _mapper.Map<UserBot>(newMemberDTO);
                        newUserBot.DateOfCreation = nowUTCTime;

                        try
                        {
                            await _unitOfWork.BeginTransactionAsync();

                            // Add new bot
                            newUserBot = await _unitOfWork.UserBotRepository.Add(newUserBot);

                            // For each skill assigned, create a entity worker instance
                            foreach (SkillLocalizedDTO skill in newMemberDTO.AssignedSkills)
                            {
                                EntityUserBot entityUserBot = new EntityUserBot
                                {
                                    EntityId = entity.EntityId,
                                    UserBotId = newUserBot.UserBotId,
                                    SkillId = skill.SkillId,
                                    ActiveWorkerStatus = true,
                                    DateOfJoin = nowUTCTime
                                };
                                
                                await _unitOfWork.GetGenericRepository<EntityUserBot>().Add(entityUserBot);
                            }

                            // Commit changes
                            await _unitOfWork.CommitAsync();

                            
                            response.Message = EntityWorkerRelatedMessages.AddNewMemberBotSuccessful;
                            response.Success = true;
                        }
                        catch (Exception ex)
                        {
                            await _unitOfWork.RollbackAsync();
                            response.Message = EntityWorkerRelatedMessages.AddNewMemberUnexpectedError;
                        }

                        EntityWorkerMemberDTO entityWorkerMemberDTO = new EntityWorkerMemberDTO
                        {
                            WorkerId = newUserBot.UserBotId.ToString(),
                            WorkerName = newMemberDTO.MemberName,
                            CanCreateSchedules = false,
                            IsOwner = false,
                            DateOfJoin = nowUTCTime,
                            SkillSet = newMemberDTO.AssignedSkills
                        };

                        response.Result = entityWorkerMemberDTO;
                    }
                    else
                    {
                        // Check if there is a entity worker with that email already in the entity
                        ApplicationUser possibleWorker = await _userManager.FindByEmailAsync(newMemberDTO.MemberEmail);

                        if(possibleWorker != null && await _unitOfWork.EntityWorkerRepository.IsWorkerInEntity(newMemberDTO.EntityIdGuid, possibleWorker.Id))
                        {
                            response.Message = EntityWorkerRelatedMessages.AddNewMemberAlreadyInEntity;
                            return response;
                        }

                        string skillsAggregated = newMemberDTO.AssignedSkills.Select(i => i.SkillId).Aggregate("", (i, j) => i + "," + j);

                        EntityWorkerInvitation entityWorkerInvitation = new EntityWorkerInvitation
                        {
                            EntityId = newMemberDTO.EntityIdGuid,
                            Email = newMemberDTO.MemberEmail,
                            ApplicationUserId = possibleWorker != null ? possibleWorker.Id : null,
                            InviteDate = nowUTCTime,
                            SkillsetIds = skillsAggregated
                        };

                        await _unitOfWork.EntityWorkerInvitationRepository.Add(entityWorkerInvitation);
                        response.Result = true;
                        response.Message = EntityWorkerRelatedMessages.AddNewMemberInvitationSuccessful;
                        response.Success = true;
                    }
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
                    response.Message = "";
                    return response;
                }

                else if(string.IsNullOrEmpty(updateEntityMemberDTO.EntityId))
                {
                    response.Message = "";
                    return response;
                }

                else if(updateEntityMemberDTO.IsBot && string.IsNullOrEmpty(updateEntityMemberDTO.WorkerName))
                {
                    response.Message = "";
                    return response;
                }

                else if(updateEntityMemberDTO.AssignedSkills.Count == 0)
                {
                    response.Message = "";
                    return response;
                }

                byte[] entityIdBytes = Convert.FromBase64String(updateEntityMemberDTO.EntityId);

                // Convert the byte array to a Guid
                Guid entityIdGuid = new Guid(entityIdBytes);

                Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(entityIdGuid);
                if(entity != null)
                {
                    try
                    {
                        await _unitOfWork.BeginTransactionAsync();

                        // If its a bot, update the name
                        if (updateEntityMemberDTO.IsBot)
                        {
                            UserBot userBot = await _unitOfWork.UserBotRepository.GetById(updateEntityMemberDTO.WorkerId);
                            if (userBot != null)
                            {
                                userBot.UserDisplayName = updateEntityMemberDTO.WorkerName;
                                await _unitOfWork.UserBotRepository.Update(userBot);
                            }
                        }

                        // Get Entity Worker Instances
                        List<EntityWorker> entityWorkerInstances = await _unitOfWork.EntityWorkerRepository.GetByWorkerAndEntity(updateEntityMemberDTO.WorkerId, entityIdGuid);
                    
                        if(entityWorkerInstances.Count == 0)
                        {
                            response.Message = "";
                            return response;
                        }

                        DateTime dateOfJoin = entityWorkerInstances.First().DateOfJoin;

                        // Delete Previous instances
                        await _unitOfWork.EntityWorkerRepository.DeleteRange(entityWorkerInstances);

                        // Add new ones
                        foreach (SkillLocalizedDTO skill in updateEntityMemberDTO.AssignedSkills)
                        {
                            EntityWorker entityWorkerInstance = new EntityWorker
                            {
                                EntityId = entityIdGuid,
                                ApplicationUserId = updateEntityMemberDTO.WorkerId,
                                ActiveWorkerStatus = true,
                                CanCreateSchedules = false,
                                IsOwner = false,
                                SkillId = skill.SkillId,
                                DateOfJoin = dateOfJoin
                            };

                            await _unitOfWork.EntityWorkerRepository.Add(entityWorkerInstance);
                        }

                        await _unitOfWork.CommitAsync();
                        response.Result = true;
                        response.Message = EntityWorkerRelatedMessages.UpdateMemberSuccess;
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
                else
                {
                    response.Message = EntitiesRelatedMessages.EntityNotFound;
                    return response;
                }
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

            if(string.IsNullOrEmpty(workerMemberDTO.WorkerId))
            {
                response.Message = "";
                return response;
            }

            else if (workerMemberDTO.EntityId == Guid.Empty)
            {
                response.Message = "";
                return response;
            }

            Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(workerMemberDTO.EntityId);
            if(entity == null)
            {

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
                        IEnumerable<EntityUserBot> entityUserBots = await _unitOfWork.EntityUserBotRepository.GetEntityUserBotsByEntityAndId(entity.EntityId, userBotGuid);
                        if (userBot != null && entityUserBots.Count() != 0)
                        {
                            // Remove Entity User Bot Instances
                            await _unitOfWork.EntityUserBotRepository.DeleteRange(entityUserBots);

                            // Remove User Bot instance
                            await _unitOfWork.UserBotRepository.Delete(userBot.UserBotId);
                        }
                    }
                    else
                    {
                        // Remove Entity Worker Instances
                        List<EntityWorker> entityWorkersInstances = await _unitOfWork.EntityWorkerRepository.GetByWorkerAndEntity(workerMemberDTO.WorkerId, workerMemberDTO.EntityId);
                        if(entityWorkersInstances.Count != 0)
                        {
                            await _unitOfWork.EntityWorkerRepository.DeleteRange(entityWorkersInstances);
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

        #endregion
    }
}
