using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    /// <summary>
    /// Indexes the single media picker the same way as the media picker: both store the same JSON value.
    /// </summary>
    public class UmbracoSingleMediaPickerConverter : UmbracoMediaPickerConverter
    {
        public UmbracoSingleMediaPickerConverter(IMediaService mediaService)
            : base(mediaService)
        {
        }

        public override string Name => Core.Constants.PropertyEditors.Aliases.SingleMediaPicker;
    }
}
