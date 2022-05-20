using System.Collections.Generic;

#if NETCOREAPP
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Extensions
{
    public static class ContentExtensions
    {
        public static Dictionary<string, string> ToContentDictionary(this IPublishedContent publishedContent)
        {
            var contentDict = new Dictionary<string, string>
            {
                {Constants.Content.Id, publishedContent.Id.ToString() },
                {Constants.Content.Name, publishedContent.Name },
                {Constants.Content.PublishDate, publishedContent.UpdateDate.ToString("s") }
            };

            foreach (var prop in publishedContent.Properties)
            {
                contentDict.Add(prop.Alias, string.Empty);
            }

            return contentDict;
        }

        public static Dictionary<string, string> ToContentDictionary(this IContent contentNode)
        {
            var contentDict = new Dictionary<string, string>
            {
                {Constants.Content.Id, contentNode.Id.ToString() },
                {Constants.Content.Name, contentNode.Name },
                {Constants.Content.PublishDate, contentNode.UpdateDate.ToString("s") }
            };

            foreach (var prop in contentNode.Properties)
            {
                contentDict.Add(prop.Alias, prop.Id == 0 || prop.Values.Count == 0 ? string.Empty : prop.GetValue().ToString());
            }

            return contentDict;
        }
    }
}
