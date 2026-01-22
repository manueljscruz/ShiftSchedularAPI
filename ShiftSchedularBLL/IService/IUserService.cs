using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;

namespace ShiftSchedularBLL.IService
{
    public interface IUserService
    {
        Task<BaseResponse<bool>> CreateUser(NewUserDTO newUser, string originLink);
        Task<BaseResponse<bool>> UpdateUser(UserDTO workerDTO, string frontendUrl);
        Task<BaseResponse<bool>> ConfirmEmail(ConfirmEmailRequestDTO request);

    }
}
