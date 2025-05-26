
using Umbraco.Cms.Api.Management.OpenApi;

namespace Umbraco.Cms.Integrations.Search.Algolia.Api.Configuration;

public class BackOfficeSecurityRequirementsOperationFilter : BackOfficeSecurityRequirementsOperationFilterBase
{
    protected override string ApiName => Constants.ManagementApi.ApiName;
}
