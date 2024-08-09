using Umbraco.Cms.Integrations.Crm.Dynamics.Models.ViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Helpers
{
    public static class DynamicsHtmlExtensions
    {
        public static IHtmlContent RenderDynamicsForm(this IHtmlHelper<dynamic> htmlHelper, FormViewModel formVM, string renderingViewPath = "")
        {
            return htmlHelper.Partial(string.IsNullOrEmpty(renderingViewPath) 
                ? Constants.RenderingComponent.DefaultViewPath : renderingViewPath,
                formVM);
        }
    }
}
