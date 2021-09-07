using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Controllers;

#if NETCOREAPP
using Microsoft.AspNetCore.Routing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;

#else
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.JavaScript;
#endif

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.NotificationHandlers
{
#if NETCOREAPP
    public class AddApiBaseUrl : INotificationHandler<ServerVariablesParsingNotification>
    {
        private LinkGenerator _linkGenerator;

        public AddApiBaseUrl(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public void Handle(ServerVariablesParsingNotification notification)
        {
            notification.ServerVariables.Add("umbracoCommerceTools", new Dictionary<string, object>
            {
                { "resourceApi", _linkGenerator.GetUmbracoApiServiceBaseUrl<ResourceController>(controller => controller.GetApi() ) },
            });
        }
    }
#else
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
#endif
}
