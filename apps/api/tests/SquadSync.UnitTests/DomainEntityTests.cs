using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class DomainEntityTests
{
    [Fact]
    public void Person_StoresIdentifierAndTrimsNames()
    {
        var id = Guid.NewGuid();

        var person = new Person(id, "  Alex ", " Morgan  ");

        Assert.Equal(id, person.Id);
        Assert.Equal("Alex", person.FirstName);
        Assert.Equal("Morgan", person.LastName);
    }

    [Fact]
    public void Person_RejectsEmptyIdentifier()
    {
        Assert.Throws<ArgumentException>(() => new Person(Guid.Empty, "Alex", "Morgan"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  \t ")]
    public void Person_RejectsInvalidFirstName(string? firstName)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Person(Guid.NewGuid(), firstName!, "Morgan"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  \t ")]
    public void Person_RejectsInvalidLastName(string? lastName)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Person(Guid.NewGuid(), "Alex", lastName!));
    }

    [Fact]
    public void Team_StoresIdentifierAndTrimsName()
    {
        var id = Guid.NewGuid();

        var team = new Team(id, "  Northside FC  ");

        Assert.Equal(id, team.Id);
        Assert.Equal("Northside FC", team.Name);
    }

    [Fact]
    public void Team_RejectsEmptyIdentifier()
    {
        Assert.Throws<ArgumentException>(() => new Team(Guid.Empty, "Northside FC"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  \t ")]
    public void Team_RejectsInvalidName(string? name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new Team(Guid.NewGuid(), name!));
    }
}
