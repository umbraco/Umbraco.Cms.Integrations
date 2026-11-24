using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;
using Xunit;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Tests.Services;

/// <summary>
/// Real-time forms are fetched while the page renders, with a stored access token that can
/// expire. These tests cover the fallback to the last form fetched successfully.
/// </summary>
public class DynamicsServiceTests : IDisposable
{
    private const string FormId = "form-1";

    private const string FormsResponse = """
        { "value": [ { "msdynmkt_marketingformid": "form-1", "msdynmkt_name": "Contact", "msdynmkt_formhtml": "<form>raw</form>", "msdynmkt_standalonehtml": "<form>standalone</form>" } ] }
        """;

    private readonly Func<HttpClient> _originalClientFactory = DynamicsService.ClientFactory;

    private HttpStatusCode _statusCode = HttpStatusCode.OK;

    private string _responseContent = FormsResponse;

    public DynamicsServiceTests()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(_statusCode) { Content = new StringContent(_responseContent) });

        var client = new HttpClient(handler.Object);
        DynamicsService.ClientFactory = () => client;
    }

    public void Dispose() => DynamicsService.ClientFactory = _originalClientFactory;

    private static DynamicsService CreateSut(AppCaches appCaches)
    {
        var storage = new Mock<IDynamicsConfigurationStorage>();
        storage
            .Setup(x => x.GetOAuthConfiguration())
            .Returns(new OAuthConfigurationDto { AccessToken = "token" });

        return new DynamicsService(
            Options.Create(new DynamicsSettings { HostUrl = "https://example.invalid/", ApiPath = "api/data/v9.2/" }),
            storage.Object,
            appCaches,
            NullLogger<DynamicsService>.Instance);
    }

    private static AppCaches CreateAppCaches() =>
        new(new ObjectCacheAppCache(), NoAppCache.Instance, new IsolatedCaches(_ => NoAppCache.Instance));

    [Fact]
    public async Task GetRealTimeForm_WhenTheFetchSucceeds_ReturnsTheForm()
    {
        var form = await CreateSut(CreateAppCaches()).GetRealTimeForm(FormId);

        Assert.NotNull(form);
        Assert.Equal("<form>raw</form>", form.RawHtml);
        Assert.Equal("<form>standalone</form>", form.StandaloneHtml);
    }

    [Fact]
    public async Task GetRealTimeForm_WhenTheTokenHasExpired_ReturnsTheLastFormFetched()
    {
        var sut = CreateSut(CreateAppCaches());
        await sut.GetRealTimeForm(FormId);

        _statusCode = HttpStatusCode.Unauthorized;
        _responseContent = string.Empty;
        var form = await sut.GetRealTimeForm(FormId);

        Assert.NotNull(form);
        Assert.Equal("<form>raw</form>", form.RawHtml);
    }

    [Fact]
    public async Task GetRealTimeForm_WhenTheTokenHasExpiredAndNothingWasFetchedBefore_ReturnsNull()
    {
        _statusCode = HttpStatusCode.Unauthorized;
        _responseContent = string.Empty;

        var form = await CreateSut(CreateAppCaches()).GetRealTimeForm(FormId);

        Assert.Null(form);
    }

    [Fact]
    public async Task GetRealTimeForm_WhenTheFormNoLongerExists_ReturnsNullAndForgetsIt()
    {
        var sut = CreateSut(CreateAppCaches());
        await sut.GetRealTimeForm(FormId);

        _responseContent = """{ "value": [] }""";
        Assert.Null(await sut.GetRealTimeForm(FormId));

        _statusCode = HttpStatusCode.Unauthorized;
        _responseContent = string.Empty;
        Assert.Null(await sut.GetRealTimeForm(FormId));
    }
}
