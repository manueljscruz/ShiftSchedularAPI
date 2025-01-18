using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class UserBot
    {
        #region Properties

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public int UserBotId { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string UserDisplayName { get; set; }

        public DateTime DateOfCreation { get; set; }

        [Required]
        [Column(TypeName = "BINARY(16)")]
        public Guid EntityId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Entity Entity { get; set; }

        #endregion
    }
}
