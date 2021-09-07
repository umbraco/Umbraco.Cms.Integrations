using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Controllers;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.JavaScript;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.NotificationHandlers
{
    public class AddApiBaseUrl : IComponent
    {
        public void Initialize()
        {
            ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;
        }

        private void ServerVariablesParser_Parsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("This method requires an HttpContext");
            }


            var urlHelper = new UrlHelper(new RequestContext(
                new HttpContextWrapper(HttpContext.Current), new RouteData()));

            e.Add("umbracoCommerceTools", new Dictionary<string, object>
            {
                { "resourceApi", urlHelper.GetUmbracoApiServiceBaseUrl<ResourceController>(controller => controller.GetApi() ) },
            });
        }

        public void Terminate()
        {
        }
    }
}
