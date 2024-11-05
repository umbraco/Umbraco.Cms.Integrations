using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Search.Algolia.Providers;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public class AlgoliaSearchPropertyIndexValueFactory : IAlgoliaSearchPropertyIndexValueFactory
    {
        private readonly PropertyEditorCollection _propertyEditorsCollection;

        private readonly ConverterCollection _converterCollection;

        private readonly IContentTypeService _contentTypeService;

        private readonly ILogger<AlgoliaSearchPropertyIndexValueFactory> _logger;

        public AlgoliaSearchPropertyIndexValueFactory(
            PropertyEditorCollection propertyEditorCollection,
            ConverterCollection converterCollection,
            IContentTypeService contentTypeService,
            ILogger<AlgoliaSearchPropertyIndexValueFactory> logger)
        {
            _propertyEditorsCollection = propertyEditorCollection;

            _converterCollection = converterCollection;

            _contentTypeService = contentTypeService;

            _logger = logger;
        }

        public virtual KeyValuePair<string, object> GetValue(IProperty property, string culture)
        {
            var propertyEditor = _propertyEditorsCollection.FirstOrDefault(p => p.Alias == property.PropertyType.PropertyEditorAlias);
            if (propertyEditor == null)
            {
                return default;
            }

            Dictionary<Guid, IContentType> contentTypeDictionary = _contentTypeService.GetAll().ToDictionary(x => x.Key);

            try
            {
                var indexValues = propertyEditor.PropertyIndexValueFactory.GetIndexValues(
                    property,
                    culture,
                    null,
                    true,
                    Enumerable.Empty<string>(),
                    contentTypeDictionary);

                if (indexValues == null || !indexValues.Any()) return new KeyValuePair<string, object>(property.Alias, string.Empty);

                var indexValue = indexValues.First();

                var converter = _converterCollection.FirstOrDefault(p => p.Name == property.PropertyType.PropertyEditorAlias);
                if (converter != null)
                {
                    var result = converter.ParseIndexValues(property, indexValue.Value);
                    return new KeyValuePair<string, object>(property.Alias, result);
                }

                return new KeyValuePair<string, object>(property.Alias, indexValue.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get values for {property.Alias}: {ex.Message}");
                return default;
            }
        }
    }
}
