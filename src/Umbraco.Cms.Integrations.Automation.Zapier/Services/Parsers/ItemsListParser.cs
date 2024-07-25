using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services.Parsers
{
    public class ItemsListParser : IZapierContentParser
    {
        public string GetValue(IPublishedProperty contentProperty)
        {
            var value = string.Empty;

            var values = contentProperty.GetValue() as IEnumerable<string>;

            return values != null
                ? string.Join(", ", values)
                : contentProperty.GetValue().ToString();
        }
    }
}
