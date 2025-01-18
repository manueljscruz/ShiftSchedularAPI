using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Foreign Key
        /// Gender Identifier
        /// </summary>
        [Required]
        [Range(1,3)]
        public int GenderId { get; set; }

        /// <summary>
        /// Name of the user
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Refresh Token Expiry Time
        /// </summary>
        public DateTime RefreshTokenExpiryTime { get; set; }

        /// <summary>
        /// Navigation Property
        /// </summary>
        public virtual Gender Gender { get; set; }
        public virtual ICollection<EntityWorker> EntityWorkers { get; set; }
        public virtual ICollection<EntityWorkerInvitation> EntityWorkerInvitations { get; set; }
        public virtual ICollection<ScheduleEntryWorkers> ScheduleEntryWorkers { get; set; }
        public virtual ICollection<EntityWorkerAbsence> EntityWorkerAbsences { get; set; }
    }
}
