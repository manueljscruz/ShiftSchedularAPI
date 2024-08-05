using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.IService
{
    public interface IEntityScheduleService
    {
        Task<EntityScheduleViewModel> GetEntityScheduleViewModel(BaseViewModelRequest viewModelRequest);
    }
}
