using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;

namespace ShiftSchedularBLL.IService
{
    public interface IScheduleGeneratorService
    {
        Task<List<ScheduleEntryDTO>> FillOutSchedule(List<ScheduleEntryDTO> scheduleEntryDTOs, List<ShiftDTO> shifts, List<EntityRuleDTO> entityRules, List<EntityWorkerMemberDTO> entityWorkerMemberDTOs, CreateEntityScheduleDTO createEntityScheduleDTO);
    }
}
