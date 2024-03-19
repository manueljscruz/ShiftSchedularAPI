using ShiftSchedularEntity.Entities;

namespace ShiftSchedularBLL.IService
{
    public interface ILocalizationService
    {
        Task<IEnumerable<Localization>> GetAllLocalizations(); 
        Task<Localization> GetLocalizationById(int localizationId);
        Task<Localization> GetLocalizationByLanguageCode(string strLanguageCode);
        Task<int> AddLocalization(string strNewLanguageCode);
        Task UpdateLocalization(Localization localizationInstance);
        Task DeleteLocalization(int localizationId);
    }
}
