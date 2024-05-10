using AutoMapper;
using ShiftSchedularEntity.Entities;
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
        }
    }
}
