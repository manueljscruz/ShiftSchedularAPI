using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.Service
{
    public class AbsenceTypeService : IAbsenceTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<AbsenceType> _absenceTypeRepository;
        private readonly IAbsenceTypeLocalizationRepository _absenceTypeLocalizationRepository;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IMapper _mapper;

        #region Constructor

        public AbsenceTypeService(IUnitOfWork unitOfWork, 
            IGenericRepository<AbsenceType> absenceTypeRepository, 
            IAbsenceTypeLocalizationRepository absenceTypeLocalizationRepository, 
            ILocalizationRepository localizationRepository, 
            IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _absenceTypeRepository = absenceTypeRepository;
            _absenceTypeLocalizationRepository = absenceTypeLocalizationRepository;
            _localizationRepository = localizationRepository;
            _mapper = mapper;
        }

        #endregion

        #region Methods

        #region Add Absence Type

        /// <summary>
        /// Adds a new absence type instance
        /// </summary>
        /// <param name="strNewAbsenceType"></param>
        /// <returns></returns>
        public async Task<int> AddAbsenceType(string strNewAbsenceType)
        {
            if (!string.IsNullOrEmpty(strNewAbsenceType))
            {
                AbsenceType newAbsenceType = new AbsenceType
                {
                    AbsenceTypeName = strNewAbsenceType
                };
                newAbsenceType = await _absenceTypeRepository.Add(newAbsenceType);

                return newAbsenceType.AbsenceTypeId;
            }

            return 0;
        }

        #endregion

        #region Delete Absence Type

        /// <summary>
        /// Deletes the absence type instance
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAbsenceType(int id)
        {
            if(id > 0)
                await _absenceTypeRepository.Delete(id);
        }

        #endregion

        #region Get Absence Type By Id

        /// <summary>
        /// Gets the absence type instance by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<AbsenceType> GetAbsenceTypeById(int id)
        {
            if (id > 0)
                return await _absenceTypeRepository.GetById(id);
            else 
                return null;
        }

        #endregion

        #region Get All Absence Types

        /// <summary>
        /// Gets all Absence Type instances
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AbsenceType>> GetAllAbsenceTypes()
        {
            return await _absenceTypeRepository.GetAll();
        }

        #endregion

        #region Get All Absence Types By Localization

        /// <summary>
        /// Gets all absence type localized values
        /// </summary>
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AbsenceTypeLocalizedDTO>> GetAllAbsenceTypesByLocalization(string lcode)
        {
            List<AbsenceTypeLocalizedDTO> absenceTypeLocalizeds = new List<AbsenceTypeLocalizedDTO>();

            if (lcode.Contains("-"))
                lcode = lcode.Split('-')[0];

            IEnumerable<AbsenceTypeLocalization> absenceTypeLocalizations = await _absenceTypeLocalizationRepository.GetAll();
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(lcode);

            // If data found, add it to list to be returned
            if (localization != null && localization.AbsenceTypeLocalizations.Count() != 0)
                absenceTypeLocalizeds = localization.AbsenceTypeLocalizations.AsQueryable().ProjectTo<AbsenceTypeLocalizedDTO>(_mapper.ConfigurationProvider).ToList();

            return absenceTypeLocalizeds;
        }

        #endregion

        #region Update Absence Type

        /// <summary>
        /// Updates the absence type instance
        /// </summary>
        /// <param name="absenceType"></param>
        /// <returns></returns>
        public async Task UpdateAbsenceType(AbsenceType absenceType)
        {
            if (absenceType != null && !string.IsNullOrEmpty(absenceType.AbsenceTypeName))
                await _absenceTypeRepository.Update(absenceType);
        }

        #endregion

        #endregion
    }
}
