using System.ComponentModel.DataAnnotations.Schema;

namespace ShiftSchedularEntity.Entities
{
    public class Worker
    {
        #region Properties

        public string WorkerId { get; set; }
        public string WorkerName { get; set; }
        public int GenderId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsBot { get; set; }

        #endregion

        #region Navigation Properties

        public virtual Gender Gender { get; set; }

        public virtual ICollection<EntityWorker> EntityWorkers { get; set; }

        #endregion
    }
}
