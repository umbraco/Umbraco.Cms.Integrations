
using System.Text.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public class ExtendedAlgoliaSearchPropertyIndexValueFactory : AlgoliaSearchPropertyIndexValueFactory
    {
        private readonly IMediaService _mediaService;

        public ExtendedAlgoliaSearchPropertyIndexValueFactory(IDataTypeService dataTypeService, IMediaService mediaService) 
            : base(dataTypeService, mediaService)
        {
            _mediaService = mediaService;

            Converters = new Dictionary<string, Func<KeyValuePair<string, IEnumerable<object>>, string>>
            {
                { Core.Constants.PropertyEditors.Aliases.MediaPicker3, ExtendedMediaPickerConverter }
            };
        }

        public override KeyValuePair<string, string> GetValue(IProperty property, string culture)
        {
            return base.GetValue(property, culture);
        }

        private string ExtendedMediaPickerConverter(KeyValuePair<string, IEnumerable<object>> indexValue)
        {
            return "my custom converter for media picker";
        }

    }
}
