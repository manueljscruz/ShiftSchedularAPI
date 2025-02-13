namespace ShiftSchedularIL.IServices
{
    public interface IGeneralService
    {
        string GenerateGuid();
        bool ValidateRegexEmail(string email);
        string GenerateBotEmail(string workerGUID);
        string GenerateBotPassword(string workerGUID);
        Guid ParseStringToGuid(string input);
    }
}
