using System.ComponentModel.DataAnnotations;

namespace SquadSync.Api.Rosters;

public sealed class CreateRosterEntryRequest
{
    [Required]
    public Guid? TeamMembershipId { get; init; }

    public string? JerseyNumber { get; init; }

    [Required]
    [RegularExpression("^(Active|Reserved|Suspended)$", ErrorMessage = "A canonical roster status name is required.")]
    public string? RosterStatus { get; init; }
}
