using Microsoft.Extensions.Logging;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoBlockListConverter : UmbracoBlockValueConverterBase, IAlgoliaIndexValueConverter
    {
        public UmbracoBlockListConverter(ILogger<UmbracoBlockListConverter> logger)
            : base(logger)
        {
        }

        public string Name => Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.BlockList;
    }
}
