using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IGenderLocalizationRepository : IGenericRepository<GenderLocalization>
    {
        Task<IEnumerable<GenderLocalization>> GetGendersByLocalization(string languageCode);
    }
}
