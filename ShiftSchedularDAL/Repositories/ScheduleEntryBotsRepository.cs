using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularDAL.Repositories
{
    public class ScheduleEntryBotsRepository : GenericRepository<ScheduleEntryBots>, IScheduleEntryBotsRepository
    {
        public ScheduleEntryBotsRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}
