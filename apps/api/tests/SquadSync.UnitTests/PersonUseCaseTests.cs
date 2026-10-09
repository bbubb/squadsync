using SquadSync.Application.People;
using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class PersonUseCaseTests
{
    [Fact]
    public async Task Create_GeneratesDistinctIdsNormalizesNamesAndPersistsWithCancellation()
    {
        var persistence = new FakePersistence();
        var useCase = new CreatePerson(persistence);
        using var cancellation = new CancellationTokenSource();

        var first = await useCase.ExecuteAsync("  Alex  ", " Player\t", cancellation.Token);
        Assert.Same(first, persistence.Saved);
        Assert.Equal(cancellation.Token, persistence.Token);
        var second = await useCase.ExecuteAsync("Alex", "Player");

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(first.Id, second.Id);
        Assert.Equal("Alex", first.FirstName);
        Assert.Equal("Player", first.LastName);
    }

    [Theory]
    [InlineData(null, "Player", "firstName")]
    [InlineData("", "Player", "firstName")]
    [InlineData(" \t", "Player", "firstName")]
    [InlineData("Alex", null, "lastName")]
    [InlineData("Alex", "", "lastName")]
    [InlineData("Alex", " \t", "lastName")]
    public async Task Create_InvalidNamesTranslateDomainFailureBeforePersistence(
        string? firstName, string? lastName, string field)
    {
        var persistence = new FakePersistence();

        var exception = await Assert.ThrowsAsync<PersonValidationException>(() =>
            new CreatePerson(persistence).ExecuteAsync(firstName!, lastName!));

        Assert.Equal(field, exception.Field);
        Assert.IsAssignableFrom<ArgumentException>(exception.InnerException);
        Assert.Null(persistence.Saved);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Get_ReturnsPersistenceResultAndForwardsIdAndCancellation(bool exists)
    {
        var id = Guid.NewGuid();
        var persistence = new FakePersistence { Saved = exists ? new Person(id, "Alex", "Player") : null };
        using var cancellation = new CancellationTokenSource();

        var result = await new GetPerson(persistence).ExecuteAsync(id, cancellation.Token);

        Assert.Same(persistence.Saved, result);
        Assert.Equal(id, persistence.RequestedId);
        Assert.Equal(cancellation.Token, persistence.Token);
    }

    [Fact]
    public async Task PersistenceFailure_IsNotMisclassifiedAsInputValidation()
    {
        var failure = new ArgumentException("Persistence failure", "firstName");
        var persistence = new FakePersistence { Failure = failure };

        Assert.Same(failure, await Assert.ThrowsAsync<ArgumentException>(() =>
            new CreatePerson(persistence).ExecuteAsync("Alex", "Player")));
    }

    private sealed class FakePersistence : IPersonPersistence
    {
        public Person? Saved { get; set; }
        public Guid RequestedId { get; private set; }
        public CancellationToken Token { get; private set; }
        public Exception? Failure { get; init; }

        public Task AddAsync(Person person, CancellationToken cancellationToken = default)
        {
            if (Failure is not null) throw Failure;
            Saved = person;
            Token = cancellationToken;
            return Task.CompletedTask;
        }

        public Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            RequestedId = id;
            Token = cancellationToken;
            return Task.FromResult(Saved);
        }
    }
}
