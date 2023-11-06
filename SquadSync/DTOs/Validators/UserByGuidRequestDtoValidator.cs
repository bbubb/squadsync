using FluentValidation;
using SquadSync.DTOs.Requests;

namespace SquadSync.DTOs.Validators
{
    public class UserByGuidRequestDtoValidator : AbstractValidator<GetUserByGuidRequestDto>
    {
        public UserByGuidRequestDtoValidator()
        {
            RuleFor(user => user.Guid)
                .Must( guid => guid != Guid.Empty ).WithMessage("Guid is required");
        }
    }
}