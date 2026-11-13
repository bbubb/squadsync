using SquadSync.Domain;

namespace SquadSync.Application.Rosters;

public sealed record NewPlayerPerson(string FirstName, string LastName);

public sealed class AddPlayer(IAddPlayerPersistence persistence)
{
    public async Task<RosterDetails> ExecuteAsync(Guid teamId, Guid? personId, NewPlayerPerson? newPerson,
        string? jerseyNumber, RosterStatus rosterStatus, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Person? person = null;
        try
        {
            if (teamId == Guid.Empty) throw new ArgumentException("A non-empty ID is required.", nameof(teamId));
            if (personId.HasValue == (newPerson is not null))
                throw new ArgumentException("Exactly one Person variant is required.", nameof(personId));
            if (personId == Guid.Empty) throw new ArgumentException("A non-empty ID is required.", nameof(personId));
            if (newPerson is not null)
                person = new Person(Guid.NewGuid(), newPerson.FirstName, newPerson.LastName);
            _ = new RosterEntry(Guid.NewGuid(), Guid.NewGuid(), jerseyNumber, rosterStatus);
        }
        catch (ArgumentException exception)
        {
            throw new RosterValidationException(exception.ParamName!, exception);
        }

        await using var transaction = await persistence.BeginAsync(cancellationToken);
        if (!await transaction.TeamExistsAsync(teamId, cancellationToken)) throw new RosterTeamNotFoundException();
        person ??= await transaction.GetPersonAsync(personId!.Value, cancellationToken)
            ?? throw new AddPlayerPersonNotFoundException();
        var existing = await transaction.GetMembershipAsync(teamId, person.Id, cancellationToken);
        if (existing is not null && existing.TeamRole != TeamRole.Player)
            throw new RosterMembershipNotPlayerException(existing.Id, existing.TeamRole);
        var membership = existing ?? new TeamMembership(Guid.NewGuid(), person.Id, teamId, TeamRole.Player);
        var entry = new RosterEntry(Guid.NewGuid(), membership.Id, jerseyNumber, rosterStatus);
        await transaction.SaveAsync(newPerson is null ? null : person, existing is null ? membership : null, entry, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new RosterDetails(entry.Id, membership.Id,
            new RosterPerson(person.Id, person.FirstName, person.LastName), entry.JerseyNumber, entry.RosterStatus);
    }
}

public sealed class AddPlayerPersonNotFoundException() : Exception("The referenced Person was not found.");
public sealed class AddPlayerConflictException(Exception? innerException = null)
    : Exception("The player could not be added because of a conflicting write.", innerException);
