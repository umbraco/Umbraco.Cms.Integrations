using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    public class CheckOAuthConfigurationController : FormsControllerBase
    {
        public CheckOAuthConfigurationController(IOptions<DynamicsSettings> options, DynamicsService dynamicsService, DynamicsConfigurationService dynamicsConfigurationService, DynamicsComposer.AuthorizationImplementationFactory authorizationImplementationFactory) : base(options, dynamicsService, dynamicsConfigurationService, authorizationImplementationFactory)
        {
        }

        [HttpGet("oauth-configuration")]
        [ProducesResponseType(typeof(OAuthConfigurationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CheckOAuthConfiguration()
        {
            var oauthConfiguration = DynamicsConfigurationService.GetOAuthConfiguration();

            if (oauthConfiguration == null) return Unauthorized(new OAuthConfigurationDto { Message = string.Empty });

            var identity = await DynamicsService.GetIdentity(oauthConfiguration.AccessToken);

            if (!identity.IsAuthorized) return Unauthorized(new OAuthConfigurationDto { Message = identity.Error != null ? identity.Error.Message : string.Empty });

            oauthConfiguration.IsAuthorized = true;

            return Ok(oauthConfiguration);
        }
    }
}
