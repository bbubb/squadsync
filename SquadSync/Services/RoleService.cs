using AutoMapper;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.DTOs;
using SquadSync.DTOs.Requests;
using SquadSync.Services.IServices;
using SquadSync.Utilities;

namespace SquadSync.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IRoleRepository roleRepository, IMapper mapper, ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<ServiceResult<RoleDto>> AddRoleToUserAsync(Guid userGuid, RoleCreateRequestDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
