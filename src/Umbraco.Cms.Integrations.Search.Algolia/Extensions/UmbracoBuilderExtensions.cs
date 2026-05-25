using Microsoft.AspNetCore.OpenApi;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
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

        public static IUmbracoBuilder AddAlgoliaOpenApi(this IUmbracoBuilder builder)
        {
            builder.AddBackOfficeOpenApiDocument(
                Constants.ManagementApi.ApiName,
                document => document
                    .WithTitle(Constants.ManagementApi.ApiTitle)
                    .WithBackOfficeAuthentication()
                    .ConfigureOpenApiOptions(options => options.AddDocumentTransformer((doc, _, _) =>
                    {
                        doc.Info.Version = "Latest";
                        doc.Info.Description = $"Describes the {Constants.ManagementApi.ApiTitle} available for handling indices.";
                        return Task.CompletedTask;
                    })));

            return builder;
        }
    }
}
