using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.IService
{
    public interface IAbsenceTypeService
    {
        Task<int> AddAbsenceType(string strNewAbsenceType);
        Task<bool> DeleteAbsenceType(int id);
        Task<AbsenceType> GetAbsenceTypeById(int id);
        Task<IEnumerable<AbsenceType>> GetAllAbsenceTypes();
        Task<IEnumerable<AbsenceTypeLocalizedDTO>> GetAllAbsenceTypesByLocalization(string lcode);
        Task<bool> UpdateAbsenceType(AbsenceType absenceType);
    }
}
