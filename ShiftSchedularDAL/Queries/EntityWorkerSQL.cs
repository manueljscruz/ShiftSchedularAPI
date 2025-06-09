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
                AND etw.ApplicationUserId = @WorkerId
            GROUP BY et.EntityId, et.EntityName, etw.IsOwner"
            ;

        
        public static readonly string GetDistinctEntityWorkersByEntityId = @"
            SELECT 
                EW.ApplicationUserId AS WorkerId,
                W.DisplayName AS WorkerName,
                EW.CanCreateSchedules,
                CAST(0 AS BIT) AS IsBot,
                EW.IsOwner,
                EW.DateOfJoin,
                EW.PartOfRotation,
                EW.WorksWeekDays,
                EW.WorksWeekends,
                EW.MultipleShiftAssignments,
                ISNULL(SkillAgg.SkillIds, '') AS SkillIds
            FROM [ShiftSchedular].[dbo].[EntityWorkers] EW
            LEFT JOIN [ShiftSchedular].[dbo].[AspNetUsers] W 
                ON W.Id = EW.ApplicationUserId
            LEFT JOIN (
                SELECT 
                    EWS.ApplicationUserId,
                    STRING_AGG(CAST(EWS.SkillId AS VARCHAR), ',') AS SkillIds
                FROM [ShiftSchedular].[dbo].[EntityWorkerSkills] EWS
                GROUP BY EWS.ApplicationUserId
            ) AS SkillAgg 
                ON SkillAgg.ApplicationUserId = EW.ApplicationUserId
            WHERE EW.EntityId = @EntityId
            {0};
        ";

        public static readonly string GetDistinctUserBotsByEntityId = @"
            SELECT 
                EUB.UserBotId AS WorkerId,
                UB.UserDisplayName AS WorkerName,
                CAST(0 AS BIT) AS CanCreateSchedules,
                CAST(1 AS BIT) AS IsBot,
                CAST(0 AS BIT) AS IsOwner,
                EUB.DateOfJoin,
                EUB.PartOfRotation,
                EUB.WorksWeekDays,
                EUB.WorksWeekends,
                EUB.MultipleShiftAssignments,
                ISNULL(SkillAgg.SkillIds, '') AS SkillIds
            FROM [ShiftSchedular].[dbo].[EntityUserBots] EUB
            LEFT JOIN [ShiftSchedular].[dbo].[UserBots] UB 
                ON UB.UserBotId = EUB.UserBotId
            LEFT JOIN (
                SELECT 
                    EUBS.UserBotId,
                    STRING_AGG(CAST(EUBS.SkillId AS VARCHAR), ',') AS SkillIds
                FROM [ShiftSchedular].[dbo].[EntityUserBotSkills] EUBS
                GROUP BY EUBS.UserBotId
            ) AS SkillAgg 
                ON SkillAgg.UserBotId = EUB.UserBotId
            WHERE EUB.EntityId = @EntityId
            {0};
        ";

        public static readonly string GetDistinctEntityWorkersListFilter = @"
            AND W.ApplicationUserId AS WorkerId IN ({0})
        ";

        public static readonly string GetEntityWorkersCount = @"
            SELECT 
                COUNT(*) AS WorkerCount
            FROM 
                [ShiftSchedular].[dbo].[EntityWorkers]
            WHERE EntityId = @EntityId
            GROUP BY 
                EntityId
        ";

        public static readonly string GetEntityUserBotsCount = @"
            SELECT
                COUNT(*) As BotsCount
            FROM
                [ShiftSchedular].[dbo].[EntityUserBots]
            WHERE EntityId = @EntityId
            GROUP BY
                EntityId
        ";

        public static readonly string GetDistinctEntitySkillsByEntityId = @"
            SELECT 
                DISTINCT SkillId
            FROM 
                EntityWorkerSkills
            WHERE 
                EntityId = @EntityId";

        public static readonly string GetDistinctUserBotsSkillsByEntityId = @"
            SELECT 
                DISTINCT SkillId
            FROM 
                EntityUserBotSkills
            WHERE 
                EntityId = @EntityId";
    }
}
