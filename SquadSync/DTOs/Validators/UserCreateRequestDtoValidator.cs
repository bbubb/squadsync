using FluentValidation;
using SquadSync.Enums;
using SquadSync.DTOs.Requests;

namespace SquadSync.DTOs.Validators;
public class UserDtoValidator : AbstractValidator<UserCreateRequestDto>
{
    public UserDtoValidator()
    {
         RuleFor(user => user.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .Length(4, 50).WithMessage("Username must be between 4 and 50 characters.");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(8, 50).WithMessage("Password must be between 8 and 50 characters.")
            .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Password must be alphanumeric.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[!@#$%^&*()_+=\[{\]};:<>|./?]").WithMessage("Password must contain at least one special.");

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
            .WithMessage("Date of birth must not be a future date.");

        RuleFor(user => user.PhoneNumber)
            .Length(10).WithMessage("Phone number must be 10 digits.")
            .Matches(@"^[0-9]+$").WithMessage("Phone number must be 10 digits.")
            .When(user => !string.IsNullOrEmpty(user.PhoneNumber));

        // RuleFor(user => user.RoleDto)
        //     .NotEmpty().WithMessage("Role is required.")
        //     .Must(BeAValidRole).WithMessage("Role must be a valid role.");
    }
}
