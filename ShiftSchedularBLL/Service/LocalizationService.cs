using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularBLL.Service
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ILocalizationRepository _localizationRepository;

        #region Constructor

        public LocalizationService(ILocalizationRepository localizationRepository)
        {
            _localizationRepository = localizationRepository;
        }

        #endregion

        #region Methods

        #region Add Localization

        public async Task<int> AddLocalization(string strNewLanguageCode)
        {
            if(!string.IsNullOrEmpty(strNewLanguageCode))
            {
                Localization newLocalization = new Localization
                {
                    LocalizationCode = strNewLanguageCode
                };

                newLocalization = await _localizationRepository.Add(newLocalization);

                return newLocalization.LocalizationId;
            }

            return 0;
        }

        #endregion

        #region Delete Localization

        public async Task DeleteLocalization(int localizationId)
        {
            if(localizationId > 0)
                await _localizationRepository.Delete(localizationId);
        }

        public async Task<IEnumerable<Localization>> GetAllLocalizations()
        {
            return await _localizationRepository.GetAll();
        }

        #endregion

        #region Get Localization By Id

        public async Task<Localization> GetLocalizationById(int localizationId)
        {
            if (localizationId > 0)
                return await _localizationRepository.GetById(localizationId);
            else
                return null;
        }

        #endregion

        #region Get Localization By Language Code

        public async Task<Localization> GetLocalizationByLanguageCode(string strLanguageCode)
        {
            if (!string.IsNullOrEmpty(strLanguageCode))
                return await _localizationRepository.GetLocalizationByLanguageCode(strLanguageCode);
            else
                return null;
        }

        #endregion

        #region Update Localization

        public async Task UpdateLocalization(Localization localizationInstance)
        {
            if (localizationInstance != null && !string.IsNullOrEmpty(localizationInstance.LocalizationCode))
                await _localizationRepository.Update(localizationInstance);
        }

        #endregion

        #endregion
    }
}
