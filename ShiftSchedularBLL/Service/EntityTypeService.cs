using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.API_Management;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.Service
{
    public class EntityTypeService : IEntityTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<EntityType> _entityTypeRepository;
        private readonly IEntityTypeLocalizationRepository _entityTypeLocalizationRepository;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IMapper _mapper;

        #region Constructor

        public EntityTypeService(IUnitOfWork unitOfWork, IGenericRepository<EntityType> entityTypeRepository, IEntityTypeLocalizationRepository entityTypeLocalizationRepository, ILocalizationRepository localizationRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _entityTypeRepository = entityTypeRepository;
            _entityTypeLocalizationRepository = entityTypeLocalizationRepository;
            _localizationRepository = localizationRepository;
            _mapper = mapper;
        }

        #endregion

        #region Methods

        #region Add Entity Type

        public async Task<int> AddEntityType(string strNewEntityTypeValue)
        {
            if(!string.IsNullOrEmpty(strNewEntityTypeValue))
            {
                EntityType newEntityType = new EntityType
                {
                    EntityTypeValue = strNewEntityTypeValue
                };

                newEntityType = await _entityTypeRepository.Add(newEntityType);

                return newEntityType.EntityTypeId;
            }

            return 0;
        }

        #endregion

        #region Add Entity Type Localization

        public async Task<bool> AddEntityTypeLocalization(EntityTypeLocalizationSubmissionModel entityTypeLocalizationSubmission)
        {
            bool result = false;

            EntityType entityType = await _entityTypeRepository.GetById(entityTypeLocalizationSubmission.EntityTypeId);
            Localization localization = await _localizationRepository.GetById(entityTypeLocalizationSubmission.LanguageID);

            if (entityType != null && localization != null)
            {
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    EntityTypeLocalization entityTypeLocalization = new EntityTypeLocalization
                    {
                        EntityTypeId = entityType.EntityTypeId,
                        LocalizationId = localization.LocalizationId,
                        EntityTypeDisplayValue = entityTypeLocalizationSubmission.EntityTypeDisplayValue
                    };

                    await _entityTypeLocalizationRepository.Add(entityTypeLocalization);
                    await _unitOfWork.CommitAsync();

                    result = true;
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

            return result;
        }

        #endregion

        #region Delete Entity Type

        public async Task DeleteEntityType(int entityTypeId)
        {
            if (entityTypeId > 0)
                await _entityTypeRepository.Delete(entityTypeId);
        }

        #endregion

        #region Get All Entity Types

        public async Task<IEnumerable<EntityType>> GetAllEntityTypes()
        {
            return await _entityTypeRepository.GetAll();
        }

        #endregion

        #region Get All Entity Types By Localization

        /// <summary>
        /// Returns a list with all of the entity types with display values in the specified language
        /// </summary>
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<List<EntityTypeLocalizedDTO>> GetAllEntityTypesByLocalization(string lcode)
        {
            List<EntityTypeLocalizedDTO> entityTypeLocalizeds = new List<EntityTypeLocalizedDTO>();

            if (lcode.Contains("-"))
                lcode = lcode.Split('-')[0];

            // Get necessary data
            IEnumerable<EntityTypeLocalization> entityTypeLocalizations = await _entityTypeLocalizationRepository.GetAll();
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(lcode);

            // If data found, add it to list to be returned
            if (localization != null && localization.EntityTypeLocalizations.Count() != 0)
                entityTypeLocalizeds = localization.EntityTypeLocalizations.AsQueryable().ProjectTo<EntityTypeLocalizedDTO>(_mapper.ConfigurationProvider).ToList();

            return entityTypeLocalizeds;
        }

        #endregion

        #region Get Entity Type By Id

        public async Task<EntityType> GetEntityTypeById(int entityTypeId)
        {
            if (entityTypeId > 0)
                return await _entityTypeRepository.GetById(entityTypeId);
            else
                return null;
        }

        #endregion

        #region Update Entity Type

        public async Task UpdateEntityType(EntityType entityType)
        {
            if (entityType != null && !string.IsNullOrEmpty(entityType.EntityTypeValue))
                await _entityTypeRepository.Update(entityType);
        }

        #endregion

        #endregion
    }
}
