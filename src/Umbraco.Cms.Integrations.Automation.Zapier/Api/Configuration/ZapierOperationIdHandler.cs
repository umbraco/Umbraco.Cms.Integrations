using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Common.OpenApi;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Api.Configuration
{
    internal class ZapierOperationIdHandler : OperationIdHandler
    {
        public ZapierOperationIdHandler(IOptions<ApiVersioningOptions> apiVersioningOptions) : base(apiVersioningOptions)
        {
        }

        protected override bool CanHandle(ApiDescription apiDescription, ControllerActionDescriptor controllerActionDescriptor)
            => controllerActionDescriptor.ControllerTypeInfo.Namespace?.StartsWith("Umbraco.Cms.Integrations.Automation.") is true;
    }
}
