using SquadSync.Domain;
using Xunit;

namespace SquadSync.UnitTests;

public class PlayerProfileTests
{
    [Theory]
    [InlineData(DominantFoot.Left)]
    [InlineData(DominantFoot.Right)]
    [InlineData(DominantFoot.Both)]
    [InlineData(null)]
    public void Constructor_PreservesIdentifiersAndApprovedFoot(DominantFoot? dominantFoot)
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var profile = new PlayerProfile(id, userId, dominantFoot, 70, 165.5m);

        Assert.Equal(id, profile.Id);
        Assert.Equal(userId, profile.UserId);
        Assert.Equal(dominantFoot, profile.DominantFoot);
        Assert.Equal(70, profile.HeightInches);
        Assert.Equal(165.5m, profile.WeightPounds);
    }

    [Fact]
    public void Constructor_RejectsEmptyIdentifier()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new PlayerProfile(Guid.Empty, Guid.NewGuid(), null, null, null));

        Assert.Equal("id", exception.ParamName);
    }

    [Fact]
    public void Constructor_RejectsEmptyUserIdentifier()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new PlayerProfile(Guid.NewGuid(), Guid.Empty, null, null, null));

        Assert.Equal("userId", exception.ParamName);
    }

    [Theory]
    [InlineData((DominantFoot)0)]
    [InlineData((DominantFoot)4)]
    [InlineData((DominantFoot)(-1))]
    public void Constructor_RejectsUndefinedFoot(DominantFoot dominantFoot)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PlayerProfile(Guid.NewGuid(), Guid.NewGuid(), dominantFoot, null, null));

        Assert.Equal("dominantFoot", exception.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(1)]
    [InlineData(70)]
    public void Constructor_AcceptsOptionalPositiveHeight(int? heightInches)
    {
        var profile = new PlayerProfile(Guid.NewGuid(), Guid.NewGuid(), null, heightInches, null);

        Assert.Equal(heightInches, profile.HeightInches);
        Assert.Null(profile.WeightPounds);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_RejectsNonPositiveHeight(int heightInches)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PlayerProfile(Guid.NewGuid(), Guid.NewGuid(), null, heightInches, null));

        Assert.Equal("heightInches", exception.ParamName);
    }

    [Fact]
    public void Constructor_AcceptsPositiveFractionalWeight()
    {
        var profile = new PlayerProfile(Guid.NewGuid(), Guid.NewGuid(), null, null, 0.1m);

        Assert.Equal(0.1m, profile.WeightPounds);
        Assert.Null(profile.HeightInches);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_RejectsNonPositiveWeight(int weightPounds)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PlayerProfile(Guid.NewGuid(), Guid.NewGuid(), null, null, weightPounds));

        Assert.Equal("weightPounds", exception.ParamName);
    }
}
