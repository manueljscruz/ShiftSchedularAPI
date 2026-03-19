using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class EntityPermissionRole
    {
        [Key]
        [Required]
        public int EntityPermissionRoleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string EntityPermissionRoleName { get; set; }

        #region Navigation Properties

        public virtual ICollection<EntityPermission> EntityPermissions { get; set; }
        public virtual ICollection<EntityPermissionRoleLocalization> EntityPermissionRoleLocalizations { get; set; }

        #endregion
    }
}
