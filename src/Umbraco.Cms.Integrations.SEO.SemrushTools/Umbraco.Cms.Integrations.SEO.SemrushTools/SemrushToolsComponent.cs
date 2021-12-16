using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Cms.Integrations.SEO.SemrushTools.Controllers;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.JavaScript;

namespace Umbraco.Cms.Integrations.SEO.SemrushTools
{
    public class SemrushToolsComponent: IComponent
    {
        public void Initialize()
        {
            ServerVariablesParser.Parsing += ServerVariablesParserOnParsing;
        }

        private void ServerVariablesParserOnParsing(object sender, Dictionary<string, object> e)
        {
            if (!e.ContainsKey("umbracoUrls"))
            {
                throw new ArgumentException("Missing umbracoUrls.");
            }

            var umbracoUrlsObject = e["umbracoUrls"];
            if (umbracoUrlsObject == null)
            {
                throw new ArgumentException("Null umbracoUrls");
            }

            if (!(umbracoUrlsObject is Dictionary<string, object> umbracoUrls))
            {
                throw new ArgumentException("Invalid umbracoUrls");
            }

            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("HttpContext is null");
            }

            var urlHelper = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

            umbracoUrls["umbracoCmsIntegrationsSemrushBaseUrl"] = urlHelper.GetUmbracoApiServiceBaseUrl<SemrushController>(controller => controller.Ping());
        }

        public void Terminate()
        {
            ServerVariablesParser.Parsing -= ServerVariablesParserOnParsing;
        }
    }
}
