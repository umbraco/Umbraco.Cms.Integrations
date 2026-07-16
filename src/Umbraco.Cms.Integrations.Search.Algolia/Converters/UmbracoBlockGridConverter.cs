using Microsoft.Extensions.Logging;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoBlockGridConverter : UmbracoBlockValueConverterBase, IAlgoliaIndexValueConverter
    {
        public UmbracoBlockGridConverter(ILogger<UmbracoBlockGridConverter> logger)
            : base(logger)
        {
        }

        public string Name => Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.BlockGrid;
    }
}
