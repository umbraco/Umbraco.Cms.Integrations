using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Hubspot.Resources;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class GetAllOAuthController : HubspotFormsControllerBase
    {
        private readonly ILogger<GetAllOAuthController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHubspotAuthorizationService _authorizationService;

        [Obsolete("Use the constructor that takes IHubspotAuthorizationServiceFactory. Scheduled for removal in the next major version.")]
        public GetAllOAuthController(
            IOptions<HubspotSettings> settingsOptions,
            ILogger<GetAllOAuthController> logger,
            IHttpClientFactory httpClientFactory,
            ITokenService tokenService)
            : this(
                settingsOptions,
                logger,
                httpClientFactory,
                tokenService,
                StaticServiceProvider.Instance.GetRequiredService<IHubspotAuthorizationServiceFactory>())
        {
        }

        [ActivatorUtilitiesConstructor]
        public GetAllOAuthController(
            IOptions<HubspotSettings> settingsOptions,
            ILogger<GetAllOAuthController> logger,
            IHttpClientFactory httpClientFactory,
            ITokenService tokenService,
            IHubspotAuthorizationServiceFactory authorizationServiceFactory)
            : base(settingsOptions)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
            _authorizationService = authorizationServiceFactory.GetAuthorizationService(settingsOptions.Value.UseUmbracoAuthorization);
        }

        [HttpGet("oauth/get", Name = Constants.OperationIdentifiers.GetFormsOAuth)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        public async Task<ResponseDto> GetAllOAuth()
        {
            var client = _httpClientFactory.CreateClient();

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
                using var response = await SendWithTokenRefreshAsync(client, accessToken);

                responseContent = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                return new ResponseDto
                {
                    IsValid = true,
                    Forms = ParseForms(responseContent, Settings.Region).ToList()
                };
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogError(string.Format(LoggingResources.OAuthFetchFormsFailed, responseContent));

                return new ResponseDto { IsExpired = true, Error = Constants.ErrorMessages.OAuthInvalidToken };
            }
            catch
            {
                _logger.LogError(string.Format(LoggingResources.OAuthFetchFormsFailed, responseContent));

                return new ResponseDto();
            }
        }

        /// <summary>
        /// Sends the forms request. HubSpot access tokens are short-lived, so on a 401 the token is
        /// refreshed once and the request retried, the same way <see cref="ValidateAccessTokenController"/> does.
        /// </summary>
        private async Task<HttpResponseMessage> SendWithTokenRefreshAsync(HttpClient client, string accessToken)
        {
            var response = await client.SendAsync(CreateRequest(accessToken));
            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            var refreshResult = await _authorizationService.RefreshAccessTokenAsync();
            if (string.IsNullOrEmpty(refreshResult)
                || refreshResult.StartsWith(BaseAuthorizationService.ErrorPrefix)
                || !_tokenService.TryGetParameters(Constants.AccessTokenDbKey, out string newAccessToken)
                || string.IsNullOrEmpty(newAccessToken))
            {
                _logger.LogWarning("The HubSpot access token has expired and could not be refreshed: {RefreshResult}", refreshResult);

                return response;
            }

            response.Dispose();

            return await client.SendAsync(CreateRequest(newAccessToken));
        }
    }
}
