using Algolia.Search.Models.Search;

using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Search.Algolia;
using Umbraco.Cms.Integrations.Search.Algolia.Configuration;
using Umbraco.Cms.Integrations.Search.Algolia.Handlers;
using Umbraco.Cms.Integrations.Search.Algolia.Migrations;
using Umbraco.Cms.Integrations.Search.Algolia.Models;
using Umbraco.Cms.Integrations.Search.Algolia.Services;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign
{
    public class AlgoliaComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, RunAlgoliaIndicesMigration>();

            builder.AddNotificationAsyncHandler<ContentPublishedNotification, ContentPublishedHandler>();

            builder.AddNotificationAsyncHandler<ContentDeletedNotification, ContentDeletedHandler>();

            builder.AddNotificationAsyncHandler<ContentUnpublishedNotification, ContentUnpublishedHandler>();

            var options = builder.Services.AddOptions<AlgoliaSettings>()
                .Bind(builder.Config.GetSection(Constants.SettingsPath));

            builder.Services.AddSingleton<IAlgoliaIndexService, AlgoliaIndexService>();

            builder.Services.AddSingleton<IAlgoliaSearchService<SearchResponse<Record>>, AlgoliaSearchService>();

            builder.Services.AddScoped<IAlgoliaIndexDefinitionStorage<AlgoliaIndex>, AlgoliaIndexDefinitionStorage>();

            builder.Services.AddScoped<IAlgoliaSearchPropertyIndexValueFactory, AlgoliaSearchPropertyIndexValueFactory>();

            // use this syntax for registering your own converter
            //builder.Services.AddScoped<IAlgoliaSearchPropertyIndexValueFactory, ExtendedAlgoliaSearchPropertyIndexValueFactory>();
        }

    }
}
