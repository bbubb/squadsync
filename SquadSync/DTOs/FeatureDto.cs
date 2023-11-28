namespace SquadSync.DTOs;

public class FeatureDto
{
    public Guid Guid { get; set; }
    public string FeatureName { get; set; }
    public string? FeatureDescription { get; set; }
    public bool IsActiveFeature { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ArchivedOn { get; set; }
}