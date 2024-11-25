using ShiftSchedularEntity.Models.DataAnnotationModels;
using ShiftSchedularRL.Resources.Home;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class LoginDTO
    {
        [RequiredWithResourceMessageAttribute(typeof(WorkerRelatedMessages), "WorkerEmailEmptyError")]
        [MaxLengthWithResourceMessageAttribute(255, typeof(WorkerRelatedMessages), "WorkerEmailExceedsLengthError")]
        [RegexWithResourceMessage(@"^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,}$", typeof(WorkerRelatedMessages), "WorkerRegistrationEmailInvalidError")]
        public string Email { get; set; }

        [RequiredWithResourceMessage(typeof(WorkerRelatedMessages), "WorkerPasswordEmptyError")]
        [MaxLengthWithResourceMessageAttribute(128, typeof(WorkerRelatedMessages), "WorkerRegistrationPasswordExceedsLengthError")]
        public string Password { get; set; }
    }
}
