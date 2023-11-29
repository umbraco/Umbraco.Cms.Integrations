using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.Search.Algolia.Converters;
using Umbraco.Cms.Integrations.Search.Algolia.Providers;

namespace Umbraco.Cms.Integrations.Search.Algolia.Extensions
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddAlgoliaConverters(this IUmbracoBuilder builder)
        {
            builder.AlgoliaConverters()
                .Append<UmbracoMediaPickerConverter>()
                .Append<UmbracoDecimalConverter>()
                .Append<UmbracoIntegerConverter>()
                .Append<UmbracoBooleanConverter>()
                .Append<UmbracoTagsConverter>();

            return builder;
        }

        public static ConverterCollectionBuilder AlgoliaConverters(this IUmbracoBuilder builder)
            => builder.WithCollectionBuilder<ConverterCollectionBuilder>();
    }
}
