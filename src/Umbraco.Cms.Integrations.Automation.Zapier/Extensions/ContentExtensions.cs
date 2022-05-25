using System;
using System.Collections.Generic;

#if NETCOREAPP
using Umbraco.Cms.Core.Models;
#else
using Umbraco.Core.Models;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Extensions
{
    public static class ContentExtensions
    {
        public static Dictionary<string, string> ToContentTypeDictionary(this IContentType contentType)
        {
            var contentDict = new Dictionary<string, string>
            {
                {Constants.ContentProperties.Id, "1" },
                {Constants.ContentProperties.Name, contentType.Name },
                {Constants.ContentProperties.PublishDate, DateTime.UtcNow.ToString("s") }
            };

            foreach (var propertyType in contentType.PropertyTypes)
            {
                contentDict.Add(propertyType.Alias, string.Empty);
            }

            return contentDict;
        }

        public static Dictionary<string, string> ToContentDictionary(this IContent contentNode)
        {
            var contentDict = new Dictionary<string, string>
            {
                {Constants.ContentProperties.Id, contentNode.Id.ToString() },
                {Constants.ContentProperties.Name, contentNode.Name },
                {Constants.ContentProperties.PublishDate, contentNode.UpdateDate.ToString("s") }
            };

            foreach (var prop in contentNode.Properties)
            {
                contentDict.Add(prop.Alias, prop.Id == 0 || prop.Values.Count == 0 ? string.Empty : prop.GetValue().ToString());
            }

            return contentDict;
        }
    }
}
