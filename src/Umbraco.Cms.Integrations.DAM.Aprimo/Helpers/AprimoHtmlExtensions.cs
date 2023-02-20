using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using Umbraco.Cms.Integrations.DAM.Aprimo.Models.ViewModels;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Helpers
{
    public static class AprimoHtmlExtensions
    {
        public static IHtmlContent RenderAprimoAsset(this IHtmlHelper<dynamic> htmlHelper,
            AprimoAssetViewModel vm, string renderingViewPath = "")
        {
            return htmlHelper.Partial(string.IsNullOrEmpty(renderingViewPath)
                ? "~/App_Plugins/UmbracoCms.Integrations/DAM/Aprimo/Render/AprimoAsset.cshtml"
                : renderingViewPath,
                vm ?? new AprimoAssetViewModel());
        }
    }
}
