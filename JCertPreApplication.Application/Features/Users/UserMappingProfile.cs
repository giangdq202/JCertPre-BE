using AutoMapper;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Features.Users
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, AppUserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.userId))
                .ForMember(dest => dest.fullName, opt => opt.MapFrom(src => src.fullName))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.email))
                .ForMember(dest => dest.phone, opt => opt.MapFrom(src => src.phone))
                .ForMember(dest => dest.avatarUrl, opt => opt.MapFrom(src => src.avatarUrl))
                .ForMember(dest => dest.credit, opt => opt.MapFrom(src => src.credit))
                .ForMember(dest => dest.createdAt, opt => opt.MapFrom(src => src.createdAt))
                .ForMember(dest => dest.lastLogin, opt => opt.MapFrom(src => src.lastLogin))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.status))
                .ForMember(dest => dest.roleId, opt => opt.MapFrom(src => src.roleId))
                .ForMember(dest => dest.roleName, opt => opt.MapFrom(src => src.Role.roleName));

            CreateMap<Role, RoleDto>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.roleId))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.roleName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description));
        }
    }
} 