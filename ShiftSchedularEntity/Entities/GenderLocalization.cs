namespace ShiftSchedularEntity.Entities
{
    public class GenderLocalization
    {
        #region Properties

        public int GenderId { get; set; }
        public int LocalizationId { get; set; }
        public string GenderDisplayValue { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Gender Gender { get; set; }
        public virtual Localization Localization { get; set; }

        #endregion

        #region Constructor

        public GenderLocalization()
        {
            GenderId = 0;
            LocalizationId = 0;
            GenderDisplayValue = string.Empty;

            Gender = new Gender();
            Localization = new Localization();
        }

        #endregion

    }
}
