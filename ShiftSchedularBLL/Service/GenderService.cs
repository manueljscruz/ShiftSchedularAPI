using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularBLL.Service
{
    public class GenderService : IGenderService
    {
        private readonly IGenericRepository<Gender> _genderRepository;

        #region Constructor

        public GenderService(IGenericRepository<Gender> genderRepository)
        {
            _genderRepository = genderRepository;
        }

        #endregion

        #region Methods

        #region Add Gender

        public async Task<int> AddGender(string strNewGenderValue)
        {
            if (!string.IsNullOrEmpty(strNewGenderValue))
            {
                Gender newGender = new Gender
                {
                    GenderValue = strNewGenderValue
                };

                newGender = await _genderRepository.Add(newGender);

                return newGender.GenderId;
            }

            return 0;
        }

        #endregion

        #region Delete Gender

        public async Task DeleteGender(int genderId)
        {
            if (genderId > 0)
                await _genderRepository.Delete(genderId);

        }

        #endregion

        #region Get All Genders

        public async Task<IEnumerable<Gender>> GetAllGenders()
        {
            return await _genderRepository.GetAll();
        }

        #endregion

        #region Get Gender By Id

        public async Task<Gender> GetGenderById(int genderId)
        {
            if (genderId > 0)
                return await _genderRepository.GetById(genderId);
            else
                return null;
        }

        #endregion

        #region Update Gender

        public async Task UpdateGender(Gender gender)
        {
            if(gender != null && !string.IsNullOrEmpty(gender.GenderValue))
                await _genderRepository.Update(gender);
        }

        #endregion

        #endregion
    }
}
