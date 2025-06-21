using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularEntity.Models
{
    public class RuleHolder
    {
        // All Rules
        public List<EntityRuleDTO> EntityRules { get; private set; }

        // Non Shift Related
        public EntityRuleDTO MaxDailyHoursRule { get; private set; }

        public EntityRuleDTO MaxWeeklyHoursRule { get; private set; }

        public EntityRuleDTO MaxConsecutiveNonRotationDaysRule { get; private set; }

        public EntityRuleDTO MinWeeklyDaysOffRule { get; private set; }

        public EntityRuleDTO MinWeekendsOffMonthRule { get; private set; }

        public EntityRuleDTO AvgWeeklyHoursRule { get; private set; }

        public EntityRuleDTO AvgMonthlyHoursRule { get; private set; }

        // Shift Related

        public RuleHolder(List<EntityRuleDTO> entityRules)
        {
            EntityRules = entityRules;
        }


    }
}
