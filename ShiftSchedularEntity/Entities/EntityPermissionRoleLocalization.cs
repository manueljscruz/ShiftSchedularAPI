using System.ComponentModel.DataAnnotations;

namespace ShiftSchedularEntity.Entities
{
    public class EntityPermissionRoleLocalization
    {
        [Required]
        public int EntityPermissionRoleId { get; set; }

        [Required]
        public int LocalizationId { get; set; }

        [MaxLength(100)]
        public string EntityPermissionRoleDisplayValue { get; set; }

        #region Navigation Properties

        public virtual EntityPermissionRole EntityPermissionRole { get; set; }
        public virtual Localization Localization { get; set; }

        #endregion
    }
}
