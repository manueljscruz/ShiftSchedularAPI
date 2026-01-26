using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.IService
{
    public interface ISearchService
    {
        Task<BaseResponse<EntityPublicProfileDTO>> GetPublicEntityProfile(BaseViewModelRequest request);
        Task<BaseResponse<WorkerPublicProfileDTO>> GetPublicProfileWorker(string workerId, string languageCode);
        Task<BaseResponse<PagedList<SearchResultDTO>>> Search(SearchRequestDTO searchRequest);
    }
}
