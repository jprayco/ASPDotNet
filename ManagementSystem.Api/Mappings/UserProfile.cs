using AutoMapper;
using ManagementSystem.Api.Models.DTOs.Request;
using ManagementSystem.Api.Models.DTOs.Response;
using ManagementSystem.Api.Models.Entities;
namespace ManagementSystem.Api.Mappings;

public class UserProfile : Profile
{
        public UserProfile()
    {
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore());   
              
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Roles,
                opt => opt.MapFrom(src => src.Roles.Select(r => r.Name)));
    }
}
