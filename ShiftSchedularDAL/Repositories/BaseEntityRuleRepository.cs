using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class BaseEntityRuleRepository : GenericRepository<BaseEntityRule>, IBaseEntityRuleRepository
    {
        private readonly DataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<BaseEntityRule> _dbSet;

        public BaseEntityRuleRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _dataContext = context;
            _unitOfWork = unitOfWork;
            _dbSet = _dataContext.Set<BaseEntityRule>();
        }

        public async Task<List<BaseEntityRule>> GetBaseEntityRules()
        {
            List<BaseEntityRule> baseEntityRules = new List<BaseEntityRule>();
            string strError = string.Empty;

            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(60)); // Set timeout to 60 seconds

                baseEntityRules = await _dbSet.ToListAsync();
            }
            catch (OperationCanceledException ex)
            {
                strError= "Operation was canceled.";
            }
            catch (TimeoutException ex)
            {
                strError = "The operation timed out.";
            }
            catch (Exception ex)
            {
                strError =  $"An unexpected error occurred: {ex.Message}";
            }

            return baseEntityRules;
        }
    }
}
