namespace SquadSync.Application.Teams;

public sealed class TeamValidationException(string field, ArgumentException innerException)
    : Exception("A team name is invalid.", innerException)
{
    public string Field { get; } = field;
}
