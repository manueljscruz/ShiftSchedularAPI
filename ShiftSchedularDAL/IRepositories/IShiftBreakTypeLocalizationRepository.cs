using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IShiftBreakTypeLocalizationRepository : IGenericRepository<ShiftBreakTypeLocalization>
    {
        Task<ShiftBreakTypeLocalization> GetBreakTypeLocalized(string lcode, int breakTypeId);
        Task<IEnumerable<ShiftBreakTypeLocalization>> GetShiftBreaksTypeLocalized(string lcode);
    }
}
