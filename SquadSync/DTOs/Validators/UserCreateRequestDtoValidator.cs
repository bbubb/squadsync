using FluentValidation;
using SquadSync.Enums;

namespace SquadSync.DTOs.Validators;
public class UserDtoValidator : AbstractValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(user => user.Guid)
            .Must(guid => guid != Guid.Empty).WithMessage("Guid is required.");

        RuleFor(user => user.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .Length(4, 50).WithMessage("Username must be between 4 and 50 characters.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");
        
        RuleFor(user => user.EmailNormalized)
            .NotEmpty().WithMessage("Email upper is required.")
            .Must(emailNormalized => emailNormalized.ToUpper() == emailNormalized).WithMessage("Email upper must be uppercase.")
            .EmailAddress().WithMessage("A valid email upper is required.");

        RuleFor(user => user.FirstName)
            .Length(1, 100).WithMessage("First name must be between 1 and 100 characters.")
            .When(user => !string.IsNullOrEmpty(user.FirstName));

        RuleFor(user => user.LastName)
            .Length(1, 100).WithMessage("Last name must be between 1 and 100 characters.")
            .When(user => !string.IsNullOrEmpty(user.LastName));

        RuleFor(user => user.DateOfBirth)
            .Matches(string.Format(@"^(((0[1-9]|1[0-2])\/(0[1-9]|1[0-9]|2[0-8]))|((0[13-9]|1[0-2])\/(29|30))|((0[13578]|1[02])\/31))\/\d{4}$")).WithMessage("Date of birth must be a valid date.")
            .When(user => !string.IsNullOrEmpty(user.DateOfBirth))
            .Must(dob => DateTime.TryParse(dob, out _)).WithMessage("Date of birth must be a valid date.");

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

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(8, 50).WithMessage("Password must be between 8 and 50 characters.")
            .Matches(@"^[a-zA-Z0-9]+$").WithMessage("Password must be alphanumeric.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
            .Matches(@"[!@#$%^&*()_+=\[{\]};:<>|./?]").WithMessage("Password must contain at least one special.");
    }
}