using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Outgoing
{
    public class ScheduleEntryDTO
    {
        #region Properties

        public Guid ScheduleEntryId { get; set; }
        public Guid ShiftId { get; set; }
        public DateTime ScheduleStartDate { get; set; }
        public DateTime ScheduleEndDate { get; set; }
        public ShiftDTO ShiftDTO { get; set; }
        // public List<EntityWorkerMemberDTO> ScheduleParticipants { get; set; }
        public List<ScheduleEntryParticipantDTO> ScheduleParticipants { get; set; }

        #endregion

        #region Constructor

        public ScheduleEntryDTO()
        {
            ScheduleParticipants = new List<ScheduleEntryParticipantDTO>();
        }

        #endregion
    }
}
