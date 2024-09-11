using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Migrations;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.Service
{
    public class ScheduleGeneratorService : IScheduleGeneratorService
    {
        private readonly IGenericRepository<BusinessAspect> _businessAspectRepository;
        private readonly ISkillService _skillService;

        #region Constructor

        public ScheduleGeneratorService(IGenericRepository<BusinessAspect> businessAspectRepository, ISkillService skillService)
        {
            _businessAspectRepository = businessAspectRepository;
            _skillService = skillService;
        }

        #endregion

        #region Methods

        public async Task<List<ScheduleEntryDTO>> FillOutSchedule(List<ScheduleEntryDTO> scheduleEntryDTOs, List<ShiftDTO> shifts, List<EntityRuleDTO> entityRules, List<EntityWorkerMemberDTO> entityWorkerMemberDTOs)
        {
            foreach(ScheduleEntryDTO scheduleEntryDTO in scheduleEntryDTOs)
            {
                List<EntityWorkerMemberDTO> assignedWorkers = new List<EntityWorkerMemberDTO>();

                // Filter valid workers for this entry
                List<EntityWorkerMemberDTO> filteredWorkers = FilterWorkersForRequiredSkillset(scheduleEntryDTO, entityRules, entityWorkerMemberDTOs);

                List<EntityWorkerMemberDTO> notEligible = new List<EntityWorkerMemberDTO>();

                // Get the max workers assignable to this shift
                int maxWorkers = ReturnMaxWorkersPerShift(entityRules, scheduleEntryDTO);

                // Get required quantity per skillset
                List<Tuple<int, int>> quantityPerSkillset = GetMinimumSkilletSetPerShift(entityRules, scheduleEntryDTO);

                // Check for minimum per skill set
                if(quantityPerSkillset.Count != 0)
                {
                    bool hasMinSkillSet = false;

                    int numberOfTries = 0;

                    while(hasMinSkillSet == false || numberOfTries != filteredWorkers.Count)
                    {
                        EntityWorkerMemberDTO entityWorkerMemberDTO = ;


                        hasMinSkillSet = IsSkillSetFullfilled(assignedWorkers, quantityPerSkillset);
                    }
                }


                // Regular assign

            }

            return scheduleEntryDTOs;
        }

        private bool ValidateWorkerSelection(ScheduleEntryDTO scheduleEntryDTO, EntityWorkerMemberDTO entityWorkerMemberDTO, List<EntityRuleDTO> entityRules, List<ShiftDTO> shiftDTOs)
        {
            EntityRuleDTO maxDailyHoursRule = entityRules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_HOURS_DAY_ID)).FirstOrDefault();
            
            // Checks Max Hours per day
            if (!CheckMaxHoursPerDay(scheduleEntryDTO, entityWorkerMemberDTO, maxDailyHoursRule, shiftDTOs))
                return false;

            // Check Max Hours per Week


            // Check for turns of the same type
        }

        #region Is Min Workers Per Shift

        /// <summary>
        /// Checks if schedule entry has the minimum number of workers required for the shift
        /// This only checks if there is a rule for minimum workers per shift
        /// Otherwise will always be true
        /// </summary>
        /// <param name="entityRuleDTOs"></param>
        /// <param name="scheduleEntryDTO"></param>
        /// <returns></returns>
        private bool IsMinWorkersPerShift(List<EntityRuleDTO> entityRuleDTOs, ScheduleEntryDTO scheduleEntryDTO)
        {
            bool validationResult = true;

            EntityRuleDTO entityRuleDTO = entityRuleDTOs.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MIN_WORKERS_SHIFT_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();
            if(entityRuleDTO != null)
            {
                validationResult = scheduleEntryDTO.ScheduleParticipants.Count >= entityRuleDTO.EntityRuleSpecificationDTOs.First().RuleSpecificationValue;
            }

            return validationResult;
        }

        #endregion

        #region Filter Workers For Required Skillset

        /// <summary>
        /// Filters and returns a list of valid workers in case of a required skillset for a shift
        /// </summary>
        /// <param name="scheduleEntryDTO"></param>
        /// <param name="entityRulesDTO"></param>
        /// <param name="entityWorkerMembers"></param>
        /// <returns></returns>
        private List<EntityWorkerMemberDTO> FilterWorkersForRequiredSkillset(ScheduleEntryDTO scheduleEntryDTO, List<EntityRuleDTO> entityRulesDTO, List<EntityWorkerMemberDTO> entityWorkerMembers)
        {
            List<EntityWorkerMemberDTO> filteredWorkers = entityWorkerMembers; 

            // Get the entity rule where its required skillset for the shift id
            EntityRuleDTO entityRuleDTO = entityRulesDTO.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_SKILLSET_SHIFT_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();
            
            if(entityRuleDTO != null)
            {
                // Get all skill identifiers required for the specifications
                List<int> wantedSkillset = (List<int>)entityRuleDTO.EntityRuleSpecificationDTOs
                    .Select(j => j.AspectReferenceId);

                List<Tuple<int, int>> minSkillsetPerShift = GetMinimumSkilletSetPerShift(entityRulesDTO, scheduleEntryDTO);

                // Filter list of workers valid for this entry
                filteredWorkers = filteredWorkers
                    .Where(worker => worker.SkillSet.Any(skill => wantedSkillset.Contains(skill.SkillId)))
                    .ToList();
            }


            return filteredWorkers;
        }

        #endregion

        #region Return Max Workers Per Shift

        /// <summary>
        /// Returns the maximum amount of workers can work on a specific shift
        /// </summary>
        /// <param name="entityRuleDTOs"></param>
        /// <param name="scheduleEntryDTO"></param>
        /// <returns></returns>
        private int ReturnMaxWorkersPerShift(List<EntityRuleDTO> entityRuleDTOs, ScheduleEntryDTO scheduleEntryDTO)
        {
            int maxPerShift = 0;

            EntityRuleDTO entityRuleDTO = entityRuleDTOs
                .Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_WORKERS_SHIFT_ID) 
                && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();

            if (entityRuleDTO != null)
                maxPerShift = entityRuleDTO.EntityRuleSpecificationDTOs.First().RuleSpecificationValue;

            return maxPerShift;
        }

        #endregion

        #region Get Minimum SkilletSet Per Shift

        private List<Tuple<int,int>> GetMinimumSkilletSetPerShift(List<EntityRuleDTO> entityRuleDTOs, ScheduleEntryDTO scheduleEntryDTO)
        {
            List<Tuple<int, int>> minSkillQuantity = new List<Tuple<int, int>>();

            EntityRuleDTO entityRuleDTO = entityRuleDTOs
                .Where(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_ID)
                && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();

            if(entityRuleDTO != null)
            {
                foreach (EntityRuleSpecificationDTO entityRuleSpecificationDTO in entityRuleDTO.EntityRuleSpecificationDTOs)
                    minSkillQuantity.Add(new Tuple<int, int>( int.Parse(entityRuleSpecificationDTO.AspectReferenceId), entityRuleSpecificationDTO.RuleSpecificationValue));
            }

            return minSkillQuantity;
        }

        #endregion

        #region Is Skill set fullfilled

        private bool IsSkillSetFullfilled(List<EntityWorkerMemberDTO> assignedWorkers, List<Tuple<int, int>> quantityPerSkillset)
        {
            // For each required skillset and quantity
            foreach (Tuple<int, int> tuple in quantityPerSkillset)
            {
                // Get the number of workers assigned with the specific skill
                int assignedSkillCount = assignedWorkers.Count(i => i.SkillSet.Any(j => j.SkillId == tuple.Item1));

                // If the number of assigned workers with this skill is less than required, return false
                if (assignedSkillCount < tuple.Item2)
                    return false;
            }

            // All required skillsets are fulfilled
            return true;
        }

        #endregion

        #region Check Max Hours Per Day

        private bool CheckMaxHoursPerDay(ScheduleEntryDTO currentScheduleEntry, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO maxDailyHoursRule, List<ShiftDTO> shiftDTOs)
        {
            // Check if current schedule entry is longer than the allowed daily hours
            ShiftDTO shiftDTO = shiftDTOs.Where(i => i.ShiftId.Equals(currentScheduleEntry.ShiftId)).FirstOrDefault();

            // If a shift is found
            if (shiftDTO == null)
                return false;

            // If shift duration is greater than the max allowed value by rule
            if (shiftDTO.ShiftDuration > TimeSpan.FromHours(maxDailyHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue))
                return false;



            return true;
        }

        #endregion


        #region Check Max Hours per Week



        #endregion

        #region Check For Turns of the same Type



        #endregion

        #endregion
    }
}
