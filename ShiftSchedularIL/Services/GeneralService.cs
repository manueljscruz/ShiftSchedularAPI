using ShiftSchedularIL.IServices;
using System.Text.RegularExpressions;
using System.Web;

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
            if (string.IsNullOrWhiteSpace(input))
                return Guid.Empty;

            string guidString = HttpUtility.UrlDecode(input.Trim());

            // Fix '+' that got decoded as space
            guidString = guidString.Replace(" ", "+");

            // Try parsing as a regular GUID
            if (Guid.TryParse(guidString, out Guid parsedGuid))
                return parsedGuid;

            try
            {
                // Handle URL-safe Base64
                string base64 = guidString
                    .Replace('-', '+')
                    .Replace('_', '/');

                // Fix padding if needed
                switch (base64.Length % 4)
                {
                    case 2: base64 += "=="; break;
                    case 3: base64 += "="; break;
                }

                byte[] bytes = Convert.FromBase64String(base64);
                return new Guid(bytes);
            }
            catch
            {
                return Guid.Empty; // or throw if you prefer strict behavior
            }
        }


        public Guid _ParseStringToGuid(string input)
        {
            string guidString = HttpUtility.UrlDecode(input);
            Guid guid = Guid.Empty;

            if (guidString == string.Empty)
                return guid;

            bool isValidGuid = Guid.TryParse(guidString, out guid);
            if (isValidGuid)
                return guid;
            else
            {
                byte[] bytes = Convert.FromBase64String(guidString);
                guid = new Guid(bytes);
            }
            return guid;
        }

        #endregion
    }
}
