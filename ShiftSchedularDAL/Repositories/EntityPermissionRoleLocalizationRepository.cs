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
    public class EntityPermissionRoleLocalizationRepository : GenericRepository<EntityPermissionRoleLocalization>, IEntityPermissionRoleLocalizationRepository
    {
        private readonly DataContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<EntityPermissionRoleLocalization> _entityPermissionRoleDbSet;

        public EntityPermissionRoleLocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _entityPermissionRoleDbSet = _context.Set<EntityPermissionRoleLocalization>();
        }
    }
}
