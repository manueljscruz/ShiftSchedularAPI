using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class EntityFinancialStatusDTO
    {
        public Guid PlanId { get; set; }
        public string PlanDisplayName { get; set; }
        public bool IsOnFreePlan { get; set; }
        public DateTime CurrentPeriodEndDate { get; set; }
    }
}
