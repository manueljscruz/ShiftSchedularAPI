namespace ShiftSchedularEntity.Entities
{
    public class RuleTypeBusinessAspect
    {
        #region Properties

        public int RuleTypeId { get; set; }
        public int BusinessAspectId { get; set; }

        #endregion

        #region Navigation Properties

        public virtual RuleType RuleType { get; set; }
        public virtual BusinessAspect BusinessAspect { get; set; }

        #endregion
    }
}
