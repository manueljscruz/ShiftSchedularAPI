using ShiftSchedularEntity.Models.DataAnnotationModels;
using ShiftSchedularRL.Resources.Dashboard;
using ShiftSchedularRL.Resources.Home;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class EntityProfileViewModelRequestDTO
    {
        #region Properties

        [RequiredWithResourceMessageAttribute(typeof(EntitiesRelatedMessages), "EntityNoIdentifierError")]
        public string EntityId { get; set; }

        [RequiredWithResourceMessageAttribute(typeof(WorkerRelatedMessages), "WorkerIdentifierIsEmpty")]
        public string WorkerId { get; set; }

        #endregion

        #region Constructor

        public EntityProfileViewModelRequestDTO()
        {
            EntityId = string.Empty;
            WorkerId = string.Empty;
        }

        #endregion
    }
}
