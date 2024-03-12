namespace ShiftSchedularDAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        bool ReturnTransactionStatus();
        Task BeginTransactionAsync();
        Task SaveChangesAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
