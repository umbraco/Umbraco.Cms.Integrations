using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Builders
{
    public class RecordBuilder
    {
        private readonly Record _record = new();

        public RecordBuilder BuildFromContent(IContent content, Func<IProperty, bool> filter = null)
        {
            _record.ObjectID = content.Key.ToString();

            foreach (var property in content.Properties.Where(filter ?? (p => true)))
            {
                if (!_record.Data.ContainsKey(property.Alias))
                {
                    string propValue = property.GetValue().ToString();
                    if (property.GetValue() is IEnumerable<object> list)
                    {
                        propValue = string.Join(",", list.Select(p => p.ToString()));
                    }
                    _record.Data.Add(property.Alias, propValue);
                }
            }

            return this;
        }

        public RecordBuilder BuildFromContent(IPublishedContent publishedContent, Func<IPublishedProperty, bool> filter = null)
        {
            _record.ObjectID = publishedContent.Key.ToString();

            foreach (var property in publishedContent.Properties.Where(filter ?? (p => true)))
            {
                if (!_record.Data.ContainsKey(property.Alias) && property.HasValue())
                {
                    string propValue = property.GetValue().ToString();
                    if (property.GetValue() is IEnumerable<object> list)
                    {
                        propValue = string.Join(",", list.Select(p => p.ToString()));
                    }
                    _record.Data.Add(property.Alias, propValue);
                }
            }

            return this;
        }

        public Record Build() => _record;
    }
}
