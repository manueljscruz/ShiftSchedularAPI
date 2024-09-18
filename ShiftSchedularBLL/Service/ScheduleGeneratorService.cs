using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.Service
{
    public class ScheduleGeneratorService : IScheduleGeneratorService
    {
        private readonly IGenericRepository<BusinessAspect> _businessAspectRepository;
        private readonly ISkillService _skillService;
        // private readonly IEntityScheduleService _entityScheduleService;

        #region Constructor

        public ScheduleGeneratorService(IGenericRepository<BusinessAspect> businessAspectRepository, ISkillService skillService) // IEntityScheduleService entityScheduleService
        {
            _businessAspectRepository = businessAspectRepository;
            _skillService = skillService;
            // _entityScheduleService = entityScheduleService;
        }

        #endregion

        #region Methods

        //#region Fill Out Schedule

        //public async Task<List<ScheduleEntryDTO>> FillOutSchedule(List<ScheduleEntryDTO> scheduleEntryDTOs, List<ShiftDTO> shifts, List<EntityRuleDTO> entityRules, List<EntityWorkerMemberDTO> entityWorkerMemberDTOs, CreateEntityScheduleDTO createEntityScheduleDTO)
        //{
        //    Random rand = new Random();

        //    foreach(ScheduleEntryDTO scheduleEntryDTO in scheduleEntryDTOs)
        //    {
        //        List<EntityWorkerMemberDTO> assignedWorkers = new List<EntityWorkerMemberDTO>();
        //        List<EntityWorkerMemberDTO> notEligible = new List<EntityWorkerMemberDTO>();

        //        // Filter valid workers for this entry
        //        List<EntityWorkerMemberDTO> filteredWorkers = FilterWorkersForRequiredSkillset(scheduleEntryDTO, entityRules, entityWorkerMemberDTOs);

        //        // Get the max workers assignable to this shift
        //        int maxWorkers = ReturnMaxWorkersPerShift(entityRules, scheduleEntryDTO);

        //        // Get required quantity per skillset
        //        List<Tuple<int, int>> quantityPerSkillset = GetMinimumSkilletSetPerShift(entityRules, scheduleEntryDTO);

        //        // Check for minimum per skill set
        //        if(quantityPerSkillset.Count != 0)
        //        {
        //            bool hasMinSkillSet = false;

        //            // While the skillset is not satisfied or all workers have been assigned
        //            while(hasMinSkillSet == false || assignedWorkers.Count + notEligible.Count != filteredWorkers.Count)
        //            {
        //                // TODO: Get worker from filtered
        //                EntityWorkerMemberDTO selectedWorker = filteredWorkers[rand.Next(filteredWorkers.Count)];

        //                // Check if the worker has already been assigned or marked as not eligible
        //                if (assignedWorkers.Contains(selectedWorker) || notEligible.Contains(selectedWorker))
        //                    continue;

        //                // Validate worker selection
        //                bool validateSelection = await ValidateWorkerSelection(scheduleEntryDTO, scheduleEntryDTOs, selectedWorker, entityRules, shifts, createEntityScheduleDTO);

        //                // Eligible for this entry
        //                if(validateSelection)
        //                {
        //                    // Add worker and signal that this worker has been assigned
        //                    scheduleEntryDTO.ScheduleParticipants.Add(selectedWorker);
        //                    assignedWorkers.Add(selectedWorker);
        //                }
        //                // Not Eligible for this entry
        //                else
        //                {
        //                    notEligible.Add(selectedWorker);
        //                }

        //                // If there is a max workers for this shift restriction and its reached
        //                if(maxWorkers != 0 && assignedWorkers.Count == maxWorkers)
        //                    break;

        //                hasMinSkillSet = IsSkillSetFullfilled(assignedWorkers, quantityPerSkillset);
        //            }
        //        }

        //        // Regular assign
        //        else
        //        {
        //            while (assignedWorkers.Count + notEligible.Count != entityWorkerMemberDTOs.Count)
        //            {
        //                // Select a random worker from the full pool of workers
        //                EntityWorkerMemberDTO selectedWorker = entityWorkerMemberDTOs[rand.Next(entityWorkerMemberDTOs.Count)];

        //                // Check if the worker has already been assigned or marked as not eligible
        //                if (assignedWorkers.Contains(selectedWorker) || notEligible.Contains(selectedWorker))
        //                    continue;

        //                // Validate worker selection
        //                bool validateSelection = await ValidateWorkerSelection(scheduleEntryDTO, scheduleEntryDTOs, selectedWorker, entityRules, shifts, createEntityScheduleDTO);

        //                // Eligible for this entry
        //                if (validateSelection)
        //                {
        //                    // Add worker and signal that this worker has been assigned
        //                    scheduleEntryDTO.ScheduleParticipants.Add(selectedWorker);
        //                    assignedWorkers.Add(selectedWorker);
        //                }
        //                // Not Eligible for this entry
        //                else
        //                {
        //                    notEligible.Add(selectedWorker);
        //                }

        //                // If there is a max workers for this shift restriction and its reached
        //                if (maxWorkers != 0 && assignedWorkers.Count == maxWorkers)
        //                    break;
        //            }
        //        }
        //    }

        //    return scheduleEntryDTOs;
        //}

        //#endregion

        //#region Validate Worker Selection

        ///// <summary>
        ///// Validates the worker selection for a current schedule entry
        ///// </summary>
        ///// <param name="currentScheduleEntry"></param>
        ///// <param name="scheduleEntryDTOs"></param>
        ///// <param name="entityWorkerMemberDTO"></param>
        ///// <param name="entityRules"></param>
        ///// <param name="shiftDTOs"></param>
        ///// <param name="createEntityScheduleDTO"></param>
        ///// <returns></returns>
        //private async Task<bool> ValidateWorkerSelection(ScheduleEntryDTO currentScheduleEntry, List<ScheduleEntryDTO> scheduleEntryDTOs, EntityWorkerMemberDTO entityWorkerMemberDTO, List<EntityRuleDTO> entityRules, List<ShiftDTO> shiftDTOs, CreateEntityScheduleDTO createEntityScheduleDTO)
        //{
        //    // Get Max Daily hours rule
        //    EntityRuleDTO maxDailyHoursRule = entityRules.FirstOrDefault((i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_HOURS_DAY_ID)));
            
        //    IEnumerable<ScheduleEntryDTO> presentDayEntries = scheduleEntryDTOs.Where(i => i.ScheduleEndDate.Date.Equals(currentScheduleEntry.ScheduleEndDate.Date) && i.ScheduleParticipants.Any(j => j.WorkerId.Equals(entityWorkerMemberDTO.WorkerId)));
        //    // Checks Max Hours per day
        //    if (maxDailyHoursRule != null && !CheckMaxHoursPerDay(currentScheduleEntry, presentDayEntries, entityWorkerMemberDTO, maxDailyHoursRule, shiftDTOs))
        //        return false;

        //    // Get Max Weekly Hours rule
        //    EntityRuleDTO maxWeeklyHoursRule = entityRules.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_HOURS_WEEK_ID)).FirstOrDefault();

        //    List<ScheduleEntryDTO> weekEntries = await FilterWeeklySessions(currentScheduleEntry, scheduleEntryDTOs, createEntityScheduleDTO);
        //    // Check Max Hours per Week
        //    if (maxWeeklyHoursRule != null && !CheckMaxHoursPerWeek(weekEntries, entityWorkerMemberDTO, maxWeeklyHoursRule, shiftDTOs))
        //        return false;

        //    // Check for turns of the same type
        //    EntityRuleDTO limitOfTurnsRule = entityRules.FirstOrDefault(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_CONSECUTIVE_SHIFTS_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(currentScheduleEntry.ShiftId)));
            
        //    if(limitOfTurnsRule != null)
        //    {
        //        List<ScheduleEntryDTO> previousShiftEntries = await FilterEntriesForConsecutiveTurns(currentScheduleEntry, scheduleEntryDTOs, createEntityScheduleDTO, limitOfTurnsRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue);
        //        if (limitOfTurnsRule != null && !CheckForTurnsOfTheSameType(currentScheduleEntry, previousShiftEntries, entityWorkerMemberDTO, limitOfTurnsRule, shiftDTOs))
        //            return false;
        //    }

        //    // Check if worker has performed a shift and requires rest
        //    EntityRuleDTO postShiftRestRule = entityRules.FirstOrDefault(i => i.RuleTypeId.Equals(RuleTypeConstants.POST_SHIFT_REST_HOURS_ID));
        //    if(limitOfTurnsRule != null && !CheckForPostShiftRest(currentScheduleEntry, scheduleEntryDTOs, entityWorkerMemberDTO, postShiftRestRule))
        //        return false;

        //    return true;
        //}

        //#endregion

        //#region Check For Turns of the same Type

        ///// <summary>
        ///// Checks if worker is part of too many turns of the same type
        ///// </summary>
        ///// <param name="currentScheduleEntry"></param>
        ///// <param name="previousShiftEntries"></param>
        ///// <param name="entityWorkerMemberDTO"></param>
        ///// <param name="limitOfTurnsRule"></param>
        ///// <param name="shiftDTOs"></param>
        ///// <returns></returns>
        //private bool CheckForTurnsOfTheSameType(ScheduleEntryDTO currentScheduleEntry, IEnumerable<ScheduleEntryDTO> previousShiftEntries, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO limitOfTurnsRule, List<ShiftDTO> shiftDTOs)
        //{
        //    if(limitOfTurnsRule != null && limitOfTurnsRule.EntityRuleSpecificationDTOs.Count != 0)
        //    {
        //        EntityRuleSpecificationDTO entityRuleSpecificationDTO = limitOfTurnsRule.EntityRuleSpecificationDTOs.FirstOrDefault();

        //        // If there is a specification 
        //        if(entityRuleSpecificationDTO.RuleSpecificationValue != 0)
        //        {
        //            // Sort previous shift entries by ScheduleStartDate (latest first)
        //            var sortedPreviousShifts = previousShiftEntries
        //                .OrderByDescending(i => i.ScheduleStartDate)
        //                .ToList();

        //            int consecutiveShiftsCount = 1; // Start with 1 since current shift counts

        //            // Iterate through the sorted previous shifts
        //            for (int i = 0; i < sortedPreviousShifts.Count - 1; i++)
        //            {
        //                var currentShift = sortedPreviousShifts[i];
        //                var nextShift = sortedPreviousShifts[i + 1];

        //                // Check if the current shift ends just before or overlaps the next shift
        //                if (currentShift.ScheduleEndDate >= nextShift.ScheduleStartDate)
        //                {
        //                    // Increase the consecutive shifts count
        //                    consecutiveShiftsCount++;
        //                }
        //                else
        //                {
        //                    // If there's a gap, break the consecutive chain
        //                    break;
        //                }

        //                // If consecutive shifts exceed the allowed limit, return false
        //                if (consecutiveShiftsCount > entityRuleSpecificationDTO.RuleSpecificationValue)
        //                {
        //                    return false;
        //                }
        //            }
        //        }
        //    }

        //    return true;
        //}

        //#endregion

        //#region Is Min Workers Per Shift

        ///// <summary>
        ///// Checks if schedule entry has the minimum number of workers required for the shift
        ///// This only checks if there is a rule for minimum workers per shift
        ///// Otherwise will always be true
        ///// </summary>
        ///// <param name="entityRuleDTOs"></param>
        ///// <param name="scheduleEntryDTO"></param>
        ///// <returns></returns>
        //private bool IsMinWorkersPerShift(List<EntityRuleDTO> entityRuleDTOs, ScheduleEntryDTO scheduleEntryDTO)
        //{
        //    bool validationResult = true;

        //    EntityRuleDTO entityRuleDTO = entityRuleDTOs.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MIN_WORKERS_SHIFT_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();
        //    if(entityRuleDTO != null)
        //    {
        //        validationResult = scheduleEntryDTO.ScheduleParticipants.Count >= entityRuleDTO.EntityRuleSpecificationDTOs.First().RuleSpecificationValue;
        //    }

        //    return validationResult;
        //}

        //#endregion

        //#region Filter Workers For Required Skillset

        ///// <summary>
        ///// Filters and returns a list of valid workers in case of a required skillset for a shift
        ///// </summary>
        ///// <param name="scheduleEntryDTO"></param>
        ///// <param name="entityRulesDTO"></param>
        ///// <param name="entityWorkerMembers"></param>
        ///// <returns></returns>
        //private List<EntityWorkerMemberDTO> FilterWorkersForRequiredSkillset(ScheduleEntryDTO scheduleEntryDTO, List<EntityRuleDTO> entityRulesDTO, List<EntityWorkerMemberDTO> entityWorkerMembers)
        //{
        //    List<EntityWorkerMemberDTO> filteredWorkers = entityWorkerMembers; 

        //    // Get the entity rule where its required skillset for the shift id
        //    EntityRuleDTO entityRuleDTO = entityRulesDTO.Where(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_SKILLSET_SHIFT_ID) && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();
            
        //    if(entityRuleDTO != null)
        //    {
        //        // Get all skill identifiers required for the specifications
        //        List<int> wantedSkillset = (List<int>)entityRuleDTO.EntityRuleSpecificationDTOs
        //            .Select(j => j.AspectReferenceId);

        //        List<Tuple<int, int>> minSkillsetPerShift = GetMinimumSkilletSetPerShift(entityRulesDTO, scheduleEntryDTO);

        //        // Filter list of workers valid for this entry
        //        filteredWorkers = filteredWorkers
        //            .Where(worker => worker.SkillSet.Any(skill => wantedSkillset.Contains(skill.SkillId)))
        //            .ToList();
        //    }


        //    return filteredWorkers;
        //}

        //#endregion

        //#region Return Max Workers Per Shift

        ///// <summary>
        ///// Returns the maximum amount of workers can work on a specific shift
        ///// </summary>
        ///// <param name="entityRuleDTOs"></param>
        ///// <param name="scheduleEntryDTO"></param>
        ///// <returns></returns>
        //private int ReturnMaxWorkersPerShift(List<EntityRuleDTO> entityRuleDTOs, ScheduleEntryDTO scheduleEntryDTO)
        //{
        //    int maxPerShift = 0;

        //    EntityRuleDTO entityRuleDTO = entityRuleDTOs
        //        .Where(i => i.RuleTypeId.Equals(RuleTypeConstants.MAX_WORKERS_SHIFT_ID) 
        //        && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();

        //    if (entityRuleDTO != null)
        //        maxPerShift = entityRuleDTO.EntityRuleSpecificationDTOs.First().RuleSpecificationValue;

        //    return maxPerShift;
        //}

        //#endregion

        //#region Get Minimum SkilletSet Per Shift

        //private List<Tuple<int,int>> GetMinimumSkilletSetPerShift(List<EntityRuleDTO> entityRuleDTOs, ScheduleEntryDTO scheduleEntryDTO)
        //{
        //    List<Tuple<int, int>> minSkillQuantity = new List<Tuple<int, int>>();

        //    EntityRuleDTO entityRuleDTO = entityRuleDTOs
        //        .Where(i => i.RuleTypeId.Equals(RuleTypeConstants.REQ_QTY_SKILL_SHIFT_ID)
        //        && i.EntityRuleSpecificationDTOs.Any(j => j.AspectReferenceId2.Equals(scheduleEntryDTO.ShiftId))).FirstOrDefault();

        //    if(entityRuleDTO != null)
        //    {
        //        foreach (EntityRuleSpecificationDTO entityRuleSpecificationDTO in entityRuleDTO.EntityRuleSpecificationDTOs)
        //            minSkillQuantity.Add(new Tuple<int, int>( int.Parse(entityRuleSpecificationDTO.AspectReferenceId), entityRuleSpecificationDTO.RuleSpecificationValue));
        //    }

        //    return minSkillQuantity;
        //}

        //#endregion

        //#region Is Skill set fullfilled

        //private bool IsSkillSetFullfilled(List<EntityWorkerMemberDTO> assignedWorkers, List<Tuple<int, int>> quantityPerSkillset)
        //{
        //    // For each required skillset and quantity
        //    foreach (Tuple<int, int> tuple in quantityPerSkillset)
        //    {
        //        // Get the number of workers assigned with the specific skill
        //        int assignedSkillCount = assignedWorkers.Count(i => i.SkillSet.Any(j => j.SkillId == tuple.Item1));

        //        // If the number of assigned workers with this skill is less than required, return false
        //        if (assignedSkillCount < tuple.Item2)
        //            return false;
        //    }

        //    // All required skillsets are fulfilled
        //    return true;
        //}

        //#endregion

        //#region Check Max Hours Per Day

        //private bool CheckMaxHoursPerDay(ScheduleEntryDTO currentScheduleEntry, IEnumerable<ScheduleEntryDTO> presentDayEntries, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO maxDailyHoursRule, List<ShiftDTO> shiftDTOs)
        //{
        //    // Check if current schedule entry is longer than the allowed daily hours
        //    ShiftDTO shiftDTO = shiftDTOs.Where(i => i.ShiftId.Equals(currentScheduleEntry.ShiftId)).FirstOrDefault();

        //    // If a shift is found
        //    if (shiftDTO == null)
        //        return false;

        //    // If shift duration is greater than the max allowed value by rule
        //    if (shiftDTO.ShiftDuration > TimeSpan.FromHours(maxDailyHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue))
        //        return false;

        //    // Check possible other entries
        //    if(presentDayEntries.Count() > 0)
        //    {
        //        TimeSpan totalHours = new TimeSpan();
        //        foreach(ScheduleEntryDTO scheduleEntryDTO in presentDayEntries)
        //        {
        //            if (scheduleEntryDTO.ScheduleEntryId.Equals(currentScheduleEntry.ScheduleEntryId))
        //                continue;

        //            ShiftDTO shift = shiftDTOs.Where(i => i.ShiftId == scheduleEntryDTO.ShiftId).FirstOrDefault();
        //            if (shift != null)
        //                totalHours.Add(shift.ShiftDuration);

        //        }
                
        //        // If the total hours of the day plus the current entry is greater than the allowed daily hours
        //        if (totalHours + shiftDTO.ShiftDuration > TimeSpan.FromHours(maxDailyHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue))
        //            return false;
        //    }

        //    return true;
        //}

        //#endregion

        //#region Check Max Hours per Week

        //private bool CheckMaxHoursPerWeek(IEnumerable<ScheduleEntryDTO> weekEntries, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO maxWeeklyHoursRule, List<ShiftDTO> shiftDTOs)
        //{
        //    TimeSpan totalHours = TimeSpan.Zero;

        //    // For each week entry
        //    foreach(ScheduleEntryDTO scheduleEntryDTO in weekEntries)
        //    {
        //        if(scheduleEntryDTO.ScheduleParticipants.Any(i => i.WorkerId.Equals(entityWorkerMemberDTO.WorkerId)))
        //        {
        //            ShiftDTO shiftDTO = shiftDTOs.Where(i => i.ShiftId.Equals(scheduleEntryDTO.ShiftId)).FirstOrDefault();
        //            if (shiftDTO != null)
        //                totalHours.Add(shiftDTO.ShiftDuration);
        //        }
        //    }

        //    if (totalHours > TimeSpan.FromHours(maxWeeklyHoursRule.EntityRuleSpecificationDTOs.First().RuleSpecificationValue))
        //        return false;
            
        //    return true;
        //}


        //#endregion

        //#region Filter Weekly Sessions

        //private async Task<List<ScheduleEntryDTO>> FilterWeeklySessions(ScheduleEntryDTO currentScheduleEntry, List<ScheduleEntryDTO> scheduleEntryDTOs, CreateEntityScheduleDTO createEntityScheduleDTO)
        //{
        //    List<ScheduleEntryDTO> filteredWeeklyEntries = new List<ScheduleEntryDTO>();
        //    int backtrackDaysQty = 0;
        //    int forwardDaysQty = 0;

        //    // Calculate how many days back and forward we need to go
        //    switch(currentScheduleEntry.ScheduleStartDate.DayOfWeek)
        //    {
        //        case DayOfWeek.Monday:
        //            backtrackDaysQty = 0;
        //            forwardDaysQty = 6;
        //            break;

        //        case DayOfWeek.Tuesday:
        //            backtrackDaysQty = 1;
        //            forwardDaysQty = 5;
        //            break;

        //        case DayOfWeek.Wednesday:
        //            backtrackDaysQty = 2;
        //            forwardDaysQty = 4;
        //            break;

        //        case DayOfWeek.Thursday:
        //            backtrackDaysQty = 3;
        //            forwardDaysQty = 3;
        //            break;

        //        case DayOfWeek.Friday:
        //            backtrackDaysQty = 4;
        //            forwardDaysQty = 2;
        //            break;

        //        case DayOfWeek.Saturday:
        //            backtrackDaysQty = 5;
        //            forwardDaysQty = 1;
        //            break;

        //        case DayOfWeek.Sunday:
        //            backtrackDaysQty = 6;
        //            forwardDaysQty = 0;
        //            break;
        //    }

        //    // Set Start Dates
        //    DateTime StartOfWeekDate = currentScheduleEntry.ScheduleStartDate.AddDays(-backtrackDaysQty);
        //    DateTime EndOfWeekDate = currentScheduleEntry.ScheduleStartDate.AddDays(forwardDaysQty);

        //    // Check if this week is part of the first day of the search
        //    if (StartOfWeekDate >= createEntityScheduleDTO.StartDate && createEntityScheduleDTO.StartDate <= EndOfWeekDate)
        //    {
        //        // If Start Date is not the first day of the week, need to get previous entries
        //        if (createEntityScheduleDTO.StartDate.DayOfWeek != DayOfWeek.Monday)
        //        {

        //            ScheduleViewModelRequestDTO weekRequest = new ScheduleViewModelRequestDTO
        //            {
        //                EntityId = createEntityScheduleDTO.EntityId,
        //                WorkerId = createEntityScheduleDTO.WorkerId,
        //                LanguageCode = createEntityScheduleDTO.LanguageCode,
        //                StartDateSearch = StartOfWeekDate,
        //                EndDateSearch = createEntityScheduleDTO.StartDate
        //            };

        //            // Get Schedule entries from Start of Week Date to Start Date
        //            filteredWeeklyEntries = filteredWeeklyEntries.Concat(await _entityScheduleService.GetScheduleEntries(weekRequest)).ToList();
        //        }
        //    }

        //    filteredWeeklyEntries = filteredWeeklyEntries.Concat(scheduleEntryDTOs.Where(i => i.ScheduleStartDate > currentScheduleEntry.ScheduleStartDate.AddDays(backtrackDaysQty) && i.ScheduleEndDate < currentScheduleEntry.ScheduleStartDate.AddDays(forwardDaysQty))).ToList();

        //    return filteredWeeklyEntries;
        //}

        //#endregion

        //#region Check For Post Shift Rest

        ///// <summary>
        ///// Checks if worker is valid for this entry in case of post shift rest
        ///// </summary>
        ///// <param name="currentScheduleEntry"></param>
        ///// <param name="previousShiftEntries"></param>
        ///// <param name="entityWorkerMemberDTO"></param>
        ///// <param name="postShiftRestRule"></param>
        ///// <returns></returns>
        //private bool CheckForPostShiftRest(ScheduleEntryDTO currentScheduleEntry, IEnumerable<ScheduleEntryDTO> previousShiftEntries, EntityWorkerMemberDTO entityWorkerMemberDTO, EntityRuleDTO? postShiftRestRule)
        //{
        //    if (postShiftRestRule != null)
        //    {
        //        // For each shift post rest rule specification 
        //        foreach (EntityRuleSpecificationDTO entityRuleSpecificationDTO in postShiftRestRule.EntityRuleSpecificationDTOs)
        //        {
        //            if (entityRuleSpecificationDTO.BusinessAspectId != null && entityRuleSpecificationDTO.RuleSpecificationValue != null)
        //            {
        //                // Check entries where the worker is part of shifts before the current entry
        //                ScheduleEntryDTO lastPreCurrentScheduleEntry = previousShiftEntries.Where(i => i.ShiftId.Equals(entityRuleSpecificationDTO.BusinessAspectId)
        //            && i.ScheduleStartDate < currentScheduleEntry.ScheduleStartDate
        //            && i.ScheduleParticipants.Any(j => j.WorkerId.Equals(entityWorkerMemberDTO.WorkerId))).OrderByDescending(i => i.ScheduleStartDate).FirstOrDefault();

        //                if (lastPreCurrentScheduleEntry != null)
        //                {
        //                    // Check if from the end of the schedule entry, plus the time of rest, if it overlaps the current entry, if so it cannot happen
        //                    if (lastPreCurrentScheduleEntry.ScheduleEndDate + TimeSpan.FromHours(entityRuleSpecificationDTO.RuleSpecificationValue) >= currentScheduleEntry.ScheduleStartDate)
        //                        return false;
        //                }
        //            }
        //        }
        //    }

        //    return true;
        //}

        //#endregion

        //#region Filter Entries for Consecutive Turns

        ///// <summary>
        ///// Checks and filters entries both to be generated or existing in the db
        ///// for consecutive turns validations
        ///// </summary>
        ///// <param name="currentScheduleEntry"></param>
        ///// <param name="scheduleEntryDTOs"></param>
        ///// <param name="createEntityScheduleDTO"></param>
        ///// <param name="ruleSpecificationValue"></param>
        ///// <returns></returns>
        //private async Task<List<ScheduleEntryDTO>> FilterEntriesForConsecutiveTurns(ScheduleEntryDTO currentScheduleEntry, List<ScheduleEntryDTO> scheduleEntryDTOs, CreateEntityScheduleDTO createEntityScheduleDTO, int ruleSpecificationValue)
        //{
        //    List<ScheduleEntryDTO> filteredEntries = new List<ScheduleEntryDTO>();
            
        //    // Get the previous shift entries
        //    IEnumerable<ScheduleEntryDTO> previousShiftEntries = scheduleEntryDTOs.Where(i => i.ScheduleStartDate < currentScheduleEntry.ScheduleStartDate && i.ShiftId.Equals(currentScheduleEntry.ShiftId)).OrderByDescending(i => i.ScheduleStartDate);

        //    filteredEntries = filteredEntries.Concat(previousShiftEntries).ToList();

        //    // if there are entries and are greater than the rule specification value
        //    if (ruleSpecificationValue != 0 && previousShiftEntries.Count() > 0 && previousShiftEntries.Count() < ruleSpecificationValue)
        //    {
        //        int daysToGoBack = ruleSpecificationValue - previousShiftEntries.Count();
        //        DateTime startDate = previousShiftEntries.Last().ScheduleStartDate.AddDays(-daysToGoBack);
        //        DateTime endDate = previousShiftEntries.Last().ScheduleStartDate;

        //        ScheduleViewModelRequestDTO requestDTO = new ScheduleViewModelRequestDTO
        //        {
        //            EntityId = createEntityScheduleDTO.EntityId,
        //            WorkerId = createEntityScheduleDTO.WorkerId,
        //            LanguageCode = createEntityScheduleDTO.LanguageCode,
        //            StartDateSearch = startDate,
        //            EndDateSearch = endDate
        //        };

        //        filteredEntries = filteredEntries.Concat(await _entityScheduleService.GetScheduleEntries(requestDTO)).ToList();
        //    }

        //    return filteredEntries.OrderByDescending(i => i.ScheduleStartDate).ToList();
        //}

        //#endregion

        #endregion
    }
}
