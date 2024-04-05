using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularDAL.Queries
{
    public class EntityWorkerSQL
    {
        public static readonly string GetEntityWorkersByWorkerId = @"
            SELECT 
                et.EntityId,
                et.EntityName, 
                et.EntityDescription,
                etw.ActiveWorkerStatus,
                etw.CanCreateSchedules,
                etw.IsOwner 
            FROM Entities et
            LEFT JOIN EntityWorkers etw on et.EntityId = etw.EntityId
            WHERE etw.WorkerId = @WorkerId";
    }
}
