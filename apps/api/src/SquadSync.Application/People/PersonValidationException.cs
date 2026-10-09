namespace SquadSync.Application.People;

public sealed class PersonValidationException(string field, ArgumentException innerException)
    : Exception("A person name is invalid.", innerException)
{
    public string Field { get; } = field;
}
