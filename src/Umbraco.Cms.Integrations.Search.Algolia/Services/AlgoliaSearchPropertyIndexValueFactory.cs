using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Integrations.Search.Algolia.Providers;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public class AlgoliaSearchPropertyIndexValueFactory : IAlgoliaSearchPropertyIndexValueFactory
    {
        private readonly PropertyEditorCollection _propertyEditorsCollection;

        private readonly ConverterCollection _converterCollection;

        public AlgoliaSearchPropertyIndexValueFactory(
            PropertyEditorCollection propertyEditorCollection,
            ConverterCollection converterCollection)
        {
            _propertyEditorsCollection = propertyEditorCollection;

            _converterCollection = converterCollection;
        }

        public virtual KeyValuePair<string, object> GetValue(IProperty property, string culture)
        {
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

            IEnumerable<KeyValuePair<string, IEnumerable<object>>> indexValues = propertyEditor.PropertyIndexValueFactory.GetIndexValues(property, culture, null, true);

            if (indexValues == null || !indexValues.Any()) return new KeyValuePair<string, object>(property.Alias, string.Empty);

            var indexValue = indexValues.First();

            return new KeyValuePair<string, object>(property.Alias, indexValue.Value);
        }
    }
}
