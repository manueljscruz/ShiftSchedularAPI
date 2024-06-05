using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IBusinessAspectLocalizationRepository : IGenericRepository<BusinessAspectLocalization>
    {
        IEnumerable<BusinessAspectLocalization> GetBusinessAspectLocalizationsById(int businessAspectId);
        Task<IEnumerable<BusinessAspectLocalization>> GetBusinessAspectsByLocalization(string languageCode);
    }
}
