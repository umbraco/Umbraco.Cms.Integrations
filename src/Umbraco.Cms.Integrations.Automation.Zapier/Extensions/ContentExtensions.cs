using System;
using System.Collections.Generic;
using System.Linq;

#if NETCOREAPP
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
#else
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Extensions
{
    public static class ContentExtensions
    {
        public static Dictionary<string, string> ToContentTypeDictionary(this IContentType contentType, IPublishedContent content)
        {
            var contentDict = new Dictionary<string, string>
            {
                {Constants.ContentProperties.Id, content != null ? content.Id.ToString() : "1" },
                {Constants.ContentProperties.Name, content != null ? content.Name : contentType.Name },
                {Constants.ContentProperties.PublishDate, content != null ? content.UpdateDate.ToString("s") : DateTime.UtcNow.ToString("s") }
            };

            foreach (var propertyType in contentType.PropertyTypes)
            {
                if (content == null)
                {
                    contentDict.Add(propertyType.Alias, string.Empty);
                    continue;
                }

                var contentProperty = content.Properties.First(p => p.Alias == propertyType.Alias);

                if(IsMedia(contentProperty, out string url))
                    contentDict.Add(propertyType.Alias, url);
                else
                    contentDict.Add(propertyType.Alias, contentProperty.GetValue().ToString());
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

        private static bool IsMedia(IPublishedProperty contentProperty, out string url)
        {
            switch(contentProperty.PropertyType.EditorAlias)
            {
                case Constants.MediaAliases.UmbracoMediaPicker:
                    var mediaPickerValue = contentProperty.GetValue() as IPublishedContent;
#if NETCOREAPP
                    url = mediaPickerValue.Url();
#else
                    url = mediaPickerValue.Url;
#endif
                    return true;
                case Constants.MediaAliases.UmbracoMediaPicker3:
                    var mediaPicker3Value = contentProperty.GetValue() as MediaWithCrops;
                    url = mediaPicker3Value.LocalCrops.Src;
                    return true;
                default:
                    url = string.Empty;
                    return false;
            }
        }
    }
}
