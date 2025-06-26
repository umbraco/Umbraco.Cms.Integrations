using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Common.OpenApi;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Api.Configuration
{
    internal class DynamicsOperationIdHandler : OperationIdHandler
    {
        public DynamicsOperationIdHandler(IOptions<ApiVersioningOptions> apiVersioningOptions) : base(apiVersioningOptions)
        {
        }

        protected override bool CanHandle(ApiDescription apiDescription, ControllerActionDescriptor controllerActionDescriptor)
            => controllerActionDescriptor.ControllerTypeInfo.Namespace?.StartsWith("Umbraco.Cms.Integrations.Crm.Dynamics") is true;
    }
}
