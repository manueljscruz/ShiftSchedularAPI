using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class EntityType
    {
        #region Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EntityTypeId { get; set; }

        public string EntityTypeValue { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<Entity> Entities { get; set; }

        public virtual ICollection<EntityTypeLocalization> EntityTypeLocalizations { get; set; }

        #endregion
    }
}
