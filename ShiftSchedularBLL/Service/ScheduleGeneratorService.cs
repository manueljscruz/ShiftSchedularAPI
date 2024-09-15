using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Migrations;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
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
        private readonly IEntityScheduleService _entityScheduleService;

        #region Constructor

        public ScheduleGeneratorService(IGenericRepository<BusinessAspect> businessAspectRepository, ISkillService skillService, IEntityScheduleService entityScheduleService)
        {
            _businessAspectRepository = businessAspectRepository;
            _skillService = skillService;
            _entityScheduleService = entityScheduleService;
        }

        #endregion

        #region Methods

        #region Fill Out Schedule

        public async Task<List<ScheduleEntryDTO>> FillOutSchedule(List<ScheduleEntryDTO> scheduleEntryDTOs, List<ShiftDTO> shifts, List<EntityRuleDTO> entityRules, List<EntityWorkerMemberDTO> entityWorkerMemberDTOs, CreateEntityScheduleDTO createEntityScheduleDTO)
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
                        // EntityWorkerMemberDTO entityWorkerMemberDTO = ;


                        hasMinSkillSet = IsSkillSetFullfilled(assignedWorkers, quantityPerSkillset);
                    }
                }


                // Regular assign

            }

            return scheduleEntryDTOs;
        }

        #endregion

        #region Validate Worker Selection

        private async Task<bool> ValidateWorkerSelection(ScheduleEntryDTO currentScheduleEntry, List<ScheduleEntryDTO> scheduleEntryDTOs, EntityWorkerMemberDTO entityWorkerMemberDTO, List<EntityRuleDTO> entityRules, List<ShiftDTO> shiftDTOs, CreateEntityScheduleDTO createEntityScheduleDTO)
        {
            // Get Max Daily hours rule
            EntityRuleDTO maxDailyHoursRule = entityRules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_HOURS_DAY_ID)).FirstOrDefault();
            
            IEnumerable<ScheduleEntryDTO> presentDayEntries = scheduleEntryDTOs.Where(i => i.ScheduleEndDate.Date.Equals(currentScheduleEntry.ScheduleEndDate.Date) && i.ScheduleParticipants.Any(j => j.WorkerId.Equals(entityWorkerMemberDTO.WorkerId)));
            // Checks Max Hours per day
            if (maxDailyHoursRule != null && !CheckMaxHoursPerDay(currentScheduleEntry, presentDayEntries, entityWorkerMemberDTO, maxDailyHoursRule, shiftDTOs))
                return false;

            // Get Max Weekly Hours rule
            EntityRuleDTO maxWeeklyHoursRule = entityRules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_HOURS_WEEK_ID)).FirstOrDefault();

            List<ScheduleEntryDTO> weekEntries = await FilterWeeklySessions(currentScheduleEntry, scheduleEntryDTOs, createEntityScheduleDTO);
            // Check Max Hours per Week
            if (maxWeeklyHoursRule != null && !CheckMaxHoursPerWeek(weekEntries, entityWorkerMemberDTO, maxWeeklyHoursRule, shiftDTOs))
                return false;

            // Check for turns of the same type
            EntityRuleDTO limitOfTurnsRule = entityRules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_CONSECUTIVE_SHIFTS_ID)).FirstOrDefault();

            IEnumerable<ScheduleEntryDTO> previousShiftEntries = scheduleEntryDTOs.Where(i => i != null); // TO DO
            if (limitOfTurnsRule != null && !CheckForTurnsOfTheSameType(currentScheduleEntry, previousShiftEntries, entityWorkerMemberDTO, limitOfTurnsRule, shiftDTOs))
                return false;

            // Check if worker has performed a shift and requires rest

            return true;
        }

        #endregion

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

        private bool CheckMaxHoursPerDay(ScheduleEntryDTO currentScheduleEntry, IEnumerable<ScheduleEntryDTO> presentDayEntries, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO maxDailyHoursRule, List<ShiftDTO> shiftDTOs)
        {
            // Check if current schedule entry is longer than the allowed daily hours
            ShiftDTO shiftDTO = shiftDTOs.Where(i => i.ShiftId.Equals(currentScheduleEntry.ShiftId)).FirstOrDefault();

            // If a shift is found
            if (shiftDTO == null)
                return false;

            // If shift duration is greater than the max allowed value by rule
            if (shiftDTO.ShiftDuration > TimeSpan.FromHours(maxDailyHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue))
                return false;

            // Check possible other entries
            if(presentDayEntries.Count() > 0)
            {
                TimeSpan totalHours = new TimeSpan();
                foreach(ScheduleEntryDTO scheduleEntryDTO in presentDayEntries)
                {
                    if (scheduleEntryDTO.ScheduleEntryId.Equals(currentScheduleEntry.ScheduleEntryId))
                        continue;

                    ShiftDTO shift = shiftDTOs.Where(i => i.ShiftId == scheduleEntryDTO.ShiftId).FirstOrDefault();
                    if (shift != null)
                        totalHours.Add(shift.ShiftDuration);

                }
                
                // If the total hours of the day plus the current entry is greater than the allowed daily hours
                if (totalHours + shiftDTO.ShiftDuration > TimeSpan.FromHours(maxDailyHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue))
                    return false;
            }

            return true;
        }

        #endregion

        #region Check Max Hours per Week

        private bool CheckMaxHoursPerWeek(IEnumerable<ScheduleEntryDTO> weekEntries, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO maxWeeklyHoursRule, List<ShiftDTO> shiftDTOs)
        {
            TimeSpan totalHours = TimeSpan.Zero;

            // For each week entry
            foreach(ScheduleEntryDTO scheduleEntryDTO in weekEntries)
            {
                if(scheduleEntryDTO.ScheduleParticipants.Any(i => i.WorkerId.Equals(entityWorkerMemberDTO.WorkerId)))
                {
                    ShiftDTO shiftDTO = shiftDTOs.Where(i => i.ShiftId.Equals(scheduleEntryDTO.ShiftId)).FirstOrDefault();
                    if (shiftDTO != null)
                        totalHours.Add(shiftDTO.ShiftDuration);
                }
            }

            if (totalHours > TimeSpan.FromHours(maxWeeklyHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue))
                return false;
            
            return true;
        }


        #endregion

        #region Check For Turns of the same Type

        private bool CheckForTurnsOfTheSameType(ScheduleEntryDTO currentScheduleEntry, IEnumerable<ScheduleEntryDTO> previousShiftEntries, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO limitOfTurnsRule, List<ShiftDTO> shiftDTOs)
        {


            return true;
        }

        #endregion

        #region Filter Weekly Sessions

        private async Task<List<ScheduleEntryDTO>> FilterWeeklySessions(ScheduleEntryDTO currentScheduleEntry, List<ScheduleEntryDTO> scheduleEntryDTOs, CreateEntityScheduleDTO createEntityScheduleDTO)
        {
            List<ScheduleEntryDTO> filteredWeeklyEntries = new List<ScheduleEntryDTO>();
            int backtrackDaysQty = 0;
            int forwardDaysQty = 0;

            // Calculate how many days back and forward we need to go
            switch(currentScheduleEntry.ScheduleStartDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    backtrackDaysQty = 0;
                    forwardDaysQty = 6;
                    break;

                case DayOfWeek.Tuesday:
                    backtrackDaysQty = 1;
                    forwardDaysQty = 5;
                    break;

                case DayOfWeek.Wednesday:
                    backtrackDaysQty = 2;
                    forwardDaysQty = 4;
                    break;

                case DayOfWeek.Thursday:
                    backtrackDaysQty = 3;
                    forwardDaysQty = 3;
                    break;

                case DayOfWeek.Friday:
                    backtrackDaysQty = 4;
                    forwardDaysQty = 2;
                    break;

                case DayOfWeek.Saturday:
                    backtrackDaysQty = 5;
                    forwardDaysQty = 1;
                    break;

                case DayOfWeek.Sunday:
                    backtrackDaysQty = 6;
                    forwardDaysQty = 0;
                    break;
            }

            // Set Start Dates
            DateTime StartOfWeekDate = currentScheduleEntry.ScheduleStartDate.AddDays(-backtrackDaysQty);
            DateTime EndOfWeekDate = currentScheduleEntry.ScheduleStartDate.AddDays(forwardDaysQty);

            // Check if this week is part of the first day of the search
            if (StartOfWeekDate >= createEntityScheduleDTO.StartDate && createEntityScheduleDTO.StartDate <= EndOfWeekDate)
            {
                // If Start Date is not the first day of the week, need to get previous entries
                if (createEntityScheduleDTO.StartDate.DayOfWeek != DayOfWeek.Monday)
                {

                    ScheduleViewModelRequestDTO weekRequest = new ScheduleViewModelRequestDTO
                    {
                        EntityId = createEntityScheduleDTO.EntityId,
                        WorkerId = createEntityScheduleDTO.WorkerId,
                        LanguageCode = createEntityScheduleDTO.LanguageCode,
                        StartDateSearch = StartOfWeekDate,
                        EndDateSearch = createEntityScheduleDTO.StartDate
                    };

                    // Get Schedule entries from Start of Week Date to Start Date
                    filteredWeeklyEntries = filteredWeeklyEntries.Concat(await _entityScheduleService.GetScheduleEntries(weekRequest)).ToList();
                }
            }

            filteredWeeklyEntries = filteredWeeklyEntries.Concat(scheduleEntryDTOs.Where(i => i.ScheduleStartDate > currentScheduleEntry.ScheduleStartDate.AddDays(backtrackDaysQty) && i.ScheduleEndDate < currentScheduleEntry.ScheduleStartDate.AddDays(forwardDaysQty))).ToList();

            return filteredWeeklyEntries;
        }

        #endregion

        #endregion
    }
}
