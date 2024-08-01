using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.Controllers;
using static Umbraco.Cms.Integrations.Crm.Dynamics.DynamicsComposer;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Controllers
{
    [PluginController("UmbracoCmsIntegrationsCrmDynamics")]
    public class FormsController : UmbracoApiController
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        private readonly DynamicsSettings _settings;

        private readonly IDynamicsAuthorizationService _authorizationService;

        private readonly DynamicsService _dynamicsService;

        private readonly DynamicsConfigurationService _dynamicsConfigurationService;

        public FormsController(IOptions<DynamicsSettings> options,
            DynamicsService dynamicsService, 
            DynamicsConfigurationService dynamicsConfigurationService,
            AuthorizationImplementationFactory authorizationImplementationFactory)
        {

            _settings = options.Value;

            _authorizationService = authorizationImplementationFactory(_settings.UseUmbracoAuthorization);

            _dynamicsService = dynamicsService;

            _dynamicsConfigurationService = dynamicsConfigurationService;
        }

        [HttpGet]
        public string GetAuthorizationUrl() => _authorizationService.GetAuthorizationUrl();

        [HttpGet]
        public async Task<OAuthConfigurationDto> CheckOAuthConfiguration()
        {
            var oauthConfiguration = _dynamicsConfigurationService.GetOAuthConfiguration();

            if (oauthConfiguration == null) return new OAuthConfigurationDto { Message = string.Empty };

            var identity = await _dynamicsService.GetIdentity(oauthConfiguration.AccessToken);

            if (!identity.IsAuthorized) return new OAuthConfigurationDto { Message = identity.Error != null ? identity.Error.Message : string.Empty };

            oauthConfiguration.IsAuthorized = true;

            return oauthConfiguration;
        }

        [HttpPost]
        public async Task<string> GetAccessToken([FromBody] OAuthRequestDto authRequestDto) =>
            await _authorizationService.GetAccessTokenAsync(authRequestDto.Code);

        [HttpGet]
        public async Task<IEnumerable<FormDto>> GetForms(string module) =>
            await _dynamicsService.GetForms((DynamicsModule)Enum.Parse(typeof(DynamicsModule), module));

        [HttpGet]
        public async Task<string> GetEmbedCode(string formId) => await _dynamicsService.GetEmbedCode(formId);

        [HttpGet]
        public string GetSystemUserFullName() => _dynamicsConfigurationService.GetSystemUserFullName();

        [HttpDelete]
        public string RevokeAccessToken() => _dynamicsConfigurationService.Delete();
    }
}
