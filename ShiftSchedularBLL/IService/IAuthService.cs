using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.IService
{
    public interface IAuthService
    {
        Task<BaseResponse<LoginResponseDTO>> Login(LoginDTO loginDTO);
        Task<BaseResponse<TokenModelDTO>> RefreshToken(TokenModelDTO tokenModelDTO);
    }
}
