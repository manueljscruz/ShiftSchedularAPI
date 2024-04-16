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
                et.EntityName 
            FROM Entities et
            LEFT JOIN EntityWorkers etw on et.EntityId = etw.EntityId
            WHERE 
                etw.ActiveWorkerStatus = 1
                AND etw.WorkerId = @WorkerId
            GROUP BY et.EntityId, et.EntityName"
            ;

        public static readonly string GetDistinctEntityWorkersByEntityId = @"
            SELECT
	            W.WorkerId,
	            W.WorkerName,
	            EW.CanCreateSchedules,
	            EW.IsOwner,
	            STRING_AGG(CAST(EW.SkillId AS VARCHAR), ',') AS SkillIds
            FROM Workers W
	            LEFT JOIN EntityWorkers EW on W.WorkerId = EW.WorkerId
            WHERE
	            EW.EntityId = @EntityId
            GROUP BY W.WorkerId, W.WorkerName, EW.CanCreateSchedules, EW.IsOwner
        ";
    }
}
