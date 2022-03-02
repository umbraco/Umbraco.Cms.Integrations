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

using Umbraco.Cms.Integrations.Commerce.Shopify.Models.ViewModels;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Helpers
{
    public static class ShopifyHtmlExtensions
    {
#if NETCOREAPP
        public static IHtmlContent RenderShopifyProductsList(this HtmlHelper htmlHelper, List<ProductViewModel> vm)
        {
            return htmlHelper.Partial($"{Constants.AppPluginFolderPath}/Render/Products.cshtml", vm);
        }
#else
        public static IHtmlString RenderShopifyProductsList(this HtmlHelper htmlHelper, List<ProductViewModel> vm)
        {
            return htmlHelper.Partial($"{Constants.AppPluginFolderPath}/Render/Products.cshtml", vm);
        }
#endif
    }
}
