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
    public class EntityRepository : GenericRepository<Entity>, IEntityRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<Entity> _entityDbSet;
        private readonly IUnitOfWork _unitOfWork;

        public EntityRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _entityDbSet = _context.Set<Entity>();
        }

        public async Task<Entity> GetEntityById(Guid entityId, string languageCode)
        {
            Localization localization = await _unitOfWork.LocalizationRepository.GetLocalizationByLanguageCode(languageCode);

            if (localization == null)
                return null;

            if (entityId != Guid.Empty)
            {
                return await _entityDbSet.Include(e => e.EntityType)
                    .ThenInclude(et => et.EntityTypeLocalizations
                        .Where(etl => etl.LocalizationId.Equals(localization.LocalizationId)))
                    .Include(e => e.ChildrenEntities)
                    .FirstOrDefaultAsync(e => e.EntityId.Equals(entityId));
            }
            else return null;
        }

        /// <summary>
        /// Searches entities by name, includes EntityType with filtered localization
        /// </summary>
        /// <param name="searchQuery">The search query (already normalized/lowercased)</param>
        /// <param name="localizationId">The localization ID for the TypeDisplay</param>
        /// <returns>List of matching entities with their EntityType loaded</returns>
        public async Task<List<Entity>> SearchEntitiesByName(string searchQuery, int localizationId)
        {
            return await _entityDbSet
                .Where(e => e.EntityName.ToLower().Contains(searchQuery))
                .Include(e => e.EntityType)
                    .ThenInclude(et => et.EntityTypeLocalizations
                        .Where(etl => etl.LocalizationId == localizationId))
                .ToListAsync();
        }
    }
}
