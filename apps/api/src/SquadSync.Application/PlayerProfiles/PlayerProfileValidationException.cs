namespace SquadSync.Application.PlayerProfiles;

public sealed class PlayerProfileValidationException(string field, ArgumentException innerException)
    : Exception("Player profile input is invalid.", innerException)
{
    public string Field { get; } = field;
}
