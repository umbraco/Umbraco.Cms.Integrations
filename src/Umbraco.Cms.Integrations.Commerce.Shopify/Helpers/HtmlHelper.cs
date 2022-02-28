using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using Umbraco.Cms.Integrations.Commerce.Shopify.Models.ViewModels;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Helpers
{
    public static class ShopifyHtmlExtensions
    {
        public static IHtmlString RenderProductsList(this HtmlHelper htmlHelper, List<ProductViewModel> vm)
        {
            return htmlHelper.Partial($"{Constants.AppPluginFolderPath}/Render/Products.cshtml", vm);
        }
    }
}
