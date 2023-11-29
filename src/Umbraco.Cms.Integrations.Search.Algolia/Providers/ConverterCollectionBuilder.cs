using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Integrations.Search.Algolia.Converters;

namespace Umbraco.Cms.Integrations.Search.Algolia.Providers
{
    public class ConverterCollectionBuilder : OrderedCollectionBuilderBase<ConverterCollectionBuilder, ConverterCollection, IAlgoliaIndexValueConverter>
    {
        protected override ConverterCollectionBuilder This => this;
    }
}
