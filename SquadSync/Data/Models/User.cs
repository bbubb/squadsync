using SquadSync.Enums;
using System.ComponentModel.DataAnnotations;

namespace SquadSync.Data.Models
{
    public class User
    {
        public Guid Guid { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string EmailNormalized { get; set; } // Derived from Email
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public UserStatusEnum UserStatus { get; set; }
        public IList<Role>? Roles { get; set; }
        public IList<RoleRequest>? RoleRequests { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
