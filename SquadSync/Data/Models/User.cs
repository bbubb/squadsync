using SquadSync.Enums;
using System.ComponentModel.DataAnnotations;

namespace SquadSync.Data.Models
{
    public class User
    {
        public Guid Guid { get; set; }
        [Key]
        public long UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
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
        public DateTime? ModifiedOn { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
