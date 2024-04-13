namespace ShiftSchedularEntity.Models.API_Management
{
    public class EntityTypeLocalizationSubmissionModel
    {
        #region Properties

        public int EntityTypeId { get; set; }

        public int LanguageID { get; set; }

        public string EntityTypeDisplayValue { get; set; }

        #endregion

        #region Constructors

        public EntityTypeLocalizationSubmissionModel()
        {
            EntityTypeId = 0;
            LanguageID = 0;
            EntityTypeDisplayValue = string.Empty;
        }

        public EntityTypeLocalizationSubmissionModel(int entityTypeId, int languageId, string entityTypeDisplayValue)
        {
            EntityTypeId = entityTypeId;
            LanguageID = languageId;
            EntityTypeDisplayValue = entityTypeDisplayValue;
        }

        #endregion
    }
}
