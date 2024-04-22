#if NETCOREAPP
using Umbraco.Cms.Core.Models.PublishedContent;
#else
using Umbraco.Core.Models.PublishedContent;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services.Parsers
{
    public class DefaultParser : IZapierContentParser
    {
        public string GetValue(IPublishedProperty contentProperty) =>
            contentProperty.GetValue()?.ToString();
    }
}
