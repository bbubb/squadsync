namespace SquadSync.DTOs.Requests
{
    public class UserUpdateRequestDto
    {
        public Guid UserGuid { get; set; }
        public string? UserName { get; set; } // Maybe controlled somewhere else
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public IList<RoleDto>? Roles { get; set; }
        public IList<RoleRequestDto>? RoleRequests { get; set; }
        public string? RoleStatus { get; set; } // Maybe not needed
    }
}
