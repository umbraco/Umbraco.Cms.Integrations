using Microsoft.Extensions.Options;
using Moq;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Automation.Zapier.Configuration;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Xunit;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Tests.Services;

public class UserValidationServiceTests
{
    private const string Username = "editor@example.com";
    private const string Password = "correct-password";
    private const string GroupAlias = "zapier";

    private readonly Mock<IBackOfficeUserManager> _userManager = new();
    private readonly Mock<IUserService> _userService = new();

    public UserValidationServiceTests()
    {
        _userManager
            .Setup(x => x.ValidateCredentialsAsync(Username, Password))
            .ReturnsAsync(true);
    }

    private UserValidationService CreateSut(ZapierSettings? settings = null, ZapierFormsSettings? formsSettings = null) =>
        new(
            Options.Create(settings ?? new ZapierSettings()),
            Options.Create(formsSettings ?? new ZapierFormsSettings()),
            _userManager.Object,
            _userService.Object);

    private void SetupUserInGroups(params string[] groupAliases)
    {
        var groups = groupAliases
            .Select(alias => Mock.Of<IReadOnlyUserGroup>(g => g.Alias == alias))
            .ToList();

        _userService
            .Setup(x => x.GetByUsername(Username))
            .Returns(Mock.Of<IUser>(u => u.Groups == groups));
    }

    [Fact]
    public async Task Validate_WithUserGroupAliasAndUserInGroup_ReturnsTrue()
    {
        SetupUserInGroups("editor", GroupAlias);

        var result = await CreateSut(new ZapierSettings { UserGroupAlias = GroupAlias })
            .Validate(Username, Password, string.Empty);

        Assert.True(result);
    }

    [Fact]
    public async Task Validate_WithUserGroupAliasAndUserNotInGroup_ReturnsFalse()
    {
        SetupUserInGroups("editor");

        var result = await CreateSut(new ZapierSettings { UserGroupAlias = GroupAlias })
            .Validate(Username, Password, string.Empty);

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_WithUserGroupAliasAndUnknownUser_ReturnsFalse()
    {
        _userService.Setup(x => x.GetByUsername(Username)).Returns((IUser?)null);

        var result = await CreateSut(new ZapierSettings { UserGroupAlias = GroupAlias })
            .Validate(Username, Password, string.Empty);

        Assert.False(result);
    }

    [Fact]
    public async Task Validate_WithWrongPassword_ReturnsFalseWithoutLookingUpTheUser()
    {
        var result = await CreateSut(new ZapierSettings { UserGroupAlias = GroupAlias })
            .Validate(Username, "wrong-password", string.Empty);

        Assert.False(result);
        _userService.Verify(x => x.GetByUsername(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Validate_WithoutUserGroupAlias_ReturnsTrueForValidCredentials()
    {
        var result = await CreateSut().Validate(Username, Password, string.Empty);

        Assert.True(result);
        _userService.Verify(x => x.GetByUsername(It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Validate_WithMatchingApiKey_ReturnsTrueWithoutCheckingCredentials(bool fromFormsSettings)
    {
        var sut = fromFormsSettings
            ? CreateSut(formsSettings: new ZapierFormsSettings { ApiKey = "key" })
            : CreateSut(new ZapierSettings { ApiKey = "key" });

        var result = await sut.Validate(string.Empty, string.Empty, "key");

        Assert.True(result);
        _userManager.Verify(x => x.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Validate_WithWrongApiKey_ReturnsFalse()
    {
        var result = await CreateSut(new ZapierSettings { ApiKey = "key" })
            .Validate(string.Empty, string.Empty, "other-key");

        Assert.False(result);
    }
}
