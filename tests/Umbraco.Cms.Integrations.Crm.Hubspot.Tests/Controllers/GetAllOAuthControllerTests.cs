using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Umbraco.Cms.Integrations.Crm.Hubspot.Api.Management.Controllers;
using Xunit;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Tests.Controllers;

/// <summary>
/// Covers the OAuth path for fetching forms, where the access token comes from the
/// token service rather than configuration.
/// </summary>
public class GetAllOAuthControllerTests
{
    private static (GetAllOAuthController Sut, Mock<ILogger<GetAllOAuthController>> Logger) CreateSut(
        bool withAccessToken,
        HttpStatusCode statusCode,
        string responseContent = "")
    {
        var logger = new Mock<ILogger<GetAllOAuthController>>();

        var sut = new GetAllOAuthController(
            TestHelpers.OAuthSettings(),
            logger.Object,
            TestHelpers.HttpClientFactory(statusCode, responseContent),
            TestHelpers.TokenService(withAccessToken).Object);

        return (sut, logger);
    }

    [Fact]
    public async Task GetAllOAuth_WithoutAccessToken_ReturnsInvalidAndLogsInformation()
    {
        var (sut, logger) = CreateSut(withAccessToken: false, HttpStatusCode.OK);

        var result = await sut.GetAllOAuth();

        Assert.False(result.IsValid);
        Assert.False(result.IsExpired);
        Assert.Equal(Constants.ErrorMessages.OAuthFetchFormsConfigurationFailed, result.Error);
        logger.VerifyLogged(LogLevel.Information, Times.Once());
    }

    [Fact]
    public async Task GetAllOAuth_WithSuccessfulResponse_ReturnsForms()
    {
        var (sut, _) = CreateSut(
            withAccessToken: true,
            HttpStatusCode.OK,
            TestHelpers.ReadFixture("mockResponseOAuthSetup.json"));

        var result = await sut.GetAllOAuth();

        Assert.True(result.IsValid);
        Assert.False(result.IsExpired);
        Assert.Single(result.Forms);
        Assert.Equal(TestHelpers.TestRegion, result.Forms[0].Region);
    }

    [Fact]
    public async Task GetAllOAuth_WithUnauthorizedResponse_ReportsExpiredAndLogsError()
    {
        var (sut, logger) = CreateSut(withAccessToken: true, HttpStatusCode.Unauthorized);

        var result = await sut.GetAllOAuth();

        Assert.True(result.IsExpired);
        Assert.False(result.IsValid);
        Assert.Equal(Constants.ErrorMessages.OAuthInvalidToken, result.Error);
        logger.VerifyLogged(LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task GetAllOAuth_WithServerError_ReturnsDefaultResponseAndLogsError()
    {
        var (sut, logger) = CreateSut(
            withAccessToken: true,
            HttpStatusCode.InternalServerError,
            TestHelpers.ReadFixture("mockResponseOAuthSetup.json"));

        var result = await sut.GetAllOAuth();

        Assert.False(result.IsValid);
        Assert.False(result.IsExpired);
        Assert.Empty(result.Forms);
        logger.VerifyLogged(LogLevel.Error, Times.Once());
    }
}
