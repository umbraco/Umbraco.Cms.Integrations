using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Umbraco.Cms.Integrations.Crm.Hubspot.Api.Management.Controllers;
using Xunit;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Tests.Controllers;

/// <summary>
/// Covers the API-key path for fetching forms.
/// </summary>
public class GetAllControllerTests
{
    private static (GetAllController Sut, Mock<ILogger<GetAllController>> Logger) CreateSut(
        Microsoft.Extensions.Options.IOptions<Configuration.HubspotSettings> settings,
        HttpStatusCode statusCode,
        string responseContent = "")
    {
        var logger = new Mock<ILogger<GetAllController>>();

        var sut = new GetAllController(
            settings,
            logger.Object,
            TestHelpers.TokenService(withAccessToken: false).Object,
            TestHelpers.HttpClientFactory(statusCode, responseContent));

        return (sut, logger);
    }

    [Fact]
    public async Task GetAll_WithoutApiKey_ReturnsInvalidAndLogsInformation()
    {
        var (sut, logger) = CreateSut(
            TestHelpers.NoAuthorizationSettings(), HttpStatusCode.OK);

        var result = await sut.GetAll();

        Assert.False(result.IsValid);
        Assert.False(result.IsExpired);
        Assert.Equal(Constants.ErrorMessages.ApiKeyMissing, result.Error);
        logger.VerifyLogged(LogLevel.Information, Times.Once());
    }

    [Fact]
    public async Task GetAll_WithSuccessfulResponse_ReturnsForms()
    {
        var (sut, _) = CreateSut(
            TestHelpers.ApiSettings(),
            HttpStatusCode.OK,
            TestHelpers.ReadFixture("mockResponseApiSetup.json"));

        var result = await sut.GetAll();

        Assert.True(result.IsValid);
        Assert.False(result.IsExpired);
        Assert.Single(result.Forms);
        Assert.Equal(TestHelpers.TestRegion, result.Forms[0].Region);
    }

    [Fact]
    public async Task GetAll_WithUnauthorizedResponse_ReportsExpiredAndLogsError()
    {
        var (sut, logger) = CreateSut(
            TestHelpers.ApiSettings(), HttpStatusCode.Unauthorized);

        var result = await sut.GetAll();

        Assert.True(result.IsExpired);
        Assert.False(result.IsValid);
        Assert.Equal(Constants.ErrorMessages.InvalidApiKey, result.Error);
        logger.VerifyLogged(LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task GetAll_WithForbiddenResponse_ReportsTokenPermissionsAndLogsError()
    {
        var (sut, logger) = CreateSut(
            TestHelpers.ApiSettings(), HttpStatusCode.Forbidden);

        var result = await sut.GetAll();

        Assert.False(result.IsValid);
        Assert.False(result.IsExpired);
        Assert.Equal(Constants.ErrorMessages.TokenPermissions, result.Error);
        logger.VerifyLogged(LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task GetAll_WithServerError_ReturnsDefaultResponseAndLogsError()
    {
        var (sut, logger) = CreateSut(
            TestHelpers.ApiSettings(),
            HttpStatusCode.InternalServerError,
            TestHelpers.ReadFixture("mockResponseApiSetup.json"));

        var result = await sut.GetAll();

        Assert.False(result.IsValid);
        Assert.False(result.IsExpired);
        Assert.Empty(result.Forms);
        logger.VerifyLogged(LogLevel.Error, Times.Once());
    }
}
