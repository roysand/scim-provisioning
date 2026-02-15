using ScimProvisioning.Core.Common;
using System.Text.RegularExpressions;

namespace ScimProvisioning.Core.ValueObjects;

/// <summary>
/// Represents an email address value object
/// </summary>
public class Email
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Gets the email address value
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this is the primary email
    /// </summary>
    public bool IsPrimary { get; private set; }

    private Email(string value, bool isPrimary)
    {
        Value = value;
        IsPrimary = isPrimary;
    }

    /// <summary>
    /// Creates an Email value object
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="isPrimary">Whether this is the primary email</param>
    /// <returns>A Result containing the Email or an error</returns>
    public static Result<Email> Create(string email, bool isPrimary = false)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<Email>("Email address is required");

        email = email.Trim().ToLowerInvariant();

        if (email.Length > 254)
            return Result.Failure<Email>("Email address is too long");

        if (!EmailRegex.IsMatch(email))
            return Result.Failure<Email>("Email address is not valid");

        return Result.Success(new Email(email, isPrimary));
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is not Email other)
            return false;

        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();
}
