using FluentValidation;
using SquadSync.DTOs.Requests;

namespace SquadSync.DTOs.Validators
{
    public class UserByEmailNormalizedRequestValidator : AbstractValidator<UserByEmailNormalizedRequestDto>
    {
        public UserByEmailNormalizedRequestValidator()
        {
            RuleFor(user => user.EmailNormalized)
                .NotEmpty().WithMessage("EmailNormalized is required")
                .Must( emailNormalized => emailNormalized == emailNormalized.ToUpper() ).WithMessage("EmailNormalized must be uppercase")
                .EmailAddress().WithMessage("EmailNormalized must be a valid email address");
        }
    }
}