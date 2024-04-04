using SquadSync.Enums;

namespace SquadSync.Data.Models
{
    public class Player 
    {
        // Player designed as composition of User to simplify data and UI with player Roles 
        public Guid PlayerGuid { get; set; }
        public long PlayerId { get; set; }
        public User PlayerUser { get; set; }
        public long PlayerUserId { get; set; }
        public string? PlayerName { get; set; }  // Optional for nicknames, but might need to pull from User if blank
        public PlayerStatusEnum? PlayerStatus { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public DominantSideEnum? DominantSide { get; set; }  // Left, Right, Ambidextrous
        public string? Position { get; set; } // JSON string for dynamic positions
        public int? PlayerNumber { get; set; }
        public ICollection<IRoleBearer> Teams { get; set; }

        // Future feature for stats
        //public ICollection<PlayerStat> PlayerStats { get; set; }
        //public ICollection<AggregateStats> AggregateStats { get; set; 
    }
}
