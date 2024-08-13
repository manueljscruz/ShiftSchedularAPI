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
            StartDateSearch = DateTime.Now.AddMonths(-3);
            EndDateSearch = DateTime.Now.AddMonths(3);
        }

        #endregion
    }
}
