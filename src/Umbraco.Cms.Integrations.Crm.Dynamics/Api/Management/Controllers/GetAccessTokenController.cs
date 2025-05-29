using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    public class GetAccessTokenController : FormsControllerBase
    {
        public GetAccessTokenController(
            IOptions<DynamicsSettings> options, 
            IDynamicsService dynamicsService, 
            IDynamicsConfigurationStorage dynamicsConfigurationStorage, 
            DynamicsComposer.AuthorizationImplementationFactory authorizationImplementationFactory) 
            : base(options, dynamicsService, dynamicsConfigurationStorage, authorizationImplementationFactory)
        {
        }

        [HttpPost("access-token")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccessToken([FromBody] OAuthRequestDto authRequestDto) =>
            Ok(await AuthorizationService.GetAccessTokenAsync(authRequestDto.Code));
    }
}
