using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.Responses;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularIL.IServices;

namespace ShiftSchedularBLL.Service
{
    public class HomeService : IHomeService
    {
        #region Properties

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        #endregion

        #region Constructor

        public HomeService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        #endregion

        #region Methods

        #region Get Home View Model

        public async Task<HomeViewModel> GetHomeViewModel(string languageCode)
        {
            HomeViewModel homeViewModel = new HomeViewModel();

            if (languageCode.Contains("-"))
                languageCode = languageCode.Split('-')[0];

            IEnumerable<GenderLocalization> genderLocalizations = await _unitOfWork.GenderLocalizationRepository.GetGendersByLocalization(languageCode);

            if (genderLocalizations.Count() != 0)
                homeViewModel.Genders = _mapper.Map<IEnumerable<GenderLocalizedDTO>>(genderLocalizations);

            return homeViewModel;
        }

        #endregion

        public async Task SendEmailTest(string email)
        {
            Worker worker = await _unitOfWork.WorkerRepository.GetByEmail(email);

            if (worker != null)
                await _emailService.SendConfirmEmail(worker);
        }

        #endregion
    }
}
