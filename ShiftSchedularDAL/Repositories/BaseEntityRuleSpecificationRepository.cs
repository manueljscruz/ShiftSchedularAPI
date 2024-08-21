using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularDAL.Repositories
{
    public class BaseEntityRuleSpecificationRepository : IBaseEntityRuleSpecificationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<BaseEntityRuleSpecification> _baseEntityRuleSpecificationDbSet;
        private readonly IUnitOfWork _unitOfWork;

        public BaseEntityRuleSpecificationRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _baseEntityRuleSpecificationDbSet = _context.Set<BaseEntityRuleSpecification>();
        }

        public async Task<BaseEntityRuleSpecification> AddBaseEntityRuleSpecification(BaseEntityRuleSpecification baseEntityRuleSpecification)
        {
            if (baseEntityRuleSpecification != null)
            {
                int maxSpecId = await _baseEntityRuleSpecificationDbSet.Where(i => i.BaseEntityRuleId.Equals(baseEntityRuleSpecification.BaseEntityRuleId)).MaxAsync(i => (int?)i.SpecificationId) ?? 0;
                baseEntityRuleSpecification.SpecificationId = maxSpecId + 1;
                await _baseEntityRuleSpecificationDbSet.AddAsync(baseEntityRuleSpecification);
                await _unitOfWork.SaveChangesAsync();
                return baseEntityRuleSpecification;
            }
            else
                return null;
        }

        public async Task<bool> DeleteBaseEntityRuleSpecification(int baseRuleId, int specId)
        {
            bool result = false;

            if(baseRuleId != 0 && specId != 0)
            {
                BaseEntityRuleSpecification baseEntityRuleSpecification = await _baseEntityRuleSpecificationDbSet.Where(i => i.BaseEntityRuleId.Equals(baseRuleId) && i.SpecificationId.Equals(specId)).FirstOrDefaultAsync();
                if(baseEntityRuleSpecification != null)
                {
                    _baseEntityRuleSpecificationDbSet.Remove(baseEntityRuleSpecification);
                    await _unitOfWork.SaveChangesAsync();
                    result = true;
                }
            }

            return result;
        }

        public async Task<List<BaseEntityRuleSpecification>> GetRuleSpecificationsById(int ruleSpecId)
        {
            if(ruleSpecId != 0)
            {
                List<BaseEntityRuleSpecification> baseEntityRuleSpecifications = new List<BaseEntityRuleSpecification>();
                baseEntityRuleSpecifications = _baseEntityRuleSpecificationDbSet.Where(i => i.BaseEntityRuleId.Equals(ruleSpecId)).ToList();
                return baseEntityRuleSpecifications;
            }
            return null;
        }


        public async Task<bool> DeleteAllBaseEntityRuleSpecificationsById(int baseRuleId)
        {
            bool result = false;

            if(baseRuleId != 0)
            {
                IEnumerable<BaseEntityRuleSpecification> baseEntityRuleSpecifications = _baseEntityRuleSpecificationDbSet.Where(i => i.BaseEntityRuleId.Equals(baseRuleId));
                _baseEntityRuleSpecificationDbSet.RemoveRange(baseEntityRuleSpecifications);
                await _unitOfWork.SaveChangesAsync();
                result = true;
            }

            return result;
        }

        public async Task<bool> UpdateBaseEntityRuleSpecification(BaseEntityRuleSpecification baseEntityRuleSpecification)
        {
            bool result = false;

            if(baseEntityRuleSpecification != null)
            {
                _baseEntityRuleSpecificationDbSet.Update(baseEntityRuleSpecification);
                await _unitOfWork.SaveChangesAsync();
                result = true;
            }

            return result;
        }
    }
}
