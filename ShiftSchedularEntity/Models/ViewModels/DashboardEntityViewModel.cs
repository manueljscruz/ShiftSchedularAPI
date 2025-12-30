using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularEntity.Models.ViewModels
{
    public class DashboardEntityViewModel
    {
        public EntityDTO EntityDTO { get; set; }
        public List<SkillLocalizedDTO> AssignedEntitySkills { get; set; }
        public List<ScheduleEntryDTO> ScheduleEntries { get; set; }
        public PagedList<EntityWorkerAbsenceDTO> EntityWorkerAbsenceEntries { get; set; }
        public EntityStatisticsDTO EntityStatistics { get; set; }

    }
}
