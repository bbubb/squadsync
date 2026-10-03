using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class RosterEntryTests
{
    [Theory]
    [InlineData(RosterStatus.Active)]
    [InlineData(RosterStatus.Reserved)]
    [InlineData(RosterStatus.Suspended)]
    public void Constructor_PreservesIdentifiersAndApprovedStatus(RosterStatus rosterStatus)
    {
        var id = Guid.NewGuid();
        var membershipId = Guid.NewGuid();

        var entry = new RosterEntry(id, membershipId, "00", rosterStatus);

        Assert.Equal(id, entry.Id);
        Assert.Equal(membershipId, entry.TeamMembershipId);
        Assert.Equal("00", entry.JerseyNumber);
        Assert.Equal(rosterStatus, entry.RosterStatus);
    }

    [Fact]
    public void Constructor_RejectsEmptyIdentifier()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new RosterEntry(Guid.Empty, Guid.NewGuid(), null, RosterStatus.Active));

        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Constructor_RejectsEmptyMembershipIdentifier()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new RosterEntry(Guid.NewGuid(), Guid.Empty, null, RosterStatus.Active));

        Assert.Equal("teamMembershipId", exception.ParamName);
    }

    [Theory]
    [InlineData((RosterStatus)0)]
    [InlineData((RosterStatus)4)]
    [InlineData((RosterStatus)(-1))]
    public void Constructor_RejectsUndefinedStatus(RosterStatus rosterStatus)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new RosterEntry(Guid.NewGuid(), Guid.NewGuid(), null, rosterStatus));

        Assert.Equal("rosterStatus", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("0")]
    [InlineData("7")]
    [InlineData("00")]
    [InlineData("10")]
    [InlineData("001")]
    [InlineData("999")]
    public void Constructor_PreservesOptionalDigitLabel(string? jerseyNumber)
    {
        var entry = new RosterEntry(Guid.NewGuid(), Guid.NewGuid(), jerseyNumber, RosterStatus.Active);

        Assert.Equal(jerseyNumber, entry.JerseyNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\r\n")]
    [InlineData(" 7")]
    [InlineData("7 ")]
    [InlineData("1 2")]
    [InlineData("7\n")]
    [InlineData("\u00a07")]
    [InlineData("\u0661")]
    [InlineData("\uff11\uff12")]
    [InlineData("A")]
    [InlineData("1A")]
    [InlineData("+1")]
    [InlineData("-1")]
    [InlineData("1.2")]
    [InlineData("1234")]
    [InlineData("0000")]
    public void Constructor_RejectsInvalidJerseyNumber(string jerseyNumber)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new RosterEntry(Guid.NewGuid(), Guid.NewGuid(), jerseyNumber, RosterStatus.Active));

        Assert.Equal("jerseyNumber", exception.ParamName);
    }
}
