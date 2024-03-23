namespace ShiftSchedularIL.IServices
{
    public interface ICryptographyService
    {
        string HashPassword(string password);

        bool VerifyPassword(string password, string hashedPassword);

    }
}
