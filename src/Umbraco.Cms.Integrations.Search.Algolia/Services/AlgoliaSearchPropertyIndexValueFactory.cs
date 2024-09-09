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

        private readonly ILocalizationService _localizationService;
        private readonly IContentTypeService _contentTypeService;

        public AlgoliaSearchPropertyIndexValueFactory(
            PropertyEditorCollection propertyEditorCollection,
            ConverterCollection converterCollection,
            ILocalizationService localizationService,
            IContentTypeService contentTypeService)
        {
            _propertyEditorsCollection = propertyEditorCollection;
            _converterCollection = converterCollection;
            _localizationService = localizationService;
            _contentTypeService = contentTypeService;
        }

        public virtual KeyValuePair<string, object> GetValue(IProperty property, string culture)
        {
            var availableCultures = _localizationService.GetAllLanguages().Select(p => p.IsoCode);
            IDictionary<Guid, IContentType> contentTypeDictionary = _contentTypeService.GetAll().ToDictionary(x => x.Key);

            var propertyEditor = _propertyEditorsCollection.FirstOrDefault(p => p.Alias == property.PropertyType.PropertyEditorAlias);
            if (propertyEditor == null)
            {
                return default;
            }

            var converter = _converterCollection.FirstOrDefault(p => p.Name == property.PropertyType.PropertyEditorAlias);
            if (converter != null)
            {
                var result = converter.ParseIndexValues(property);
                return new KeyValuePair<string, object>(property.Alias, result);
            }

            IEnumerable<KeyValuePair<string, IEnumerable<object>>> indexValues =
#if NET6_0 || NET7_0
                propertyEditor.PropertyIndexValueFactory.GetIndexValues(
                    property, 
                    culture, 
                    null, 
                    true);
#else
                propertyEditor.PropertyIndexValueFactory.GetIndexValues(
                    property,
                    culture,
                    null,
                    true,
                    availableCultures,
                    contentTypeDictionary);
#endif
            if (indexValues == null || !indexValues.Any()) return new KeyValuePair<string, object>(property.Alias, string.Empty);

            var indexValue = indexValues.First();

            return new KeyValuePair<string, object>(property.Alias, indexValue.Value);
        }
    }
}
