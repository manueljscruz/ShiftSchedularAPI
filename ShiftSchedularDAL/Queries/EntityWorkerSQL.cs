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
                W.IsBot,
	            EW.CanCreateSchedules,
	            EW.IsOwner,
                EW.DateOfJoin,
	            STRING_AGG(CAST(EW.SkillId AS VARCHAR), ',') AS SkillIds
            FROM Workers W
	            LEFT JOIN EntityWorkers EW on W.WorkerId = EW.WorkerId
            WHERE
	            EW.EntityId = @EntityId
                {0}
            GROUP BY W.WorkerId, W.WorkerName, W.isBot, EW.CanCreateSchedules, EW.IsOwner, EW.DateofJoin
        ";

        public static readonly string GetDistinctEntityWorkersListFilter = @"
            AND W.WorkerId IN ({0})
        ";

        public static readonly string GetEntityWorkersCount = @"
            SELECT 
                COUNT(DISTINCT WorkerId) AS WorkerCount
            FROM 
                [ShiftSchedular].[dbo].[EntityWorkers]
            WHERE EntityId = @EntityId
            GROUP BY 
                EntityId
        ";

        public static readonly string GetDistinctEntitySkillsByEntityId = @"
            SELECT 
                DISTINCT SkillId
            FROM 
                EntityWorkers
            WHERE 
                EntityId = @EntityId";
    }
}
