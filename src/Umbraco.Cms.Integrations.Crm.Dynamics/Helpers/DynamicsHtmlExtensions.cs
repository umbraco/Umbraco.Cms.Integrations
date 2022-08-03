using Umbraco.Cms.Integrations.Crm.Dynamics.Models.ViewModels;

#if NETCOREAPP
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
#else
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Helpers
{
    public static class DynamicsHtmlExtensions
    {
#if NETCOREAPP
        public static IHtmlContent RenderDynamicsForm(this IHtmlHelper<dynamic> htmlHelper, FormViewModel formVM, string renderingViewPath = "")
        {
            return htmlHelper.Partial(string.IsNullOrEmpty(renderingViewPath) 
                ? Constants.RenderingComponent.DefaultViewPath : renderingViewPath,
                formVM);
        }
#else
        public static IHtmlString RenderDynamicsForm(this HtmlHelper helper, FormViewModel formVM, string renderingViewPath = "")
        {
            return helper.Partial(string.IsNullOrEmpty(renderingViewPath)
                ? Constants.RenderingComponent.DefaultV8ViewPath : renderingViewPath,
                formVM);
        }
#endif
    }
}
