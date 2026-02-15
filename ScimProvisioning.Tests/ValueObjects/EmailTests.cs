using ScimProvisioning.Core.ValueObjects;
using Xunit;

namespace ScimProvisioning.Tests.ValueObjects;

/// <summary>
/// Unit tests for Email value object
/// </summary>
public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@example.co.uk")]
    [InlineData("test+tag@example.com")]
    public void Create_WithValidEmail_ReturnsSuccessResult(string emailAddress)
    {
        // Act
        var result = Email.Create(emailAddress);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(emailAddress.ToLowerInvariant(), result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test")]
    public void Create_WithInvalidEmail_ReturnsFailureResult(string emailAddress)
    {
        // Act
        var result = Email.Create(emailAddress);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_WithTooLongEmail_ReturnsFailureResult()
    {
        // Arrange
        var longEmail = new string('a', 255) + "@example.com";

        // Act
        var result = Email.Create(longEmail);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("too long", result.Error);
    }

    [Fact]
    public void Equals_WithSameEmail_ReturnsTrue()
    {
        // Arrange
        var email1 = Email.Create("test@example.com").Value;
        var email2 = Email.Create("test@example.com").Value;

        // Act & Assert
        Assert.Equal(email1, email2);
    }
}
