namespace ShiftSchedularEntity.Entities
{
    public class EntityTypeLocalization
    {
        #region Properties

        public int EntityTypeId { get; set; }
        public int LocalizationId { get; set; }
        public string EntityTypeDisplayValue { get; set; }

        #endregion

        #region Navigation Properties

        public virtual EntityType EntityType { get; set; }
        public virtual Localization Localization { get; set; }

        #endregion
    }
}
