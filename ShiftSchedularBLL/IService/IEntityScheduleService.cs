using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;

namespace ShiftSchedularBLL.IService
{
    public interface IEntityScheduleService
    {
        Task<EntityScheduleViewModel> GetEntityScheduleViewModel(ScheduleViewModelRequestDTO viewModelRequest);
        Task<List<ScheduleEntryDTO>> GetScheduleEntries(ScheduleViewModelRequestDTO viewModelRequest);
        Task<BaseResponse<ScheduleEntryDTO>> AssignEntry(AssignEntryDTO assignEntryDTO);
        Task<BaseResponse<ScheduleEntryDTO>> AddScheduleEntry(AddScheduleEntryDTO addScheduleEntryDTO);
        Task<BaseResponse<ScheduleEntryDTO>> AddScheduleParticipant(ScheduleParticipantOpDTO scheduleParticipantOp);
        Task<ScheduleEntryDTO> GetScheduleEntryById(Guid scheduleEntryId, string languageCode);
        Task<BaseResponse<bool>> SaveScheduleEntries(ScheduleEntryDTO[] scheduleEntryDTOs);
        Task<BaseResponse<bool>> DeleteWorkerScheduleEntries(DeleteIntervalWorkerScheduleEntriesDTO intervalWorkerScheduleEntriesDTO);
        Task<BaseResponse<bool>> DeleteScheduleEntries(DeleteIntervalWorkerScheduleEntriesDTO intervalWorkerScheduleEntriesDTO);
        Task<ScheduleEntry> CreateBaseScheduleEntry(ShiftDTO shift, DateTime date);
    }
}
