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
        /// Gets all direct child entities of a given parent entity
        /// </summary>
        /// <param name="parentEntityId">The identifier of the parent entity</param>
        /// <param name="languageCode">Language code for EntityType localization</param>
        /// <returns>List of child entities with EntityType localization loaded</returns>
        public async Task<List<Entity>> GetChildEntities(Guid parentEntityId, string languageCode)
        {
            Localization localization = await _unitOfWork.LocalizationRepository.GetLocalizationByLanguageCode(languageCode);

            if (localization == null || parentEntityId == Guid.Empty)
                return new List<Entity>();

            return await _entityDbSet
                .Include(e => e.EntityType)
                    .ThenInclude(et => et.EntityTypeLocalizations
                        .Where(etl => etl.LocalizationId == localization.LocalizationId))
                .Where(e => e.ParentEntityId == parentEntityId)
                .ToListAsync();
        }

        /// <summary>
        /// Returns all ancestors of the given entity ordered root-first down to the direct parent.
        /// The entity itself is NOT included. Returns an empty list for root entities.
        /// Uses iterative application-level traversal — suitable for shallow hierarchies (max 3-5 levels).
        /// </summary>
        public async Task<List<Entity>> GetAncestorChain(Guid entityId)
        {
            List<Entity> ancestors = new List<Entity>();

            if (entityId == Guid.Empty)
                return ancestors;

            Entity current = await _entityDbSet.AsNoTracking()
                .Select(e => new Entity { EntityId = e.EntityId, EntityName = e.EntityName, ParentEntityId = e.ParentEntityId })
                .FirstOrDefaultAsync(e => e.EntityId == entityId);

            Guid? parentId = current?.ParentEntityId;
            while (parentId != null && parentId != Guid.Empty)
            {
                Entity parent = await _entityDbSet.AsNoTracking()
                    .Select(e => new Entity { EntityId = e.EntityId, EntityName = e.EntityName, ParentEntityId = e.ParentEntityId })
                    .FirstOrDefaultAsync(e => e.EntityId == parentId);

                if (parent == null) break;
                ancestors.Add(parent);
                parentId = parent.ParentEntityId;
            }

            ancestors.Reverse();
            return ancestors;
        }

        /// <summary>
        /// Batch fetches a set of entities by their IDs.
        /// Returns only EntityId, EntityName, and ParentEntityId — no navigation properties loaded.
        /// </summary>
        public async Task<List<Entity>> GetEntitiesByIds(List<Guid> entityIds)
        {
            if (entityIds == null || entityIds.Count == 0)
                return new List<Entity>();

            return await _entityDbSet.AsNoTracking()
                .Where(e => entityIds.Contains(e.EntityId))
                .Select(e => new Entity { EntityId = e.EntityId, EntityName = e.EntityName, ParentEntityId = e.ParentEntityId })
                .ToListAsync();
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
