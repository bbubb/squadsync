using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class DomainEntityTests
{
    [Fact]
    public void User_StoresIdentifierAndTrimsNames()
    {
        var id = Guid.NewGuid();

        var user = new User(id, "  Alex ", " Morgan  ");

        Assert.Equal(id, user.Id);
        Assert.Equal("Alex", user.FirstName);
        Assert.Equal("Morgan", user.LastName);
    }

    [Fact]
    public void User_RejectsEmptyIdentifier()
    {
        Assert.Throws<ArgumentException>(() => new User(Guid.Empty, "Alex", "Morgan"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  \t ")]
    public void User_RejectsInvalidFirstName(string? firstName)
    {
        Assert.ThrowsAny<ArgumentException>(() => new User(Guid.NewGuid(), firstName!, "Morgan"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  \t ")]
    public void User_RejectsInvalidLastName(string? lastName)
    {
        Assert.ThrowsAny<ArgumentException>(() => new User(Guid.NewGuid(), "Alex", lastName!));
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
