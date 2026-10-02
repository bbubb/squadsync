using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class TeamMembershipTests
{
    [Theory]
    [InlineData(TeamRole.Owner)]
    [InlineData(TeamRole.Coach)]
    [InlineData(TeamRole.AssistantCoach)]
    [InlineData(TeamRole.Manager)]
    [InlineData(TeamRole.Player)]
    [InlineData(TeamRole.Viewer)]
    public void Constructor_PreservesIdentifiersAndApprovedRole(TeamRole teamRole)
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var teamId = Guid.NewGuid();

        var membership = new TeamMembership(id, userId, teamId, teamRole);

        Assert.Equal(id, membership.Id);
        Assert.Equal(userId, membership.UserId);
        Assert.Equal(teamId, membership.TeamId);
        Assert.Equal(teamRole, membership.TeamRole);
    }

    [Fact]
    public void Constructor_RejectsEmptyIdentifier()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new TeamMembership(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), TeamRole.Player));

        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Constructor_RejectsEmptyUserIdentifier()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new TeamMembership(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), TeamRole.Player));

        Assert.Equal("userId", exception.ParamName);
    }

    [Fact]
    public void Constructor_RejectsEmptyTeamIdentifier()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new TeamMembership(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, TeamRole.Player));

        Assert.Equal("teamId", exception.ParamName);
    }

    [Theory]
    [InlineData((TeamRole)0)]
    [InlineData((TeamRole)7)]
    [InlineData((TeamRole)(-1))]
    public void Constructor_RejectsUndefinedRole(TeamRole teamRole)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new TeamMembership(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), teamRole));

        Assert.Equal("teamRole", exception.ParamName);
    }
}
