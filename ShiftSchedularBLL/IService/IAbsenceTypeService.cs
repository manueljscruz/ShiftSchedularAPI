using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.IService
{
    public interface IAbsenceTypeService
    {
        Task<int> AddAbsenceType(string strNewAbsenceType);
        Task DeleteAbsenceType(int id);
        Task<AbsenceType> GetAbsenceTypeById(int id);
        Task<IEnumerable<AbsenceType>> GetAllAbsenceTypes();
        Task<IEnumerable<AbsenceTypeLocalizedDTO>> GetAllAbsenceTypesByLocalization(string lcode);
        Task UpdateAbsenceType(AbsenceType absenceType);
    }
}
