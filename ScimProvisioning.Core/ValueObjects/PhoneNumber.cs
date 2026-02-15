using ScimProvisioning.Core.Common;
using System.Text.RegularExpressions;

namespace ScimProvisioning.Core.ValueObjects;

/// <summary>
/// Represents a phone number value object
/// </summary>
public class PhoneNumber
{
    private static readonly Regex PhoneRegex = new(
        @"^\+?[1-9]\d{1,14}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Gets the phone number value
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// Gets the phone number type
    /// </summary>
    public string Type { get; private set; }

    private PhoneNumber(string value, string type)
    {
        Value = value;
        Type = type;
    }

    /// <summary>
    /// Creates a PhoneNumber value object
    /// </summary>
    /// <param name="number">The phone number</param>
    /// <param name="type">The type (e.g., "work", "mobile", "home")</param>
    /// <returns>A Result containing the PhoneNumber or an error</returns>
    public static Result<PhoneNumber> Create(string number, string type = "work")
    {
        if (string.IsNullOrWhiteSpace(number))
            return Result.Failure<PhoneNumber>("Phone number is required");

        // Remove spaces, dashes, and parentheses for validation
        var cleanedNumber = Regex.Replace(number, @"[\s\-\(\)]", "");

        if (!PhoneRegex.IsMatch(cleanedNumber))
            return Result.Failure<PhoneNumber>("Phone number is not valid (use E.164 format, e.g., +1234567890)");

        if (string.IsNullOrWhiteSpace(type))
            type = "work";

        return Result.Success(new PhoneNumber(cleanedNumber, type.ToLowerInvariant()));
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is not PhoneNumber other)
            return false;

        return Value == other.Value && Type == other.Type;
    }

    public override int GetHashCode() => HashCode.Combine(Value, Type);
}
