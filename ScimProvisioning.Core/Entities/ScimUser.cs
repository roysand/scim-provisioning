using ScimProvisioning.Core.Common;
using ScimProvisioning.Core.ValueObjects;
using ScimProvisioning.Core.Events;

namespace ScimProvisioning.Core.Entities;

/// <summary>
/// Represents a SCIM User aggregate root
/// </summary>
public class ScimUser : BaseEntity
{
    private readonly List<Email> _emails = new();
    private readonly List<PhoneNumber> _phoneNumbers = new();

    /// <summary>
    /// Gets the external identifier for the user (from source system)
    /// </summary>
    public string ExternalId { get; private set; }

    /// <summary>
    /// Gets the username
    /// </summary>
    public string UserName { get; private set; }

    /// <summary>
    /// Gets the display name
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// Gets the primary email address
    /// </summary>
    public string PrimaryEmail { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the user is active
    /// </summary>
    public bool Active { get; private set; }

    /// <summary>
    /// Gets the user's emails
    /// </summary>
    public IReadOnlyCollection<Email> Emails => _emails.AsReadOnly();

    /// <summary>
    /// Gets the user's phone numbers
    /// </summary>
    public IReadOnlyCollection<PhoneNumber> PhoneNumbers => _phoneNumbers.AsReadOnly();

    // EF Core constructor
    private ScimUser() { }

    private ScimUser(string externalId, string userName, string displayName, string primaryEmail)
    {
        Id = Guid.NewGuid();
        ExternalId = externalId;
        UserName = userName;
        DisplayName = displayName;
        PrimaryEmail = primaryEmail;
        Active = true;
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new SCIM user
    /// </summary>
    public static Result<ScimUser> Create(
        string externalId,
        string userName,
        string displayName,
        string primaryEmail)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            return Result.Failure<ScimUser>("External ID is required");

        if (string.IsNullOrWhiteSpace(userName))
            return Result.Failure<ScimUser>("Username is required");

        if (string.IsNullOrWhiteSpace(displayName))
            return Result.Failure<ScimUser>("Display name is required");

        var emailResult = Email.Create(primaryEmail, true);
        if (emailResult.IsFailure)
            return Result.Failure<ScimUser>(emailResult.Error);

        var user = new ScimUser(externalId, userName, displayName, primaryEmail);
        user._emails.Add(emailResult.Value);
        user.AddDomainEvent(new UserProvisionedDomainEvent(user.Id, externalId, userName));

        return Result.Success(user);
    }

    /// <summary>
    /// Updates the user's information
    /// </summary>
    public Result Update(string? displayName = null, string? primaryEmail = null, bool? active = null)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
            DisplayName = displayName;

        if (!string.IsNullOrWhiteSpace(primaryEmail))
        {
            var emailResult = Email.Create(primaryEmail, true);
            if (emailResult.IsFailure)
                return Result.Failure(emailResult.Error);

            PrimaryEmail = primaryEmail;
            _emails.Clear();
            _emails.Add(emailResult.Value);
        }

        if (active.HasValue)
            Active = active.Value;

        ModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new UserUpdatedDomainEvent(Id, ExternalId, UserName));

        return Result.Success();
    }

    /// <summary>
    /// Marks the user as deleted
    /// </summary>
    public void Delete()
    {
        Active = false;
        ModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new UserDeletedDomainEvent(Id, ExternalId, UserName));
    }

    /// <summary>
    /// Adds an email address
    /// </summary>
    public Result AddEmail(Email email)
    {
        if (_emails.Any(e => e.Value == email.Value))
            return Result.Failure("Email already exists");

        _emails.Add(email);
        ModifiedAt = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Adds a phone number
    /// </summary>
    public Result AddPhoneNumber(PhoneNumber phoneNumber)
    {
        if (_phoneNumbers.Any(p => p.Value == phoneNumber.Value))
            return Result.Failure("Phone number already exists");

        _phoneNumbers.Add(phoneNumber);
        ModifiedAt = DateTime.UtcNow;

        return Result.Success();
    }
}
