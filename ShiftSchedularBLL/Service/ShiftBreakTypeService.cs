using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.Service
{
    public class ShiftBreakTypeService : IShiftBreakTypeService
    {
        private readonly IGenericRepository<ShiftBreakType> _shiftBreakTypeRepository;
        private readonly IShiftBreakTypeLocalizationRepository _shiftBreakTypeLocalizationRepository;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IMapper _mapper;

        #region Constructor

        public ShiftBreakTypeService(IGenericRepository<ShiftBreakType> shiftBreakTypeRepository, 
            IShiftBreakTypeLocalizationRepository shiftBreakTypeLocalizationRepository,
            ILocalizationRepository localizationRepository,
            IMapper mapper)
        {
            _shiftBreakTypeRepository = shiftBreakTypeRepository;
            _shiftBreakTypeLocalizationRepository = shiftBreakTypeLocalizationRepository;
            _localizationRepository = localizationRepository;
            _mapper = mapper;
        }

        #endregion

        #region Methods

        #region Add Shift Break Type

        /// <summary>
        /// Adds a Shift Break type to the database
        /// </summary>
        /// <param name="strNewShiftBreakType"></param>
        /// <returns></returns>
        public async Task<int> AddShiftBreakType(string strNewShiftBreakType)
        {
            if (!string.IsNullOrEmpty(strNewShiftBreakType))
            {
                ShiftBreakType newShiftBreakType = new ShiftBreakType
                {
                    ShiftBreakTypeValue = strNewShiftBreakType
                };

                newShiftBreakType = await _shiftBreakTypeRepository.Add(newShiftBreakType);

                return newShiftBreakType.ShiftBreakTypeId;
            }

            return 0;
        }

        #endregion

        #region Add Shift Break Type Localization

        /// <summary>
        /// Add Shift Break Type in localized instance
        /// </summary>
        /// <param name="submissionModel"></param>
        /// <returns></returns>
        public async Task<bool> AddShiftBreakTypeLocalization(ShiftBreakTypeLocalizationSubmissionModel submissionModel)
        {
            bool result = false;

            if(submissionModel != null)
            {
                if(submissionModel.ShiftBreakTypeId != 0 && submissionModel.LanguageId != 0 && !string.IsNullOrEmpty(submissionModel.ShiftBreakTypeDisplayValue))
                {
                    ShiftBreakTypeLocalization shiftBreakTypeLocalization = new ShiftBreakTypeLocalization
                    {
                        LocalizationId = submissionModel.LanguageId,
                        ShiftBreakTypeId = submissionModel.ShiftBreakTypeId,
                        ShiftBreakTypeDisplayValue = submissionModel.ShiftBreakTypeDisplayValue
                    };

                    await _shiftBreakTypeLocalizationRepository.Add(shiftBreakTypeLocalization);
                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region Delete Shift Break Type

        /// <summary>
        /// Deletes Shift Break Type instance
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteShiftBreakType(int id)
        {
            if (id > 0)
                await _shiftBreakTypeRepository.Delete(id);
        }

        #endregion

        #region Get All Shift Break Types

        /// <summary>
        /// Get all Shift Break Types
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ShiftBreakType>> GetAllShiftBreakTypes()
        {
            return await _shiftBreakTypeRepository.GetAll();
        }

        #endregion

        #region Get All Shift Break Types By Localization

        /// <summary>
        /// Gets all of shift break types by localization
        /// </summary>
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<List<ShiftBreakTypeLocalizedDTO>> GetAllShiftBreakTypesByLocalization(string lcode)
        {
            List<ShiftBreakTypeLocalizedDTO> shiftBreakTypeLocalizeds = new List<ShiftBreakTypeLocalizedDTO>();
            
            if (lcode.Contains("-"))
                lcode = lcode.Split('-')[0];

            IEnumerable<ShiftBreakTypeLocalization> shiftBreakTypeLocalizations = await _shiftBreakTypeLocalizationRepository.GetShiftBreaksTypeLocalized(lcode);
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(lcode);

            if (localization != null & localization.ShiftBreakTypeLocalizations.Count() != 0)
                shiftBreakTypeLocalizations = localization.ShiftBreakTypeLocalizations.AsQueryable().ProjectTo<ShiftBreakTypeLocalization>(_mapper.ConfigurationProvider).ToList();

            return shiftBreakTypeLocalizeds;
        }

        #endregion

        #region Get Shift Break Type By Id

        /// <summary>
        /// Get Shift break type by its identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ShiftBreakType> GetShiftBreakTypeById(int id)
        {
            if (id > 0)
                return await _shiftBreakTypeRepository.GetById(id);
            else return null;
        }

        #endregion

        #region Update Shift Break Type

        /// <summary>
        /// Updates a shift break type instance
        /// </summary>
        /// <param name="shiftBreakType"></param>
        /// <returns></returns>
        public async Task UpdateShiftBreakType(ShiftBreakType shiftBreakType)
        {
            if(shiftBreakType != null)
            {
                if (!string.IsNullOrEmpty(shiftBreakType.ShiftBreakTypeValue))
                    await _shiftBreakTypeRepository.Update(shiftBreakType);
            }
        }

        #endregion

        #endregion
    }
}
