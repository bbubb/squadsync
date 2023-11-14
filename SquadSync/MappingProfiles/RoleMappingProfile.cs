using AutoMapper;
using SquadSync.Data.Models;
using SquadSync.DTOs;
using SquadSync.DTOs.Requests;

namespace SquadSync.MappingProfiles
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<Role, RoleDto>();
            CreateMap<Role, RoleCreateRequestDto>();
        }
    }
}
