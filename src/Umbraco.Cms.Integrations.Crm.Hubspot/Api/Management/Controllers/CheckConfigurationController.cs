using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Hubspot.Configuration;
using Umbraco.Cms.Integrations.Crm.Hubspot.Models;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    public class CheckConfigurationController : HubspotFormsControllerBase
    {
        private readonly HubspotOAuthSettings _oauthSettings;

        public CheckConfigurationController(
            IOptions<HubspotSettings> settingsOptions, 
            IOptions<HubspotOAuthSettings> oauthSettingsOptions) : base(settingsOptions)
        {
            _oauthSettings = oauthSettingsOptions.Value;
        }

        [HttpGet("check-configuration")]
        [ProducesResponseType(typeof(HubspotFormPickerSettings), StatusCodes.Status200OK)]
        public IActionResult CheckConfiguration()
        {
            var settings = !string.IsNullOrEmpty(Settings.ApiKey)
                    ? new HubspotFormPickerSettings { IsValid = true, Type = ConfigurationType.Api }
                    : Settings.UseUmbracoAuthorization
                        ? new HubspotFormPickerSettings { IsValid = true, Type = ConfigurationType.OAuth }
                        : !string.IsNullOrEmpty(_oauthSettings.ClientId)
                       && !string.IsNullOrEmpty(_oauthSettings.Scopes)
                       && !string.IsNullOrEmpty(_oauthSettings.ClientSecret)
                       && !string.IsNullOrEmpty(_oauthSettings.TokenEndpoint)
                        ? new HubspotFormPickerSettings { IsValid = true, Type = ConfigurationType.OAuth }
                        : new HubspotFormPickerSettings();

            return Ok(settings);
        }
    }
}
