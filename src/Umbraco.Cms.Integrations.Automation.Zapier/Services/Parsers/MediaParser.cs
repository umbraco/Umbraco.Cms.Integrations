using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services.Parsers
{
    public class MediaParser : IZapierContentParser
    {
        public string GetValue(IPublishedProperty contentProperty)
        {
            // The single media picker yields one MediaWithCrops. The multiple picker (MediaPicker3) yields a
            // collection, which has always produced an empty value here, so that payload is kept as is.
            return contentProperty.GetValue() is MediaWithCrops media
                ? media.LocalCrops.Src
                : string.Empty;
        }
    }
}
