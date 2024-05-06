using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.IdentityModel.Tokens;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.API_Management;
using ShiftSchedularEntity.Models.DataTransferObjects;

namespace ShiftSchedularBLL.Service
{
    public class SkillService : ISkillService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISkillRepository _skillRepository;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IGenericRepository<SkillLocalization> _skillLocalizationRepository;
        private readonly IMapper _mapper;

        public SkillService(IUnitOfWork unitOfWork, ISkillRepository skillRepository, ILocalizationRepository localizationRepository, IGenericRepository<SkillLocalization> skillLocalizationRepository, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _skillRepository = skillRepository;
            _localizationRepository = localizationRepository;
            _skillLocalizationRepository = skillLocalizationRepository;
            _mapper = mapper;

        }

        #region Add Skill Localization

        public async Task<bool> AddSkillLocalization(SkillLocalizationSubmissionModel skillLocalizationSubmission)
        {
            bool result = false;

            Skill skill = await _skillRepository.GetById(skillLocalizationSubmission.SkillId);
            Localization localization = await _localizationRepository.GetById(skillLocalizationSubmission.LanguageId);

            if(skill != null && localization != null)
            {
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    SkillLocalization skillLocalization = new SkillLocalization
                    {
                        SkillId = skill.SkillId,
                        LocalizationId = localization.LocalizationId,
                        SkillDisplayValue = skillLocalizationSubmission.SkillDisplayValue
                    };

                    await _skillLocalizationRepository.Add(skillLocalization);
                    await _unitOfWork.CommitAsync();

                    result = true;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                }
                finally
                {
                    _unitOfWork.Dispose();
                }
            }

            return result;
        }

        #endregion

        #region Add Skill

        public async Task<int> AddSkill(SkillSubmissionModel newSkillSubModel)
        {
            if (!string.IsNullOrEmpty(newSkillSubModel.SkillName))
            {
                Skill newSkill = new Skill
                {
                    SkillName = newSkillSubModel.SkillName,
                    HexBGColor = newSkillSubModel.SkillHEXBgColor,
                    HexFontColor = newSkillSubModel.SkillHEXFontColor
                };

                newSkill = await _skillRepository.Add(newSkill);

                return newSkill.SkillId;
            }

            return 0;
        }

        #endregion

        #region Delete Skill

        public async Task DeleteSkill(int skillId)
        {
            if (skillId > 0)
                await _skillRepository.Delete(skillId);
        }

        #endregion

        #region Get All Skills

        public async Task<IEnumerable<Skill>> GetAllSkills()
        {
            return await _skillRepository.GetAll();
        }

        #endregion

        #region Get All Skills By Localization

        public async Task<List<SkillLocalizedDTO>> GetAllSkillsByLocalization(string lcode)
        {
            List<SkillLocalizedDTO> skillLocalizeds = new List<SkillLocalizedDTO>();

            if (lcode.Contains("-"))
                lcode = lcode.Split('-')[0];

            // Get necessary data
            IEnumerable<SkillLocalization> skillLocalizations = await _skillLocalizationRepository.GetAll();
            IEnumerable<Skill> skills = await _skillRepository.GetAll();
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(lcode);

            // If data found, add it to list to be returned
            if (localization != null && localization.SkillLocalizations.Count() != 0)
                skillLocalizeds = localization.SkillLocalizations.AsQueryable().ProjectTo<SkillLocalizedDTO>(_mapper.ConfigurationProvider).ToList();

            return skillLocalizeds;
        }

        #endregion

        #region Get Skill By Id

        public async Task<Skill> GetSkillById(int skillId)
        {
            if (skillId > 0)
                return await _skillRepository.GetById(skillId);
            else
                return null;
        }

        #endregion

        #region Update Skill

        public async Task UpdateSkill(Skill skill)
        {
            if (skill != null && !string.IsNullOrEmpty(skill.SkillName))
                await _skillRepository.Update(skill);
        }

        #endregion
    }
}
