using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Umbraco.Cms.Integrations.Crm.Hubspot.Api.Management.Controllers;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;
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
            TestHelpers.TokenService(withAccessToken).Object,
            AuthorizationServiceFactory(BaseAuthorizationService.ErrorPrefix).Object);

        return (sut, logger);
    }

    private delegate void TryGetParametersCallback(string key, out string value);

    private static Mock<IHubspotAuthorizationServiceFactory> AuthorizationServiceFactory(
        string refreshResult,
        Action? onRefresh = null)
    {
        var authorizationService = new Mock<IHubspotAuthorizationService>();
        authorizationService
            .Setup(x => x.RefreshAccessTokenAsync())
            .Callback(() => onRefresh?.Invoke())
            .ReturnsAsync(refreshResult);

        var factory = new Mock<IHubspotAuthorizationServiceFactory>();
        factory
            .Setup(x => x.GetAuthorizationService(It.IsAny<bool>()))
            .Returns(authorizationService.Object);

        return factory;
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
    public async Task GetAllOAuth_WithUnauthorizedResponseAndFailedRefresh_ReportsExpiredAndLogsError()
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

    [Fact]
    public async Task GetAllOAuth_WithExpiredToken_RefreshesAndRetriesWithTheNewToken()
    {
        const string newAccessToken = "new-access-token";
        var currentToken = TestHelpers.TestAccessToken;

        var tokenService = new Mock<ITokenService>();
        tokenService
            .Setup(x => x.TryGetParameters(Constants.AccessTokenDbKey, out It.Ref<string>.IsAny))
            .Callback(new TryGetParametersCallback((string _, out string value) => value = currentToken))
            .Returns(true);

        var sentTokens = new List<string?>();
        var handler = new Mock<HttpMessageHandler>();
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken _) =>
            {
                var token = request.Headers.Authorization?.Parameter;
                sentTokens.Add(token);

                return token == newAccessToken
                    ? new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(TestHelpers.ReadFixture("mockResponseOAuthSetup.json"))
                    }
                    : new HttpResponseMessage(HttpStatusCode.Unauthorized);
            });

        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(() => new HttpClient(handler.Object));

        var sut = new GetAllOAuthController(
            TestHelpers.OAuthSettings(),
            new Mock<ILogger<GetAllOAuthController>>().Object,
            httpClientFactory.Object,
            tokenService.Object,
            AuthorizationServiceFactory("{}", onRefresh: () => currentToken = newAccessToken).Object);

        var result = await sut.GetAllOAuth();

        Assert.True(result.IsValid);
        Assert.False(result.IsExpired);
        Assert.Single(result.Forms);
        Assert.Equal(new[] { TestHelpers.TestAccessToken, newAccessToken }, sentTokens);
    }
}
