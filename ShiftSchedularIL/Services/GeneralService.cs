using ShiftSchedularIL.IServices;
using System.Text.RegularExpressions;

namespace ShiftSchedularIL.Services
{
    public class GeneralService : IGeneralService
    {
        public string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public bool ValidateRegexEmail(string email)
        {
            // Regular expression pattern for email validation
            string pattern = @"^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,}$";

            // Create Regex object
            Regex regex = new Regex(pattern);

            // Use Regex.IsMatch method to check if the email matches the pattern
            return regex.IsMatch(email);
        }
    }
}
