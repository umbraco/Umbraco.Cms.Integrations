using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Library.Parsing;
using static Umbraco.Cms.Core.Models.Property;

namespace Umbraco.Cms.Integrations.Library.Services
{
    public class ParserService : IParserService
    {
        private readonly IMediaService _mediaService;

        public ParserService(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public string GetParsedValue(IProperty property, string culture = "")
        {
            var propertyValue = string.IsNullOrEmpty(culture)
                ? property.GetValue()?.ToString() ?? string.Empty
                : property.GetValue(culture) != null
                    ? property.GetValue(culture)?.ToString() ?? string.Empty
                    : property.GetValue()?.ToString() ?? string.Empty;

            switch (property.PropertyType.PropertyEditorAlias)
            {
                case Core.Constants.PropertyEditors.Aliases.MediaPicker3:
                    return new ContentMediaParser(_mediaService)
                        .Parse(propertyValue);
                default: return propertyValue;
            }
        }

        public string GetParsedValue(IPublishedProperty property, string culture = "")
        {
            var propertyValue = string.IsNullOrEmpty(culture)
                ? property.GetValue()
                : (property.HasValue(culture)
                    ? property.GetValue(culture)
                    : property.GetValue());

            switch (propertyValue)
            {
                case MediaWithCrops:
                    return new MediaWithCropsParser().Parse(propertyValue as MediaWithCrops);
                case List<MediaWithCrops>:
                    return new MediaWithCropsParser().Parse(propertyValue as List<MediaWithCrops>);
                case IEnumerable<object>:
                    return new EnumerableParser().Parse(propertyValue as IEnumerable<object>);
                default:
                    return propertyValue?.ToString() ?? string.Empty;
            }
        }
    }
}
