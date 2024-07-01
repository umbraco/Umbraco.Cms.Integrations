using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class GetAuthorizationUrlController : HubspotFormsControllerBase
    {
        private readonly IHubspotAuthorizationService _authorizationService;

        public GetAuthorizationUrlController(
            IOptions<HubspotSettings> settingsOptions,
            IHubspotAuthorizationService authorizationService) 
            : base(settingsOptions) => _authorizationService = authorizationService;

        [HttpGet("authorization-url")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetAuthorizationUrl() => Ok(_authorizationService.GetAuthorizationUrl());
    }
}
