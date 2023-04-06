using Moq;
using Moq.Protected;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Controllers;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;
using Umbraco.Core.Composing;
using static Umbraco.Cms.Integrations.Crm.Hubspot.HubspotComposer;
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

        private HubspotSettings MockedAppSettingsApiSetup;

        private HubspotSettings MockedAppSettingsOAuthSetup;

        private HubspotSettings MockedAppSettingsNoSetup;

        private Mock<ILogger> MockedLogger;

        public FormsControllerTests()
        {
            Current.Factory = new Mock<IFactory>().Object;
        }

        [SetUp]
        public void Init()
        {
            MockedAppSettingsApiSetup = CreateMockedAppSettings(true);

            MockedAppSettingsOAuthSetup = CreateMockedAppSettings();

            MockedAppSettingsNoSetup = CreateMockedAppSettings();

            MockedLogger = new Mock<ILogger>();
        }

        #region CheckConfiguration

        [Test]
        public void CheckApiConfiguration_WithApiConfig_ShouldReturnValidConfigurationResponseObjectWithType()
        {
            var sut = new FormsController(Mock.Of<ITokenService>(), Mock.Of<ILogger>(), Mock.Of<AuthorizationImplementationFactory>())
            {
                Options = MockedAppSettingsApiSetup
            };

            var result = sut.CheckConfiguration();

            Assert.That(result.IsValid, Is.True);
            Assert.AreEqual(result.Type.Value, ConfigurationType.Api.Value);
        }

        [Test]
        public void CheckOAuthConfiguration_WithOAuthConfigAndNoApiConfig_ShouldReturnValidConfigurationResponseObjectWithType()
        {
            var sut = new FormsController(Mock.Of<ITokenService>(), Mock.Of<ILogger>(), Mock.Of<AuthorizationImplementationFactory>())
            {
                Options = MockedAppSettingsOAuthSetup
            };

            var result = sut.CheckConfiguration();

            Assert.That(result.IsValid, Is.True);
            Assert.AreEqual(result.Type.Value, ConfigurationType.OAuth.Value);
        }

        #endregion

        #region GetAllUsingApiKey

        [Test]
        public async Task GetAll_WithoutApiKey_ShouldReturnInvalidResponseObjectWithLoggedInfo()
        {
            var sut = new FormsController(Mock.Of<ITokenService>(), MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>())
            {
                Options = MockedAppSettingsNoSetup
            };

            var result = await sut.GetAll();

            MockedLogger.Verify(x => x.Info(It.Is<Type>(y => y == typeof(FormsController)), It.IsAny<string>()), Times.Once);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.IsExpired, Is.False);
        }

        [Test]
        public async Task GetAll_WithUnauthorizedRequest_ShouldReturnExpiredResponseObjectWithLoggedError()
        {
            var sut = new FormsController(Mock.Of<ITokenService>(), MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());
            sut.Options = MockedAppSettingsApiSetup;

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
            var sut = new FormsController(Mock.Of<ITokenService>(), MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());
            sut.Options = MockedAppSettingsApiSetup;

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
            var sut = new FormsController(Mock.Of<ITokenService>(), MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());
            sut.Options = MockedAppSettingsApiSetup;

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

            var sut = new FormsController(mockedTokenService.Object, MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());

            var result = await sut.GetAllOAuth();

            MockedLogger.Verify(x => x.Info(It.Is<Type>(y => y == typeof(FormsController)), It.IsAny<string>()), Times.Once);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.IsExpired, Is.False);
        }

        [Test]
        public async Task GetAllOAuth_WithUnauthorizedRequest_ShouldReturnExpiredResponseObjectWithLoggedError()
        {
            var mockedTokenService = CreateMockedTokenService(true);

            var sut = new FormsController(mockedTokenService.Object, MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());

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

            var sut = new FormsController(mockedTokenService.Object, MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());

            var response = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\Data\\mockResponseOAuthSetup.json");

            var httpClient = CreateMockedHttpClient(HttpStatusCode.OK, response);
            FormsController.ClientFactory = () => httpClient;

            var result = await sut.GetAllOAuth();

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.IsExpired, Is.False);
            Assert.AreEqual(1, result.Forms.Count);
        }

        [Test]
        public async Task GetAllOAuth_WithFailedRequest_ShouldReturnDefaultResponseObjectWithLoggedError()
        {
            var mockedTokenService = CreateMockedTokenService(true);

            var sut = new FormsController(mockedTokenService.Object, MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());

            var response = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\Data\\mockResponseApiSetup.json");

            var httpClient = CreateMockedHttpClient(HttpStatusCode.InternalServerError, response);
            FormsController.ClientFactory = () => httpClient;

            var result = await sut.GetAllOAuth();

            MockedLogger.Verify(x => x.Error(It.Is<Type>(y => y == typeof(FormsController)), It.IsAny<string>()), Times.Once);

            Assert.That(result.IsExpired, Is.False);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Forms, Is.Empty);
        }

        #endregion

        private static HubspotSettings CreateMockedAppSettings(bool includeApiKeySettings = false)
        {
            return includeApiKeySettings
                ? new HubspotSettings {ApiKey = "test-api-key", Region = "eu1" }
                : new HubspotSettings { Region = "eu1" };
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
