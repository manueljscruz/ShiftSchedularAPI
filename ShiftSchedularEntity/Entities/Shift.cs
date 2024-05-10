namespace ShiftSchedularEntity.Entities
{
    public class Shift
    {
        #region Properties

        /// <summary>
        /// Primary Key
        /// </summary>
        public string ShiftId { get; set; }

        /// <summary>
        /// Foreign Key
        /// </summary>
        public string EntityId { get; set; }
        public string ShiftName { get; set; }
        public string ShiftAlias { get; set; }
        public string ShiftDescription { get; set; }
        public TimeSpan ShiftStartHour { get; set; }
        public TimeSpan ShiftDuration { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Entity Entity { get; set; }
        public virtual IEnumerable<ShiftBreak> ShiftBreaks { get; set; }

        #endregion
    }
}
