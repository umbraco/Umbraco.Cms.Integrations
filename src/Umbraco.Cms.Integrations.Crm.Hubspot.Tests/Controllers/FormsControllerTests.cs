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
using static Umbraco.Cms.Integrations.Crm.Hubspot.HubspotComposer;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

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

		private IOptions<HubspotSettings> MockedOptionsApiSetup;

		private IOptions<HubspotSettings> MockedOptionsOAuthSetup;

		private IOptions<HubspotSettings> MockedOptionsNoSetup;

		private IOptions<HubspotOAuthSettings> MockedOAuthOptionsSetup;

		private Mock<ILogger<FormsController>> MockedLogger;

		public FormsControllerTests()
		{
		}

		[SetUp]
		public void Init()
		{
			MockedOptionsApiSetup = Options.Create(CreateMockedAppSettings(true));

			MockedOptionsOAuthSetup = Options.Create(CreateMockedAppSettings());

			MockedOptionsNoSetup = Options.Create(CreateMockedAppSettings());

			MockedOAuthOptionsSetup = Options.Create(new HubspotOAuthSettings());

			MockedLogger = new Mock<ILogger<FormsController>>();
		}

		#region CheckConfiguration

		[Test]
		public void CheckApiConfiguration_WithApiConfig_ShouldReturnValidConfigurationResponseObjectWithType()
		{
			var sut = new FormsController(MockedOptionsApiSetup, MockedOAuthOptionsSetup,
				Mock.Of<ITokenService>(), Mock.Of<ILogger<FormsController>>(), Mock.Of<AuthorizationImplementationFactory>());

			var result = sut.CheckConfiguration();

			Assert.That(result.IsValid, Is.True);
			Assert.AreEqual(result.Type.Value, ConfigurationType.Api.Value);
		}

		[Test]
		public void CheckOAuthConfiguration_WithOAuthConfigAndNoApiConfig_ShouldReturnValidConfigurationResponseObjectWithType()
		{
			var sut = new FormsController(MockedOptionsOAuthSetup, MockedOAuthOptionsSetup,
				Mock.Of<ITokenService>(), Mock.Of<ILogger<FormsController>>(), Mock.Of<AuthorizationImplementationFactory>());

			var result = sut.CheckConfiguration();

			Assert.That(result.IsValid, Is.True);
			Assert.AreEqual(result.Type.Value, ConfigurationType.OAuth.Value);
		}

		#endregion

		#region GetAllUsingApiKey

		[Test]
		public async Task GetAll_WithoutApiKey_ShouldReturnInvalidResponseObjectWithLoggedInfo()
		{
			IOptions<HubspotSettings> options = Options.Create(new HubspotSettings());
			IOptions<HubspotOAuthSettings> oauthOptions = Options.Create(new HubspotOAuthSettings());

			var sut = new FormsController(options, oauthOptions, Mock.Of<ITokenService>(),
				MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());

			var result = await sut.GetAll();

			MockedLogger.Verify(x => x.Log(
			   LogLevel.Information,
			   It.IsAny<EventId>(),
			   It.IsAny<It.IsAnyType>(),
			   It.IsAny<Exception>(),
			   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.IsExpired, Is.False);
		}

		[Test]
		public async Task GetAll_WithUnauthorizedRequest_ShouldReturnExpiredResponseObjectWithLoggedError()
		{
			var sut = new FormsController(MockedOptionsApiSetup, MockedOAuthOptionsSetup, 
				Mock.Of<ITokenService>(), MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());

			var httpClient = CreateMockedHttpClient(HttpStatusCode.Unauthorized, InvalidApiKey);
			FormsController.ClientFactory = () => httpClient;

			var result = await sut.GetAll();

			MockedLogger.Verify(x => x.Log(
			   LogLevel.Error,
			   It.IsAny<EventId>(),
			   It.IsAny<It.IsAnyType>(),
			   It.IsAny<Exception>(),
			   (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.IsExpired, Is.True);
		}

		[Test]
		public async Task GetAll_WithSuccessfulRequest_ShouldReturnResponseObjectWithFormsCollection()
		{
			var sut = new FormsController(MockedOptionsApiSetup, MockedOAuthOptionsSetup, Mock.Of<ITokenService>(),
				MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());

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
			var sut = new FormsController(MockedOptionsApiSetup, MockedOAuthOptionsSetup, 
				Mock.Of<ITokenService>(), MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());

			var response = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\Data\\mockResponseApiSetup.json");

			var httpClient = CreateMockedHttpClient(HttpStatusCode.InternalServerError, response);
			FormsController.ClientFactory = () => httpClient;

			var result = await sut.GetAll();

			MockedLogger.Verify(x =>
				 x.Log(LogLevel.Error,
					 It.IsAny<EventId>(),
					 It.IsAny<It.IsAnyType>(),
					 It.IsAny<Exception>(),
					 It.IsAny<Func<It.IsAnyType, Exception, string>>()
				 ), Times.Once);

			Assert.That(result.IsExpired, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Forms, Is.Empty);
		}

		#endregion

		#region GetAllUsingOAuth

		[Test]
		public async Task GetAllOAuth_WithoutAccessToken_ShouldReturnInvalidResponseObjectWithLoggedInfo()
		{
			IOptions<HubspotSettings> options = Options.Create(new HubspotSettings());
			IOptions<HubspotOAuthSettings> oauthOptions = Options.Create(new HubspotOAuthSettings());

			var mockedTokenService = CreateMockedTokenService(false);

			var sut = new FormsController(options, oauthOptions, mockedTokenService.Object, MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());

			var result = await sut.GetAllOAuth();

			MockedLogger.Verify(x =>
				 x.Log(LogLevel.Information,
					 It.IsAny<EventId>(),
					 It.IsAny<It.IsAnyType>(),
					 It.IsAny<Exception>(),
					 It.IsAny<Func<It.IsAnyType, Exception, string>>()
				 ), Times.Once);

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.IsExpired, Is.False);
		}

		[Test]
		public async Task GetAllOAuth_WithUnauthorizedRequest_ShouldReturnExpiredResponseObjectWithLoggedError()
		{
			IOptions<HubspotSettings> options = Options.Create(new HubspotSettings());
			IOptions<HubspotOAuthSettings> oauthOptions = Options.Create(new HubspotOAuthSettings());

			var mockedTokenService = CreateMockedTokenService(true);

			var sut = new FormsController(options, oauthOptions, mockedTokenService.Object, MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());

			var httpClient = CreateMockedHttpClient(HttpStatusCode.Unauthorized);
			FormsController.ClientFactory = () => httpClient;

			var result = await sut.GetAllOAuth();

			MockedLogger.Verify(x =>
				 x.Log(LogLevel.Error,
					 It.IsAny<EventId>(),
					 It.IsAny<It.IsAnyType>(),
					 It.IsAny<Exception>(),
					 It.IsAny<Func<It.IsAnyType, Exception, string>>()
				 ), Times.Once);

			Assert.That(result.IsValid, Is.False);
			Assert.That(result.IsExpired, Is.True);
		}

		[Test]
		public async Task GetAllOAuth_WithSuccessfulRequest_ShouldReturnResponseObjectWithFormsCollection()
		{
			IOptions<HubspotSettings> options = Options.Create(new HubspotSettings());
			IOptions<HubspotOAuthSettings> oauthOptions = Options.Create(new HubspotOAuthSettings());

			var mockedTokenService = CreateMockedTokenService(true);

			var sut = new FormsController(options, oauthOptions, mockedTokenService.Object, MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());

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
			IOptions<HubspotSettings> options = Options.Create(new HubspotSettings());
			IOptions<HubspotOAuthSettings> oauthOptions = Options.Create(new HubspotOAuthSettings());

			var mockedTokenService = CreateMockedTokenService(true);

			var sut = new FormsController(options, oauthOptions, mockedTokenService.Object, MockedLogger.Object, Mock.Of<AuthorizationImplementationFactory>());

			var response = File.ReadAllText(TestContext.CurrentContext.TestDirectory + "\\Data\\mockResponseApiSetup.json");

			var httpClient = CreateMockedHttpClient(HttpStatusCode.InternalServerError, response);
			FormsController.ClientFactory = () => httpClient;

			var result = await sut.GetAllOAuth();

			MockedLogger.Verify(x =>
				 x.Log(LogLevel.Error,
					 It.IsAny<EventId>(),
					 It.IsAny<It.IsAnyType>(),
					 It.IsAny<Exception>(),
					 It.IsAny<Func<It.IsAnyType, Exception, string>>()
				 ), Times.Once);

			Assert.That(result.IsExpired, Is.False);
			Assert.That(result.IsValid, Is.False);
			Assert.That(result.Forms, Is.Empty);
		}

		#endregion

		private static HubspotSettings CreateMockedAppSettings(bool includeApiKeySettings = false)
		{
			return includeApiKeySettings
				? new HubspotSettings { ApiKey = "test-api-key", Region = "eu1" }
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
