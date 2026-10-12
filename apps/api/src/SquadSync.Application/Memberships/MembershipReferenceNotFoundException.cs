namespace SquadSync.Application.Memberships;

public sealed class MembershipReferenceNotFoundException(Exception? innerException = null)
    : Exception("The referenced team or person was not found.", innerException);
