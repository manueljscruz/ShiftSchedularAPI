using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;

namespace ShiftSchedularBLL.IService
{
    public interface IWorkerService
    {
        Task<BaseResponse<bool>> CreateWorker(NewWorkerDTO newWorker);

        Task<BaseResponse<WorkerDTO>> Login(LoginDTO loginDTO);
    }
}
