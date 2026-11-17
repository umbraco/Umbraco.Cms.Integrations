using Microsoft.Extensions.Logging;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoSingleBlockConverter : UmbracoBlockValueConverterBase, IAlgoliaIndexValueConverter
    {
        public UmbracoSingleBlockConverter(ILogger<UmbracoSingleBlockConverter> logger)
            : base(logger)
        {
        }

        public string Name => Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.SingleBlock;
    }
}
