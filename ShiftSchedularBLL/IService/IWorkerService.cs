using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;

namespace ShiftSchedularBLL.IService
{
    public interface IWorkerService
    {
        Task<BaseResponse<bool>> CreateWorker(NewWorkerDTO newWorker);
        Task<BaseResponse<WorkerDTO>> Login(LoginDTO loginDTO);
        Task<BaseResponse<bool>> UpdateWorker(WorkerDTO workerDTO);
    }
}
