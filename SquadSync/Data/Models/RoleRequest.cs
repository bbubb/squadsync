using System.ComponentModel.DataAnnotations.Schema;
using SquadSync.Enums;

namespace SquadSync.Data.Models
{
    public class RoleRequest
    {
        // Stores information about a role request, including who made the request and for which team.
        // Used to track the status of the request.
        public Guid Guid { get; set; }
        public long RoleRequestId { get; set; }
        public long RoleId { get; set; }
        public virtual Role Role { get; set; }
        public long UserId { get; set; }
        public virtual User User { get; set; }
        public long OrgUnitId { get; set; }
        public virtual OrgUnit OrgUnit { get; set; }
        public RoleRequestStatusEnum Status { get; set; }
        public string? Reason { get; set; }
        public DateTime RequestedOn { get; set; }
        public DateTime? RespondedOn { get; set; }
        public DateTime? ArchivedOn{ get; set; }
    }
}
