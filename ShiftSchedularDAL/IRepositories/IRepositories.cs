namespace ShiftSchedularDAL.IRepositories
{
    public interface IGenericRepository<T>
    {
        Task<T> GetById(int id);
        Task<T> GetById(long id);
        Task<IEnumerable<T>> GetAll();
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
        Task Delete(long id);
        Task DeleteRange(IEnumerable<T> entities);
    }
}
