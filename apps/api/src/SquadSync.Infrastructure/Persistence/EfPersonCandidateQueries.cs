using Microsoft.EntityFrameworkCore;
using SquadSync.Application.People;

namespace SquadSync.Infrastructure.Persistence;

public sealed class EfPersonCandidateQueries(SquadSyncDbContext context) : IPersonCandidateQueries
{
    public async Task<IReadOnlyList<PersonCandidateRow>> SearchAsync(string query, int take,
        PersonCandidatePosition? after, CancellationToken cancellationToken = default)
    {
        // Treat wildcard characters as literal name text, including backslash itself.
        var pattern = "%" + query.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";
        var matches = context.People.AsNoTracking()
            .Where(person => EF.Functions.ILike(person.FirstName, pattern, "\\") ||
                EF.Functions.ILike(person.LastName, pattern, "\\") ||
                EF.Functions.ILike(person.FirstName + " " + person.LastName, pattern, "\\"))
            .Select(person => new
            {
                person.Id, person.FirstName, person.LastName,
                SortLast = EF.Functions.Collate(person.LastName.ToLower(), "C"),
                SortFirst = EF.Functions.Collate(person.FirstName.ToLower(), "C")
            });
        if (after is { LastName: null, FirstName: null })
        {
            // Resolve unusually long cursor keys in this same SQL statement, without
            // fetching an extra row or depending on CLR normalization. No snapshot is promised.
            var anchor = context.People.Where(person => person.Id == after.PersonId).Select(person => new
            {
                SortLast = EF.Functions.Collate(person.LastName.ToLower(), "C"),
                SortFirst = EF.Functions.Collate(person.FirstName.ToLower(), "C")
            });
            matches = from person in matches
                      from position in anchor
                      where string.Compare(person.SortLast, position.SortLast) > 0 ||
                          (person.SortLast == position.SortLast && string.Compare(person.SortFirst, position.SortFirst) > 0) ||
                          (person.SortLast == position.SortLast && person.SortFirst == position.SortFirst && person.Id.CompareTo(after.PersonId) > 0)
                      select person;
        }
        else if (after is not null)
        {
            matches = matches.Where(person => string.Compare(person.SortLast, after.LastName) > 0 ||
                (person.SortLast == after.LastName && string.Compare(person.SortFirst, after.FirstName) > 0) ||
                (person.SortLast == after.LastName && person.SortFirst == after.FirstName && person.Id.CompareTo(after.PersonId) > 0));
        }
        return await matches.OrderBy(person => person.SortLast).ThenBy(person => person.SortFirst).ThenBy(person => person.Id)
            .Take(take)
            .Select(person => new PersonCandidateRow(new PersonCandidate(person.Id, person.FirstName, person.LastName),
                new PersonCandidatePosition(person.SortLast, person.SortFirst, person.Id)))
            .ToListAsync(cancellationToken);
    }
}
