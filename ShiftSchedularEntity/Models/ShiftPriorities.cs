namespace ShiftSchedularEntity.Models
{
    public class ShiftPriorities
    {
        public Guid ShiftId { get; set; }
        public int OrderNo { get; set; }
        public int MinWorkers { get; set; }
        public int MaxWorkers { get; set; }
        //public List<Tuple<int, int>> MinSkillset { get; set; }
        public int SkillSetSpecsCount { get; set; }
        public int ScorePriority
        {
            get
            {
                int score = 0;

                if (MinWorkers != 0)
                    score++;

                if (MaxWorkers != 0)
                    score++;
                
                if(SkillSetSpecsCount != 0)
                {
                    score += SkillSetSpecsCount;
                }

                return score;
            }
        }

        #region Constructor

        public ShiftPriorities(Guid shiftId)
        {
            ShiftId = shiftId;
        }

        #endregion
    }
}
