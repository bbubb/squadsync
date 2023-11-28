

using System.Net;

namespace SquadSync.Data.Models
{
    public class Location
    {
        public Guid LocationGuid { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string? LocationDescription { get; set; }
        public string Address { get; set; }
        public GlobalProxySelection? GPSCoordinates { get; set; }
        public string? LocationImage { get; set; }  // Map image
        public ICollection<Site> Sites { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
