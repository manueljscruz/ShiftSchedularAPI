using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;

namespace ShiftSchedularDAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DataContext _context;
        private readonly DbSet<T> _dbSet;
        private readonly IUnitOfWork _unitOfWork;

        public GenericRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _dbSet = context.Set<T>();
            _unitOfWork = unitOfWork;
        }

        public async Task<T> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> GetById(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> GetById(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> Add(T entity)
        {
            await _dbSet.AddAsync(entity);


            await _unitOfWork.SaveChangesAsync();


            return entity;
        }


        public async Task<List<T>> AddRange(List<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _unitOfWork.SaveChangesAsync();
            return entities;
        }

        public async Task Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;


            await _unitOfWork.SaveChangesAsync();

        }

        public async Task UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            T entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);

                await _unitOfWork.SaveChangesAsync();

            }
        }

        public async Task Delete(string id)
        {
            T entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteRange(IEnumerable<T> entities)
        {
            if (entities.Count() != 0)
            {
                _dbSet.RemoveRange(entities);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task Delete(Guid id)
        {
            T entity = await _dbSet.FindAsync(id);
            if(entity != null)
            {
                _dbSet.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
