namespace ShiftSchedularEntity.Models.APIManagement
{
    public class BusinessAspectLocalizationSubmissionModel : BaseLocalizationSubmissionModel
    {
        public int BusinessAspectId { get; set; }

        public BusinessAspectLocalizationSubmissionModel() : base()
        {
            BusinessAspectId = 0;
        }
    }
}
