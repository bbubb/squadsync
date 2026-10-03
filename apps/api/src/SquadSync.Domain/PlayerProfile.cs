namespace SquadSync.Domain;

public sealed class PlayerProfile
{
    public PlayerProfile(Guid id, Guid userId, DominantFoot? dominantFoot, int? heightInches, decimal? weightPounds)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Player profile ID cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }

        if (dominantFoot.HasValue && !Enum.IsDefined(dominantFoot.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(dominantFoot), dominantFoot, "Dominant foot must be a defined value.");
        }

        if (heightInches <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(heightInches), heightInches, "Height in inches must be greater than zero.");
        }

        if (weightPounds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(weightPounds), weightPounds, "Weight in pounds must be greater than zero.");
        }

        Id = id;
        UserId = userId;
        DominantFoot = dominantFoot;
        HeightInches = heightInches;
        WeightPounds = weightPounds;
    }

    public Guid Id { get; }

    public Guid UserId { get; }

    public DominantFoot? DominantFoot { get; }

    public int? HeightInches { get; }

    public decimal? WeightPounds { get; }
}
