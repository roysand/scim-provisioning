using FluentValidation;
using ScimProvisioning.Application.DTOs;

namespace ScimProvisioning.Application.Validators;

/// <summary>
/// Validator for creating a group
/// </summary>
public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
{
    public CreateGroupRequestValidator()
    {
        RuleFor(x => x.ExternalId)
            .NotEmpty().WithMessage("External ID is required")
            .MaximumLength(255).WithMessage("External ID must not exceed 255 characters");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required")
            .MaximumLength(255).WithMessage("Display name must not exceed 255 characters");
    }
}

/// <summary>
/// Validator for updating a group
/// </summary>
public class UpdateGroupRequestValidator : AbstractValidator<UpdateGroupRequest>
{
    public UpdateGroupRequestValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () =>
        {
            RuleFor(x => x.DisplayName)
                .MaximumLength(255).WithMessage("Display name must not exceed 255 characters");
        });
    }
}

/// <summary>
/// Validator for adding a group member
/// </summary>
public class AddGroupMemberRequestValidator : AbstractValidator<AddGroupMemberRequest>
{
    public AddGroupMemberRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required")
            .MaximumLength(255).WithMessage("Display name must not exceed 255 characters");
    }
}
