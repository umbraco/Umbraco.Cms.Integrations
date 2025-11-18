using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Services;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    public class RefreshAccessTokenController : HubspotFormsControllerBase
    {
        private readonly IHubspotAuthorizationService _authorizationService;

        public RefreshAccessTokenController(
            IOptions<HubspotSettings> settingsOptions,
            IHubspotAuthorizationServiceFactory authorizationServiceFactory) 
            : base(settingsOptions) => _authorizationService = authorizationServiceFactory.GetAuthorizationService(Settings.UseUmbracoAuthorization);

        [HttpPost("refresh", Name = Constants.OperationIdentifiers.RefreshAccessToken)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshAccessToken()
        {
            var response = await _authorizationService.RefreshAccessTokenAsync();
            return Ok(response);
        }
    }
}
