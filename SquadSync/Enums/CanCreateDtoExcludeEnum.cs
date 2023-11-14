namespace SquadSync.Enums
{
    public enum CanCreateDtoExcludeEnum
    {
        // Defines exclusions for manually mapping object creation
        // Example:  A_Excludes_B
        // ***A gets created, but not dependent B within***
        Coach_Excludes_User,
        Coach_Excludes_Player,
        Player_Excludes_Coach,
        Role_Excludes_User
    }
}