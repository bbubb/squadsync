namespace SquadSync.DTOs
{
    public class UserDto
    {
        public Guid Guid { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string EmailNormalized { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string UserStatus { get; set; }
        //public RoleDto RoleDto { get; set; }
    }
}
