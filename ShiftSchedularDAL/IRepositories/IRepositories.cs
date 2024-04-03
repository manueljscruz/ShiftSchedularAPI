namespace ShiftSchedularDAL.IRepositories
{
    public interface IGenericRepository<T>
    {
        Task<T> GetById(int id);
        Task<T> GetById(string id);
        Task<IEnumerable<T>> GetAll();
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
        Task Delete(string id);
        Task DeleteRange(IEnumerable<T> entities);
    }
}
