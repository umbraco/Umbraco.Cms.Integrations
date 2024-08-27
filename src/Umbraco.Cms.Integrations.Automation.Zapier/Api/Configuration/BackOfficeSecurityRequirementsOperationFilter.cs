using Umbraco.Cms.Api.Management.OpenApi;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Configuration
{
    public class BackOfficeSecurityRequirementsOperationFilter : BackOfficeSecurityRequirementsOperationFilterBase
    {
        protected override string ApiName => Constants.ManagementApi.ApiName;
    }
}
