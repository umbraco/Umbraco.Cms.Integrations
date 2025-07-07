using Umbraco.Cms.Api.Management.OpenApi;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Configuration
{
    internal class BackOfficeSecurityRequirementsOperationFilter : BackOfficeSecurityRequirementsOperationFilterBase
    {
        protected override string ApiName => Constants.ManagementApi.ApiName;
    }
}
