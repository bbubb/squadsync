namespace SquadSync.Application.PlayerProfiles;

public sealed class PlayerProfilePersonNotFoundException(Exception? innerException = null)
    : Exception("The referenced person was not found.", innerException);
