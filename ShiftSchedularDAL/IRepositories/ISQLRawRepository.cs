namespace ShiftSchedularDAL.IRepositories
{
    public interface ISQLRawRepository<T> where T : class
    {
        Task<IEnumerable<T>> ExecuteQuery<T>(string query, Dictionary<string, object> parameters);
    }
}
