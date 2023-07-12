using System.Web;

using Umbraco.Cms.Integrations.Crm.Hubspot.Models.ViewModels;

using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Helpers
{
    public static class HubspotHtmlExtensions
    {
        public static IHtmlString RenderHubspotForm(this HtmlHelper htmlHelper, HubspotFormViewModel hubspotFormViewModel)
        {
            return htmlHelper.Partial("~/App_Plugins/UmbracoCms.Integrations/Crm/Hubspot/Render/HubspotForm.cshtml", hubspotFormViewModel ?? new HubspotFormViewModel());
        }
    }
}
