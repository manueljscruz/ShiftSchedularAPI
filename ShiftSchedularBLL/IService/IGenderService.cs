using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.Responses;

namespace ShiftSchedularBLL.IService
{
    public interface IGenderService
    {
        Task<Gender> GetGenderById(int genderId);
        Task<IEnumerable<Gender>> GetAllGenders();
        Task<int> AddGender(string strNewGenderValue);
        Task UpdateGender(Gender gender);
        Task DeleteGender(int genderId);
        Task<bool> AddGenderLocalization(GenderLocalizationSubmissionModel genderLocalizationSubmission);
        Task<List<GenderLocalizedModel>> GetAllGendersByLocalization(string lcode);
    }
}
    