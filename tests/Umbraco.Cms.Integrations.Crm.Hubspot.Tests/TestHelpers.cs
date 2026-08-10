using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Tests;

/// <summary>
/// Shared fixtures for the HubSpot controller tests.
/// </summary>
internal static class TestHelpers
{
    public const string TestApiKey = "test-api-key";
    public const string TestRegion = "eu1";
    public const string TestAccessToken = "test-access-token";

    /// <summary>
    /// Settings with an API key present, which is what makes the API path valid.
    /// </summary>
    public static IOptions<HubspotSettings> ApiSettings() =>
        Options.Create(new HubspotSettings { ApiKey = TestApiKey, Region = TestRegion });

    /// <summary>
    /// Settings with no API key. <see cref="HubspotSettings.UseUmbracoAuthorization"/>
    /// defaults to <c>true</c>, so this is the OAuth-via-Umbraco path.
    /// </summary>
    public static IOptions<HubspotSettings> OAuthSettings() =>
        Options.Create(new HubspotSettings { Region = TestRegion });

    /// <summary>
    /// Settings with no API key and Umbraco authorization explicitly off, so validity
    /// depends solely on the <see cref="HubspotOAuthSettings"/> passed alongside.
    /// </summary>
    public static IOptions<HubspotSettings> NoAuthorizationSettings() =>
        Options.Create(new HubspotSettings { Region = TestRegion, UseUmbracoAuthorization = false });

    public static IOptions<HubspotOAuthSettings> EmptyOAuthSettings() =>
        Options.Create(new HubspotOAuthSettings());

    public static IOptions<HubspotOAuthSettings> CompleteOAuthSettings() =>
        Options.Create(new HubspotOAuthSettings
        {
            ClientId = "client-id",
            ClientSecret = "client-secret",
            Scopes = "forms",
            TokenEndpoint = "https://example.invalid/token"
        });

    /// <summary>
    /// A token service that either yields an access token or none.
    /// </summary>
    public static Mock<ITokenService> TokenService(bool withAccessToken)
    {
        var mock = new Mock<ITokenService>();

        var value = withAccessToken ? TestAccessToken : string.Empty;
        mock.Setup(x => x.TryGetParameters(Constants.AccessTokenDbKey, out value))
            .Returns(withAccessToken);

        return mock;
    }

    /// <summary>
    /// An <see cref="IHttpClientFactory"/> whose clients always return the given
    /// status and body. Replaces the static client factory the controllers used
    /// before they took <see cref="IHttpClientFactory"/> by injection.
    /// </summary>
    public static IHttpClientFactory HttpClientFactory(HttpStatusCode statusCode, string responseContent = "")
    {
        var handler = new Mock<HttpMessageHandler>();
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(responseContent)
            });

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(() => new HttpClient(handler.Object));

        return factory.Object;
    }

    /// <summary>
    /// Reads one of the checked-in HubSpot API response fixtures.
    /// </summary>
    public static string ReadFixture(string fileName) =>
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Data", fileName));

    /// <summary>
    /// Asserts the logger recorded exactly one message at the given level.
    /// <c>ILogger.LogInformation</c>/<c>LogError</c> are extension methods, so the
    /// verification has to target the underlying <c>Log</c> method.
    /// </summary>
    public static void VerifyLogged<T>(this Mock<ILogger<T>> logger, LogLevel level, Times times)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            times);
    }
}
