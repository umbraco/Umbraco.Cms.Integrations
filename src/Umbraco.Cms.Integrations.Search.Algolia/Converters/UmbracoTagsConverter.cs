using System.Text.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Extensions;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoTagsConverter : IAlgoliaIndexValueConverter
    {
        public string Name => Core.Constants.PropertyEditors.Aliases.Tags;

        public object ParseIndexValues(IProperty property)
        {
            if (!property.TryGetPropertyIndexValue(out string value))
            {
                return Enumerable.Empty<string>();
            }

            var valuesArr = JsonSerializer.Deserialize<List<string>>(value);
            if (valuesArr != null && valuesArr.Any())
            {
                return valuesArr.Select(p => p);
            }

            return Enumerable.Empty<string>();
        }
    }
}
