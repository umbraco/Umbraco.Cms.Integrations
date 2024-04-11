using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Core.Models;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Core.Controllers;

public class CheckConfigurationController : HubSpotFormsControllerBase
{
    public HubspotSettings Settings;

    public HubspotOAuthSettings OAuthSettings;

    public CheckConfigurationController(IOptions<HubspotSettings> options, IOptions<HubspotOAuthSettings> oauthOptions)
    {
        Settings = options.Value;
        OAuthSettings = oauthOptions.Value;
    }

    [HttpGet("check-configuration")]
    public HubspotFormPickerSettings CheckConfiguration()
    {
        return
            !string.IsNullOrEmpty(Settings.ApiKey)
                ? new HubspotFormPickerSettings { IsValid = true, Type = ConfigurationType.Api }
                : Settings.UseUmbracoAuthorization
                    ? new HubspotFormPickerSettings { IsValid = true, Type = ConfigurationType.OAuth }
                    : !string.IsNullOrEmpty(OAuthSettings.ClientId)
                   && !string.IsNullOrEmpty(OAuthSettings.Scopes)
                   && !string.IsNullOrEmpty(OAuthSettings.ClientSecret)
                   && !string.IsNullOrEmpty(OAuthSettings.TokenEndpoint)
                    ? new HubspotFormPickerSettings { IsValid = true, Type = ConfigurationType.OAuth }
                    : new HubspotFormPickerSettings();
    }
}
