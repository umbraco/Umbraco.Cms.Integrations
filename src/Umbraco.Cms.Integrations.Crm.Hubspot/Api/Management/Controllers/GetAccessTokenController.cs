using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models.Dtos;
using static Umbraco.Cms.Integrations.Crm.Hubspot.HubspotComposer;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    public class GetAccessTokenController : HubspotFormsControllerBase
    {
        private readonly IHubspotAuthorizationService _authorizationService;

        public GetAccessTokenController(
            IOptions<HubspotSettings> settingsOptions,
            AuthorizationImplementationFactory authorizationImplementationFactory) 
            : base(settingsOptions) 
            => _authorizationService = authorizationImplementationFactory(Settings.UseUmbracoAuthorization);

        [HttpPost("access-token", Name = Constants.OperationIdentifiers.GetAccessToken)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccessToken([FromBody] OAuthRequestDto authRequestDto)
        {
            var accessToken = await _authorizationService.GetAccessTokenAsync(authRequestDto.Code);
            return Ok(accessToken);
        }
    }
}
