using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Integrations.Crm.Dynamics.Configuration;
using Umbraco.Cms.Integrations.Crm.Dynamics.Services;
using static Umbraco.Cms.Integrations.Crm.Dynamics.DynamicsComposer;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Management.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = Constants.ManagementApi.GroupName)]
    [Route($"{Constants.ManagementApi.RootPath}/v{{version:apiVersion}}/forms")]
    public class FormsControllerBase : DynamicsControllerBase
    {
        protected readonly DynamicsSettings DynamicsSettings;

        protected readonly IDynamicsAuthorizationService AuthorizationService;

        protected readonly DynamicsService DynamicsService;

        protected readonly DynamicsConfigurationService DynamicsConfigurationService;

        public FormsControllerBase(IOptions<DynamicsSettings> options,
            DynamicsService dynamicsService,
            DynamicsConfigurationService dynamicsConfigurationService,
            AuthorizationImplementationFactory authorizationImplementationFactory)
        {

            DynamicsSettings = options.Value;

            AuthorizationService = authorizationImplementationFactory(DynamicsSettings.UseUmbracoAuthorization);

            DynamicsService = dynamicsService;

            DynamicsConfigurationService = dynamicsConfigurationService;
        }
    }
}
