using Umbraco.Cms.Api.Management.OpenApi;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Api.Configuration
{
    internal class BackOfficeSecurityRequirementsOperationFilter : BackOfficeSecurityRequirementsOperationFilterBase
    {
        protected override string ApiName => Constants.ManagementApi.ApiName;
    }
}
