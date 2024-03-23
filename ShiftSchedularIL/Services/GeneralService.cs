using ShiftSchedularIL.IServices;

namespace ShiftSchedularIL.Services
{
    public class GeneralService : IGeneralService
    {
        public string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
