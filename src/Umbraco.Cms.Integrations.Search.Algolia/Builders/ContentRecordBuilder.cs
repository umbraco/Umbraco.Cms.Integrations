using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Library.Parsing;
using Umbraco.Cms.Integrations.Library.Services;
using Umbraco.Cms.Integrations.Search.Algolia.Models;

namespace Umbraco.Cms.Integrations.Search.Algolia.Builders
{
    public class ContentRecordBuilder
    {
        private readonly Record _record = new();

        private readonly IPublishedUrlProvider _urlProvider;

        private readonly IParserService _parserService;

        public ContentRecordBuilder(IPublishedUrlProvider urlProvider, IParserService parserService)
        {
            _urlProvider = urlProvider;

            _parserService = parserService;
        }

        public ContentRecordBuilder BuildFromContent(IContent content, Func<IProperty, bool> filter = null)
        {
            _record.ObjectID = content.Key.ToString();

            _record.Id = content.Id;
            _record.Name = content.Name;
            _record.Url = _urlProvider.GetUrl(content.Id);

            if (content.AvailableCultures.Count() > 0)
            {
                foreach (var culture in content.AvailableCultures)
                {
                    _record.Data.Add($"name-{culture}", content.CultureInfos[culture].Name);
                    _record.Data.Add($"url-{culture}", _urlProvider.GetUrl(content.Id, culture: culture));
                }
            }
            
            foreach (var property in content.Properties.Where(filter ?? (p => true)))
            {
                if (!_record.Data.ContainsKey(property.Alias))
                {
                    if (content.AvailableCultures.Count() > 0)
                    {
                        foreach (var culture in content.AvailableCultures)
                        {
                            string propValue = _parserService.GetParsedValue(property, culture);
                            _record.Data.Add($"{property.Alias}-{culture}", propValue);
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
