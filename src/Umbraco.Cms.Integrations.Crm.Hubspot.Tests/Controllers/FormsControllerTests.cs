using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Controllers;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;
using Umbraco.Core.Composing;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Logging;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;
using ILogger = Umbraco.Core.Logging.ILogger;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Tests.Controllers
{
    [TestFixture]
    public class FormsControllerTests
    {
        private readonly string InvalidApiKey = @"{
            ""status"": ""error"",
            ""message"": ""This hapikey doesn't exist."",
            ""correlationId"": ""73f4a25b-7b0a-4537-a490-e5c226619d59""
        }";

        private readonly string ExpiredOAuthToken = @"{
            ""status"": ""error"",
            ""message"": ""This oauth-token is expired! expiresAt: 1643194402611, now: 1643972072320"",
            ""correlationId"": ""1ae46a55-f5b9-4f68-811a-695a12aaa4f5"",
            ""category"": ""EXPIRED_AUTHENTICATION"",
            ""errors"": [
                {
                    ""message"": ""The OAuth token used to make this call expired 9 day(s) ago.""
                }
            ]
        }";

        private Mock<IAppSettings> MockedAppSettingsApiSetup;

        private Mock<IAppSettings> MockedAppSettingsOAuthSetup;

        private Mock<ILogger> MockedLogger;

        public FormsControllerTests()
        {
            Current.Factory = new Mock<IFactory>().Object;
        }

        [SetUp]
        public void Init()
        {
            MockedAppSettingsApiSetup = CreateMockedAppSettings(true);

            MockedAppSettingsOAuthSetup = CreateMockedAppSettings(includeOAuthSettings: true);

            MockedLogger = new Mock<ILogger>();
        }

        [Test]
        public void CheckApiConfiguration_GetApiKey_ShouldReturnTrue()
        {
            var sut = new FormsController(MockedAppSettingsApiSetup.Object, Mock.Of<IHubspotService>(), Mock.Of<ITokenService>(), Mock.Of<ILogger>());
        
            var result = sut.CheckApiConfiguration();

            Assert.IsTrue(result);
        }

        [Test]
        public void CheckOAuthConfiguration_GetOAuthConfig_ShouldReturnTrue()
        {
            var sut = new FormsController(MockedAppSettingsOAuthSetup.Object, Mock.Of<IHubspotService>(), Mock.Of<ITokenService>(), Mock.Of<ILogger>());

            var result = sut.CheckOAuthConfiguration();

            Assert.IsTrue(result);
        }

        #region GetAllUsingApiKey

        [Test]
        public async Task GetAll_WithoutApiKey_ShouldReturnInvalidResponseObjectWithLoggedInfo()
        {
            var sut = new FormsController(Mock.Of<IAppSettings>(), Mock.Of<IHubspotService>(), Mock.Of<ITokenService>(), MockedLogger.Object);

            var result = await sut.GetAll();

            MockedLogger.Verify(x => x.Info(It.Is<Type>(y => y == typeof(FormsController)), It.IsAny<string>()), Times.Once);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.IsExpired, Is.False);
        }

        [Test]
        public async Task GetAll_WithUnauthorizedRequest_ShouldReturnExpiredResponseObjectWithLoggedError()
        {
            var sut = new FormsController(MockedAppSettingsApiSetup.Object, Mock.Of<IHubspotService>(), Mock.Of<ITokenService>(), MockedLogger.Object);

            var httpClient = CreateMockedHttpClient(HttpStatusCode.Unauthorized, InvalidApiKey);
            FormsController.ClientFactory = () => httpClient;

            var result = await sut.GetAll();

            MockedLogger.Verify(x => x.Error(It.Is<Type>(y => y == typeof(FormsController)), It.IsAny<string>()), Times.Once);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.IsExpired, Is.True);
        }

        [Test]
        public async Task GetAll_WithSuccessfulRequest_ShouldReturnResponseObjectWithFormsCollection()
        {
            var sut = new FormsController(MockedAppSettingsApiSetup.Object, Mock.Of<IHubspotService>(), Mock.Of<ITokenService>(), MockedLogger.Object);

            var response = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\Data\\mockResponseApiSetup.json");

            var httpClient = CreateMockedHttpClient(HttpStatusCode.OK, response);
            FormsController.ClientFactory = () => httpClient;

            var result = await sut.GetAll();

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.IsExpired, Is.False);
            Assert.AreEqual(1, result.Forms.Count);
        }

        [Test]
        public async Task GetAll_WithFailedRequest_ShouldReturnDefaultResponseObjectWithLoggedError()
        {
            var sut = new FormsController(MockedAppSettingsApiSetup.Object, Mock.Of<IHubspotService>(), Mock.Of<ITokenService>(), MockedLogger.Object);

            var response = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\Data\\mockResponseApiSetup.json");

            var httpClient = CreateMockedHttpClient(HttpStatusCode.InternalServerError, response);
            FormsController.ClientFactory = () => httpClient;

            var result = await sut.GetAll();

            MockedLogger.Verify(x => x.Error(It.Is<Type>(y => y == typeof(FormsController)), It.IsAny<string>()), Times.Once);

            Assert.That(result.IsExpired, Is.False);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Forms, Is.Empty);
        }

        #endregion

        #region GetAllUsingOAuth

        [Test]
        public async Task GetAllOAuth_WithoutAccessToken_ShouldReturnInvalidResponseObjectWithLoggedInfo()
        {
            var mockedTokenService = CreateMockedTokenService(false);

            var sut = new FormsController(Mock.Of<IAppSettings>(), Mock.Of<IHubspotService>(), mockedTokenService.Object, MockedLogger.Object);

            var result = await sut.GetAllOAuth();

            MockedLogger.Verify(x => x.Info(It.Is<Type>(y => y == typeof(FormsController)), It.IsAny<string>()), Times.Once);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.IsExpired, Is.False);
        }

        [Test]
        public async Task GetAllOAuth_WithUnauthorizedRequest_ShouldReturnExpiredResponseObjectWithLoggedError()
        {
            var mockedTokenService = CreateMockedTokenService(true);

            var sut = new FormsController(MockedAppSettingsApiSetup.Object, Mock.Of<IHubspotService>(), mockedTokenService.Object, MockedLogger.Object);

            var httpClient = CreateMockedHttpClient(HttpStatusCode.Unauthorized);
            FormsController.ClientFactory = () => httpClient;

            var result = await sut.GetAllOAuth();

            MockedLogger.Verify(x => x.Error(It.Is<Type>(y => y == typeof(FormsController)), It.IsAny<string>()), Times.Once);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.IsExpired, Is.True);
        }

        [Test]
        public async Task GetAllOAuth_WithSuccessfulRequest_ShouldReturnResponseObjectWithFormsCollection()
        {
            var mockedTokenService = CreateMockedTokenService(true);

            var sut = new FormsController(MockedAppSettingsApiSetup.Object, Mock.Of<IHubspotService>(), mockedTokenService.Object, MockedLogger.Object);

            var response = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\Data\\mockResponseOAuthSetup.json");

            var httpClient = CreateMockedHttpClient(HttpStatusCode.OK, response);
            FormsController.ClientFactory = () => httpClient;

            var result = await sut.GetAll();

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.IsExpired, Is.False);
            Assert.AreEqual(1, result.Forms.Count);
        }

        [Test]
        public async Task GetAllOAuth_WithFailedRequest_ShouldReturnDefaultResponseObjectWithLoggedError()
        {
            var mockedTokenService = CreateMockedTokenService(true);

            var sut = new FormsController(MockedAppSettingsApiSetup.Object, Mock.Of<IHubspotService>(), mockedTokenService.Object, MockedLogger.Object);

            var response = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\Data\\mockResponseApiSetup.json");

            var httpClient = CreateMockedHttpClient(HttpStatusCode.InternalServerError, response);
            FormsController.ClientFactory = () => httpClient;

            var result = await sut.GetAll();

            MockedLogger.Verify(x => x.Error(It.Is<Type>(y => y == typeof(FormsController)), It.IsAny<string>()), Times.Once);

            Assert.That(result.IsExpired, Is.False);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Forms, Is.Empty);
        }

        #endregion

        private static Mock<IAppSettings> CreateMockedAppSettings(bool includeApiKeySettings = false, bool includeOAuthSettings = false)
        {
            var mockedAppSettings = new Mock<IAppSettings>();

            if (includeApiKeySettings)
            {
                mockedAppSettings
                    .Setup(c => c[AppSettingsConstants.UmbracoCmsIntegrationsCrmHubspotApiKey])
                    .Returns("test-api-key");
            }

            if (includeOAuthSettings)
            {
                mockedAppSettings
                    .Setup(c => c[AppSettingsConstants.UmbracoCmsIntegrationsCrmHubspotOAuthClientId])
                    .Returns("74d31aa7-337d-40fb-b44b-940e8733eb65");
                mockedAppSettings
                    .Setup(c => c[AppSettingsConstants.UmbracoCmsIntegrationsCrmHubspotOAuthScopes])
                    .Returns("oauth,forms");
                mockedAppSettings
                    .Setup(c => c[AppSettingsConstants.UmbracoCmsIntegrationsCrmHubspotOAuthRedirectUrl])
                    .Returns("https://localhost://oauth/umbraco");
                mockedAppSettings
                    .Setup(c => c[AppSettingsConstants.UmbracoCmsIntegrationsOAuthProxyUrl])
                    .Returns("https://localhost");
            }

            return mockedAppSettings;
        }

        private static Mock<ITokenService> CreateMockedTokenService(bool includeAccessToken)
        {
            var mockedTokenService = new Mock<ITokenService>();

            if (includeAccessToken)
            {
                string key = "Umbraco.Cms.Integrations.Hubspot.AccessTokenDbKey";
                string expectedValue = "CNzV5ansLxICBAMY7N6UDCDJ3LYNKNuEJjIUnFi8abi-btslTeL14maOJ3J7CK86KABAIEEAAAAAAAAwAAAABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFCFEZUslFv3AAjejJzrxfHYDNIvbcnSgNldTFSAFoA";

                mockedTokenService.Setup(x => x.TryGetParameters(key, out expectedValue)).Returns(true);
            }

            return mockedTokenService;
        }


        private static HttpClient CreateMockedHttpClient(HttpStatusCode statusCode, string responseContent = "")
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = statusCode,
                    Content = new StringContent(responseContent)
                });

            var httpClient = new HttpClient(handlerMock.Object);

            return httpClient;
        }
    }
}
