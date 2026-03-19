using ShiftSchedularEntity.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularEntity.Entities
{
    public class EntityPermission : BaseEntity
    {
        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public int EntityPermissionRoleId { get; set; }

        public bool CanManageChildren { get; set; }

        public bool PartOfRoster { get; set; }

        #region Navigation Properties

        public virtual Entity Entity { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual EntityPermissionRole EntityPermissionRole { get; set; }

        #endregion
    }
}
