using SquadSync.Utilities.IUtilities;

namespace SquadSync.Utilities
{
    public class EmailNormalizationService : IEmailNormalizationService
    {
        public string NormalizeEmail(string email)
        {
            return email.ToLowerInvariant().Trim();
        }
    }
}
