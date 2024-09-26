using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.IRepositories;
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

namespace ShiftSchedularBLL.Service
{
    public class EntityService : IEntityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICryptographyService _cryptographyService;
        private readonly IWorkerRepository _workerRepository;
        private readonly IGenericRepository<EntityType> _entityTypeRepository;
        private readonly IGenericRepository<Entity> _entityRepository;
        private readonly IGenericRepository<Gender> _genderRepository;
        private readonly ISkillRepository _skillRepository;
        private readonly ISkillService _skillService;
        private readonly IEntityTypeService _entityTypeService;
        private readonly IEntityWorkerRepository _entityWorkerRepository;
        private readonly IEntityTypeLocalizationRepository _entityTypeLocalizationRepository;
        private readonly IEntityWorkerInvitationRepository _entityWorkerInvitationRepository;
        private readonly IGeneralService _generalService;

        #region Constructor

        public EntityService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ICryptographyService cryptographyService,
            IWorkerRepository workerRepository,
            IGenericRepository<EntityType> entityTypeRepository,
            IGenericRepository<Entity> entityRepository,
            IGenericRepository<Gender> genderRepository,
            ISkillRepository skillRepository,
            ISkillService skillService,
            IEntityTypeService entityTypeService,
            IEntityWorkerRepository entityWorkerRepository,
            IEntityTypeLocalizationRepository entityTypeLocalizationRepository,
            IEntityWorkerInvitationRepository entityWorkerInvitationRepository,
            IGeneralService generalService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cryptographyService = cryptographyService;
            _workerRepository = workerRepository;
            _entityRepository = entityRepository;
            _skillService = skillService;
            _skillRepository = skillRepository;
            _genderRepository = genderRepository;
            _entityTypeService = entityTypeService;
            _entityTypeRepository = entityTypeRepository;
            _entityWorkerRepository = entityWorkerRepository;
            _entityTypeLocalizationRepository = entityTypeLocalizationRepository;
            _entityWorkerInvitationRepository = entityWorkerInvitationRepository;
            _generalService = generalService;
        }

        #endregion

        #region Methods

        #region Add Entity

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

                EntityType entityTypeInstance = await _entityTypeRepository.GetById(newEntity.EntityTypeId);

                if (entityTypeInstance != null && !string.IsNullOrEmpty(newEntity.WorkerId))
                {
                    await _unitOfWork.BeginTransactionAsync();

                    try
                    {
                        // Add Entity
                        Entity entity = _mapper.Map<Entity>(newEntity);
                        entity.EntityId = _generalService.GenerateGuid();
                        entity = await _entityRepository.Add(entity);

                        // If there is no identifier associated to the entity
                        if (string.IsNullOrEmpty(entity.EntityId))
                        {
                            await _unitOfWork.RollbackAsync();
                            response.Message = EntitiesRelatedMessages.CreateEntityUnexpectedError;
                            return response;
                        }

                        List<Skill> Skills = await _skillRepository.GetSkillsByNames(new List<string> { SkillsConstants.GENERAL_WORKER, SkillsConstants.MANAGEMENT });
                        ; List<EntityWorker> entityWorkers = new List<EntityWorker>();

                        foreach (Skill skill in Skills)
                        {
                            EntityWorker entityWorker = new EntityWorker
                            {
                                WorkerId = newEntity.WorkerId,
                                EntityId = entity.EntityId,
                                SkillId = skill.SkillId,
                                ActiveWorkerStatus = true,
                                IsOwner = true,
                                CanCreateSchedules = true,
                                DateOfJoin = DateTime.UtcNow
                            };

                            entityWorkers.Add(entityWorker);
                        }

                        // Add entity worker and commit
                        await _entityWorkerRepository.AddRange(entityWorkers);
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

        public async Task<BaseResponse<bool>> DeleteEntityById(string entityId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            // if entity identifier is different than null
            if (!string.IsNullOrEmpty(entityId))
            {
                Entity entityInstance = await _entityRepository.GetById(entityId);
                IEnumerable<EntityWorker> entityWorkers = await _entityWorkerRepository.GetByEntityId(entityId);

                if (entityInstance != null && entityInstance.EntityWorkers.Count != 0)
                {
                    await _unitOfWork.BeginTransactionAsync();

                    try
                    {
                        await _entityWorkerRepository.DeleteRange(entityInstance.EntityWorkers);
                        await _entityWorkerInvitationRepository.DeleteAllByEntity(entityId);
                        await _entityRepository.Delete(entityInstance.EntityId);
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

        public async Task<IEnumerable<Entity>> GetAllEntities()
        {
            return await _entityRepository.GetAll();
        }

        #endregion

        #region Get Entity By Id

        public async Task<Entity> GetEntityById(string entityId)
        {
            if (!string.IsNullOrEmpty(entityId))
                return await _entityRepository.GetById(entityId);
            else
                return null;
        }

        #endregion

        #region Update Entity

        public async Task<BaseResponse<bool>> UpdateEntity(FormEntityDTO entity)
        {
            // if(entity != null && !string.IsNullOrEmpty(entity.EntityName) && entity.EntityTypeId != 0)
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

            Entity entityToUpdate = await _entityRepository.GetById(entity.EntityId);
            if (entityToUpdate == null)
            {
                response.Message = EntitiesRelatedMessages.UpdateEntityNotFound;
                return response;
            }

            entityToUpdate.EntityName = entity.EntityName;
            entityToUpdate.EntityTypeId = entity.EntityTypeId;
            entityToUpdate.EntityDescription = entity.EntityDescription;


            // Update the entity and set the message
            await _entityRepository.Update(entityToUpdate);

            response.Success = true;
            response.Message = EntitiesRelatedMessages.UpdateEntitySuccess;

            return response;
        }

        #endregion

        #region Get Entities By Worker Id

        public async Task<List<EntityWorkerDTO>> GetEntitiesByWorkerId(string workerId)
        {
            List<EntityWorkerDTO> entityWorkers = new List<EntityWorkerDTO>();

            if (!string.IsNullOrEmpty(workerId))
            {
                try
                {
                    // Get entity worker instances by worker identifier
                    IEnumerable<EntityWorkerDTO> entityWorkerDTOs = await _entityWorkerRepository.GetByWorkerId(workerId);

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

        public async Task<EntityMembersViewModel> GetEntitiesMembersViewModel(string entityId, string lcode)
        {
            EntityMembersViewModel viewModel = new EntityMembersViewModel();

            viewModel.Skills = await _skillService.GetAllSkillsByLocalization(lcode);

            IEnumerable<EntityWorkerMemberModel> entityWorkerMembers = await _entityWorkerRepository.GetDistinctMembersByEntityId(entityId);

            if(entityWorkerMembers != null)
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
            
            viewModel.EntityOwnerId = await _entityWorkerRepository.GetEntityOwnerId(entityId);

            return viewModel;
        }

        #endregion

        #region Get Entity Member By Id

        public async Task<List<EntityWorkerMemberDTO>> GetEntityMembersByList(string entityId, List<string> workers, string lcode)
        {
            List<EntityWorkerMemberDTO> entityWorkerMembers = new List<EntityWorkerMemberDTO>();
            if (!string.IsNullOrEmpty(entityId) && workers.Count != 0 && !string.IsNullOrEmpty(lcode))
            {
                List<SkillLocalizedDTO> skillLocalizeds = await _skillService.GetAllSkillsByLocalization(lcode);
                IEnumerable<EntityWorkerMemberModel> entityWorkerMemberModels = await _entityWorkerRepository.GetDistinctMembersByEntityId(entityId, workers);

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
                // Gets all skills
                List<SkillLocalizedDTO> allSkills = await _skillService.GetAllSkillsByLocalization(lcode);

                // Gets all working members
                IEnumerable<int> entitySkills = await _entityWorkerRepository.GetDistinctSkillsByEntityId(entityId);

                // Add the localized skills into the list, based on what exists in the entity skillset
                skillLocalizedDTOs = allSkills
                    .Where(skill => entitySkills.Contains(skill.SkillId))
                    .ToList();
            }

            return skillLocalizedDTOs;
        }

        #endregion

        #region Get Entity Profile View Model

        public async Task<EntityProfileViewModel> GetEntityProfileViewModel(EntityProfileViewModelRequestDTO entityProfileViewModelRequest)
        {
            EntityProfileViewModel entityProfileViewModel = new EntityProfileViewModel();

            if (entityProfileViewModelRequest != null)
            {
                Entity entity = await _entityRepository.GetById(entityProfileViewModelRequest.EntityId);
                List<EntityWorker> entityWorkerInstances = await _entityWorkerRepository.GetByWorkerAndEntity(entityProfileViewModelRequest.WorkerId, entityProfileViewModelRequest.EntityId);
                EntityType entityType = await _entityTypeRepository.GetById(entity.EntityTypeId);
                EntityTypeLocalization entityTypeLocalization = await _entityTypeLocalizationRepository.GetEntityTypeLocalizationByIds(entityType.EntityTypeId, entityProfileViewModelRequest.LanguageCode);

                entityProfileViewModel.EntityDTO = new EntityDTO(entityId: entity.EntityId, entityName: entity.EntityName, entityDescription: entity.EntityDescription, entityTypeLocalized: entityTypeLocalization.EntityTypeDisplayValue, await _entityWorkerRepository.GetTotalCountByEntity(entity.EntityId));
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

        public async Task<BaseResponse<object>> AddNewEntityMember(AddNewMemberDTO newMemberDTO)
        {
            BaseResponse<object> response = new BaseResponse<object>();
            response.Message = EntityWorkerRelatedMessages.AddNewMemberUnexpectedError;
            response.Success = false;

            if (newMemberDTO != null)
            {
                // Checks if there is an destination entity
                if (string.IsNullOrEmpty(newMemberDTO.DestinationEntityId))
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

                Entity entity = await _entityRepository.GetById(newMemberDTO.DestinationEntityId);

                // Destination Entity exists
                if(entity != null)
                {
                    // Adding a bot member
                    if (newMemberDTO.IsBot)
                    {
                        // Worker of non-binary gender
                        IEnumerable<Gender> genders = await _genderRepository.GetAll();
                        Gender gender = genders.Where(i => i.GenderValue == GenderConstants.NONBINARY).FirstOrDefault();

                        // If gender not found
                        if(gender == null)
                        {
                            response.Message = EntityWorkerRelatedMessages.AddNewMemberUnexpectedError;
                            return response;
                        }

                        // Generate ID, email and password for bot
                        string workerGUID = _generalService.GenerateGuid();
                        string generatedBotEmail = _generalService.GenerateBotEmail(workerGUID);
                        string password = _cryptographyService.HashPassword(_generalService.GenerateBotPassword(workerGUID));

                        // Save record of time instance
                        DateTime nowUTCTime = DateTime.UtcNow;

                        try
                        {
                            await _unitOfWork.BeginTransactionAsync();

                            // Create new worker instance
                            Worker newBotWorker = new Worker
                            {
                                WorkerId = workerGUID,
                                WorkerName = newMemberDTO.MemberName,
                                GenderId = gender.GenderId,
                                Email = generatedBotEmail,
                                Password = password,
                                IsActive = true,
                                IsBot = true
                            };

                            // Add new worker instance
                            newBotWorker = await _workerRepository.Add(newBotWorker);

                            // For each skill assigned, create a entity worker instance
                            foreach (SkillLocalizedDTO skill in newMemberDTO.AssignedSkills)
                            {
                                EntityWorker entityWorkerInstance = new EntityWorker
                                {
                                    EntityId = entity.EntityId,
                                    WorkerId = workerGUID,
                                    ActiveWorkerStatus = true,
                                    CanCreateSchedules = false,
                                    IsOwner = false,
                                    SkillId = skill.SkillId,
                                    DateOfJoin = nowUTCTime
                                };

                                await _entityWorkerRepository.Add(entityWorkerInstance);
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
                            WorkerId = workerGUID,
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
                        Worker possibleWorker = await _workerRepository.GetByEmail(newMemberDTO.MemberEmail);

                        if(possibleWorker != null && await _entityWorkerRepository.IsWorkerInEntity(newMemberDTO.DestinationEntityId, possibleWorker.WorkerId))
                        {
                            response.Message = EntityWorkerRelatedMessages.AddNewMemberAlreadyInEntity;
                            return response;
                        }

                        string skillsAggregated = newMemberDTO.AssignedSkills.Select(i => i.SkillId).Aggregate("", (i, j) => i + "," + j);

                        EntityWorkerInvitation entityWorkerInvitation = new EntityWorkerInvitation
                        {
                            EntityId = newMemberDTO.DestinationEntityId,
                            Email = newMemberDTO.MemberEmail,
                            WorkerId = possibleWorker != null ? possibleWorker.WorkerId : null,
                            InviteDate = DateTime.UtcNow,
                            SkillsetIds = skillsAggregated
                        };

                        await _entityWorkerInvitationRepository.Add(entityWorkerInvitation);
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

                Entity entity = await _entityRepository.GetById(updateEntityMemberDTO.EntityId);
                if(entity != null)
                {
                    try
                    {
                        await _unitOfWork.BeginTransactionAsync();

                        Worker worker = await _workerRepository.GetById(updateEntityMemberDTO.WorkerId);
                        if (worker != null && worker.IsBot)
                        {
                            worker.WorkerName = updateEntityMemberDTO.WorkerName;
                            await _workerRepository.Update(worker);
                        }

                        List<EntityWorker> entityWorkerInstances = await _entityWorkerRepository.GetByWorkerAndEntity(updateEntityMemberDTO.WorkerId, updateEntityMemberDTO.EntityId);
                    
                        if(entityWorkerInstances.Count == 0)
                        {
                            response.Message = "";
                            return response;
                        }

                        DateTime dateOfJoin = entityWorkerInstances.First().DateOfJoin;

                        await _entityWorkerRepository.DeleteRange(entityWorkerInstances);

                        foreach (SkillLocalizedDTO skill in updateEntityMemberDTO.AssignedSkills)
                        {
                            EntityWorker entityWorkerInstance = new EntityWorker
                            {
                                EntityId = entity.EntityId,
                                WorkerId = worker.WorkerId,
                                ActiveWorkerStatus = true,
                                CanCreateSchedules = false,
                                IsOwner = false,
                                SkillId = skill.SkillId,
                                DateOfJoin = dateOfJoin
                            };

                            await _entityWorkerRepository.Add(entityWorkerInstance);
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

                }

            }

            return response;
        }


        #endregion

        #endregion
    }
}
