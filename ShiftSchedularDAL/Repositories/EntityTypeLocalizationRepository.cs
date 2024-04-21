using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityTypeLocalizationRepository : GenericRepository<EntityTypeLocalization>, IEntityTypeLocalizationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityTypeLocalization> _entityTypeLocalizationDbSet;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public EntityTypeLocalizationRepository(DataContext context, IUnitOfWork unitOfWork, ILocalizationRepository localizationRepository) : base(context, unitOfWork)
        {
            _context = context;
            _entityTypeLocalizationDbSet = _context.Set<EntityTypeLocalization>();
            _unitOfWork = unitOfWork;
            _localizationRepository = localizationRepository;
        }

        #endregion

        #region Get Entity Type Localization By Ids

        public async Task<EntityTypeLocalization> GetEntityTypeLocalizationByIds(int entityTypeId, string languageCode)
        {
            if (entityTypeId != 0 && !string.IsNullOrEmpty(languageCode))
            {
                Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(languageCode);
                if(localization != null)
                    return await _entityTypeLocalizationDbSet.Where(x => x.EntityTypeId == entityTypeId && x.LocalizationId == localization.LocalizationId).FirstOrDefaultAsync();
            }
            else
            {
                return null;
            }
            return null;
        }
        #endregion
    }
}
