using FluentValidation;
using SquadSync.DTOs.Responses;

namespace SquadSync.DTOs.Validators
{
    public class UsersAllResponseDtoValidator : AbstractValidator<UsersAllResponseDto>
    {
        public UsersAllResponseDtoValidator()
        {
            RuleFor(usersAllResponseDto => usersAllResponseDto.Users)
                .NotEmpty().WithMessage("Users not found.");
        }
    }
}