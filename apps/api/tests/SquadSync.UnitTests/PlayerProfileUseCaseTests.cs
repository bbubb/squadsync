using SquadSync.Application.People;
using SquadSync.Application.PlayerProfiles;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class PlayerProfileUseCaseTests
{
    [Theory]
    [InlineData(null, null, null)]
    [InlineData(DominantFoot.Left, 70, 152.5123)]
    [InlineData(DominantFoot.Right, 1, 0.001)]
    [InlineData(DominantFoot.Both, null, null)]
    public async Task ExplicitCreate_GeneratesIdentityAndRoundTripsOptionalValues(DominantFoot? foot, int? height, double? weight)
    {
        var fake = new Persistence();
        using var source = new CancellationTokenSource();
        var result = await new CreatePlayerProfile(fake, fake).ExecuteAsync(fake.Person.Id, foot, height, (decimal?)weight, source.Token);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(new PlayerProfileDetails(result.Id, fake.Person.Id, foot, height, (decimal?)weight), result);
        Assert.Equal(result, await new GetPlayerProfile(fake).ExecuteAsync(fake.Person.Id, source.Token));
        Assert.All(fake.Tokens, token => Assert.Equal(source.Token, token));
        await Assert.ThrowsAsync<PlayerProfileConflictException>(() => new CreatePlayerProfile(fake, fake)
            .ExecuteAsync(fake.Person.Id, null, null, null));
        Assert.Equal(1, fake.Writes);
    }

    [Theory]
    [InlineData("personId")]
    [InlineData("dominantFoot")]
    [InlineData("heightInches")]
    [InlineData("weightPounds")]
    public async Task InvalidInput_IsRejectedBeforePersistence(string field)
    {
        var fake = new Persistence();
        var exception = await Assert.ThrowsAsync<PlayerProfileValidationException>(() => new CreatePlayerProfile(fake, fake)
            .ExecuteAsync(field == "personId" ? Guid.Empty : fake.Person.Id,
                field == "dominantFoot" ? (DominantFoot)99 : null,
                field == "heightInches" ? 0 : null, field == "weightPounds" ? -1 : null));
        Assert.Equal(field, exception.Field);
        Assert.Empty(fake.Tokens);
        await Assert.ThrowsAsync<PlayerProfileValidationException>(() => new GetPlayerProfile(fake).ExecuteAsync(Guid.Empty));
        Assert.Empty(fake.Tokens);
    }

    [Fact]
    public async Task MissingPersonOrProfile_DoesNotCreateImplicitly()
    {
        var fake = new Persistence();
        await Assert.ThrowsAsync<PlayerProfilePersonNotFoundException>(() => new CreatePlayerProfile(fake, fake)
            .ExecuteAsync(Guid.NewGuid(), null, null, null));
        Assert.Null(await new GetPlayerProfile(fake).ExecuteAsync(fake.Person.Id));
        Assert.Null(await new GetPlayerProfile(fake).ExecuteAsync(Guid.NewGuid()));
        Assert.Equal(0, fake.Writes);
    }

    [Fact]
    public async Task PreCancelledCalls_DoNotVisitPersistence()
    {
        var fake = new Persistence();
        var token = new CancellationToken(true);
        await Assert.ThrowsAsync<OperationCanceledException>(() => new CreatePlayerProfile(fake, fake)
            .ExecuteAsync(fake.Person.Id, null, null, null, token));
        await Assert.ThrowsAsync<OperationCanceledException>(() => new GetPlayerProfile(fake).ExecuteAsync(fake.Person.Id, token));
        Assert.Empty(fake.Tokens);
    }

    [Theory]
    [InlineData("conflict")]
    [InlineData("missing")]
    [InlineData("cancel")]
    [InlineData("failure")]
    public async Task WriteFailures_PropagateUnchanged(string kind)
    {
        Exception failure = kind switch
        {
            "conflict" => new PlayerProfileConflictException(),
            "missing" => new PlayerProfilePersonNotFoundException(),
            "cancel" => new OperationCanceledException(),
            _ => new InvalidOperationException()
        };
        var fake = new Persistence { Failure = failure };
        Assert.Same(failure, await Assert.ThrowsAnyAsync<Exception>(() => new CreatePlayerProfile(fake, fake)
            .ExecuteAsync(fake.Person.Id, null, null, null)));
        Assert.Null(fake.Profile);
    }

    private sealed class Persistence : IPlayerProfilePersistence, IPersonPersistence
    {
        public Person Person { get; } = new(Guid.NewGuid(), "Alex", "Player");
        public PlayerProfileDetails? Profile { get; private set; }
        public List<CancellationToken> Tokens { get; } = [];
        public int Writes { get; private set; }
        public Exception? Failure { get; init; }
        public Task<PlayerProfileDetails?> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
        {
            Tokens.Add(cancellationToken);
            return Task.FromResult(Profile?.PersonId == personId ? Profile : null);
        }
        public Task AddAsync(PlayerProfile profile, CancellationToken cancellationToken = default)
        {
            Tokens.Add(cancellationToken);
            if (Failure is not null) throw Failure;
            Writes++;
            Profile = PlayerProfileDetails.From(profile);
            return Task.CompletedTask;
        }
        public Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Tokens.Add(cancellationToken);
            return Task.FromResult(id == Person.Id ? Person : null);
        }
        public Task AddAsync(Person person, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
