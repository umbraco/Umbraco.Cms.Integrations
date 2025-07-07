using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
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

        public GetAllOAuthController(
            IOptions<HubspotSettings> settingsOptions, 
            ILogger<GetAllOAuthController> logger,
            IHttpClientFactory httpClientFactory,
            ITokenService tokenService) 
            : base(settingsOptions)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
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
                var requestMessage = CreateRequest(accessToken);

                var response = await client.SendAsync(requestMessage);

                responseContent = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();

                var forms = await response.Content.ReadAsStringAsync();

                return new ResponseDto
                {
                    IsValid = true,
                    Forms = ParseForms(forms, Settings.Region).ToList()
                };
            }
            catch (HttpRequestException ex) when (ex.Message.Contains(HttpStatusCode.Unauthorized.ToString()))
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
    }
}
