using System.Web;

using Umbraco.Cms.Integrations.Crm.Hubspot.Models.ViewModels;

#if NETCOREAPP
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#else
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Helpers
{
    public static class HubspotHtmlExtensions
    {
#if NETCOREAPP
        public static IHtmlContent RenderHubspotForm(this IHtmlHelper<dynamic> htmlHelper, HubspotFormViewModel hubspotFormViewModel)
        {
            return htmlHelper.Partial("~/App_Plugins/UmbracoCms.Integrations/Crm/Hubspot/Render/HubspotFormV9.cshtml", hubspotFormViewModel ?? new HubspotFormViewModel());
        }
#else
        public static IHtmlString RenderHubspotForm(this HtmlHelper htmlHelper, HubspotFormViewModel hubspotFormViewModel)
        {
            return htmlHelper.Partial("~/App_Plugins/UmbracoCms.Integrations/Crm/Hubspot/Render/HubspotForm.cshtml", hubspotFormViewModel ?? new HubspotFormViewModel());
        }
#endif
    }
}
