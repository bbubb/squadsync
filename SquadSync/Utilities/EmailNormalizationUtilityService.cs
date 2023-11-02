using SquadSync.Utilities.IUtilities;

namespace SquadSync.Utilities
{
    public class EmailNormalizationUtilityService : IEmailNormalizationUtilityService
    {
        public string NormalizeEmail(string email)
        {
            return email.ToLowerInvariant().Trim();
        }
    }
}
