using ShiftSchedularEntity.Models.DataAnnotationModels;
using ShiftSchedularRL.Resources.Home;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class NewUserDTO
    {
        [RequiredWithResourceMessage(typeof(WorkerRelatedMessages), "WorkerRegistrationNameEmptyError")]
        [MaxLengthWithResourceMessage(255, typeof(WorkerRelatedMessages), "WorkerNameExceedsLengthError")]
        public string Name { get; set; }

        [Required]
        [RangeWithResourceMessage(1,3, typeof(WorkerRelatedMessages), "WorkerGenderUnexpectedValueError")]
        public int GenderId { get; set; }

        [RequiredWithResourceMessageAttribute(typeof(WorkerRelatedMessages), "WorkerEmailEmptyError")]
        [MaxLengthWithResourceMessageAttribute(255, typeof(WorkerRelatedMessages), "WorkerEmailExceedsLengthError")]
        [RegexWithResourceMessage(@"^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,}$", typeof(WorkerRelatedMessages), "WorkerRegistrationEmailInvalidError")]
        public string Email { get; set; }

        [RequiredWithResourceMessage(typeof(WorkerRelatedMessages), "WorkerPasswordEmptyError")]
        [MaxLengthWithResourceMessageAttribute(128, typeof(WorkerRelatedMessages), "WorkerRegistrationPasswordExceedsLengthError")]
        public string Password { get; set; }
    }
}
