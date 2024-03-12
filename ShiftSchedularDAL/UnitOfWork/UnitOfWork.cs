using Microsoft.EntityFrameworkCore.Storage;
using ShiftSchedularDAL.Data;

namespace ShiftSchedularDAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private IDbContextTransaction _transaction;
        public bool activeTransaction;

        #region Constructor

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods

        public bool ReturnTransactionStatus()
        {
            return activeTransaction;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            activeTransaction = true;
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            activeTransaction = false;
        }



        #endregion
    }
}
