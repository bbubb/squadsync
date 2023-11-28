namespace SquadSync.Data.Models;

public class Feature 
{
    public Guid FeatureGuid { get; set; }
    public long FeatureId { get; set; }
    public string FeatureName { get; set; }
    public string? FeatureDescription { get; set; }
    public bool IsActiveFeature { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ArchivedOn { get; set; }
}