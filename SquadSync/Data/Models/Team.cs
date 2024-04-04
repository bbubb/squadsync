using SquadSync.Enums;

namespace SquadSync.Data.Models
{
    public class Team : OrgUnit
    {
        //// Future feature for custom kits
        //public string TeamHomeKit { get; set; }
        //public string TeamAwayKit { get; set; }
        //public string TeamAlternateKit { get; set; }
        public ICollection<Player> PlayerRoster { get; set; }
        //public ICollection<TeamStat> TeamStats { get; set; } // Future feature for stats
    }
}
