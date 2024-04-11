using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services;

using static Umbraco.Cms.Integrations.Crm.Hubspot.Core.HubspotComposer;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Controllers;

public class GetAuthorizationUrlController : HubSpotFormsControllerBase
{
    private readonly HubspotSettings _settings;
    private readonly IHubspotAuthorizationService _authorizationService;

    public GetAuthorizationUrlController(
        IOptions<HubspotSettings> options,
        AuthorizationImplementationFactory authorizationImplementationFactory) =>
        _authorizationService = authorizationImplementationFactory(_settings.UseUmbracoAuthorization);

    [HttpGet("getauthorizationurl")]
    public string GetAuthorizationUrl() =>
            _authorizationService.GetAuthorizationUrl();
}
