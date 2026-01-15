namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class ScheduleViewModelRequestDTO : BaseViewModelRequest
    {
        #region Properties

        public DateTime StartDateSearch { get; set; }
        public DateTime EndDateSearch { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with default 6 month interval
        /// </summary>
        public ScheduleViewModelRequestDTO()
        {
            StartDateSearch = DateTime.UtcNow.AddMonths(-3);
            EndDateSearch = DateTime.UtcNow.AddMonths(3);
        }

        public ScheduleViewModelRequestDTO(Guid entityId, string workerId, string languageCode, DateTime startDateSearch, DateTime endDateSearch)
        {
            EntityId = entityId;
            WorkerId = workerId;
            LanguageCode = languageCode;
            StartDateSearch = startDateSearch;
            EndDateSearch = endDateSearch;
        }

        #endregion
    }
}
