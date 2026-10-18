namespace SquadSync.Application.Rosters;

public sealed class RosterValidationException(string field, ArgumentException innerException)
    : Exception("Roster input is invalid.", innerException)
{
    public string Field { get; } = field;
}
