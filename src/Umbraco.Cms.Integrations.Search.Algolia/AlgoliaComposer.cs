using Algolia.Search.Models.Search;
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Search.Algolia;
using Umbraco.Cms.Integrations.Search.Algolia.Builders;
using Umbraco.Cms.Integrations.Search.Algolia.Configuration;
using Umbraco.Cms.Integrations.Search.Algolia.Extensions;
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

            builder.AddNotificationAsyncHandler<ContentCacheRefresherNotification, AlgoliaContentCacheRefresherHandler>();

            builder.Services.AddOptions<AlgoliaSettings>()
                 .Bind(builder.Config.GetSection(Constants.SettingsPath));

            builder.Services.AddSingleton<IAlgoliaIndexService, AlgoliaIndexService>();

            builder.Services.AddSingleton<IAlgoliaSearchService<SearchResponse<Record>>, AlgoliaSearchService>();

            builder.Services.AddScoped<IAlgoliaIndexDefinitionStorage<AlgoliaIndex>, AlgoliaIndexDefinitionStorage>();

            builder.Services.AddScoped<IRecordBuilderFactory, RecordBuilderFactory>();

            builder.Services.AddScoped<IAlgoliaSearchPropertyIndexValueFactory, AlgoliaSearchPropertyIndexValueFactory>();

            builder.AddAlgoliaConverters();
        }

    }
}
