using AutoMapper;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects;
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

            CreateMap<NewEntityDTO, Entity>()
                .ForMember(dest => dest.EntityTypeId, opt => opt.MapFrom(src => src.EntityTypeId))
                .ForMember(dest => dest.EntityName, opt => opt.MapFrom(src => src.EntityName))
                .ForMember(dest => dest.EntityDescription, opt => opt.MapFrom(src => src.EntityDescription));

        }
    }
}
