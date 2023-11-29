using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Integrations.Search.Algolia.Converters;

namespace Umbraco.Cms.Integrations.Search.Algolia.Providers
{
    public class ConverterCollection : BuilderCollectionBase<IAlgoliaIndexValueConverter>
    {
        public ConverterCollection(Func<IEnumerable<IAlgoliaIndexValueConverter>> items)
            : base(items) 
        { 
        }
    }
}
