using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.IService
{
    public interface IUserService
    {
        Task<BaseResponse<bool>> Register(NewUserDTO newUserDTO);
        Task<BaseResponse<LoginResponseDTO>> Login(LoginDTO loginDTO);
        Task<BaseResponse<TokenModelDTO>> RefreshToken(TokenModelDTO tokenModelDTO);
        Task<BaseResponse<bool>> ConfirmEmail(string email, string token);
        Task<BaseResponse<bool>> UpdateUser();
    }
}
