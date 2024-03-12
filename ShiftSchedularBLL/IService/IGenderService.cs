using ShiftSchedularEntity.Entities;

namespace ShiftSchedularBLL.IService
{
    public interface IGenderService
    {
        Task<Gender> GetGenderById(int genderId);
        Task<IEnumerable<Gender>> GetAllGenders();
        Task<int> AddGender(string strNewGenderValue);
        Task UpdateGender(Gender gender);
        Task DeleteGender(int genderId);
    }
}
