using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Umbraco.Cms.Integrations.Search.Algolia.Converters
{
    /// <summary>
    /// Base converter that flattens the text content of block editors (Block List / Block Grid) into a
    /// list of strings suitable for indexing in Algolia. Walks the persisted block JSON directly, using the
    /// <c>editorAlias</c> stored against each block property value, so no content type lookup is required.
    /// </summary>
    public abstract class UmbracoBlockValueConverterBase
    {
        private const int CharLimit = 250;

        private readonly ILogger _logger;

        protected UmbracoBlockValueConverterBase(ILogger logger) => _logger = logger;

        // Editor aliases whose stored value is plain text.
        private static readonly HashSet<string> TextEditorAliases = new(StringComparer.Ordinal)
        {
            global::Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.TextBox,
            global::Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.TextArea,
            global::Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.MarkdownEditor,
            global::Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.MultipleTextstring,
        };

        // Rich text editor aliases whose stored value contains markup (and optionally nested blocks).
        private static readonly HashSet<string> RichTextEditorAliases = new(StringComparer.Ordinal)
        {
            global::Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.RichText,
            "Umbraco.TinyMCE",
        };

        // Block editor aliases whose stored value contains nested content data.
        private static readonly HashSet<string> BlockEditorAliases = new(StringComparer.Ordinal)
        {
            global::Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.BlockList,
            global::Umbraco.Cms.Core.Constants.PropertyEditors.Aliases.BlockGrid,
        };

        public object ParseIndexValues(IProperty property, IndexValue indexValue)
        {
            var result = new List<string>();

            // Prefer the value for the culture being indexed, falling back to the invariant value for
            // block properties that don't vary by culture.
            var culture = indexValue?.Culture;
            var rawValue = property.GetValue(culture)?.ToString();
            if (string.IsNullOrWhiteSpace(rawValue) && culture is not null)
            {
                rawValue = property.GetValue()?.ToString();
            }

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return result;
            }

            try
            {
                ExtractBlockValue(JsonNode.Parse(rawValue), result);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to parse block editor value for property {Alias} when building Algolia index values.", property.Alias);
            }

            return result;
        }

        // Returns false once the character limit has been reached and processing should stop entirely.
        private bool ExtractBlockValue(JsonNode? blockValue, List<string> result)
        {
            if (blockValue?["contentData"] is not JsonArray contentData)
            {
                return true;
            }

            foreach (var block in contentData)
            {
                if (block?["values"] is not JsonArray values)
                {
                    continue;
                }

                foreach (var valueItem in values)
                {
                    var editorAlias = valueItem?["editorAlias"]?.GetValue<string>();
                    var value = valueItem?["value"];
                    if (string.IsNullOrEmpty(editorAlias) || value is null)
                    {
                        continue;
                    }

                    if (!ExtractPropertyValue(editorAlias, value, result))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ExtractPropertyValue(string editorAlias, JsonNode value, List<string> result)
        {
            if (BlockEditorAliases.Contains(editorAlias))
            {
                return ExtractBlockValue(value, result);
            }

            if (RichTextEditorAliases.Contains(editorAlias))
            {
                return ExtractRichText(value, result);
            }

            if (TextEditorAliases.Contains(editorAlias))
            {
                return ExtractText(value, result);
            }

            // Not a text-bearing editor - skip, but keep processing the rest.
            return true;
        }

        private bool ExtractRichText(JsonNode value, List<string> result)
        {
            // Umbraco.RichText is an object { markup, blocks }; the legacy TinyMCE value is a plain HTML string.
            if (value is JsonObject richText)
            {
                var markup = richText["markup"]?.GetValue<string>();
                if (!string.IsNullOrWhiteSpace(markup) && !AddValue(markup.StripHtml(), result))
                {
                    return false;
                }

                return richText["blocks"] is null || ExtractBlockValue(richText["blocks"], result);
            }

            return AddValue(value.ToString().StripHtml(), result);
        }

        private bool ExtractText(JsonNode value, List<string> result)
        {
            // MultipleTextstring can be stored as an array of strings; other text editors are a scalar string.
            if (value is JsonArray array)
            {
                foreach (var item in array)
                {
                    if (item is not null && !AddValue(item.ToString(), result))
                    {
                        return false;
                    }
                }

                return true;
            }

            return AddValue(value.ToString(), result);
        }

        // Adds the value if it fits within the character limit; returns false to signal a hard stop once an
        // entry would exceed the limit (mirrors the original behaviour from PR #300).
        private static bool AddValue(string? value, List<string> result)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            if (result.Sum(s => s.Length) + value.Length < CharLimit)
            {
                result.Add(value);
                return true;
            }

            return false;
        }
    }
}
