#if NETCOREAPP
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
#else
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Models;
using Umbraco.Web;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services.Parsers
{
    public class MediaParser : IZapierContentParser
    {
        public string GetValue(IPublishedProperty contentProperty)
        {
            string value = string.Empty;
            switch (contentProperty.PropertyType.EditorAlias)
            {
                case Core.Constants.PropertyEditors.Aliases.MediaPicker:
                    var mediaPickerValue = contentProperty.GetValue() as IPublishedContent;
                    value = mediaPickerValue != null ? mediaPickerValue.Url() : string.Empty;
                    break;
                case Core.Constants.PropertyEditors.Aliases.MediaPicker3:
                    var mediaPicker3Value = contentProperty.GetValue() as MediaWithCrops;
                    value = mediaPicker3Value != null ? mediaPicker3Value.LocalCrops.Src : string.Empty;
                    break;
            }

            return value;
        }
    }
}
