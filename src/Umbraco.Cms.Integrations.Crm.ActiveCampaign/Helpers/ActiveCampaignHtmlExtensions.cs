using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core.Models.ViewModels;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Helpers
{
    public static class ActiveCampaignHtmlExtensions
    {
        public static IHtmlContent RenderActiveCampaignForm(this IHtmlHelper<dynamic> htmlHelper, 
            FormViewModel formVM, string renderingViewPath = "")
        {
            return htmlHelper.Partial(string.IsNullOrEmpty(renderingViewPath) 
                ? "~/App_Plugins/UmbracoCms.Integrations/Crm/ActiveCampaign/Render/ActiveCampaignForm.cshtml" 
                : renderingViewPath,
                formVM ?? new FormViewModel());
        }
    }
}
