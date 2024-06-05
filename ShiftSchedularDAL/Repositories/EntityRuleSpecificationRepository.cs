using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    internal class EntityRuleSpecificationRepository : IEntityRuleSpecificationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityRuleSpecification> _entityRuleSpecificationsDbSet;
        private readonly IUnitOfWork _unitOfWork;


        public EntityRuleSpecificationRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _entityRuleSpecificationsDbSet = _context.Set<EntityRuleSpecification>();
        }

        #region Add Entity Rule Specification

        public async Task<EntityRuleSpecification> AddEntityRuleSpecification(EntityRuleSpecification entityRuleSpecification)
        {
            if (entityRuleSpecification != null)
            {
                int maxSpecId = await _entityRuleSpecificationsDbSet.Where(i => i.EntityRuleId.Equals(entityRuleSpecification)).MaxAsync(i => (int?)i.SpecificationId) ?? 0;
                entityRuleSpecification.SpecificationId = maxSpecId + 1;
                await _entityRuleSpecificationsDbSet.AddAsync(entityRuleSpecification);
                await _unitOfWork.SaveChangesAsync();
                return entityRuleSpecification;

            }
            else return null;
        }

        #endregion

        #region Delete Entity Rule Specification

        public async Task<bool> DeleteEntityRuleSpecification(string entityRuleId, int specId)
        {
            bool result = false;
            if(!string.IsNullOrEmpty(entityRuleId) && specId != 0)
            {
                EntityRuleSpecification entityRuleSpecification = await _entityRuleSpecificationsDbSet.Where(i=>i.EntityRuleId.Equals(entityRuleId) && i.SpecificationId.Equals(specId)).FirstOrDefaultAsync();
                if(entityRuleSpecification != null)
                {
                    _entityRuleSpecificationsDbSet.Remove(entityRuleSpecification);
                    await _unitOfWork.SaveChangesAsync();
                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region Delete Entity Rule Specifications By Rule Id

        public async Task<bool> DeleteEntityRuleSpecificationsByRuleId(string entityRuleId)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(entityRuleId))
            {
                IEnumerable<EntityRuleSpecification> entityRuleSpecifications = await _entityRuleSpecificationsDbSet.Where(i => i.EntityRuleId.Equals(entityRuleId)).ToListAsync();
                if (entityRuleSpecifications != null)
                {
                    _entityRuleSpecificationsDbSet.RemoveRange(entityRuleSpecifications);
                    await _unitOfWork.SaveChangesAsync();
                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region Get Entity Rule Specification

        public async Task<EntityRuleSpecification> GetEntityRuleSpecification(string entityRuleId, int specificationId)
        {
            if (!string.IsNullOrEmpty(entityRuleId) && specificationId != 0)
            {
                return await _entityRuleSpecificationsDbSet.Where(i => i.EntityRuleId.Equals(entityRuleId) && i.SpecificationId.Equals(specificationId)).FirstOrDefaultAsync();
            }
            else return null;
        }

        #endregion

        #region Get Entity Rule Specifications

        public async Task<IEnumerable<EntityRuleSpecification>> GetEntityRuleSpecifications(string entityRuleId)
        {
            if (!string.IsNullOrEmpty(entityRuleId))
            {
                return await _entityRuleSpecificationsDbSet.Where(i => i.EntityRuleId.Equals(entityRuleId)).ToListAsync();
            }
            else return null;
        }

        #endregion

        #region Update Entity Rule Specification

        public async Task<bool> UpdateEntityRuleSpecification(EntityRuleSpecification entityRuleSpecification)
        {
            bool result = false;
            if(entityRuleSpecification != null)
            {
                _entityRuleSpecificationsDbSet.Update(entityRuleSpecification);
                await _unitOfWork.SaveChangesAsync();
                result = true;
            }

            return result;
        }

        #endregion
    }
}
