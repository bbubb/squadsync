using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SquadSync.Application.People;

public sealed class PersonCandidateValidationException(string field, string message) : Exception(message)
{
    public string Field { get; } = field;
}

public sealed class GetPersonCandidates(IPersonCandidateQueries queries)
{
    public const int MaximumCursorLength = 4096;

    public async Task<PersonCandidatePage> ExecuteAsync(string? q, int limit = 20, string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var query = q?.Trim();
        if (query is null || query.Length is < 2 or > 100)
            throw new PersonCandidateValidationException("q", "Enter a name query between 2 and 100 characters.");
        if (limit is < 1 or > 20)
            throw new PersonCandidateValidationException("limit", "Limit must be between 1 and 20.");

        var binding = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(query)));
        var after = cursor is null ? null : Decode(cursor, binding);
        var rows = await queries.SearchAsync(query, limit + 1, after, cancellationToken);
        var items = rows.Take(limit).Select(row => row.Candidate).ToArray();
        var next = rows.Count > limit ? Encode(new Cursor(1, binding, rows[limit - 1].Position)) : null;
        return new PersonCandidatePage(items, next);
    }

    private static string Encode(Cursor value)
    {
        var token = Serialize(value);
        // Person names have no persisted length limit. Keep even these cursors bounded;
        // Infrastructure can resolve the same normalized position by its unique Person ID.
        return token.Length <= MaximumCursorLength ? token : Serialize(value with
        {
            Position = new PersonCandidatePosition(null, null, value.Position.PersonId)
        });
    }

    private static string Serialize(Cursor value)
        => Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(value)).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static PersonCandidatePosition Decode(string token, string binding)
    {
        try
        {
            if (token.Length is 0 or > MaximumCursorLength || token.Any(character =>
                !char.IsAsciiLetterOrDigit(character) && character != '-' && character != '_'))
                throw new FormatException();
            var base64 = token.Replace('-', '+').Replace('_', '/');
            base64 = base64.PadRight((base64.Length + 3) / 4 * 4, '=');
            var cursor = JsonSerializer.Deserialize<Cursor>(Convert.FromBase64String(base64));
            if (cursor is null || cursor.Version != 1 || cursor.Query != binding || cursor.Position is null ||
                cursor.Position.PersonId == Guid.Empty || cursor.Position.FirstName == "" || cursor.Position.LastName == "" ||
                (cursor.Position.FirstName is null) != (cursor.Position.LastName is null))
                throw new FormatException();
            return cursor.Position;
        }
        catch (Exception exception) when (exception is FormatException or JsonException)
        {
            throw new PersonCandidateValidationException("cursor", "The continuation cursor is invalid for this query.");
        }
    }

    private sealed record Cursor(int Version, string Query, PersonCandidatePosition Position);
}
