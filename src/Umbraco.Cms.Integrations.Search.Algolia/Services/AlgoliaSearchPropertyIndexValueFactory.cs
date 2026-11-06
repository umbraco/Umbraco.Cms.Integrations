using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Search.Algolia.Providers;
using Umbraco.Extensions;

namespace Umbraco.Cms.Integrations.Search.Algolia.Services
{
    public class AlgoliaSearchPropertyIndexValueFactory : IAlgoliaSearchPropertyIndexValueFactory
    {
        private static readonly HashSet<string> NotIndexedEditorAliases = new(StringComparer.Ordinal)
        {
            Core.Constants.PropertyEditors.Aliases.ColorPicker,
            Core.Constants.PropertyEditors.Aliases.EntityDataPicker,
            Core.Constants.PropertyEditors.Aliases.ImageCropper,
            Core.Constants.PropertyEditors.Aliases.MultiUrlPicker,
        };

        private readonly PropertyEditorCollection _propertyEditorsCollection;

        private readonly ConverterCollection _converterCollection;

        private readonly ILogger<AlgoliaSearchPropertyIndexValueFactory> _logger;

        // The content type service is no longer needed (it only fed the CMS index value factories, removed in
        // CMS 19) but stays in the constructor so that existing derived classes keep compiling.
        public AlgoliaSearchPropertyIndexValueFactory(
            PropertyEditorCollection propertyEditorCollection,
            ConverterCollection converterCollection,
            IContentTypeService contentTypeService,
            ILogger<AlgoliaSearchPropertyIndexValueFactory> logger)
        {
            _propertyEditorsCollection = propertyEditorCollection;

            _converterCollection = converterCollection;

            _logger = logger;
        }

        public virtual KeyValuePair<string, object> GetValue(IProperty property, string culture)
        {
            var editorAlias = property.PropertyType.PropertyEditorAlias;

            var propertyEditor = _propertyEditorsCollection.FirstOrDefault(p => p.Alias == editorAlias);
            if (propertyEditor == null)
            {
                return default;
            }

            try
            {
                var converter = _converterCollection.FirstOrDefault(p => p.Name == editorAlias);
                if (converter != null)
                {
                    var result = converter.ParseIndexValues(property, culture);
                    return new KeyValuePair<string, object>(property.Alias, result);
                }

                return new KeyValuePair<string, object>(property.Alias, GetDefaultValue(property, culture, editorAlias));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get values for {property.Alias}: {ex.Message}");
                return default;
            }
        }

        // Reproduces what the CMS 18 property index value factories produced for editors without an Algolia
        // converter: the published value in a single item array, except for the editors below.
        private object GetDefaultValue(IProperty property, string? culture, string editorAlias)
        {
            if (NotIndexedEditorAliases.Contains(editorAlias))
            {
                return string.Empty;
            }

            var value = property.GetValue(culture, null, true);

            return editorAlias switch
            {
                Core.Constants.PropertyEditors.Aliases.RichText => GetRichTextMarkup(property, value),
                Core.Constants.PropertyEditors.Aliases.DateOnly => GetDateValue(value, "yyyy-MM-dd"),
                Core.Constants.PropertyEditors.Aliases.TimeOnly => GetDateValue(value, "HH:mm:ss"),
                Core.Constants.PropertyEditors.Aliases.DateTimeUnspecified => GetDateValue(value, "yyyy-MM-ddTHH:mm:ss"),
                Core.Constants.PropertyEditors.Aliases.DateTimeWithTimeZone => GetDateValue(value, "yyyy-MM-ddTHH:mm:ssZ"),
                _ => new[] { value },
            };
        }

        private object GetRichTextMarkup(IProperty property, object? value)
        {
            var stringValue = value?.ToString();
            if (stringValue is null)
            {
                return string.Empty;
            }

            // Raw markup is stored for older values; newer ones are a JSON object with the markup and blocks.
            if (!stringValue.DetectIsJson())
            {
                return new object?[] { stringValue };
            }

            try
            {
                var richText = JsonSerializer.Deserialize<RichTextValue>(stringValue);

                return richText is null ? string.Empty : new object?[] { richText.Markup };
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Could not parse rich text editor value for property {Alias}.", property.Alias);
                return string.Empty;
            }
        }

        private object GetDateValue(object? value, string format)
        {
            DateTimeOffset? date = value switch
            {
                DateTime dateTime => FromDateTime(dateTime),
                string stringValue => ParseDate(stringValue),
                _ => null,
            };

            return date.HasValue
                ? new object?[] { date.Value.UtcDateTime.ToString(format, CultureInfo.InvariantCulture) }
                : Array.Empty<object?>();
        }

        private DateTimeOffset? FromDateTime(DateTime dateTime)
        {
            try
            {
                return new DateTimeOffset(dateTime, TimeSpan.Zero);
            }
            catch (ArgumentException ex)
            {
                // A local-kind DateTime can't be given a zero offset.
                _logger.LogWarning(ex, "Could not convert date time editor value.");
                return null;
            }
        }

        private DateTimeOffset? ParseDate(string value)
        {
            try
            {
                return JsonSerializer.Deserialize<DateValue>(value)?.Date;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Could not parse date time editor value.");
                return null;
            }
        }

        private sealed class RichTextValue
        {
            [JsonPropertyName("markup")]
            public string? Markup { get; init; }
        }

        private sealed class DateValue
        {
            [JsonPropertyName("date")]
            public DateTimeOffset Date { get; init; }
        }
    }
}
