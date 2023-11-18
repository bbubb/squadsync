using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using SquadSync.DTOs.Responses;
using SquadSync.Enums;

namespace SquadSync.DTOs.Validators
{
    public class UserResponseDtoValidator : AbstractValidator<UserResponseDto>
    {
        public UserResponseDtoValidator()
        {
            RuleFor(user => user.Guid)
                .NotEmpty().WithMessage("Guid is required.");

            RuleFor(user => user.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .Length(4, 50).WithMessage("Username must be between 4 and 50 characters.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(user => user.FirstName)
                .Length(1, 100).WithMessage("First name must be between 1 and 100 characters.")
                .When(user => !string.IsNullOrEmpty(user.FirstName));

            RuleFor(user => user.LastName)
                .Length(1, 100).WithMessage("Last name must be between 1 and 100 characters.")
                .When(user => !string.IsNullOrEmpty(user.LastName));

            RuleFor(user => user.DateOfBirth)
                .Must(dob => dob == null || dob.Value <= DateTime.Today)
                .Must(dob => typeof(DateTime).IsAssignableFrom(dob.GetType()))
                .WithMessage("Date of birth must not be a future date.");

            RuleFor(user => user.PhoneNumber)
                .Length(10).WithMessage("Phone number must be 10 digits.")
                .Matches(@"^[0-9]+$").WithMessage("Phone number must be 10 digits.")
                .When(user => !string.IsNullOrEmpty(user.PhoneNumber));

            RuleFor(user => user.UserStatus)
                .NotEmpty().WithMessage("User status is required.")
                .IsEnumName(typeof(UserStatusEnum)).WithMessage("User status must be a valid user status.");

            // RuleFor(user => user.RoleDto)
            //     .NotEmpty().WithMessage("Role is required.")
            //     .Must(BeAValidRole).WithMessage("Role must be a valid role.");
        }
    }
}
