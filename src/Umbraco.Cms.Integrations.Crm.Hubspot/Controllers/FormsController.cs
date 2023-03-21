using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

#if NETCOREAPP
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
#else
using System.Web.Http;
using System.Configuration;

using Umbraco.Core.Logging;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;
#endif

using Newtonsoft.Json;

using Umbraco.Cms.Integrations.Crm.Hubspot.Resources;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;

using static Umbraco.Cms.Integrations.Crm.Hubspot.HubspotComposer;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCrmHubspot")]
    public class FormsController : UmbracoAuthorizedApiController
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        // public property to support unit tests
        public HubspotSettings Options;

        public HubspotOAuthSettings OAuthOptions;

        private readonly ITokenService _tokenService;

        private readonly IHubspotAuthorizationService _authorizationService;

        private const string HubspotFormsApiEndpoint = "https://api.hubapi.com/forms/v2/forms";

#if NETCOREAPP
        private readonly ILogger<FormsController> _logger;
#else
        private readonly ILogger _logger;
#endif

#if NETCOREAPP
        public FormsController(
            IOptions<HubspotSettings> options,
            IOptions<HubspotOAuthSettings> oauthOptions,
            ITokenService tokenService, 
            ILogger<FormsController> logger,
            AuthorizationImplementationFactory authorizationImplementationFactory)
        {
            Options = options.Value;

            OAuthOptions = oauthOptions.Value;

            _tokenService = tokenService;

            _logger = logger;

            _authorizationService = authorizationImplementationFactory(Options.UseUmbracoAuthorization);
        }
#else
        public FormsController(ITokenService tokenService, ILogger logger, AuthorizationImplementationFactory authorizationImplementationFactory)
        {
            Options = new HubspotSettings(ConfigurationManager.AppSettings);

            OAuthOptions = new HubspotOAuthSettings(ConfigurationManager.AppSettings);

            _tokenService = tokenService;

            _logger = logger;

            _authorizationService = authorizationImplementationFactory(Options.UseUmbracoAuthorization);
        }
#endif

        public async Task<ResponseDto> GetAll()
        {
            var hubspotApiKey = Options.ApiKey;

            if (string.IsNullOrEmpty(hubspotApiKey))
            {
#if NETCOREAPP
                _logger.LogInformation(message: Constants.ErrorMessages.ApiKeyMissing);
#else
                _logger.Info<FormsController>(message: Constants.ErrorMessages.ApiKeyMissing);
#endif

                return new ResponseDto { IsValid = false, Error = Constants.ErrorMessages.ApiKeyMissing };
            }

            string responseContent = string.Empty;
            try
            {
                var requestMessage = CreateRequest(hubspotApiKey);

                var response = await ClientFactory().SendAsync(requestMessage);

                responseContent = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                var forms = await response.Content.ReadAsStringAsync();

                return new ResponseDto
                {
                    IsValid = true,
                    Forms = ParseForms(forms).ToList()
                };
            }
            catch (HttpRequestException ex) when (ex.Message.Contains(HttpStatusCode.Forbidden.ToString()))
            {
#if NETCOREAPP
                _logger.LogError(string.Format(LoggingResources.ApiFetchFormsFailed, responseContent));
#else
                _logger.Error<FormsController>(string.Format(LoggingResources.ApiFetchFormsFailed, responseContent));
#endif
                return new ResponseDto { IsValid = false, Error = Constants.ErrorMessages.TokenPermissions };
            }
            catch (HttpRequestException ex) when (ex.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
            {
#if NETCOREAPP
                _logger.LogError(string.Format(LoggingResources.ApiFetchFormsFailed, responseContent));
#else
                _logger.Error<FormsController>(string.Format(LoggingResources.ApiFetchFormsFailed, responseContent));
#endif

                return new ResponseDto { IsExpired = true, Error = Constants.ErrorMessages.InvalidApiKey };
            }
            catch
            {
#if NETCOREAPP
                _logger.LogError(string.Format(LoggingResources.ApiFetchFormsFailed, responseContent));
#else
                _logger.Error<FormsController>(string.Format(LoggingResources.ApiFetchFormsFailed, responseContent));
#endif

                return new ResponseDto();
            }
        }

        public async Task<ResponseDto> GetAllOAuth()
        {
            _tokenService.TryGetParameters(Constants.AccessTokenDbKey, out string accessToken);
            if (string.IsNullOrEmpty(accessToken))
            {
#if NETCOREAPP
                _logger.LogInformation(Constants.ErrorMessages.AccessTokenMissing);
#else
                _logger.Info<FormsController>(Constants.ErrorMessages.AccessTokenMissing);
#endif

                return new ResponseDto 
                { 
                    IsValid = false,
                    Error = Constants.ErrorMessages.OAuthFetchFormsConfigurationFailed
                };
            }

            string responseContent = string.Empty;
            try
            {
                var requestMessage = CreateRequest(accessToken);

                var response = await ClientFactory().SendAsync(requestMessage);

                responseContent = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                var forms = await response.Content.ReadAsStringAsync();

                return new ResponseDto
                {
                    IsValid = true,
                    Forms = ParseForms(forms).ToList()
                };
            }
            catch(HttpRequestException ex) when (ex.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
            {
#if NETCOREAPP
                _logger.LogError(string.Format(LoggingResources.OAuthFetchFormsFailed, responseContent));
#else
                _logger.Error<FormsController>(string.Format(LoggingResources.OAuthFetchFormsFailed, responseContent));
#endif   
                return new ResponseDto { IsExpired = true, Error = Constants.ErrorMessages.InvalidApiKey };
            }
            catch
            {
#if NETCOREAPP
                _logger.LogError(string.Format(LoggingResources.OAuthFetchFormsFailed, responseContent));
#else
                _logger.Error<FormsController>(string.Format(LoggingResources.OAuthFetchFormsFailed, responseContent));
#endif

                return new ResponseDto();
            }
        }

        private IEnumerable<HubspotFormDto> ParseForms(string json)
        {
            var hubspotForms = HubspotForms.FromJson(json);
            foreach (var hubspotForm in hubspotForms)
            {
                var hubspotFormDto = new HubspotFormDto
                {
                    Name = hubspotForm.Name,
                    PortalId = hubspotForm.PortalId.ToString(),
                    Id = hubspotForm.Guid,
                    Region = Options.Region,
                    Fields = string.Join(", ", hubspotForm.FormFieldGroups.SelectMany(x => x.Fields).Select(y => y.Label))
                };

                yield return hubspotFormDto;
            }
        }

        private HttpRequestMessage CreateRequest(string accessToken)
        { 
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(HubspotFormsApiEndpoint)
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return requestMessage;
        }

        [HttpGet]
        public HubspotFormPickerSettings CheckConfiguration()
        {
            return
                !string.IsNullOrEmpty(Options.ApiKey)
                    ? new HubspotFormPickerSettings { IsValid = true, Type = ConfigurationType.Api }
                    : Options.UseUmbracoAuthorization 
                        ? new HubspotFormPickerSettings { IsValid = true, Type = ConfigurationType.OAuth }
                        : !string.IsNullOrEmpty(OAuthOptions.ClientId)
                       && !string.IsNullOrEmpty(OAuthOptions.Scopes)
                       && !string.IsNullOrEmpty(OAuthOptions.ClientSecret)
                       && !string.IsNullOrEmpty(OAuthOptions.TokenEndpoint)
                        ? new HubspotFormPickerSettings { IsValid = true, Type = ConfigurationType.OAuth }
                        : new HubspotFormPickerSettings();
        }

        [HttpGet]
        public string GetAuthorizationUrl() =>
            _authorizationService.GetAuthorizationUrl();

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] OAuthRequestDto authRequestDto) =>
            await _authorizationService.GetAccessTokenAsync(authRequestDto.Code);

        [HttpPost]
        public async Task<string> RefreshAccessToken() =>
            await _authorizationService.RefreshAccessTokenAsync();

        [HttpGet]
        public async Task<ResponseDto> ValidateAccessToken()
        {
            _tokenService.TryGetParameters(Constants.AccessTokenDbKey, out string accessToken);

            if (string.IsNullOrEmpty(accessToken)) 
                return new ResponseDto 
                { 
                    IsValid = false, 
                    Error = Constants.ErrorMessages.OAuthInvalidToken
                };

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(HubspotFormsApiEndpoint)
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await ClientFactory().SendAsync(requestMessage);

            return new ResponseDto
            {
                IsValid = response.IsSuccessStatusCode,
                IsExpired = response.StatusCode == HttpStatusCode.Unauthorized,
                Error = !response.IsSuccessStatusCode ? Constants.ErrorMessages.OAuthInvalidToken : string.Empty
            };
        }

        [HttpPost]
        public void RevokeAccessToken()
        {
            _tokenService.RemoveParameters(Constants.AccessTokenDbKey);
            _tokenService.RemoveParameters(Constants.RefreshTokenDbKey);
        }
    }
}
