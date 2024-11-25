using ShiftSchedularEntity.Models.DataAnnotationModels;
using ShiftSchedularRL.Resources.Home;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class NewWorkerDTO
    {
        /// <summary>
        /// Worker name input
        /// Cannot be empty or longer than 50 characters
        /// </summary>
        [RequiredWithResourceMessageAttribute(typeof(WorkerRelatedMessages), "WorkerRegistrationNameEmptyError")]
        [MaxLengthWithResourceMessageAttribute(50, typeof(WorkerRelatedMessages), "WorkerNameExceedsLengthError")]
        public string WorkerName { get; set; }

        /// <summary>
        /// Gender identifier
        /// Value can only be between 1 and 3
        /// </summary>
        [Required]
        [RangeWithResourceMessage(1,3, typeof(WorkerRelatedMessages), "WorkerGenderUnexpectedValueError")]
        public int GenderId { get; set; }

        /// <summary>
        /// Worker Email input
        /// Cannot be empty, longer than 100 characteres
        /// </summary>
        [RequiredWithResourceMessageAttribute(typeof(WorkerRelatedMessages), "WorkerEmailEmptyError")]
        [MaxLengthWithResourceMessageAttribute(255, typeof(WorkerRelatedMessages), "WorkerEmailExceedsLengthError")]
        [RegexWithResourceMessage(@"^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,}$", typeof(WorkerRelatedMessages), "WorkerRegistrationEmailInvalidError")]
        public string Email { get; set; }

        /// <summary>
        /// Password input
        /// Cannot be empty or longer than 128 characters
        /// </summary>
        [RequiredWithResourceMessage(typeof(WorkerRelatedMessages), "WorkerPasswordEmptyError")]
        [MaxLengthWithResourceMessageAttribute(128, typeof(WorkerRelatedMessages), "WorkerRegistrationPasswordExceedsLengthError")]
        public string Password { get; set; }
    }
}
