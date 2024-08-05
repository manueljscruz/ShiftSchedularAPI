using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularEntity.Entities
{
    public class ScheduleEntryWorkers
    {
        public string ScheduleEntryId { get; set; }
        public string WorkerId { get; set; }

        #region Navigation Properties

        public virtual Worker Worker { get; set; }
        public virtual ScheduleEntry ScheduleEntry { get; set; }

        #endregion
    }
}
