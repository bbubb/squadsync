namespace SquadSync.Data.Models
{
    public class Site
    {
        public Guid SiteGuid { get; set; }
        public long SiteId { get; set; }
        public string SiteName { get; set; }
        public string? SiteDescription { get; set; }
        public virtual Location Location { get; set; }
        public long LocationId { get; set; }
        public string? Dimensions { get; set; }
        public string? SurfaceType { get; set; }
        public string? Amenities { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? ArchivedOn { get; set; }
    }
}
