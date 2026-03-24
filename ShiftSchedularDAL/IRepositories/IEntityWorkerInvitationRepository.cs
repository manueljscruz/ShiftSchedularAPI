using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityWorkerInvitationRepository
    {
        /// <summary>
        /// Gets the entity worker invitation by composite key.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="email"></param>
        /// <param name="workerId"></param>
        /// <returns></returns>
        Task<EntityWorkerInvitation> GetByCompositeKey(Guid entityId, string email, string workerId = null);
        
        /// <summary>
        /// Adds a new Entity Worker Invitation
        /// </summary>
        /// <param name="entityWorkerInvitation"></param>
        /// <returns></returns>
        Task<EntityWorkerInvitation> Add(EntityWorkerInvitation entityWorkerInvitation);

        /// <summary>
        /// Gets all entity worker Invitations by the worker id
        /// Will be used for the user to see which worker entities have invited him
        /// </summary>
        /// <param name="workerId"></param>
        /// <returns></returns>
        Task<IEnumerable<EntityWorkerInvitation>> GetAllByWorker(string workerId);

        /// <summary>
        /// Delete Entity Worker Invitation
        /// Used when accepting or refusing invitations by the user
        /// </summary>
        /// <param name="entityWorkerInvitation"></param>
        /// <returns></returns>
        Task Delete(EntityWorkerInvitation entityWorkerInvitation);

        /// <summary>
        /// Delete Entity Worker Invitation by composite key (EntityId + Email)
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Task DeleteByCompositeKey(Guid entityId, string email);

        /// <summary>
        /// Deletes all invitation given by an entity
        /// Used when the entity has chosen to be erased
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task DeleteAllByEntity(Guid entityId);

        /// <summary>
        /// Gets an invitation by entity and worker (ApplicationUserId)
        /// Used for accept/decline operations when the invited user is already registered
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="workerId"></param>
        /// <returns></returns>
        Task<EntityWorkerInvitation?> GetByEntityAndWorker(Guid entityId, string workerId);

        /// <summary>
        /// Returns true if there is already a pending (non-deleted) invitation for this entity/email pair.
        /// </summary>
        Task<bool> HasPendingInvitation(Guid entityId, string email);

        /// <summary>
        /// Finds an invitation by (EntityId, Email) regardless of IsDeleted status.
        /// Used to detect soft-deleted rows before re-inviting.
        /// </summary>
        Task<EntityWorkerInvitation?> FindByEntityAndEmail(Guid entityId, string email);
    }
}
