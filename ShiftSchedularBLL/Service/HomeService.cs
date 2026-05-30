using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;
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
        private readonly DataContext _context;

        #endregion

        #region Constructor

        public HomeService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, DataContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _context = context;
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

            //if (worker != null)
            //    await _emailService.SendConfirmEmail(worker);
        }

        #endregion

        #region Get Admin Dashboard View Model

        public async Task<AdminDashboardViewModel> GetAdminDashboardViewModel()
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

            int totalUsers = await _context.ApplicationUsers.CountAsync();

            int totalEntities = await _context.Entities.CountAsync(e => !e.IsDeleted);

            int activeMembers = await _context.EntityWorkers.CountAsync(
                ew => !ew.IsDeleted && ew.DateToExit >= DateTime.UtcNow);

            var recentEntities = await _context.Entities
                .Where(e => !e.IsDeleted && e.CreatedAt >= sevenDaysAgo)
                .OrderByDescending(e => e.CreatedAt)
                .Include(e => e.EntityType)
                .Select(e => new RecentEntityDTO
                {
                    EntityName = e.EntityName,
                    EntityTypeName = e.EntityType.EntityTypeValue,
                    CreatedAt = e.CreatedAt
                })
                .Take(10)
                .ToListAsync();

            var recentMembers = await _context.EntityWorkers
                .Where(ew => !ew.IsDeleted && ew.CreatedAt >= sevenDaysAgo)
                .OrderByDescending(ew => ew.CreatedAt)
                .Include(ew => ew.ApplicationUser)
                .Include(ew => ew.Entity)
                .Select(ew => new RecentMemberDTO
                {
                    UserDisplayName = ew.ApplicationUser.DisplayName,
                    EntityName = ew.Entity.EntityName,
                    JoinedAt = ew.CreatedAt
                })
                .Take(10)
                .ToListAsync();

            return new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalEntities = totalEntities,
                ActiveMembers = activeMembers,
                RecentEntities = recentEntities,
                RecentMembers = recentMembers
            };
        }

        #endregion

    }
}
