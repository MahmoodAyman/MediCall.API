using System;
using AutoMapper;
using Core.DTOs.Certificate;
using Core.DTOs.Illness;
using Core.DTOs.Nurse;
using Core.Models;

namespace Core.Mapping;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Certificate, CertificateDetailsDto>().ReverseMap();


        CreateMap<Certificate, CreatedCertificateDto>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
        .ReverseMap();

        CreateMap<Certificate, CertificateDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
        .ReverseMap();

        CreateMap<Certificate, UpdatedCertificateDto>().ReverseMap();

        CreateMap<Illness, IllnessDto>().ReverseMap();
        CreateMap<Nurse, NurseDto>().ReverseMap();
        CreateMap<Nurse, CreatedNurseDto>().ReverseMap();
    }
}
