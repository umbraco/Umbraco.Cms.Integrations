using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services.Parsers
{
    public class DefaultParser : IZapierContentParser
    {
        public string GetValue(IPublishedProperty contentProperty) =>
            contentProperty.GetValue()?.ToString();
    }
}
