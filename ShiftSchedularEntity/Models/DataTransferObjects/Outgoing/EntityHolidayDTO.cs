using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class EntityHolidayDTO
    {
        public Guid EntityHolidayId { get; set; }
        public Guid EntityId { get; set; }
        public HolidayCatalogLocalizedDTO? HolidayCatalog { get; set; }
        public HolidayBehaviourLocalizedDTO HolidayBehaviourLocalized { get; set; }
        public string CustomHolidayName { get; set; }
        public int CustomDay { get; set; }
        public int CustomMonth { get; set; }
        public TimeSpan? OperatingStartTime { get; set; }
        public TimeSpan? OperatingEndTime { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
    }
}
