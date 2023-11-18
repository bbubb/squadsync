namespace SquadSync.Enums
{
    public enum RoleNameEnum
    {
        Coach,
        Director,
        Guardian,
        Manager,
        Player          
    }

    public static class RoleNameEnumExtensions
    {
        public static string GetDescription(this RoleNameEnum roleName)
        {
            switch (roleName)
            {

                case RoleNameEnum.Coach:
                    return "Controls team strategies and player development.";
                case RoleNameEnum.Director:
                    return "Owns the organization.";
                case RoleNameEnum.Guardian:
                    return "Management and oversight of a specific player.";
                case RoleNameEnum.Manager:
                    return "Manages administrative duties.";
                case RoleNameEnum.Player:
                    return "Participates in games and team activities.";
                default:
                    return String.Empty;
            }
        }
    }
}