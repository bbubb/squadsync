namespace SquadSync.Application.People;

public sealed record PersonCandidate(Guid PersonId, string FirstName, string LastName);

public sealed record PersonCandidatePage(IReadOnlyList<PersonCandidate> Items, string? NextCursor);

// Keys are normalized by the database, using the same expressions as its ordering.
// Null keys reference the Person's database sort position when names cannot fit in a bounded cursor.
public sealed record PersonCandidatePosition(string? LastName, string? FirstName, Guid PersonId);

public sealed record PersonCandidateRow(PersonCandidate Candidate, PersonCandidatePosition Position);

public interface IPersonCandidateQueries
{
    Task<IReadOnlyList<PersonCandidateRow>> SearchAsync(string query, int take,
        PersonCandidatePosition? after, CancellationToken cancellationToken = default);
}
