using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Integrations.Search.Algolia.Converters;

namespace Umbraco.Cms.Integrations.Search.Algolia.Providers
{
    public class ConverterCollection : BuilderCollectionBase<IConverter>
    {
        public ConverterCollection(Func<IEnumerable<IConverter>> items)
            : base(items) 
        { 
        }
    }
}
