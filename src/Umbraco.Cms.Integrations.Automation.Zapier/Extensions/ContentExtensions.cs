using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;



#if NETCOREAPP
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
#else
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
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

                if (IsList(contentProperty, out string value))
                {
                    contentDict.Add(propertyType.Alias, value);
                } else if (IsMedia(contentProperty, out string url))
                {
                    contentDict.Add(propertyType.Alias, url);
                }
                else
                    contentDict.Add(propertyType.Alias, contentProperty.GetValue()?.ToString());
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
            switch (contentProperty.PropertyType.EditorAlias)
            {
                case Core.Constants.PropertyEditors.Aliases.MediaPicker:
                    var mediaPickerValue = contentProperty.GetValue() as IPublishedContent;
                    url = mediaPickerValue != null ? mediaPickerValue.Url() : string.Empty;
                    return true;
                case Core.Constants.PropertyEditors.Aliases.MediaPicker3:
                    var mediaPicker3Value = contentProperty.GetValue() as MediaWithCrops;
                    url = mediaPicker3Value != null ? mediaPicker3Value.LocalCrops.Src : string.Empty;
                    return true;
                default:
                    url = string.Empty;
                    return false;
            }
        }

        private static bool IsList(IPublishedProperty contentProperty, out string value)
        {
            List<string> items = new List<string>();
            value = string.Empty;
            bool isList = false;

            if (contentProperty.GetValue() == null) { return false; }

            var type = contentProperty.GetValue().GetType();
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (typeof(IList).IsAssignableFrom(type))
            {
                isList = true;
            }

            foreach (var item in type.GetInterfaces())
            {
                if (item.IsGenericType && typeof(IList<>) == item.GetGenericTypeDefinition())
                {
                    isList = true;
                }
            }

            if (isList)
            {
                var contentPropertyValue = contentProperty.GetValue();
                var contentPropertyValueType = contentPropertyValue.GetType();

                if (contentPropertyValueType.GetProperties().Count(p => p.Name == "Item") == 1)
                {
                    var count = contentPropertyValueType.GetProperty("Count").GetValue(contentPropertyValue);
                    PropertyInfo indexer = contentPropertyValueType.GetProperty("Item");

                    for (int i = 0; i < (int)count; i++)
                    {
                        object item = indexer.GetValue(contentProperty.GetValue(), new object[] { i });
                        var val = item.GetType().GetProperty("Name").GetValue(item).ToString();

                        items.Add(val);
                    }

                    value = string.Join(", ", items.ToArray());
                } else
                {
                    // is array
                    foreach (var item in contentPropertyValue as Array)
                    {
                        items.Add(item.ToString());
                    }

                    value = string.Join(", ", items.ToArray());
                }
            }

            return isList;
        }
    }
}
