using FluentValidation;
using FluentValidation.Internal;
using SquadSync.Data.Models;
using SquadSync.DTOs.Responses;
using SquadSync.Enums;
using System;
using System.Linq;

namespace SquadSync.DTOs.Validators
{
    public class OrgUnitDtoValidator : AbstractValidator<OrgUnitDto>
    {
        public OrgUnitDtoValidator()
        {
            RuleFor(orgUnit => orgUnit.Guid)
                .NotEmpty().WithMessage("Guid is required.");

            RuleFor(orgUnit => orgUnit.OrgUnitName)
                .NotEmpty().WithMessage("OrgUnitName is required.")
                .Length(3, 100).WithMessage("OrgUnitName must be between 3 and 100 characters.");

            RuleFor(orgUnit => orgUnit.OrgUnitDescription)
                .MaximumLength(500).WithMessage("OrgUnitDescription must be no more than 500 characters.");

            RuleFor(orgUnit => orgUnit.OrgUnitStatus)
                .NotEmpty().WithMessage("OrgUnitStatus is required.")
                .IsEnumName(typeof(OrgUnitStatusEnum), caseSensitive: false)
                .WithMessage("OrgUnitStatus must be a valid OrgUnitStatus.");

            RuleFor(orgUnit => orgUnit.Owner)
                .NotNull().WithMessage("Owner is required.")
                .Must(bearer => bearer != null && bearer.Guid != Guid.Empty)
                .WithMessage("Owner must be a valid IRoleBearer.");

            RuleFor(orgUnit => orgUnit.OUMembers)
                .NotNull().WithMessage("OUMembers list cannot be null.")
                .Must(bearers => bearers.All(bearer => bearer != null && bearer.RoleBearerGuid != Guid.Empty))
                .WithMessage("Each member in OUMembers must be valid.");

            RuleFor(orgUnit => orgUnit.OURoles)
               .NotNull().WithMessage("OURoles list cannot be null.")
               .Must(roles => roles.All(role => role != null && role.Guid != Guid.Empty))
               .WithMessage("Each role in OURoles must be valid.");

            When(orgUnit => orgUnit.OURoles != null, () =>
            {
                RuleFor(orgUnit => orgUnit.OURoleRequests)
                    .Must(roleRequests => roleRequests == null || roleRequests.All(roleRequest => roleRequest != null && roleRequest.Guid != Guid.Empty))
                    .WithMessage("Each role request in OURoleRequests must be valid.");
            });

            When(orgUnit => orgUnit.Roles != null, () =>
            {
                RuleFor(orgUnit => orgUnit.Roles)
                    .NotNull().WithMessage("Roles list cannot be null.")
                    .Must(roles => roles.All(role => role != null && role.Guid != Guid.Empty))
                    .WithMessage("Each role in Roles must be valid.");
            });

            When(orgUnit => orgUnit.Roles != null, () =>
            {
                RuleFor(orgUnit => orgUnit.RoleRequests)
                    .Must(roleRequests => roleRequests == null || roleRequests.All(roleRequest => roleRequest != null && roleRequest.Guid != Guid.Empty))
                    .WithMessage("Each role request in RoleRequests must be valid.");
            });

            RuleFor(orgUnit => orgUnit.CreatedOn)
                .NotEmpty().WithMessage("CreatedOn is required.")
                .Must(createdOn => createdOn <= DateTime.UtcNow)
                .WithMessage("CreatedOn must not be in the future.");

            When(orgUnit => orgUnit.Modified.HasValue, () =>
                {
                    RuleFor(orgUnit => orgUnit.Modified)
                    .Must(modified => modified.Value <= DateTime.UtcNow)
                    .WithMessage("Modified date must not be in the future.");
                });

            When(orgUnit => orgUnit.ArchivedOn.HasValue, () =>
            {
                RuleFor(orgUnit => orgUnit.ArchivedOn)
                    .Must(archivedOn => archivedOn.Value <= DateTime.UtcNow)
                    .WithMessage("ArchivedOn date must not be in the future.");
            });
        }
    }
}