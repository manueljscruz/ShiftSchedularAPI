using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.IService
{
    public interface IHolidayService
    {
        Task<BaseResponse<EntityHolidayDTO>> AddEntityHoliday(AddEntityHolidayDTO addEntityHolidayDTO);
        Task<int> AddHolidayBehaviour(string strNewHolidayBehaviour);
        Task<int> AddHolidayCatalog(AddHolidayCatalogDTO addHolidayCatalogDTO);
        Task<int> AddHolidayType(string strNewHolidayType);
        Task<BaseResponse<bool>> DeleteEntityHoliday(DeleteEntityObjectDTO entityHolidayDTO);
        Task<BaseResponse<bool>> DeleteHolidayBehaviour(int id);
        Task<BaseResponse<bool>> DeleteHolidayCatalog(int id);
        Task<BaseResponse<bool>> DeleteHolidayType(int id);
        Task<EntityHolidayDTO> GetEntityHolidayById(Guid id);
        Task<List<EntityHolidayDTO>> GetEntityHolidaysByPeriod(Guid entityId, DateTime startDate, DateTime endDate);
        Task<PagedList<EntityHolidayDTO>> GetEntityHolidaysPagination(PagedModelRequest entityHolidaysPaginationRequest);
        Task<EntityHolidaysViewModel> GetEntityHolidaysViewModel(PagedModelRequest viewModelRequestDTO);
        Task<HolidayBehaviour> GetHolidayBehaviourById(int id);
        Task<HolidayCatalog> GetHolidayCatalogById(int id);
        Task<HolidayType> GetHolidayTypeById(int id);
        Task<BaseResponse<bool>> UpdateEntityHoliday(EntityHolidayDTO entityHolidayDTO);
        Task<BaseResponse<bool>> UpdateHolidayBehaviour(HolidayBehaviour holidayBehaviour);
        Task<BaseResponse<bool>> UpdateHolidayCatalog(HolidayCatalog holidayCatalog);
        Task<BaseResponse<bool>> UpdateHolidayType(HolidayType holidayType);
    }
}
