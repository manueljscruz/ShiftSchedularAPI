using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;

namespace ShiftSchedularBLL.IService
{
    public interface IUserService
    {
        Task<BaseResponse<bool>> CreateUser(NewUserDTO newUser);
        Task<BaseResponse<bool>> UpdateUser(UserDTO workerDTO);
        Task<BaseResponse<bool>> ConfirmEmail(string email, string token);
         
    }
}
