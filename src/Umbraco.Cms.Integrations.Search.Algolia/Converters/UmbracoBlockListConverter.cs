using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PropertyEditors.ValueConverters;
using Umbraco.Cms.Core.Serialization;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Extensions;
using static Umbraco.Cms.Core.Constants;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters {    

    public class UmbracoBlockListConverter : IAlgoliaIndexValueConverter {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly BlockEditorConverter _blockEditorConverter;
        private readonly ILogger<UmbracoBlockListConverter> _logger;
        private const int charLimit = 250; 

        public UmbracoBlockListConverter(IJsonSerializer jsonSerializer, BlockEditorConverter blockEditorConverter, ILogger<UmbracoBlockListConverter> logger) {
            _jsonSerializer = jsonSerializer;
            _blockEditorConverter = blockEditorConverter;
            _logger = logger;
        }

        public string Name => PropertyEditors.Aliases.BlockList;

        public object ParseIndexValues(IProperty property) {
            var rawValue = property.GetValue();
            List<string> stringValues = new List<string>();

            if (rawValue == null || string.IsNullOrWhiteSpace(rawValue.ToString())) {
                return stringValues;
            }

            // Use Umbraco's built-in IJsonSerializer to deserialize the raw JSON
            var blockListModel = _jsonSerializer.Deserialize<BlockListModelFromJson>(rawValue.ToString());

            if (blockListModel == null) {
                return stringValues;
            }

            //go through the content blocks and try to get them as some useful type
            foreach (var block in blockListModel.contentData) {

                var content = _blockEditorConverter.ConvertToElement(block, PropertyCacheLevel.None, false);
                if (content == null) continue;

                //now we can go through the properties and convert their values, creating a big old list to return 
                foreach(var prop in content.Properties) {                 
                    var alias = prop.PropertyType.EditorAlias;
                    bool isText =
                        alias == PropertyEditors.Aliases.TextBox
                        || alias == PropertyEditors.Aliases.TextArea
                        || alias == PropertyEditors.Aliases.MarkdownEditor
                        || alias == PropertyEditors.Aliases.TinyMce
                        || alias == PropertyEditors.Aliases.MultipleTextstring;

                    if (isText && !string.IsNullOrEmpty(content.Value<string>(prop.Alias))) {
                        var toAdd = content.Value<string>(prop.Alias) ?? "";

                        if ((stringValues.Sum(s => s.Length) + toAdd.Length) < charLimit)
                            stringValues.Add(toAdd);
                        else 
                            break;
                        
                    }
                }

            }

            _logger.LogDebug("Converted block list to string: {stringValues}", stringValues);

            return stringValues;
        }

    }
}
