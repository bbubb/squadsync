using AutoMapper;
using SquadSync.Data.Models;
using SquadSync.DTOs;

namespace SquadSync.MappingProfiles
{
    public class RoleRequestMappingProfile : Profile
    {
        public RoleRequestMappingProfile()
        {
            CreateMap<RoleRequest, RoleRequestDto>();
        }
    }
}
