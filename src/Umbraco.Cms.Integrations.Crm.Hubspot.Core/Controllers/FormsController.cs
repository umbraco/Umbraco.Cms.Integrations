using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Resources;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services;

using static Umbraco.Cms.Integrations.Crm.Hubspot.Core.HubspotComposer;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Controllers
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

        private readonly ILogger<FormsController> _logger;

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

        public async Task<ResponseDto> GetAll()
        {
            var hubspotApiKey = Options.ApiKey;

            if (string.IsNullOrEmpty(hubspotApiKey))
            {
                _logger.LogInformation(message: Constants.ErrorMessages.ApiKeyMissing);

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
                _logger.LogError(string.Format(LoggingResources.ApiFetchFormsFailed, responseContent));

                return new ResponseDto { IsValid = false, Error = Constants.ErrorMessages.TokenPermissions };
            }
            catch (HttpRequestException ex) when (ex.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
            {
                _logger.LogError(string.Format(LoggingResources.ApiFetchFormsFailed, responseContent));

                return new ResponseDto { IsExpired = true, Error = Constants.ErrorMessages.InvalidApiKey };
            }
            catch
            {
                _logger.LogError(string.Format(LoggingResources.ApiFetchFormsFailed, responseContent));

                return new ResponseDto();
            }
        }

        public async Task<ResponseDto> GetAllOAuth()
        {
            _tokenService.TryGetParameters(Constants.AccessTokenDbKey, out string accessToken);
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogInformation(Constants.ErrorMessages.AccessTokenMissing);

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
                _logger.LogError(string.Format(LoggingResources.OAuthFetchFormsFailed, responseContent));

                return new ResponseDto { IsExpired = true, Error = Constants.ErrorMessages.InvalidApiKey };
            }
            catch
            {
                _logger.LogError(string.Format(LoggingResources.OAuthFetchFormsFailed, responseContent));

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
