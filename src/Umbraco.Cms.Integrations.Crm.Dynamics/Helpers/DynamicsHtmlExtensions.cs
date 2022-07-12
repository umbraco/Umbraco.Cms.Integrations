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
        public static IHtmlContent RenderDynamicsForm(this IHtmlHelper<dynamic> htmlHelper, FormViewModel formVM)
        {
            return htmlHelper.Partial("~/App_Plugins/UmbracoCms.Integrations/Crm/Dynamics/Render/DynamicsFormV9.cshtml", formVM);
        }
#else
        public static IHtmlString RenderDynamicsForm(this HtmlHelper helper, FormViewModel formVM)
        {
            return helper.Partial("~/App_Plugins/UmbracoCms.Integrations/Crm/Dynamics/Render/DynamicsFormV8.cshtml", formVM);
        }
#endif
    }
}
