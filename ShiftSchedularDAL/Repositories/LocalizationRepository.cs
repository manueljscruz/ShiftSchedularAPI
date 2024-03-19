using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class LocalizationRepository : GenericRepository<Localization>, ILocalizationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<Localization> _localizationDbSet;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public LocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _localizationDbSet = _context.Set<Localization>();
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Get Localization By Language Code

        public async Task<Localization> GetLocalizationByLanguageCode(string strLanguageCode)
        {
            if (!string.IsNullOrEmpty(strLanguageCode))
            {
                return await _localizationDbSet.Where(x => x.LocalizationCode == strLanguageCode).FirstOrDefaultAsync();
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
