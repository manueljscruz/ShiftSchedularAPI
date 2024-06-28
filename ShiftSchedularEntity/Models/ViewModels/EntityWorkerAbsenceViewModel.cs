using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.ViewModels
{
    public class EntityWorkerAbsenceViewModel
    {
        public bool IsOwner { get; set; }
        public List<EntityWorkerAbsenceDTO> EntityWorkerAbsences { get; set; }
        public List<AbsenceTypeLocalizedDTO> AbsenceTypeLocalizeds { get; set; }

        public EntityWorkerAbsenceViewModel()
        {
            EntityWorkerAbsences = new List<EntityWorkerAbsenceDTO>();
            AbsenceTypeLocalizeds = new List<AbsenceTypeLocalizedDTO>();
        }
    }
}
