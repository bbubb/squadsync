namespace SquadSync.Domain;

public sealed class PlayerProfile
{
    public PlayerProfile(Guid id, Guid personId, DominantFoot? dominantFoot, int? heightInches, decimal? weightPounds)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Player profile ID cannot be empty.", nameof(id));
        }

        if (personId == Guid.Empty)
        {
            throw new ArgumentException("Person ID cannot be empty.", nameof(personId));
        }

        ValidateAttributes(dominantFoot, heightInches, weightPounds);

        Id = id;
        PersonId = personId;
        DominantFoot = dominantFoot;
        HeightInches = heightInches;
        WeightPounds = weightPounds;
    }

    public Guid Id { get; }

    public Guid PersonId { get; }

    public DominantFoot? DominantFoot { get; private set; }

    public int? HeightInches { get; private set; }

    public decimal? WeightPounds { get; private set; }

    public void Update(DominantFoot? dominantFoot, int? heightInches, decimal? weightPounds)
    {
        ValidateAttributes(dominantFoot, heightInches, weightPounds);

        DominantFoot = dominantFoot;
        HeightInches = heightInches;
        WeightPounds = weightPounds;

    }

    private static void ValidateAttributes(DominantFoot? dominantFoot, int? heightInches, decimal? weightPounds)
    {
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
    }
}
