using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class Gender
    {
        #region Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GenderId { get; set; }
        public string GenderValue { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<GenderLocalization> GenderLocalizations { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
        #endregion
    }
}
