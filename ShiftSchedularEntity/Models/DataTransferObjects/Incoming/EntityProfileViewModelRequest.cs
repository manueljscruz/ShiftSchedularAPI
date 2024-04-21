namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class EntityProfileViewModelRequest
    {
        #region Properties

        public string EntityId { get; set; }
        public string WorkerId { get; set; }
        public string LanguageCode { get; set; }

        #endregion

        #region Constructor

        public EntityProfileViewModelRequest()
        {
            EntityId = string.Empty;
            WorkerId = string.Empty;
            LanguageCode = string.Empty;
        }

        #endregion
    }
}
