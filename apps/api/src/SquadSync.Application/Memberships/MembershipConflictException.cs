namespace SquadSync.Application.Memberships;

public sealed class MembershipConflictException(Exception? innerException = null)
    : Exception("The person already has a membership in this team.", innerException);
