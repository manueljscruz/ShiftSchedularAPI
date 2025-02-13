using ShiftSchedularIL.IServices;
using System.Text.RegularExpressions;

namespace ShiftSchedularIL.Services
{
    public class GeneralService : IGeneralService
    {
        private const int BOT_GUID_SUBTRACTION_LENGTH = 10;
        private const string EMAIL = "@ssbot.com";

        #region Generate GUID

        public string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion

        #region Validate Regex Email

        public bool ValidateRegexEmail(string email)
        {
            // Regular expression pattern for email validation
            string pattern = @"^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,}$";

            // Create Regex object
            Regex regex = new Regex(pattern);

            // Use Regex.IsMatch method to check if the email matches the pattern
            return regex.IsMatch(email);
        }

        #endregion

        #region Generate Bot Email

        public string GenerateBotEmail(string workerGUID)
        {
            return workerGUID.Substring(workerGUID.Length - BOT_GUID_SUBTRACTION_LENGTH) + EMAIL;
        }

        #endregion

        #region Generate Bot Password

        public string GenerateBotPassword(string workerGUID)
        {
            return workerGUID.Substring(workerGUID.Length - BOT_GUID_SUBTRACTION_LENGTH);
        }

        public Guid ParseStringToGuid(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return new Guid(bytes);
        }

        #endregion
    }
}
