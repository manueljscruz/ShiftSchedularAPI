using AutoMapper;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.QueryModels;
using ShiftSchedularEntity.Models.Responses;

namespace ShiftSchedularIL.Mappers
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<GenderLocalization, GenderLocalizedDTO>()
                .ForMember(dest => dest.GenderId, opt => opt.MapFrom(src => src.GenderId))
                .ForMember(dest => dest.GenderLocalizedName, opt => opt.MapFrom(src => src.GenderDisplayValue));

            CreateMap<SkillLocalization, SkillLocalizedDTO>()
                .ForMember(dest => dest.SkillId, opt => opt.MapFrom(src => src.SkillId))
                .ForMember(dest => dest.SkillLocalizedName, opt => opt.MapFrom(src => src.SkillDisplayValue));

            CreateMap<NewWorkerDTO, Worker>()
                .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.WorkerName))
                .ForMember(dest => dest.GenderId, opt => opt.MapFrom(src => src.GenderId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<Worker, WorkerDTO>()
                .ForMember(dest => dest.WorkerId, opt => opt.MapFrom(src => src.WorkerId))
                .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.WorkerName))
                .ForMember(dest => dest.GenderId, opt => opt.MapFrom(src => src.GenderId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<EntityTypeLocalization, EntityTypeLocalizedDTO>()
                .ForMember(dest => dest.EntityTypeId, opt => opt.MapFrom(src => src.EntityTypeId))
                .ForMember(dest => dest.EntityTypeLocalizedName, opt => opt.MapFrom(src => src.EntityTypeDisplayValue));

            CreateMap<FormEntityDTO, Entity>()
                .ForMember(dest => dest.EntityTypeId, opt => opt.MapFrom(src => src.EntityTypeId))
                .ForMember(dest => dest.EntityName, opt => opt.MapFrom(src => src.EntityName))
                .ForMember(dest => dest.EntityDescription, opt => opt.MapFrom(src => src.EntityDescription));

            CreateMap<EntityWorkerMemberModel, EntityWorkerMemberDTO>()
                .ForMember(dest => dest.WorkerId, opt => opt.MapFrom(src => src.WorkerId))
                .ForMember(dest => dest.WorkerName, opt => opt.MapFrom(src => src.WorkerName))
                .ForMember(dest => dest.CanCreateSchedules, opt => opt.MapFrom(src => src.CanCreateSchedules))
                .ForMember(dest => dest.IsOwner, opt => opt.MapFrom(src => src.IsOwner))
                .ForMember(dest => dest.DateOfJoin, opt => opt.MapFrom(src => src.DateOfJoin));

            CreateMap<AddShiftDTO, Shift>()
                .ForMember(dest => dest.ShiftName, opt => opt.MapFrom(src => src.ShiftName))
                .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId))
                .ForMember(dest => dest.ShiftAlias, opt => opt.MapFrom(src => src.ShiftAlias))
                .ForMember(dest => dest.ShiftDescription, opt => opt.MapFrom(src => src.ShiftDescription))
                .ForMember(dest => dest.ShiftStartHour, opt => opt.MapFrom(src => src.ShiftStartHour))
                .ForMember(dest => dest.ShiftDuration, opt => opt.MapFrom(src => src.ShiftDuration));

            CreateMap<Shift, ShiftDTO>()
                .ForMember(dest => dest.ShiftId, opt => opt.MapFrom(src => src.ShiftId))
                .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId))
                .ForMember(dest => dest.ShiftName, opt => opt.MapFrom(src => src.ShiftName))
                .ForMember(dest => dest.ShiftAlias, opt => opt.MapFrom(src => src.ShiftAlias))
                .ForMember(dest => dest.ShiftDescription, opt => opt.MapFrom(src => src.ShiftDescription))
                .ForMember(dest => dest.ShiftStartHour, opt => opt.MapFrom(src => src.ShiftStartHour))
                .ForMember(dest => dest.ShiftDuration, opt => opt.MapFrom(src => src.ShiftDuration));

            CreateMap<AddShiftBreakDTO, ShiftBreak>()
                .ForMember(dest => dest.ShiftBreakTypeId, opt => opt.MapFrom(src => src.ShiftBreakTypeId))
                .ForMember(dest => dest.ShiftBreakStartTime, opt => opt.MapFrom(src => src.ShiftBreakStartTime))
                .ForMember(dest => dest.ShiftBreakDuration, opt => opt.MapFrom(src => src.ShiftBreakDuration))
                .ForMember(dest => dest.IncludedInShift, opt => opt.MapFrom(src => src.IncludedInShift))
                .ForMember(dest => dest.IsTimeFlexible, opt => opt.MapFrom(src => src.IsTimeFlexible));

            CreateMap<ShiftDTO, Shift>()
                .ForMember(dest => dest.ShiftId, opt => opt.MapFrom(src => src.ShiftId))
                .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId))
                .ForMember(dest => dest.ShiftName, opt => opt.MapFrom(src => src.ShiftName))
                .ForMember(dest => dest.ShiftAlias, opt => opt.MapFrom(src => src.ShiftAlias))
                .ForMember(dest => dest.ShiftDescription, opt => opt.MapFrom(src => src.ShiftDescription))
                .ForMember(dest => dest.ShiftStartHour, opt => opt.MapFrom(src => src.ShiftStartHour))
                .ForMember(dest => dest.ShiftDuration, opt => opt.MapFrom(src => src.ShiftDuration));

            CreateMap<ShiftBreakDTO, ShiftBreak>()
                .ForMember(dest => dest.ShiftBreakId, opt => opt.MapFrom(src => src.ShiftBreakId))
                .ForMember(dest => dest.ShiftId, opt => opt.MapFrom(src => src.ShiftParentId))
                .ForMember(dest => dest.ShiftBreakTypeId, opt => opt.MapFrom(src => src.ShiftBreakTypeId))
                .ForMember(dest => dest.ShiftBreakStartTime, opt => opt.MapFrom(src => src.ShiftBreakStartTime))
                .ForMember(dest => dest.ShiftBreakDuration, opt => opt.MapFrom(src => src.ShiftBreakDuration))
                .ForMember(dest => dest.IncludedInShift, opt => opt.MapFrom(src => src.IncludedInShift))
                .ForMember(dest => dest.IsTimeFlexible, opt => opt.MapFrom(src => src.IsTimeFlexible));

            CreateMap<ShiftBreak, ShiftBreakDTO>()
                .ForMember(dest => dest.ShiftBreakId, opt => opt.MapFrom(src => src.ShiftBreakId))
                .ForMember(dest => dest.ShiftParentId, opt => opt.MapFrom(src => src.ShiftId))
                .ForMember(dest => dest.ShiftBreakTypeId, opt => opt.MapFrom(src => src.ShiftBreakTypeId))
                .ForMember(dest => dest.ShiftBreakStartTime, opt => opt.MapFrom(src => src.ShiftBreakStartTime))
                .ForMember(dest => dest.ShiftBreakDuration, opt => opt.MapFrom(src => src.ShiftBreakDuration))
                .ForMember(dest => dest.IncludedInShift, opt => opt.MapFrom(src => src.IncludedInShift))
                .ForMember(dest => dest.IsTimeFlexible, opt => opt.MapFrom(src => src.IsTimeFlexible));

            CreateMap<ShiftBreakTypeLocalization, ShiftBreakTypeLocalizedDTO>()
                .ForMember(dest => dest.ShiftBreakTypeId, opt => opt.MapFrom(src => src.ShiftBreakTypeId))
                .ForMember(dest => dest.ShiftBreakTypeLocalizedName, opt => opt.MapFrom(src => src.ShiftBreakTypeDisplayValue));

            CreateMap<ShiftBreakTemplate, ShiftBreakTemplateDTO>()
                .ForMember(dest => dest.ShiftBreakTemplateId, opt => opt.MapFrom(src => src.ShiftBreakTemplateId))
                .ForMember(dest => dest.ShiftBreakTemplateName, opt => opt.MapFrom(src => src.ShiftBreakTemplateName))
                .ForMember(dest => dest.ShiftBreakTypeId, opt => opt.MapFrom(src => src.ShiftBreakTypeId))
                .ForMember(dest => dest.ShiftBreakStartHour, opt => opt.MapFrom(src => src.ShiftBreakStartTime))
                .ForMember(dest => dest.ShiftBreakDuration, opt => opt.MapFrom(src => src.ShiftBreakDuration))
                .ForMember(dest => dest.IncludedInShift, opt => opt.MapFrom(src => src.IncludedInShift))
                .ForMember(dest => dest.IsTimeFlexible, opt => opt.MapFrom(src => src.IsTimeFlexible));

            CreateMap<ShiftTemplate, ShiftTemplateDTO>()
                .ForMember(dest => dest.ShiftTemplateId, opt => opt.MapFrom(src => src.ShiftTemplateId))
                .ForMember(dest => dest.ShiftTemplateName, opt => opt.MapFrom(src => src.ShiftName))
                .ForMember(dest => dest.ShiftTemplateAlias, opt => opt.MapFrom(src => src.ShiftAlias))
                .ForMember(dest => dest.ShiftStartHour, opt => opt.MapFrom(src => src.ShiftStartHour))
                .ForMember(dest => dest.ShiftDuration, opt => opt.MapFrom(src => src.ShiftDuration));

            CreateMap<ShiftBreakTemplateSubmissionModel, ShiftBreakTemplate>()
                .ForMember(dest => dest.ShiftBreakTypeId, opt => opt.MapFrom(src => src.ShiftBreakTypeId))
                .ForMember(dest => dest.ShiftBreakTemplateName, opt => opt.MapFrom(src => src.ShiftBreakTemplateName))
                .ForMember(dest => dest.ShiftBreakStartTime, opt => opt.MapFrom(src => src.ShiftBreakStartTime))
                .ForMember(dest => dest.ShiftBreakDuration, opt => opt.MapFrom(src => src.ShiftBreakDuration))
                .ForMember(dest => dest.IncludedInShift, opt => opt.MapFrom(src => src.IncludedInShift))
                .ForMember(dest => dest.IsTimeFlexible, opt => opt.MapFrom(src => src.IsTimeFlexible));

            CreateMap<ShiftTemplateSubmissionModel, ShiftTemplate>()
                .ForMember(dest => dest.ShiftName, opt => opt.MapFrom(src => src.ShiftName))
                .ForMember(dest => dest.ShiftAlias, opt => opt.MapFrom(src => src.ShiftAlias))
                .ForMember(dest => dest.ShiftStartHour, opt => opt.MapFrom(src => src.ShiftStartHour))
                .ForMember(dest => dest.ShiftDuration, opt => opt.MapFrom(src => src.ShiftDuration));

            CreateMap<RuleTypeLocalization, RuleTypeLocalizedDTO>()
                .ForMember(dest => dest.RuleTypeId, opt => opt.MapFrom(src => src.RuleTypeId))
                .ForMember(dest => dest.RuleTypeLocalizedName, opt => opt.MapFrom(src => src.RuleTypeDisplayValue));

            CreateMap<BusinessAspectLocalization, BusinessAspectLocalizedDTO>()
                .ForMember(dest => dest.BusinessAspectId, opt => opt.MapFrom(src => src.BusinessAspectId))
                .ForMember(dest => dest.BusinessAspectLocalizedName, opt => opt.MapFrom(src => src.BusinessAspectDisplayValue));

            CreateMap<AddEntityRuleDTO, EntityRule>()
                .ForMember(dest => dest.RuleTypeId, opt => opt.MapFrom(src => src.RuleTypeId))
                .ForMember(dest => dest.RuleTypeDescription, opt => opt.MapFrom(src => src.RuleTypeDescription))
                .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId));

            CreateMap<EntityRule, EntityRuleDTO>()
                .ForMember(dest => dest.EntityRuleId, opt => opt.MapFrom(src => src.EntityRuleId))
                .ForMember(dest => dest.RuleTypeId, opt => opt.MapFrom(src => src.RuleTypeId))
                .ForMember(dest => dest.RuleTypeDescription, opt => opt.MapFrom(src => src.RuleTypeDescription))
                .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.EntityId));

            CreateMap<AddEntityRuleSpecificationDTO, EntityRuleSpecification>()
                .ForMember(dest => dest.EntityRuleId, opt => opt.MapFrom(src => src.EntityRuleId))
                .ForMember(dest => dest.SpecificationValue, opt => opt.MapFrom(src => src.RuleSpecificationValue))
                .ForMember(dest => dest.AspectReferenceId, opt => opt.MapFrom(src => src.AspectReferenceId))
                .ForMember(dest => dest.BusinessAspectId, opt => opt.MapFrom(src => src.BusinessAspectId))
                .ForMember(dest => dest.AspectReferenceId2, opt => opt.MapFrom(src => src.AspectReferenceId2))
                .ForMember(dest => dest.BusinessAspectId2, opt => opt.MapFrom(src => src.BusinessAspectId2));


            CreateMap<EntityRuleSpecification, EntityRuleSpecificationDTO>()
                .ForMember(dest => dest.SpecificationId, opt => opt.MapFrom(src => src.SpecificationId))
                .ForMember(dest => dest.EntityRuleId, opt => opt.MapFrom(src => src.EntityRuleId))
                .ForMember(dest => dest.RuleSpecificationValue, opt => opt.MapFrom(src => src.SpecificationValue))
                .ForMember(dest => dest.AspectReferenceId, opt => opt.MapFrom(src => src.AspectReferenceId))
                .ForMember(dest => dest.BusinessAspectId, opt => opt.MapFrom(src => src.BusinessAspectId))
                .ForMember(dest => dest.AspectReferenceId2, opt => opt.MapFrom(src => src.AspectReferenceId2))
                .ForMember(dest => dest.BusinessAspectId2, opt => opt.MapFrom(src => src.BusinessAspectId2));

        }
    }
}
