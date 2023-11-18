﻿using FluentValidation;
using SquadSync.Data.Models;
using SquadSync.DTOs.Requests;
using SquadSync.Enums;

namespace SquadSync.DTOs.Validators
{
    public class OrgUnitUpdateRequestDtoValidator : AbstractValidator<OrgUnitUpdateRequestDto>
    {
        public OrgUnitUpdateRequestDtoValidator()
        {
            RuleFor(orgUnit => orgUnit.Guid)
                .NotEmpty().WithMessage("Guid is required.");


            RuleFor(orgUnit => orgUnit.OrgUnitName)
                .NotEmpty().WithMessage("OrgUnitName is required.")
                .Length(3, 100).WithMessage("OrgUnitName must be between 3 and 100 characters.")
                .When(orgUnit => !string.IsNullOrEmpty(orgUnit.OrgUnitName));

            RuleFor(orgUnit => orgUnit.OrgUnitDescription)
                .MaximumLength(500).WithMessage("OrgUnitDescription must be no more than 500 characters.")
                .When(orgUnit => !string.IsNullOrEmpty(orgUnit.OrgUnitDescription));

            RuleFor(orgUnit => orgUnit.OrgUnitStatus)
                .NotEmpty().WithMessage("OrgUnitStatus is required.")
                .IsEnumName(typeof(OrgUnitStatusEnum), caseSensitive: false)
                .WithMessage("OrgUnitStatus must be a valid OrgUnitStatus.")
                .When(orgUnit => !string.IsNullOrEmpty(orgUnit.OrgUnitStatus));

            RuleFor(orgUnit => orgUnit.Owner)
                .NotNull().WithMessage("Owner is required.")
                .NotEmpty().WithMessage("Owner must be a valid user.")
                .When(orgUnit => orgUnit.Owner != null);

            RuleFor(orgUnit => orgUnit.Users)
                .NotNull().WithMessage("Users list cannot be null.")
                .Must(users => users.All(user => user != null && user.Guid != Guid.Empty))
                .WithMessage("Each user in the list must be valid.")
                .When(orgUnit => orgUnit.Users != null);

            // Apply when Roles are fully implemented
            //RuleFor(orgUnit => orgUnit.Roles)
            //    .NotNull().WithMessage("Roles list cannot be null.")
            //    .Must(roles => roles != null && roles.All(role => role != null && role.Guid != Guid.Empty))
            //    .WithMessage("Each role in the list must be valid.");

            // Apply when RoleRequests are fully implemented
            // Applies when Roles are not null
            //When (orgUnit => orgUnit.Roles != null, () =>
            //{
            //    RuleFor(orgUnit => orgUnit.RoleRequests)
            //    .Must(roleRequests => roleRequests == null || roleRequests.All(roleRequest => roleRequest != null && roleRequest.Guid != Guid.Empty))
            //    .WithMessage("Each role request in the list must be valid.");
            //});

            // Probably don't need this, since it's applied in services
            //When(orgUnit => orgUnit.Modified.HasValue, () =>
            //{
            //    RuleFor(orgUnit => orgUnit.Modified)
            //        .Must(modified => modified.Value <= DateTime.UtcNow);
            //});
        }
    }
}
