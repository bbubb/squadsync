namespace SquadSync.Enums
{
    public enum CanCreateDtoExcludeEnum
    {
        //Defines exclusions for manually mapping object creation
        //Example:  A_EXCLUDES_B
        ///***A gets created, but not dependent B within***
        COACH_EXCLUDES_USER,
        COACH_EXCLUDES_PLAYER,
        PLAYER_EXCLUDES_COACH,
        ROLE_EXCLUDES_USER
    }
}
