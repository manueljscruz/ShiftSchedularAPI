using System.Data;

namespace ShiftSchedularDAL.IRepositories
{
    public interface ISQLRawRepository<T> where T : class
    {
        Task<T> ExecuteScalar<T>(string query, Dictionary<string, object> parameters);
        Task<DataTable> GetDataTableAsync(string query, Dictionary<string, object> parameters);
        Task<IEnumerable<T>> ExecuteQuery<T>(string query, Dictionary<string, object> parameters);
    }
}
