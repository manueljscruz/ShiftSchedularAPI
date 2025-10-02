using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.IService
{
    public interface IScheduleGeneratorService
    {

        Task<BaseResponse<List<ScheduleEntryDTO>>> CreateEntitySchedule(CreateEntityScheduleDTO createEntityScheduleDTO, bool isOpRotation = false);
        Task<BaseResponse<List<ScheduleEntryDTO>>> ApplyRotationCycle(ApplyRotationCycleDTO rotationCycleDTO);

    }
}
