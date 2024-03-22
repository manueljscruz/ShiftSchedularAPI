using ShiftSchedularEntity.Entities;

namespace ShiftSchedularEntity.Models.Responses
{
    public class GenderLocalizedModel
    {
        public int GenderId { get; private set; }
        public string GenderLocalizedName { get; private set; }

        public GenderLocalizedModel(GenderLocalization genderLocalization)
        {
            GenderId = genderLocalization.GenderId;
            GenderLocalizedName = genderLocalization.GenderDisplayValue;
        }
    }
}
