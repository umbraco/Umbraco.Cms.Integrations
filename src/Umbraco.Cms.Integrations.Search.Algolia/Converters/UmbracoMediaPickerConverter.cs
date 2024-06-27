using System.Text.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    public class UmbracoMediaPickerConverter : IAlgoliaIndexValueConverter
    {
        private readonly IMediaService _mediaService;

        public UmbracoMediaPickerConverter(IMediaService mediaService) => _mediaService = mediaService;

        public string Name => Core.Constants.PropertyEditors.Aliases.MediaPicker3;

        public object ParseIndexValues(IProperty property, IEnumerable<object> indexValues)
        {
            var list = new List<string>();

            var parsedIndexValue = ParseIndexValue(indexValues);

            if (string.IsNullOrEmpty(parsedIndexValue)) return list;

            var inputMedia = JsonSerializer.Deserialize<IEnumerable<MediaItem>>(parsedIndexValue);

            if (inputMedia == null) return string.Empty;

            foreach (var item in inputMedia)
            {
                if (item == null) continue;

                var mediaItem = _mediaService.GetById(Guid.Parse(item.MediaKey));

                if (mediaItem == null) continue;

                list.Add(mediaItem.GetValue("umbracoFile")?.ToString() ?? string.Empty);
            }

            return list;
        }

        private string ParseIndexValue(IEnumerable<object> values)
        {
            if (values != null && values.Any())
            {
                var value = values.FirstOrDefault();

                if (value == null) return string.Empty;

                return value.ToString();
            }

            return string.Empty;
        }
    }
}
