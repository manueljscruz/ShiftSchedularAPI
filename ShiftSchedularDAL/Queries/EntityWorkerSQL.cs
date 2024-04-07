namespace ShiftSchedularDAL.Queries
{
    public class EntityWorkerSQL
    {
        /// <summary>
        /// Gets related records between the entity and worker tables
        /// Where the worker is active
        /// Currently being used to populate the navbar
        /// </summary>
        public static readonly string GetEntityWorkersByWorkerId = @"
            SELECT 
                et.EntityId,
                et.EntityName, 
                et.EntityDescription,
                etw.CanCreateSchedules,
                etw.IsOwner 
            FROM Entities et
            LEFT JOIN EntityWorkers etw on et.EntityId = etw.EntityId
            WHERE 
                etw.ActiveWorkerStatus = 1
                AND etw.WorkerId = @WorkerId";
    }
}
