namespace ShiftSchedularDAL.Queries
{
    public class EntityWorkerSQL
    {
        public static readonly string GetDistinctEntityWorkersByEntityId = @"
            SELECT
                EW.ApplicationUserId AS WorkerId,
                W.DisplayName AS WorkerName,
                CAST(0 AS BIT) AS IsBot,
                EW.DateOfJoin,
                EW.PartOfRotation,
                EW.WorksWeekDays,
                EW.WorksWeekends,
                EW.MultipleShiftAssignments,
                ISNULL(SkillAgg.SkillIds, '') AS SkillIds,
                CASE WHEN EP.RoleId = 1 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsGeneralManager
            FROM [dbo].[EntityWorkers] EW
            LEFT JOIN [dbo].[AspNetUsers] W
                ON W.Id = EW.ApplicationUserId
            LEFT JOIN (
                SELECT
                    EWS.ApplicationUserId,
                    STRING_AGG(CAST(EWS.SkillId AS VARCHAR), ',') AS SkillIds
                FROM [dbo].[EntityWorkerSkills] EWS
                GROUP BY EWS.ApplicationUserId
            ) AS SkillAgg
                ON SkillAgg.ApplicationUserId = EW.ApplicationUserId
            LEFT JOIN [dbo].[EntityPermissions] EP
                ON EP.ApplicationUserId = EW.ApplicationUserId
                AND EP.EntityId = @EntityId
                AND EP.RoleId = 1
            WHERE EW.EntityId = @EntityId
            {0};
        ";

        public static readonly string GetDistinctUserBotsByEntityId = @"
            SELECT
                EUB.UserBotId AS WorkerId,
                UB.UserDisplayName AS WorkerName,
                CAST(1 AS BIT) AS IsBot,
                EUB.DateOfJoin,
                EUB.PartOfRotation,
                EUB.WorksWeekDays,
                EUB.WorksWeekends,
                EUB.MultipleShiftAssignments,
                ISNULL(SkillAgg.SkillIds, '') AS SkillIds,
                CAST(0 AS BIT) AS IsGeneralManager
            FROM [dbo].[EntityUserBots] EUB
            LEFT JOIN [dbo].[UserBots] UB
                ON UB.UserBotId = EUB.UserBotId
            LEFT JOIN (
                SELECT
                    EUBS.UserBotId,
                    STRING_AGG(CAST(EUBS.SkillId AS VARCHAR), ',') AS SkillIds
                FROM [dbo].[EntityUserBotSkills] EUBS
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
                [dbo].[EntityWorkers]
            WHERE EntityId = @EntityId
            GROUP BY 
                EntityId
        ";

        public static readonly string GetEntityUserBotsCount = @"
            SELECT
                COUNT(*) As BotsCount
            FROM
                [dbo].[EntityUserBots]
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
