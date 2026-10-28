using Microsoft.VisualBasic;
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
        var personId = Guid.NewGuid();

        var profile = new PlayerProfile(id, personId, dominantFoot, 70, 165.5m);

        Assert.Equal(id, profile.Id);
        Assert.Equal(personId, profile.PersonId);
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
    public void Constructor_RejectsEmptyPersonIdentifier()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new PlayerProfile(Guid.NewGuid(), Guid.Empty, null, null, null));

        Assert.Equal("personId", exception.ParamName);
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

    [Fact]
    // All three attributes change; both GUIDs remain unchanged
    public void Update_ReplacesAttributesAndPreservesIdentity()
    {
        var profile = new PlayerProfile(
            id: Guid.NewGuid(),
            personId: Guid.NewGuid(),
            dominantFoot: DominantFoot.Left,
            heightInches: 70,
            weightPounds: 165.5m);

        var originalId = profile.Id;
        var originalPersonId = profile.PersonId;

        profile.Update(DominantFoot.Right, 72, 175.25m);

        Assert.Equal(profile.Id, originalId);
        Assert.Equal(profile.PersonId, originalPersonId);
        Assert.Equal(DominantFoot.Right, profile.DominantFoot);
        Assert.Equal(72, profile.HeightInches);
        Assert.Equal(175.25m, profile.WeightPounds);
    }


    [Fact]
    // Passing three nulls clears all attributes
    public void Update_AllowsClearingOptionalAttributes()
    {
        var profile = new PlayerProfile(
        id: Guid.NewGuid(),
        personId: Guid.NewGuid(),
        dominantFoot: DominantFoot.Left,
        heightInches: 70,
        weightPounds: 165.5m);

        var originalId = profile.Id;
        var originalPersonId = profile.PersonId;

        profile.Update(null, null, null);

        Assert.Equal(profile.Id, originalId);
        Assert.Equal(profile.PersonId, originalPersonId);
        Assert.Null(profile.DominantFoot);
        Assert.Null(profile.HeightInches);
        Assert.Null(profile.WeightPounds);
    }

    [Theory]
    [InlineData((DominantFoot)0, 72, 180, "dominantFoot")]
    [InlineData((DominantFoot)4, 72, 180, "dominantFoot")]
    [InlineData(DominantFoot.Right, 0, 180, "heightInches")]
    [InlineData(DominantFoot.Right, -1, 180, "heightInches")]
    [InlineData(DominantFoot.Right, 72, 0, "weightPounds")]
    [InlineData(DominantFoot.Right, 72, -1, "weightPounds")]
    // Invalid foot, height, or weight throws and leaves every property unchanged
    public void Update_RejectsInvalidValuesWithoutChangingState(
    DominantFoot foot,
    int heightInches,
    int weightPounds,
    string invalidParameter)
    {
        var profile = new PlayerProfile(
            id: Guid.NewGuid(),
            personId: Guid.NewGuid(),
            dominantFoot: DominantFoot.Left,
            heightInches: 70,
            weightPounds: 165.5m);

        var originalProfile = new PlayerProfile(
            id: profile.Id,
            personId: profile.PersonId,
            dominantFoot: profile.DominantFoot,
            heightInches: profile.HeightInches,
            weightPounds: profile.WeightPounds);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => profile.Update(foot, heightInches, weightPounds));

        Assert.Equal(invalidParameter, exception.ParamName);

        Assert.Equal(originalProfile.Id, profile.Id);
        Assert.Equal(originalProfile.PersonId, profile.PersonId);
        Assert.Equal(originalProfile.DominantFoot, profile.DominantFoot);
        Assert.Equal(originalProfile.HeightInches, profile.HeightInches);
        Assert.Equal(originalProfile.WeightPounds, profile.WeightPounds);
    }
}
