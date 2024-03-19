using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class Localization
    {
        #region Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LocalizationId { get; set; }
        public string LocalizationCode { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<GenderLocalization> GenderLocalizations { get; set; }

        #endregion

    }
}
