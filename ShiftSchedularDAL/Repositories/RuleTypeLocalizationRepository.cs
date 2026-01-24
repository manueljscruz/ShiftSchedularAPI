using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class RuleTypeLocalizationRepository : GenericRepository<RuleTypeLocalization>, IRuleTypeLocalizationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<RuleTypeLocalization> _rulesTypeLocalizationDbSet;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RuleTypeLocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _localizationRepository = unitOfWork.LocalizationRepository;
            _rulesTypeLocalizationDbSet = _context.Set<RuleTypeLocalization>();
        }

        #region Get Rule Type Localizations By Rule Id

        public IEnumerable<RuleTypeLocalization> GetRuleTypeLocalizationsByRuleId(int ruleTypeId)
        {
            if (ruleTypeId != 0)
            {
                return _rulesTypeLocalizationDbSet.Where(i => i.RuleTypeId.Equals(ruleTypeId)).ToList();
            }
            else return null;
        }

        #endregion

        #region Get Rule Types By Localization

        public async Task<IEnumerable<RuleTypeLocalization>> GetRuleTypesByLocalization(string languageCode)
        {
            if (!string.IsNullOrEmpty(languageCode))
            {
                try
                {
                    Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(languageCode);
                    if (localization != null)
                        return await _rulesTypeLocalizationDbSet.Where(i => i.LocalizationId.Equals(localization.LocalizationId)).ToListAsync();
                }
                catch (Exception ex)
                {
                    string strError = ex.Message;
                    return null;
                }

            }
            else
                return null;
            return null;
        }

        #endregion
    }
}
