using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services.Parsers
{
    public class MultiNodeTreePickerParser : IZapierContentParser
    {
        public string GetValue(IPublishedProperty contentProperty)
        {
            string value = string.Empty;

            var items = contentProperty.GetValue() as IEnumerable<IPublishedContent>;

            return items != null && items.Any()
                ? string.Join(", ", items.Select(p => p.Name))
                : string.Empty;
        }
    }
}
