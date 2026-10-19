namespace SquadSync.Application.PlayerProfiles;

public sealed class PlayerProfileConflictException(Exception? innerException = null)
    : Exception("The person already has a player profile.", innerException);
