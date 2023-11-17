using AutoMapper;
using SquadSync.Data.Models;
using SquadSync.Data.Repositories.IRepositories;
using SquadSync.DTOs.Requests;
using SquadSync.DTOs.Responses;
using SquadSync.Enums;
using SquadSync.Services.IServices;
using SquadSync.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SquadSync.Services
{
    public class OrgUnitService : IOrgUnitService
    {
        readonly IOrgUnitRepository _orgUnitRepository;
        readonly IMapper _mapper;
        readonly ILogger<OrgUnitService> _logger;


        public OrgUnitService(
            IOrgUnitRepository orgUnitRepository,
            IMapper mapper,
            ILogger<OrgUnitService> logger)
        {
            _orgUnitRepository = orgUnitRepository ?? throw new ArgumentException(nameof(orgUnitRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ServiceResult<OrgUnitDto>> ArchiveOrgUnitAsync(Guid orgGuid)
        {
            _logger.LogInformation("OrgUnitService: Archiving OrgUnit with Guid: {orgGuid}", orgGuid);
            try
            {
                var orgUnit = await _orgUnitRepository.GetByGuidAsync(orgGuid);

                if (orgUnit == null)
                {
                    _logger.LogWarning("OrgUnitService: OrgUnit with Guid: {orgGuid} not found", orgGuid);
                    return ServiceResult<OrgUnitDto>.Failure($"OrgUnit with Guid: {orgGuid} not found");
                }
                if (orgUnit.OrgUnitStatus == OrgUnitStatusEnum.RegisteredArchived ||
                    orgUnit.OrgUnitStatus == OrgUnitStatusEnum.UnregisteredArchived)
                {
                    _logger.LogWarning("OrgUnitService: OrgUnit with Guid: {orgGuid} already archived", orgGuid);
                    return ServiceResult<OrgUnitDto>.Failure($"OrgUnitService: OrgUnit with Guid: {orgGuid} already archived");
                }
                if (orgUnit.ArchivedOn != null)
                {
                    _logger.LogWarning("OrgUnitService: OrgUnit with Guid: {orgGuid} already archived", orgGuid);
                    return ServiceResult<OrgUnitDto>.Failure($"OrgUnitService: OrgUnit with Guid: {orgGuid} already archived");
                }

                orgUnit.ArchivedOn = DateTime.UtcNow;
                // Consider how to handle Archiving considering Registered or Unregistered
                orgUnit.OrgUnitStatus = OrgUnitStatusEnum.RegisteredArchived;

                await _orgUnitRepository.UpdateAsync(orgUnit);

                _logger.LogInformation("OrgUnitService: Finished archiving OrgUnit with Guid: {orgGuid}", orgGuid);
                return ServiceResult<OrgUnitDto>.SuccessResult(_mapper.Map<OrgUnitDto>(orgUnit));
            }
            catch (Exception ex)
            {
                _logger.LogError("OrgUnitService: An error occurred while processing your request: {Message}", ex.Message);
                return ServiceResult<OrgUnitDto>.Failure("An error occurred while processing your request.");
            }
        }

        public async Task<ServiceResult<OrgUnitDto>> CreateOrgUnitAsync(OrgUnitCreateRequestDto dto)
        {
            _logger.LogInformation("OrgUnitService: Creating OrgUnit with Name: {orgUnitName}", dto.OrgUnitName);
            try
            {
                var orgUnit = _mapper.Map<OrgUnit>(dto);

                orgUnit.Guid = Guid.NewGuid();
                orgUnit.CreatedOn = DateTime.UtcNow;
                orgUnit.OrgUnitStatus = OrgUnitStatusEnum.RegisteredPending;

                await _orgUnitRepository.CreateAsync(orgUnit);

                _logger.LogInformation("OrgUnitService: Finished creating OrgUnit with Name: {orgUnitName}", dto.OrgUnitName);
                return ServiceResult<OrgUnitDto>.SuccessResult(_mapper.Map<OrgUnitDto>(orgUnit));
            }
            catch (Exception ex)
            {
                _logger.LogError("OrgUnitService: An error occurred while processing your request: {Message}", ex.Message);
                return ServiceResult<OrgUnitDto>.Failure("An error occurred while processing your request.");
            }
        }

        public async Task<ServiceResult> DeleteOrgUnitAsync(Guid orgGuid)
        {
            _logger.LogInformation("OrgUnitService: Deleting OrgUnit with Guid: {orgGuid}", orgGuid);
            try
            {
                var orgUnit = await _orgUnitRepository.GetByGuidAsync(orgGuid);

                if (orgUnit == null)
                {
                    _logger.LogWarning("OrgUnitService: OrgUnit with Guid: {orgGuid} not found", orgGuid);
                    return ServiceResult.Failure($"OrgUnit with Guid: {orgGuid} not found");
                }

                await _orgUnitRepository.DeleteAsync(orgUnit);

                _logger.LogInformation("OrgUnitService: Finished deleting OrgUnit with Guid: {orgGuid}", orgGuid);
                return ServiceResult.SuccessResult();
            }
            catch (Exception ex)
            {
                _logger.LogError("OrgUnitService: An error occurred while processing your request: {Message}", ex.Message);
                return ServiceResult.Failure("An error occurred while processing your request.");
            }
        }

        public async Task<ServiceResult<IEnumerable<OrgUnitDto>>> GetAllOrgUnitDtosAsync()
        {
            _logger.LogInformation("OrgUnitService: Getting all OrgUnits");
            try
            {
                var orgUnits = await _orgUnitRepository.GetAllAsync();

                _logger.LogInformation("OrgUnitService: Finished getting all OrgUnits");
                return ServiceResult<IEnumerable<OrgUnitDto>>.SuccessResult(_mapper.Map<IEnumerable<OrgUnitDto>>(orgUnits));
            }
            catch (Exception ex)
            {
                _logger.LogError("OrgUnitService: An error occurred while processing your request: {Message}", ex.Message);
                return ServiceResult<IEnumerable<OrgUnitDto>>.Failure("An error occurred while processing your request.");
            }
        }

        public async Task<ServiceResult<OrgUnitDto>> GetOrgUnitByGuidAsync(Guid orgGuid)
        {
            _logger.LogInformation("OrgUnitService: Getting OrgUnit with Guid: {orgGuid}", orgGuid);
            try
            {
                var orgUnit = await _orgUnitRepository.GetByGuidAsync(orgGuid);

                if (orgUnit == null)
                {
                    _logger.LogWarning("OrgUnitService: OrgUnit with Guid: {orgGuid} not found", orgGuid);
                    return ServiceResult<OrgUnitDto>.Failure($"OrgUnit with Guid: {orgGuid} not found");
                }

                _logger.LogInformation("OrgUnitService: Finished getting OrgUnit with Guid: {orgGuid}", orgGuid);
                return ServiceResult<OrgUnitDto>.SuccessResult(_mapper.Map<OrgUnitDto>(orgUnit));
            }
            catch (Exception ex)
            {
                _logger.LogError("OrgUnitService: An error occurred while processing your request: {Message}", ex.Message);
                return ServiceResult<OrgUnitDto>.Failure("An error occurred while processing your request.");
            }
        }

        // This might need to be a call from UserService
        // Create method in OrgUnitRepository or UserService?
        //public async Task<ServiceResult<IEnumerable<OrgUnitDto>>> GetOrgUnitDtosByUserGuidAsync(Guid userGuid)
        //{
        //    _logger.LogInformation("OrgUnitService: Getting OrgUnits with User Guid: {userGuid}", userGuid);
        //    try
        //    {
        //        var orgUnits = await _orgUnitRepository.GetByUserGuidAsync(userGuid);

        //        if (orgUnits == null)
        //        {
        //            _logger.LogWarning("OrgUnitService: OrgUnits with User Guid: {userGuid} not found", userGuid);
        //            return ServiceResult<OrgUnitDto>.Failure($"OrgUnits with User Guid: {userGuid} not found");
        //        }

        //        _logger.LogInformation("OrgUnitService: Finished getting OrgUnits with User Guid: {userGuid}", userGuid);
        //        return ServiceResult<OrgUnitDto>.SuccessResult(_mapper.Map<IEnumerable<OrgUnitDto>>(orgUnits));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("OrgUnitService: An error occurred while processing your request: {Message}", ex.Message);
        //        return ServiceResult<OrgUnitDto>.Failure("An error occurred while processing your request.");
        //    }
        //}

        public async Task<ServiceResult<OrgUnitDto>> UpdateOrgUnitAsync(Guid requestUserGuid, OrgUnitUpdateRequestDto dto)
        {
            _logger.LogInformation("OrgUnitService: Updating OrgUnit with Guid: {orgGuid}", dto.Guid);
            try
            {
                var orgUnit = await _orgUnitRepository.GetByGuidAsync(dto.Guid);

                if (orgUnit == null)
                {
                    _logger.LogWarning("OrgUnitService: OrgUnit with Guid: {orgGuid} not found", dto.Guid);
                    return ServiceResult<OrgUnitDto>.Failure($"OrgUnit with Guid: {dto.Guid} not found");
                }

                // Can I user AutoMapper here?
                // Consider which User/Role can modify which properties
                //  **Maybe only the owner for now** Might need special method to transfer ownership
                if (orgUnit.Owner.Guid != requestUserGuid)
                {
                    _logger.LogWarning("OrgUnitService: OrgUnit with Guid: {orgGuid} can only be modified by the owner", dto.Guid);
                    return ServiceResult<OrgUnitDto>.Failure($"OrgUnit with Guid: {dto.Guid} can only be modified by the owner");
                }
                orgUnit.OrgUnitName = dto.OrgUnitName ?? orgUnit.OrgUnitName;
                orgUnit.OrgUnitDescription = dto.OrgUnitDescription ?? orgUnit.OrgUnitDescription;
                orgUnit.OrgUnitStatus = Enum.Parse<OrgUnitStatusEnum>(dto.OrgUnitStatus ?? orgUnit.OrgUnitStatus.ToString());
                orgUnit.Owner = _mapper.Map<User>(dto.Owner) ?? orgUnit.Owner;
                orgUnit.Users = _mapper.Map<IList<User>>(dto.Users) ?? orgUnit.Users;
                orgUnit.Roles = _mapper.Map<IList<Role>>(dto.Roles) ?? orgUnit.Roles;
                orgUnit.RoleRequests = _mapper.Map<IList<RoleRequest>>(dto.RoleRequests) ?? orgUnit.RoleRequests;
                orgUnit.Modified = DateTime.UtcNow;

                await _orgUnitRepository.UpdateAsync(orgUnit);

                _logger.LogInformation("OrgUnitService: Finished updating OrgUnit with Guid: {orgGuid}", dto.Guid);
                return ServiceResult<OrgUnitDto>.SuccessResult(_mapper.Map<OrgUnitDto>(orgUnit));
            }
            catch (Exception ex)
            {
                _logger.LogError("OrgUnitService: An error occurred while processing your request: {Message}", ex.Message);
                return ServiceResult<OrgUnitDto>.Failure("An error occurred while processing your request.");
            }
        }
    }
}
