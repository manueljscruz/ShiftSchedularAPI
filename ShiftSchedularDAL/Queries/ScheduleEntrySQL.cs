namespace ShiftSchedularDAL.Queries
{
    public class ScheduleEntrySQL
    {
        public static readonly string GetWorkerScheduleEntries = @"
			SELECT
				se.ScheduleEntryId,
				se.ShiftId,
				se.ScheduleStartDate,
				se.ScheduleEndDate
			FROM ScheduleEntry se
			LEFT JOIN ScheduleEntryWorkers sew ON se.ScheduleEntryId = sew.ScheduleEntryId
			LEFT JOIN Shifts sh ON se.ShiftId = sh.ShiftId
			LEFT JOIN Entities et ON et.EntityId = sh.EntityId
			WHERE
				et.EntityId = @EntityId
				AND se.ScheduleStartDate >= @StartDateSearch
				AND se.ScheduleEndDate <= @EndDateSearch
				AND (@WorkerId IS NULL OR sew.ApplicationUserId = @WorkerId)
        ";
    }
}
