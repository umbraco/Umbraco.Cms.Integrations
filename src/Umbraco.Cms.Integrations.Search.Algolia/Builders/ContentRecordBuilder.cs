using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Builders
{
    public class ContentRecordBuilder
    {
        private readonly Record _record = new();

        private readonly IPublishedUrlProvider _urlProvider;

        private readonly IAlgoliaSearchPropertyIndexValueFactory _algoliaSearchPropertyIndexValueFactory;

        public ContentRecordBuilder(IPublishedUrlProvider urlProvider, IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory)
        {
            _urlProvider = urlProvider;

            _algoliaSearchPropertyIndexValueFactory = algoliaSearchPropertyIndexValueFactory;
        }

        public ContentRecordBuilder BuildFromContent(IContent content, Func<IProperty, bool> filter = null)
        {
            _record.ObjectID = content.Key.ToString();

            _record.Id = content.Id;
            _record.Name = content.Name;
            _record.CreateDate = content.CreateDate.ToString();
            _record.UpdateDate = content.UpdateDate.ToString();
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
                            var indexValue = _algoliaSearchPropertyIndexValueFactory.GetValue(property, culture);
                            _record.Data.Add($"{indexValue.Key}-{culture}", indexValue.Value);
                        }
                    }
                    else
                    {
                        var indexValue = _algoliaSearchPropertyIndexValueFactory.GetValue(property, string.Empty);
                        _record.Data.Add(indexValue.Key, indexValue.Value);
                    }

                }
            }

            return this;
        }

        public Record Build() => _record;
    }
}
