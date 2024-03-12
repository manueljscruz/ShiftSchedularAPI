using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularEntity.Entities
{
    public class Localization
    {
        #region Properties

        public int LocalizationId { get; set; }
        public string LocalizationCode { get; set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<GenderLocalization> GenderLocalizations { get; set; }

        #endregion

        #region Constructor

        public Localization()
        {
            LocalizationId = 0;
            LocalizationCode = string.Empty;
        }

        #endregion
    }
}
