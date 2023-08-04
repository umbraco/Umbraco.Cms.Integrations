using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;
using Umbraco.Extensions;

namespace Umbraco.Cms.Integrations.Search.Algolia.Builders
{
    public class ContentRecordBuilder
    {
        private readonly Record _record = new();

        private readonly IUserService _userService;

        private readonly IPublishedUrlProvider _urlProvider;

        private readonly IAlgoliaSearchPropertyIndexValueFactory _algoliaSearchPropertyIndexValueFactory;

        public ContentRecordBuilder(IUserService userService, IPublishedUrlProvider urlProvider, IAlgoliaSearchPropertyIndexValueFactory algoliaSearchPropertyIndexValueFactory)
        {
            _userService = userService;

            _urlProvider = urlProvider;

            _algoliaSearchPropertyIndexValueFactory = algoliaSearchPropertyIndexValueFactory;
        }

        public ContentRecordBuilder BuildFromContent(IContent content, Func<IProperty, bool> filter = null)
        {
            _record.ObjectID = content.Key.ToString();

            var creator = _userService.GetProfileById(content.CreatorId);
            var writer = _userService.GetProfileById(content.WriterId);

            _record.Id = content.Id;
            _record.Name = content.Name;
            
            _record.CreateDate = content.CreateDate.ToString();
            _record.CreatorName = creator.Name;
            _record.UpdateDate = content.UpdateDate.ToString();
            _record.WriterName = writer.Name;

            _record.TemplateId = content.TemplateId.HasValue ? content.TemplateId.Value : -1;
            _record.Level = content.Level;
            _record.Path = content.Path;
            _record.ContentTypeAlias = content.ContentType.Alias;
            _record.Url = _urlProvider.GetUrl(content.Id);

            if (content.PublishedCultures.Count() > 0)
            {
                foreach (var culture in content.PublishedCultures)
                {
                    _record.Data.Add($"name-{culture}", content.CultureInfos[culture].Name);
                    _record.Data.Add($"url-{culture}", _urlProvider.GetUrl(content.Id, culture: culture));
                }
            }
            
            foreach (var property in content.Properties.Where(filter ?? (p => true)))
            {
                if (!_record.Data.ContainsKey(property.Alias))
                {
                    if (property.PropertyType.VariesByCulture())
                    {
                        foreach (var culture in content.PublishedCultures)
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
