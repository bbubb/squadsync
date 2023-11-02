using AutoMapper;
using SquadSync.Data.Models;
using SquadSync.DTOs;

namespace SquadSync.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile() 
        {
            CreateMap<UserModel, UserDto>();
        }
    }
}
