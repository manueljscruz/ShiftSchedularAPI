using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.Responses;

namespace ShiftSchedularBLL.Service
{
    public class GenderService : IGenderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Gender> _genderRepository;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IGenericRepository<GenderLocalization> _genderLocalizationRepository;

        #region Constructor

        public GenderService(IUnitOfWork unitOfWork, IGenericRepository<Gender> genderRepository, ILocalizationRepository localizationRepository, IGenericRepository<GenderLocalization> genderLocalizationRepository)
        {
            _unitOfWork = unitOfWork;
            _genderRepository = genderRepository;
            _localizationRepository = localizationRepository;
            _genderLocalizationRepository = genderLocalizationRepository;
        }

        #endregion

        #region Methods

        #region Add Gender

        public async Task<int> AddGender(string strNewGenderValue)
        {
            if (!string.IsNullOrEmpty(strNewGenderValue))
            {
                Gender newGender = new Gender
                {
                    GenderValue = strNewGenderValue
                };

                newGender = await _genderRepository.Add(newGender);

                return newGender.GenderId;
            }

            return 0;
        }

        #endregion

        #region Delete Gender

        public async Task DeleteGender(int genderId)
        {
            if (genderId > 0)
                await _genderRepository.Delete(genderId);

        }

        #endregion

        #region Get All Genders

        public async Task<IEnumerable<Gender>> GetAllGenders()
        {
            return await _genderRepository.GetAll();
        }

        #endregion

        #region Get All Genders By Localization

        /// <summary>
        /// Returns a list will all of the genders with display values in the specified language
        /// </summary>
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<List<GenderLocalizedModel>> GetAllGendersByLocalization(string lcode)
        {
            List<GenderLocalizedModel> genderLocalizeds = new List<GenderLocalizedModel>();

            // Get necessary data
            IEnumerable<GenderLocalization> genderLocalizations = await _genderLocalizationRepository.GetAll();
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(lcode);

            // If data found, add it to list to be returned
            if (localization != null && localization.GenderLocalizations.Count() != 0)
            {
                foreach (GenderLocalization genderLocalization in localization.GenderLocalizations)
                    genderLocalizeds.Add(new GenderLocalizedModel(genderLocalization));
            }

            return genderLocalizeds;
        }

        #endregion

        #region Get Gender By Id

        public async Task<Gender> GetGenderById(int genderId)
        {
            if (genderId > 0)
                return await _genderRepository.GetById(genderId);
            else
                return null;
        }

        #endregion

        #region Update Gender

        public async Task UpdateGender(Gender gender)
        {
            if(gender != null && !string.IsNullOrEmpty(gender.GenderValue))
                await _genderRepository.Update(gender);
        }

        #endregion

        #region Add Gender Localization

        /// <summary>
        /// Creates a new entry of Gender Localization
        /// </summary>
        /// <param name="genderLocalizationSubmission"></param>
        /// <returns></returns>
        public async Task<bool> AddGenderLocalization(GenderLocalizationSubmissionModel genderLocalizationSubmission)
        {
            bool result = false;

            Gender gender = await _genderRepository.GetById(genderLocalizationSubmission.GenderId);
            Localization localization = await _localizationRepository.GetById(genderLocalizationSubmission.LanguageId);

            if(gender != null && localization != null)
            {
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    GenderLocalization genderLocalization = new GenderLocalization
                    {
                        GenderId = gender.GenderId,
                        LocalizationId = localization.LocalizationId,
                        GenderDisplayValue = genderLocalizationSubmission.GenderDisplayValue
                    };

                    await _genderLocalizationRepository.Add(genderLocalization);
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


        #endregion
    }
}
