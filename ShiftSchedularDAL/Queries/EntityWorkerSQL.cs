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
                etw.IsOwner
            FROM Entities et
            LEFT JOIN EntityWorkers etw on et.EntityId = etw.EntityId
            WHERE 
                etw.ActiveWorkerStatus = 1
                AND etw.WorkerId = @WorkerId
            GROUP BY et.EntityId, et.EntityName, etw.IsOwner"
            ;

        public static readonly string GetDistinctEntityWorkersByEntityId = @"
            SELECT
	            W.WorkerId,
	            W.WorkerName,
	            EW.CanCreateSchedules,
	            EW.IsOwner,
                EW.DateOfJoin,
	            STRING_AGG(CAST(EW.SkillId AS VARCHAR), ',') AS SkillIds
            FROM Workers W
	            LEFT JOIN EntityWorkers EW on W.WorkerId = EW.WorkerId
            WHERE
	            EW.EntityId = @EntityId
            GROUP BY W.WorkerId, W.WorkerName, EW.CanCreateSchedules, EW.IsOwner, EW.DateofJoin
        ";

        public static readonly string GetEntityWorkersCount = @"
            SELECT 
                DISTINCT COUNT(*)
            FROM EntityWorkers
            WHERE 
                EntityId = @EntityId
            GROUP BY EntityId, WorkerId, ActiveWorkerStatus, CanCreateSchedules, IsOwner, SkillId;
        ";
    }
}
