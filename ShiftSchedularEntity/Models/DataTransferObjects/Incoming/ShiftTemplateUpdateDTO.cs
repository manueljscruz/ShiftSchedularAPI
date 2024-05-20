using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularEntity.Models.DataTransferObjects.Incoming
{
    public class ShiftTemplateUpdateDTO : ShiftTemplateDTO
    {
        /// <summary>
        /// New Shift Break Templates to be associated with Shift Template
        /// </summary>
        public List<int> ShiftBreakTemplatesIdsIn { get; set; }

        /// <summary>
        /// Shift Break Templates to be removed from the Shift Template
        /// </summary>
        public List<int> ShiftBreakTemplatesIdsOut { get; set; }

        #region Constructors

        /// <summary>
        /// Instantiates a new Shift Template Update DTO
        /// </summary>
        public ShiftTemplateUpdateDTO()
        {
            ShiftBreakTemplatesIdsIn = new List<int>();
            ShiftBreakTemplatesIdsOut = new List<int>();
        }

        /// <summary>
        /// Sets the Shift Break Templates Ids In and Out
        /// </summary>
        public ShiftTemplateUpdateDTO(List<int> shiftBreakTemplatesIdsIn, List<int> shiftBreakTemplatesIdsOut)
        {
            ShiftBreakTemplatesIdsIn = shiftBreakTemplatesIdsIn;
            ShiftBreakTemplatesIdsOut = shiftBreakTemplatesIdsOut;
        }

        #endregion
    }
}
