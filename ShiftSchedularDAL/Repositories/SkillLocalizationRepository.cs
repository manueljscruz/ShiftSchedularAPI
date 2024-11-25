using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class SkillLocalizationRepository : GenericRepository<SkillLocalization>, ISkillLocalizationRepository 
    {
        
        private readonly DataContext _dataContext;
        private readonly DbSet<SkillLocalization> _skillLocalizationRepository;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public SkillLocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dataContext = context;
            _unitOfWork = unitOfWork;
            _localizationRepository = unitOfWork.LocalizationRepository;
            _skillLocalizationRepository = _dataContext.Set<SkillLocalization>();
        }

        #endregion

        #region Get Skill By Code And Id

        public async Task<SkillLocalization> GetSkillByCodeAndId(int skillId, string lcode)
        {
            if (skillId > 0 && !string.IsNullOrEmpty(lcode))
            {
                Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(lcode);
                if(localization != null)
                {
                    return _skillLocalizationRepository.Where(i => i.LocalizationId.Equals(localization.LocalizationId) && i.SkillId.Equals(skillId)).FirstOrDefault();
                    
                }
            }
            return null;
        }

        #endregion
    }
}
