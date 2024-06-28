using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IAbsenceTypeLocalizationRepository : IGenericRepository<AbsenceTypeLocalization>
    {
        IEnumerable<AbsenceTypeLocalization> GetAbsenceTypeLocalizationsById(int absenceTypeId);
        Task<IEnumerable<AbsenceTypeLocalization>> GetAbsenceTypesByLocalization(string languageCode);
    }
}
