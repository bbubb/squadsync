using AutoMapper;
using SquadSync.Data.Models;
using SquadSync.DTOs;

namespace SquadSync.MappingProfiles
{
    public class TeamMappingProfile : Profile
    {
        public TeamMappingProfile()
        {
            CreateMap<Team, TeamDto>();
        }
    }
}
