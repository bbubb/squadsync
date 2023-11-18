using FluentValidation;
using SquadSync.DTOs.Requests;
using SquadSync.Enums;

namespace SquadSync.DTOs.Validators
{
    public class OrgUnitCreateRequestDtoValidator : AbstractValidator<OrgUnitCreateRequestDto>
    {
        public OrgUnitCreateRequestDtoValidator()
        {
            RuleFor(orgUnit => orgUnit.OrgUnitName)
                .NotEmpty().WithMessage("OrgUnitName is required.")
                .Length(3, 100).WithMessage("OrgUnitName must be between 3 and 100 characters.");

            RuleFor(orgUnit => orgUnit.OrgUnitDescription)
                .MaximumLength(500).WithMessage("OrgUnitDescription must be no more than 500 characters.");

            // Probably don't need this since it's applied in services
            //RuleFor(orgUnit => orgUnit.OrgUnitStatus)
            //    .NotEmpty().WithMessage("OrgUnitStatus is required.")
            //    .IsEnumName(typeof(OrgUnitStatusEnum), caseSensitive: false)
            //    .WithMessage("OrgUnitStatus must be a valid OrgUnitStatus.");

            RuleFor(orgUnit => orgUnit.OwnerUserGuid)
                .NotEmpty().WithMessage("OwnerUserGuid is required.")
                .NotEqual(Guid.Empty).WithMessage("OwnerUserGuid must be a valid user.");

            // Probably don't need this, since it's applied in services
            //RuleFor(orgUnit => orgUnit.CreatedOn)
            //    .NotEmpty().WithMessage("CreatedOn is required.")
            //    .Must(createdOn => createdOn <= DateTime.UtcNow)
            //    .WithMessage("CreatedOn must not be in the future.");
        }   
    }
}
