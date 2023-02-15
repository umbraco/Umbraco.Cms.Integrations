using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Integrations.Library.Parsing;
using Umbraco.Cms.Integrations.Library.Services;
using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Builders
{
    public class PublishedContentRecordBuilder
    {
        private readonly Record _record = new();

        private readonly IPublishedUrlProvider _urlProvider;

        private readonly IParserService _parserService;

        public PublishedContentRecordBuilder(IPublishedUrlProvider urlProvider, IParserService parserService)
        {
            _urlProvider = urlProvider;

            _parserService = parserService;
        }

        public PublishedContentRecordBuilder BuildFromContent(IPublishedContent publishedContent, Func<IPublishedProperty, bool> filter = null)
        {
            _record.ObjectID = publishedContent.Key.ToString();

            _record.Id = publishedContent.Id;
            _record.Name = publishedContent.Name;
            _record.Url = _urlProvider.GetUrl(publishedContent.Id);

            if (publishedContent.Cultures.Count() > 0)
            {
                foreach (var culture in publishedContent.Cultures)
                {
                    _record.Data.Add($"name-{culture.Key}", publishedContent.Cultures[culture.Key].Name);
                    _record.Data.Add($"url-{culture.Key}", _urlProvider.GetUrl(publishedContent.Id, culture: culture.Key));
                }
            }

            foreach (var property in publishedContent.Properties.Where(filter ?? (p => true)))
            {
                if (!_record.Data.ContainsKey(property.Alias))
                {
                    if (publishedContent.Cultures.Count() > 0)
                    {
                        foreach (var culture in publishedContent.Cultures)
                        {
                            string propValue = _parserService.GetParsedValue(property, culture.Key);
                            _record.Data.Add($"{property.Alias}-{culture.Key}", propValue);
                        }
                    }
                    else
                    {
                        string propValue = _parserService.GetParsedValue(property);
                        _record.Data.Add(property.Alias, propValue);
                    }
                }
            }

            return this;
        }

        public Record Build() => _record;
    }
}
