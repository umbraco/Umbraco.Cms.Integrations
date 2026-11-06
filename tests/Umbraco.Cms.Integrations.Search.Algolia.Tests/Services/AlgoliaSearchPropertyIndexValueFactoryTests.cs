using System.Collections;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Integrations.Search.Algolia.Converters;
using Umbraco.Cms.Integrations.Search.Algolia.Providers;
using Umbraco.Cms.Integrations.Search.Algolia.Services;
using Xunit;

namespace Umbraco.Cms.Integrations.Search.Algolia.Tests.Services;

// Expected values are what the CMS 18 property index value factories (release-18.0.0) produced for the same
// stored values, which is what existing Algolia indexes were built from. Two deliberate exceptions: times are
// formatted with the invariant culture, and a Single Block property is indexed by UmbracoSingleBlockConverter
// (see docs/upgrades/v19.md, Decisions).
public class AlgoliaSearchPropertyIndexValueFactoryTests
{
    private const string TextBox = "Umbraco.TextBox";

    private static AlgoliaSearchPropertyIndexValueFactory CreateFactory(params string[] editorAliases)
    {
        var editors = editorAliases
            .Select(alias => Mock.Of<IDataEditor>(e => e.Alias == alias))
            .ToArray();

        return new AlgoliaSearchPropertyIndexValueFactory(
            new PropertyEditorCollection(new DataEditorCollection(() => editors)),
            new ConverterCollection(() =>
            [
                new UmbracoBooleanConverter(),
                new UmbracoIntegerConverter(),
                new UmbracoDecimalConverter(),
                new UmbracoTagsConverter(),
                new UmbracoBlockListConverter(NullLogger<UmbracoBlockListConverter>.Instance),
                new UmbracoSingleBlockConverter(NullLogger<UmbracoSingleBlockConverter>.Instance),
                new UmbracoMediaPickerConverter(CreateMediaService()),
                new UmbracoSingleMediaPickerConverter(CreateMediaService()),
            ]),
            Mock.Of<IContentTypeService>(),
            NullLogger<AlgoliaSearchPropertyIndexValueFactory>.Instance);
    }

    private const string MediaKey = "44444444-4444-4444-4444-444444444444";
    private const string MediaPickerJson = "[{\"key\":\"55555555-5555-5555-5555-555555555555\",\"mediaKey\":\"" + MediaKey + "\"}]";

    private static IMediaService CreateMediaService()
    {
        var media = new Mock<IMedia>();
        media.Setup(m => m.GetValue("umbracoFile")).Returns("/media/a.jpg");

        var mediaService = new Mock<IMediaService>();
        mediaService.Setup(s => s.GetById(Guid.Parse(MediaKey))).Returns(media.Object);
        return mediaService.Object;
    }

    private static object? GetValue(string editorAlias, object? storedValue)
    {
        var propertyType = Mock.Of<IPropertyType>(t => t.PropertyEditorAlias == editorAlias);

        var property = new Mock<IProperty>();
        property.SetupGet(p => p.Alias).Returns("field");
        property.SetupGet(p => p.PropertyType).Returns(propertyType);
        property.Setup(p => p.GetValue(null, null, true)).Returns(storedValue);
        property.Setup(p => p.GetValue(null, null, false)).Returns(storedValue);

        var pair = CreateFactory(editorAlias).GetValue(property.Object, null!);

        Assert.Equal("field", pair.Key);
        return pair.Value;
    }

    private static void AssertSequence(object? actual, params object?[] expected)
        => Assert.Equal(expected, Assert.IsAssignableFrom<IEnumerable>(actual).Cast<object?>().ToArray());

    [Fact]
    public void Unknown_editor_returns_default_pair()
    {
        var property = Mock.Of<IProperty>(p => p.PropertyType == Mock.Of<IPropertyType>(t => t.PropertyEditorAlias == "Not.Registered"));

        Assert.Equal(default, CreateFactory().GetValue(property, null!));
    }

    [Fact]
    public void Default_editor_indexes_the_value_in_a_single_item_array()
        => AssertSequence(GetValue(TextBox, "hello"), "hello");

    [Fact]
    public void Default_editor_with_no_value_indexes_a_single_null_item()
        => AssertSequence(GetValue(TextBox, null), (object?)null);

    [Theory]
    [InlineData("Umbraco.ColorPicker")]
    [InlineData("Umbraco.EntityDataPicker")]
    [InlineData("Umbraco.ImageCropper")]
    [InlineData("Umbraco.MultiUrlPicker")]
    [InlineData("Umbraco.UrlPicker.Single")]
    public void Editors_that_were_not_indexed_in_18_index_an_empty_string(string alias)
        => Assert.Equal(string.Empty, GetValue(alias, "[{\"x\":1}]"));

    [Fact]
    public void RichText_raw_markup_is_indexed_as_is()
        => AssertSequence(GetValue("Umbraco.RichText", "<p>hi</p>"), "<p>hi</p>");

    [Fact]
    public void RichText_json_indexes_the_markup()
        => AssertSequence(GetValue("Umbraco.RichText", "{\"markup\":\"<p>hi</p>\",\"blocks\":null}"), "<p>hi</p>");

    [Fact]
    public void RichText_invalid_json_indexes_an_empty_string()
        => Assert.Equal(string.Empty, GetValue("Umbraco.RichText", "{not json}"));

    [Fact]
    public void RichText_null_indexes_an_empty_string()
        => Assert.Equal(string.Empty, GetValue("Umbraco.RichText", null));

    private const string DateJson = "{\"date\":\"2024-05-06T10:30:00+02:00\",\"timeZone\":\"Europe/Copenhagen\"}";

    [Theory]
    [InlineData("Umbraco.DateOnly", "2024-05-06")]
    [InlineData("Umbraco.TimeOnly", "08:30:00")]
    [InlineData("Umbraco.DateTimeUnspecified", "2024-05-06T08:30:00")]
    [InlineData("Umbraco.DateTimeWithTimeZone", "2024-05-06T08:30:00Z")]
    public void Date_editors_index_the_utc_value_in_their_format(string alias, string expected)
        => AssertSequence(GetValue(alias, DateJson), expected);

    [Theory]
    [InlineData("Umbraco.DateOnly")]
    [InlineData("Umbraco.TimeOnly")]
    [InlineData("Umbraco.DateTimeUnspecified")]
    [InlineData("Umbraco.DateTimeWithTimeZone")]
    public void Date_editors_with_no_or_invalid_value_index_an_empty_array(string alias)
    {
        AssertSequence(GetValue(alias, null));
        AssertSequence(GetValue(alias, "{not json}"));
    }

    // A local DateTime can only be read as UTC when the machine's offset is zero (as on the CI agents);
    // otherwise CMS 18 caught the failure and indexed nothing.
    [Theory]
    [InlineData("Umbraco.DateOnly", "2024-05-06")]
    [InlineData("Umbraco.TimeOnly", "10:30:00")]
    [InlineData("Umbraco.DateTimeUnspecified", "2024-05-06T10:30:00")]
    [InlineData("Umbraco.DateTimeWithTimeZone", "2024-05-06T10:30:00Z")]
    public void Date_editors_index_a_local_DateTime_only_when_local_time_is_utc(string alias, string expectedAtUtc)
    {
        var localDateTime = new DateTime(2024, 5, 6, 10, 30, 0, DateTimeKind.Local);
        var actual = GetValue(alias, localDateTime);

        if (TimeZoneInfo.Local.GetUtcOffset(localDateTime) == TimeSpan.Zero)
        {
            AssertSequence(actual, expectedAtUtc);
        }
        else
        {
            AssertSequence(actual);
        }
    }

    [Fact]
    public void Date_editor_accepts_a_utc_DateTime()
        => AssertSequence(GetValue("Umbraco.DateOnly", new DateTime(2024, 5, 6, 10, 30, 0, DateTimeKind.Utc)), "2024-05-06");

    [Fact]
    public void Tags_with_no_value_index_an_empty_string()
    {
        Assert.Equal(string.Empty, GetValue("Umbraco.Tags", null));
        Assert.Equal(string.Empty, GetValue("Umbraco.Tags", "   "));
    }

    [Theory]
    [InlineData(",")]
    [InlineData(",,")]
    [InlineData("[]")]
    public void Tags_with_only_separators_or_an_empty_json_array_index_an_empty_array(string stored)
        => AssertSequence(GetValue("Umbraco.Tags", stored));

    [Fact]
    public void Tags_json_array_is_indexed()
        => AssertSequence(GetValue("Umbraco.Tags", "[\"a\",\"b\"]"), "a", "b");

    [Fact]
    public void Tags_comma_separated_list_is_indexed()
        => AssertSequence(GetValue("Umbraco.Tags", "a,b"), "a", "b");

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    [InlineData(null, false)]
    public void Boolean_indexes_true_only_for_one(object? stored, bool expected)
        => Assert.Equal(expected, GetValue("Umbraco.TrueFalse", stored));

    [Fact]
    public void Integer_indexes_the_value_or_zero()
    {
        Assert.Equal(5, GetValue("Umbraco.Integer", 5));
        Assert.Equal(0, GetValue("Umbraco.Integer", null));
    }

    [Fact]
    public void Decimal_indexes_the_value_or_zero()
    {
        Assert.Equal(12.5m, GetValue("Umbraco.Decimal", 12.5m));
        Assert.Equal(0m, GetValue("Umbraco.Decimal", null));
    }

    // Single pickers store the same JSON array as the multiple picker, so both index the file path.
    [Theory]
    [InlineData("Umbraco.MediaPicker3")]
    [InlineData("Umbraco.MediaPicker.Single")]
    public void Media_pickers_index_the_file_of_the_picked_media(string alias)
        => AssertSequence(GetValue(alias, MediaPickerJson), "/media/a.jpg");

    // No converter handles dropdowns; the stored JSON array is indexed in a single item array in 18 and 19.
    [Theory]
    [InlineData("Umbraco.DropDown.Flexible")]
    [InlineData("Umbraco.DropDown.Single")]
    public void Dropdowns_index_the_stored_value_in_a_single_item_array(string alias)
        => AssertSequence(GetValue(alias, "[\"a\"]"), "[\"a\"]");

    private const string SingleBlockJson =
        "{\"layout\":{\"Umbraco.SingleBlock\":[{\"contentKey\":\"11111111-1111-1111-1111-111111111111\"}]}," +
        "\"contentData\":[{\"contentTypeKey\":\"22222222-2222-2222-2222-222222222222\",\"key\":\"11111111-1111-1111-1111-111111111111\"," +
        "\"values\":[{\"editorAlias\":\"Umbraco.TextBox\",\"alias\":\"title\",\"culture\":null,\"segment\":null,\"value\":\"Hello\"}]}]," +
        "\"settingsData\":[],\"expose\":[{\"contentKey\":\"11111111-1111-1111-1111-111111111111\",\"culture\":null,\"segment\":null}]}";

    [Fact]
    public void SingleBlock_indexes_the_text_of_the_block()
        => AssertSequence(GetValue("Umbraco.SingleBlock", SingleBlockJson), "Hello");

    [Fact]
    public void BlockList_skips_a_nested_single_block()
    {
        // 18 walked only text, rich text and nested block list/grid values; a nested single block isn't walked.
        var blockList =
            "{\"layout\":{\"Umbraco.BlockList\":[]},\"contentData\":[{\"key\":\"33333333-3333-3333-3333-333333333333\"," +
            "\"values\":[" +
            "{\"editorAlias\":\"Umbraco.TextBox\",\"alias\":\"title\",\"value\":\"Outer\"}," +
            "{\"editorAlias\":\"Umbraco.SingleBlock\",\"alias\":\"inner\",\"value\":" + SingleBlockJson + "}" +
            "]}],\"settingsData\":[],\"expose\":[]}";

        AssertSequence(GetValue("Umbraco.BlockList", blockList), "Outer");
    }
}
