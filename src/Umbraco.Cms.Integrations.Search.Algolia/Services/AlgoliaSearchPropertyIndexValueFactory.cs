using System.Text.Json;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public class AlgoliaSearchPropertyIndexValueFactory : IAlgoliaSearchPropertyIndexValueFactory
    {
        private readonly IDataTypeService _dataTypeService;

        private readonly IMediaService _mediaService;

        public AlgoliaSearchPropertyIndexValueFactory(IDataTypeService dataTypeService, IMediaService mediaService)
        {
            _dataTypeService = dataTypeService;

            _mediaService = mediaService;

            Converters = new Dictionary<string, Func<KeyValuePair<string, IEnumerable<object>>, string>>
            {
                {  Core.Constants.PropertyEditors.Aliases.MediaPicker3, ConvertMediaPicker }
            };
        }

        public Dictionary<string, Func<KeyValuePair<string, IEnumerable<object>>, string>> Converters { get; set; }

        public virtual KeyValuePair<string, string> GetValue(IProperty property, string culture)
        {
            var dataType = _dataTypeService.GetByEditorAlias(property.PropertyType.PropertyEditorAlias)
                .FirstOrDefault(p => p.Id == property.PropertyType.DataTypeId);

            if (dataType == null) return default;

            var indexValues = dataType.Editor.PropertyIndexValueFactory.GetIndexValues(property, culture, string.Empty, true);

            if (indexValues == null || !indexValues.Any()) return new KeyValuePair<string, string>(property.Alias, string.Empty);

            var indexValue = indexValues.FirstOrDefault();

            if (Converters.ContainsKey(property.PropertyType.PropertyEditorAlias))
            {
                var result = Converters[property.PropertyType.PropertyEditorAlias].Invoke(indexValue);
                return new KeyValuePair<string, string>(property.Alias, result);
            }

            return new KeyValuePair<string, string>(indexValue.Key, ParseIndexValue(indexValue.Value));
        }

        public string ParseIndexValue(IEnumerable<object> values)
        {
            if (values != null && values.Any())
            {
                var value = values.FirstOrDefault();

                if (value == null) return string.Empty;

                return value.ToString();
            }

            return string.Empty;
        }

        private string ConvertMediaPicker(KeyValuePair<string, IEnumerable<object>> indexValue)
        {
            var list = new List<string>();

            var parsedIndexValue = ParseIndexValue(indexValue.Value);

            if (string.IsNullOrEmpty(parsedIndexValue)) return string.Empty;

            var inputMedia = JsonSerializer.Deserialize<IEnumerable<MediaItem>>(parsedIndexValue);

            if (inputMedia == null) return string.Empty;

            foreach (var item in inputMedia)
            {
                if (item == null) continue;

                var mediaItem = _mediaService.GetById(Guid.Parse(item.MediaKey));

                if (mediaItem == null) continue;

                list.Add(mediaItem.GetValue("umbracoFile")?.ToString() ?? string.Empty);
            }

            return JsonSerializer.Serialize(list);
        }
    }
}
