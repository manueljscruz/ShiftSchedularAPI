namespace ShiftSchedularIL.IServices
{
    public interface IGeneralService
    {
        string GenerateGuid();

        bool ValidateRegexEmail(string email);
    }
}
