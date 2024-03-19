using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class Worker
    {
        #region Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long WorkerId { get; set; }
        public string WorkerName { get; set; }
        public int GenderId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Gender Gender { get; set; }

        #endregion
    }
}
