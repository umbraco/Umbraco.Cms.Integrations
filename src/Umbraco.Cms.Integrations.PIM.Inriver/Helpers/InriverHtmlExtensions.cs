using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using Umbraco.Cms.Integrations.PIM.Inriver.Models.ViewModels;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Helpers
{
    public static class InriverHtmlExtensions
    {
        public static IHtmlContent RenderInriverEntity(this IHtmlHelper<dynamic> htmlHelper,
            InriverEntityViewModel vm, string renderingViewPath = "")
        {
            return htmlHelper.Partial(string.IsNullOrEmpty(renderingViewPath)
                ? "~/App_Plugins/UmbracoCms.Integrations/PIM/Inriver/Render/InriverEntity.cshtml"
                : renderingViewPath,
                vm ?? new InriverEntityViewModel());
        }
    }
}
