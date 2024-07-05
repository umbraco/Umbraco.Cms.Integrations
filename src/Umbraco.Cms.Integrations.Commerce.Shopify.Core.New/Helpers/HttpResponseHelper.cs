using System;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Helpers
{
    public static class HttpResponseHelper
    {
        /// <summary>
        /// Retrieve Shopify page_info query string parameters used for pagination from forward/backwards operations.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Tuple<string, string> GetPageInfo(this HttpResponseMessage response)
        {
            string previousPageInfo = string.Empty;
            string nextPageInfo = string.Empty;

            if (!response.Headers.Contains("Link"))
            {
                return default;
            }

            var linkHeader = response.Headers.GetValues("Link").FirstOrDefault();
            if (linkHeader != null && linkHeader.Contains("rel"))
            {
                if (linkHeader.Contains("previous") && linkHeader.Contains("next"))
                {
                    var relArr = linkHeader.Split(',');
                    foreach (var item in relArr)
                    {
                        var link = item.Split(';');
                        var servicePageInfo = HttpUtility.ParseQueryString(link[0].Replace("<", string.Empty).Replace(">", string.Empty)).Get("page_info");
                        if (link[1].Contains("previous"))
                        {
                            previousPageInfo = servicePageInfo;
                        }
                        else if (link[1].Contains("next"))
                        {
                            nextPageInfo = servicePageInfo;
                        }
                    }
                }
                else
                {
                    var link = linkHeader.Split(';');
                    var servicePageInfo = HttpUtility.ParseQueryString(link[0].Replace("<", string.Empty).Replace(">", string.Empty)).Get("page_info");
                    if (link[1].Contains("previous"))
                    {
                        previousPageInfo = servicePageInfo;
                    }
                    else if (link[1].Contains("next"))
                    {
                        nextPageInfo = servicePageInfo;
                    }
                }

            }

            return new Tuple<string, string>(previousPageInfo, nextPageInfo);
        }
    }
}
