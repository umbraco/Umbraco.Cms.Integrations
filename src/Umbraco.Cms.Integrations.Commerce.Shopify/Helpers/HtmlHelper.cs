using System.Collections.Generic;

#if NETCOREAPP
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
#else
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
#endif

using Umbraco.Cms.Integrations.Commerce.Shopify.Core.Models.ViewModels;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Core.Helpers
{
    public static class ShopifyHtmlExtensions
    {
#if NETCOREAPP
        public static IHtmlContent RenderShopifyProductsList(this IHtmlHelper<dynamic> htmlHelper, List<ProductViewModel> vm, string renderingViewPath = "")
        {
            return htmlHelper.Partial(string.IsNullOrEmpty(renderingViewPath) ? Constants.RenderingComponent.DefaultV9ViewPath : renderingViewPath, vm);
        }
#else
        public static IHtmlString RenderShopifyProductsList(this HtmlHelper htmlHelper, List<ProductViewModel> vm, string renderingViewPath = "")
        {
            return htmlHelper.Partial(string.IsNullOrEmpty(renderingViewPath) ? Constants.RenderingComponent.DefaultV8ViewPath : renderingViewPath, vm);
        }
#endif
    }
}
