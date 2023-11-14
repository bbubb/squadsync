namespace SquadSync.Enums
{
    public enum RoleNameEnum
    {
        Player,
        Coach,
        Manager,
        Guardian
    }

    public static class RoleNameEnumExtensions
    {
        public static string GetDescription(this RoleNameEnum roleName)
        {
            switch (roleName)
            {
                case RoleNameEnum.Player:
                    return "Participates in games and team activities.";
                case RoleNameEnum.Coach:
                    return "Manages team strategies and player development.";
                case RoleNameEnum.Manager:
                    return "Handles team administrative duties.";
                case RoleNameEnum.Guardian:
                    return "Manages player's account and team participation.";
                default:
                    return String.Empty;
            }
        }
    }
}