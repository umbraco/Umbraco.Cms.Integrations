using Algolia.Search.Models.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Search.Algolia.Api.Configuration;
using Umbraco.Cms.Integrations.Search.Algolia.Builders;
using Umbraco.Cms.Integrations.Search.Algolia.Configuration;
using Umbraco.Cms.Integrations.Search.Algolia.Extensions;
using Umbraco.Cms.Integrations.Search.Algolia.Handlers;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Search.Algolia
{
    public class AlgoliaComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, RunAlgoliaIndicesMigration>();

            builder.AddNotificationAsyncHandler<ContentCacheRefresherNotification, AlgoliaContentCacheRefresherHandler>();
            builder.AddNotificationAsyncHandler<ContentPublishedNotification, AlgoliaContentPublishedHandler>();

            builder.Services.AddOptions<AlgoliaSettings>()
                 .Bind(builder.Config.GetSection(Constants.SettingsPath));

            builder.Services.AddSingleton<IAlgoliaIndexService, AlgoliaIndexService>();

            builder.Services.AddSingleton<IAlgoliaSearchService<SearchResponse<Record>>, AlgoliaSearchService>();

            builder.Services.AddScoped<IAlgoliaIndexDefinitionStorage<AlgoliaIndex>, AlgoliaIndexDefinitionStorage>();

            builder.Services.AddScoped<IRecordBuilderFactory, RecordBuilderFactory>();

            builder.Services.AddScoped<IAlgoliaSearchPropertyIndexValueFactory, AlgoliaSearchPropertyIndexValueFactory>();

            builder.Services.AddSingleton<IAlgoliaGeolocationProvider, AlgoliaNullGeolocationProvider>();

            builder.AddAlgoliaConverters();

            // Generate Swagger documentation for Algolia Search API
            builder.Services.Configure<SwaggerGenOptions>(options =>
            {
                options.SwaggerDoc(
                    Constants.ManagementApi.ApiName,
                    new OpenApiInfo
                    {
                        Title = Constants.ManagementApi.ApiTitle,
                        Version = "Latest",
                        Description = $"Describes the {Constants.ManagementApi.ApiTitle} available for handling indices."
                    });
                options.OperationFilter<BackOfficeSecurityRequirementsOperationFilter>();
            })
            .AddSingleton<IOperationIdHandler, AlgoliaOperationIdHandler>(); ;
        }

    }
}
