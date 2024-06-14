using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.IService
{
    public interface IRuleTypeService
    {
        Task<int> AddRuleType(AddRuleTypeDTO newRuleType);
        Task<bool> AddRuleTypeLocalization(RuleTypeLocalizationSubmissionModel ruleTypeLocalizationSubmissionModel);
        Task DeleteRuleTypeById(int id);
        Task<IEnumerable<RuleType>> GetAllRuleTypes();
        Task<List<RuleTypeLocalizedDTO>> GetAllRuleTypesByLocalization(string lcode);
        Task<RuleType> GetRuleTypeById(int id);
        Task UpdateRuleType(RuleType ruleType);
    }
}
