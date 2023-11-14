using AutoMapper;
using SquadSync.Data.Models;
using SquadSync.DTOs.Responses;
using SquadSync.DTOs.Requests;

namespace SquadSync.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile() 
        {
            CreateMap<User, UserResponseDto>();
            CreateMap<User, UserCreateRequestDto>();
            CreateMap<User, UserUpdateRequestDto>();
        }
    }
}
