using ShiftSchedularEntity.Models.Responses;

namespace ShiftSchedularEntity.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<GenderLocalizedDTO> Genders { get; set; }
        // Add SubscriptionPlans Here

        public HomeViewModel()
        {
        }
    }
}
