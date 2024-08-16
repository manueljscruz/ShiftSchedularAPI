using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.Service
{
    public class ScheduleGeneratorService : IScheduleGeneratorService
    {
        private readonly IGenericRepository<BusinessAspect> _businessAspectRepository;

        #region Constructor

        public ScheduleGeneratorService(IGenericRepository<BusinessAspect> businessAspectRepository)
        {
            _businessAspectRepository = businessAspectRepository;
        }

        #endregion

        #region Methods

        public async Task<List<ScheduleEntryDTO>> FillOutSchedule(List<ScheduleEntryDTO> scheduleEntryDTOs, List<ShiftDTO> shifts, List<EntityRuleDTO> entityRules, List<EntityWorkerMemberDTO> entityWorkerMemberDTOs)
        {
            IEnumerable<BusinessAspect> businessAspects = await _businessAspectRepository.GetAll();

            foreach (ScheduleEntryDTO scheduleEntryDTO in scheduleEntryDTOs)
            {
                ShiftDTO shift = shifts.Where(i => i.ShiftId.Equals(scheduleEntryDTO.ShiftId)).FirstOrDefault();


            }

            return scheduleEntryDTOs;
        }

        #endregion
    }
}
