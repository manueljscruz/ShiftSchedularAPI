using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Repositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.QueryModels;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.Dashboard;

namespace ShiftSchedularBLL.Service
{
    public class EntityService : IEntityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWorkerRepository _workerRepository;
        private readonly IGenericRepository<EntityType> _entityTypeRepository;
        private readonly IGenericRepository<Entity> _entityRepository;
        private readonly ISkillService _skillService;
        private readonly IEntityTypeService _entityTypeService;
        private readonly IEntityWorkerRepository _entityWorkerRepository;
        private readonly IEntityTypeLocalizationRepository _entityTypeLocalizationRepository;
        private readonly IGeneralService _generalService;

        #region Constructor

        public EntityService(IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IWorkerRepository workerRepository, 
            IGenericRepository<EntityType> entityTypeRepository, 
            IGenericRepository<Entity> entityRepository,
            ISkillService skillService,
            IEntityTypeService entityTypeService,
           IEntityWorkerRepository entityWorkerRepository,
           IEntityTypeLocalizationRepository entityTypeLocalizationRepository,
           IGeneralService generalService) 
        { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _workerRepository = workerRepository;
            _entityRepository = entityRepository;
            _skillService = skillService;
            _entityTypeService = entityTypeService;
            _entityTypeRepository = entityTypeRepository;
            _entityWorkerRepository = entityWorkerRepository;
            _entityTypeLocalizationRepository = entityTypeLocalizationRepository;
            _generalService = generalService;
        }

        #endregion

        #region Methods

        #region Add Entity

        public async Task<BaseResponse<Entity>> AddEntity(FormEntityDTO newEntity)
        {
            BaseResponse<Entity> response = new BaseResponse<Entity>();

            if(newEntity != null)
            {
                if(string.IsNullOrEmpty(newEntity.EntityName))
                {
                    response.Message = Entities.EntityNameEmptyError;
                    return response;
                }

                if (string.IsNullOrEmpty(newEntity.WorkerId))
                {
                    response.Message = Entities.CreateEntityWorkerOwnerEmptyError;
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
                        if(string.IsNullOrEmpty(entity.EntityId))
                        {
                            await _unitOfWork.RollbackAsync();
                            response.Message = Entities.CreateEntityUnexpectedError;
                            return response;
                        }

                        // Create entity worker instance
                        EntityWorker entityWorker = new EntityWorker
                        {
                            WorkerId = newEntity.WorkerId,
                            EntityId = entity.EntityId,
                            ActiveWorkerStatus = true,
                            IsOwner = true,
                            CanCreateSchedules = true,
                            // DateOfJoin = DateTime.UtcNow
                        };

                        // Add entity worker and commit
                        await _entityWorkerRepository.Add(entityWorker);
                        await _unitOfWork.CommitAsync();

                        // Set response values
                        response.Result = entity;
                        response.Success = true;
                        response.Message = Entities.CreateEntitySuccess;
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                        response.Message = Entities.CreateEntityUnexpectedError;
                    }
                    finally
                    {
                        _unitOfWork.Dispose();
                    }
                }
                else
                {
                    response.Message = Entities.CreateEntityUnexpectedError;
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
            if(!string.IsNullOrEmpty(entityId))
            {
                Entity entityInstance = await _entityRepository.GetById(entityId);

                if(entityInstance != null && entityInstance.EntityWorkers.Count != 0) 
                {
                    await _unitOfWork.BeginTransactionAsync();

                    try
                    {
                        await _entityWorkerRepository.DeleteRange(entityInstance.EntityWorkers);
                        await _entityRepository.Delete(entityInstance.EntityId);
                        await _unitOfWork.CommitAsync();

                        response.Success = true;
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                        response.Message = Entities.DeleteEntityUnexpectedError;
                    }
                    finally
                    {
                        _unitOfWork.Dispose();
                    }
                }
            }
            else
            {
                response.Message = Entities.DeleteEntityNoIdentifierError;
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
            response.Message = Entities.UpdateEntityUnexpectedError;

            if (entity == null)
            {
                response.Message = Entities.EntityIsNullError;
                return response;
            }

            else if (string.IsNullOrEmpty(entity.EntityName))
            {
                response.Message = Entities.EntityNameEmptyError;
                return response;
            }

            else if(entity.EntityTypeId == 0)
            {
                response.Message = Entities.EntityTypeInvalidValueError;
                return response;
            }

            Entity entityToUpdate = await _entityRepository.GetById(entity.EntityId);
            if(entityToUpdate == null)
            {
                response.Message = Entities.UpdateEntityNotFound;
                return response;
            }

            entityToUpdate.EntityName = entity.EntityName;
            entityToUpdate.EntityTypeId = entity.EntityTypeId;
            entityToUpdate.EntityDescription = entity.EntityDescription;


            // Update the entity and set the message
            await _entityRepository.Update(entityToUpdate);

            response.Success = true;
            response.Message = Entities.UpdateEntitySuccess;

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

            foreach(EntityWorkerMemberModel entityWorkerMember in entityWorkerMembers)
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

            viewModel.EntityOwnerId = await _entityWorkerRepository.GetEntityOwnerId(entityId);

            return viewModel;
        }

        #endregion

        #region Get Entity Profile View Model
        
        public async Task<EntityProfileViewModel> GetEntityProfileViewModel(EntityProfileViewModelRequestDTO entityProfileViewModelRequest)
        {
            EntityProfileViewModel entityProfileViewModel = new EntityProfileViewModel();

            if(entityProfileViewModelRequest != null)
            {
                Entity entity = await _entityRepository.GetById(entityProfileViewModelRequest.EntityId);
                EntityWorker entityWorker = await _entityWorkerRepository.GetByWorkerAndEntity(entityProfileViewModelRequest.WorkerId, entityProfileViewModelRequest.EntityId);
                EntityType entityType = await _entityTypeRepository.GetById(entity.EntityTypeId);
                EntityTypeLocalization entityTypeLocalization = await _entityTypeLocalizationRepository.GetEntityTypeLocalizationByIds(entityType.EntityTypeId, entityProfileViewModelRequest.LanguageCode);

                entityProfileViewModel.EntityDTO = new EntityDTO(entityId: entity.EntityId, entityName: entity.EntityName, entityDescription: entity.EntityDescription, entityTypeLocalized: entityTypeLocalization.EntityTypeDisplayValue, await _entityWorkerRepository.GetTotalCountByEntity(entity.EntityId));
                entityProfileViewModel.AllowEdit = entityWorker.IsOwner;

                if (entityProfileViewModel.AllowEdit)
                {
                    entityProfileViewModel.EntityTypeLocalizeds = await _entityTypeService.GetAllEntityTypesByLocalization(entityProfileViewModelRequest.LanguageCode);
                }
            }

            return entityProfileViewModel;
        }

        #endregion

        #endregion
    }
}
