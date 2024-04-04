using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
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
        private readonly IGenericRepository<EntityWorker> _entityWorkerRepository;
        private readonly IGeneralService _generalService;

        #region Constructor

        public EntityService(IUnitOfWork unitOfWork, IMapper mapper, IWorkerRepository workerRepository, IGenericRepository<EntityType> entityTypeRepository, IGenericRepository<Entity> entityRepository,
           IGenericRepository<EntityWorker> entityWorkerRepository, IGeneralService generalService) 
        { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _workerRepository = workerRepository;
            _entityRepository = entityRepository;
            _entityTypeRepository = entityTypeRepository;
            _entityWorkerRepository = entityWorkerRepository;
            _generalService = generalService;
        }

        #endregion

        #region Methods

        #region Add Entity

        public async Task<BaseResponse<Entity>> AddEntity(NewEntityDTO newEntity)
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
                            CanCreateSchedules = true
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

        public async Task<BaseResponse<bool>> UpdateEntity(Entity entity)
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

            // Update the entity and set the message
            await _entityRepository.Update(entity);

            response.Success = true;
            response.Message = Entities.UpdateEntitySuccess;

            return response;
        }

        #endregion

        #endregion
    }
}
