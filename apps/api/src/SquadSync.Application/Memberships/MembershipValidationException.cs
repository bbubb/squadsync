namespace SquadSync.Application.Memberships;

public sealed class MembershipValidationException(string field, ArgumentException innerException)
    : Exception("Membership input is invalid.", innerException)
{
    public string Field { get; } = field;
}
