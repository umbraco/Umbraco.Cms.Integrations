using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    public class ValidateAccessTokenController : HubspotFormsControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenService _tokenService;

        public ValidateAccessTokenController(
            IOptions<HubspotSettings> settingsOptions,
            IHttpClientFactory httpClientFactory,
            ITokenService tokenService) 
            : base(settingsOptions)
        {
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
        }

        [HttpGet("validate", Name = Constants.OperationIdentifiers.ValidateAccessToken)]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateAccessToken()
        {
            var client = _httpClientFactory.CreateClient();

            _tokenService.TryGetParameters(Constants.AccessTokenDbKey, out string accessToken);

            if (string.IsNullOrEmpty(accessToken))
                return Ok(new ResponseDto
                {
                    IsValid = false,
                    Error = Constants.ErrorMessages.OAuthInvalidToken
                });

            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(HubspotFormsApiEndpoint)
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(requestMessage);

            return Ok(new ResponseDto
            {
                IsValid = response.IsSuccessStatusCode,
                IsExpired = response.StatusCode == HttpStatusCode.Unauthorized,
                Error = !response.IsSuccessStatusCode ? Constants.ErrorMessages.OAuthInvalidToken : string.Empty
            });
        }
    }
}
