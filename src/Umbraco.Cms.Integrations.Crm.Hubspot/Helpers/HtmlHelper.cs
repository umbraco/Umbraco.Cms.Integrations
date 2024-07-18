using Umbraco.Cms.Integrations.Crm.Hubspot.Models.ViewModels;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Helpers
{
    public static class HubspotHtmlExtensions
    {
        public static IHtmlContent RenderHubspotForm(this IHtmlHelper<dynamic> htmlHelper, HubspotFormViewModel hubspotFormViewModel)
        {
            return htmlHelper.Partial("~/App_Plugins/Hubspot/Render/HubspotForm.cshtml", hubspotFormViewModel ?? new HubspotFormViewModel());
        }
    }
}
