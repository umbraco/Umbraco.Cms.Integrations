using System.Text.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoMediaPickerConverter : IAlgoliaIndexValueConverter
    {
        const string udiPrefix = "umb://media/";

        private readonly IMediaService _mediaService;

        public UmbracoMediaPickerConverter(IMediaService mediaService) => _mediaService = mediaService;

        public string Name => Core.Constants.PropertyEditors.Aliases.MediaPicker3;

        public object ParseIndexValues(IProperty property, string? culture)
        {
            var list = new List<string>();

            var parsedIndexValue = property.GetValue(culture, null, true)?.ToString();

            if (string.IsNullOrEmpty(parsedIndexValue)) return list;

            if (parsedIndexValue.StartsWith(udiPrefix))
            {
                var guidPart = parsedIndexValue.Substring(udiPrefix.Length);
                if (Guid.TryParse(guidPart, out Guid guid))
                {
                    var mediaItem = _mediaService.GetById(guid);
                    if (mediaItem != null)
                    {
                        list.Add(mediaItem.GetValue("umbracoFile")?.ToString() ?? string.Empty);
                    }
                }
            }
            else
            {
                var inputMedia = JsonSerializer.Deserialize<IEnumerable<MediaItem>>(parsedIndexValue);

                if (inputMedia == null) return string.Empty;

                foreach (var item in inputMedia)
                {
                    if (item == null) continue;

                    var mediaItem = _mediaService.GetById(Guid.Parse(item.MediaKey));

                    if (mediaItem == null) continue;

                    list.Add(mediaItem.GetValue("umbracoFile")?.ToString() ?? string.Empty);
                }
            }

            return list;
        }
    }
}
