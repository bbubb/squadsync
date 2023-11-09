using SquadSync.Utilities.IUtilities;
using System.Net.Mail;

namespace SquadSync.Utilities
{
    public class EmailValidationService : IEmailValidationService
    {
        public bool IsValidEmail(string email)
        {
            try 
            {
                var mailAddress = new MailAddress(email);
                return true;
            } catch 
            { 
                return false;
            }
        }
    }
}
