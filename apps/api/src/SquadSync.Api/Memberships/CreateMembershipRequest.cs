using System.ComponentModel.DataAnnotations;

namespace SquadSync.Api.Memberships;

public sealed class CreateMembershipRequest
{
    [Required]
    public Guid? PersonId { get; init; }

    [Required]
    [RegularExpression("^(Owner|Coach|AssistantCoach|Manager|Player|Viewer)$",
        ErrorMessage = "A canonical team role name is required.")]
    public string? TeamRole { get; init; }
}
