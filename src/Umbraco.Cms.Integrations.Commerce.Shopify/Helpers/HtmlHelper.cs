using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using Umbraco.Cms.Integrations.Commerce.Shopify.Models.ViewModels;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Helpers
{
    public static class ShopifyHtmlExtensions
    {
        public static IHtmlContent RenderShopifyProductsList(this IHtmlHelper<dynamic> htmlHelper, List<ProductViewModel> vm, string renderingViewPath = "")
        {
            return htmlHelper.Partial(string.IsNullOrEmpty(renderingViewPath) ? Constants.RenderingComponent.DefaultV9ViewPath : renderingViewPath, vm);
        }
    }
}
