namespace SquadSync.Application.PlayerProfiles;

public sealed class PlayerProfileNotFoundException(
    Exception? innerException = null)
    : Exception("Player profile no longer exists.", innerException);