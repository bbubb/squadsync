using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SquadSync.Application.People;

namespace SquadSync.Api.People;

public sealed class PersonCandidateRequest
{
    [Required]
    [FromQuery(Name = "q")]
    public string? Q { get; init; }

    [Range(1, 20)]
    [FromQuery(Name = "limit")]
    public int Limit { get; init; } = 20;

    [StringLength(GetPersonCandidates.MaximumCursorLength)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    [FromQuery(Name = "cursor")]
    public string? Cursor { get; init; }
}
