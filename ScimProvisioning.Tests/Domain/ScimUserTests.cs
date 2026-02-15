using ScimProvisioning.Core.Entities;
using Xunit;

namespace ScimProvisioning.Tests.Domain;

/// <summary>
/// Unit tests for ScimUser entity
/// </summary>
public class ScimUserTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSuccessResult()
    {
        // Arrange
        var externalId = "ext-123";
        var userName = "john.doe";
        var displayName = "John Doe";
        var primaryEmail = "john.doe@example.com";

        // Act
        var result = ScimUser.Create(externalId, userName, displayName, primaryEmail);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(externalId, result.Value.ExternalId);
        Assert.Equal(userName, result.Value.UserName);
        Assert.Equal(displayName, result.Value.DisplayName);
        Assert.Equal(primaryEmail, result.Value.PrimaryEmail);
        Assert.True(result.Value.Active);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
    }

    [Fact]
    public void Create_WithEmptyExternalId_ReturnsFailureResult()
    {
        // Arrange
        var externalId = "";
        var userName = "john.doe";
        var displayName = "John Doe";
        var primaryEmail = "john.doe@example.com";

        // Act
        var result = ScimUser.Create(externalId, userName, displayName, primaryEmail);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("External ID is required", result.Error);
    }

    [Fact]
    public void Create_WithInvalidEmail_ReturnsFailureResult()
    {
        // Arrange
        var externalId = "ext-123";
        var userName = "john.doe";
        var displayName = "John Doe";
        var primaryEmail = "invalid-email";

        // Act
        var result = ScimUser.Create(externalId, userName, displayName, primaryEmail);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Email address is not valid", result.Error);
    }

    [Fact]
    public void Update_WithValidData_UpdatesUserAndReturnsSucess()
    {
        // Arrange
        var user = ScimUser.Create("ext-123", "john.doe", "John Doe", "john.doe@example.com").Value;
        var newDisplayName = "John Updated Doe";
        var newEmail = "john.updated@example.com";

        // Act
        var result = user.Update(displayName: newDisplayName, primaryEmail: newEmail);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newDisplayName, user.DisplayName);
        Assert.Equal(newEmail, user.PrimaryEmail);
    }

    [Fact]
    public void Delete_MarksUserAsInactive()
    {
        // Arrange
        var user = ScimUser.Create("ext-123", "john.doe", "John Doe", "john.doe@example.com").Value;

        // Act
        user.Delete();

        // Assert
        Assert.False(user.Active);
    }

    [Fact]
    public void Create_RaisesDomainEvent()
    {
        // Arrange & Act
        var user = ScimUser.Create("ext-123", "john.doe", "John Doe", "john.doe@example.com").Value;

        // Assert
        Assert.Single(user.DomainEvents);
        Assert.Equal("UserProvisionedDomainEvent", user.DomainEvents.First().GetType().Name);
    }
}
