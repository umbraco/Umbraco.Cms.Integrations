using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Services;

using static Umbraco.Cms.Integrations.Crm.Hubspot.Core.HubspotComposer;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Controllers;

public class GetAccessTokenController : HubSpotFormsControllerBase
{
    private readonly HubspotSettings _settings;
    private readonly IHubspotAuthorizationService _authorizationService;

    public GetAccessTokenController(
        IOptions<HubspotSettings> options,
        AuthorizationImplementationFactory authorizationImplementationFactory) =>
        _authorizationService = authorizationImplementationFactory(_settings.UseUmbracoAuthorization);

    [HttpPost("getaccesstoken")]
    public async Task<string> GetAccessToken([FromBody] OAuthRequestDto authRequestDto) =>
            await _authorizationService.GetAccessTokenAsync(authRequestDto.Code);
}
