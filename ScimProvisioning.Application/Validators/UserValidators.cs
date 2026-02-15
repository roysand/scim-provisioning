using FluentValidation;
using ScimProvisioning.Application.DTOs;

namespace ScimProvisioning.Application.Validators;

/// <summary>
/// Validator for creating a user
/// </summary>
public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.ExternalId)
            .NotEmpty().WithMessage("External ID is required")
            .MaximumLength(255).WithMessage("External ID must not exceed 255 characters");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(255).WithMessage("Username must not exceed 255 characters");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required")
            .MaximumLength(255).WithMessage("Display name must not exceed 255 characters");

        RuleFor(x => x.PrimaryEmail)
            .NotEmpty().WithMessage("Primary email is required")
            .EmailAddress().WithMessage("Primary email must be a valid email address")
            .MaximumLength(254).WithMessage("Primary email must not exceed 254 characters");
    }
}

/// <summary>
/// Validator for updating a user
/// </summary>
public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () =>
        {
            RuleFor(x => x.DisplayName)
                .MaximumLength(255).WithMessage("Display name must not exceed 255 characters");
        });

        When(x => !string.IsNullOrWhiteSpace(x.PrimaryEmail), () =>
        {
            RuleFor(x => x.PrimaryEmail)
                .EmailAddress().WithMessage("Primary email must be a valid email address")
                .MaximumLength(254).WithMessage("Primary email must not exceed 254 characters");
        });
    }
}
