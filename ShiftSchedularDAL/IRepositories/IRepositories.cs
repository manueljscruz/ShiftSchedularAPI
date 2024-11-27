namespace ShiftSchedularDAL.IRepositories
{
    public interface IGenericRepository<T>
    {
        Task<T> GetById(int id);
        Task<T> GetById(string id);
        Task<T> GetById(Guid id);
        Task<IEnumerable<T>> GetAll();
        Task<T> Add(T entity);
        Task<List<T>> AddRange(List<T> entities);
        Task Update(T entity);
        Task Delete(int id);
        Task Delete(string id);
        Task Delete(Guid id);
        Task DeleteRange(IEnumerable<T> entities);
    }
}
