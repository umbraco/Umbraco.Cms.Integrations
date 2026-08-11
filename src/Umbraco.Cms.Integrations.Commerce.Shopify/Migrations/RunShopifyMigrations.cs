using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace Umbraco.Cms.Integrations.Commerce.Shopify.Migrations
{
    public class RunShopifyMigrations : INotificationAsyncHandler<UmbracoApplicationStartingNotification>
    {
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public RunShopifyMigrations(
            ICoreScopeProvider coreScopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState)
        {
            _coreScopeProvider = coreScopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
        }

        public async Task HandleAsync(UmbracoApplicationStartingNotification notification, CancellationToken cancellationToken)
        {
            if (_runtimeState.Level < Cms.Core.RuntimeLevel.Run) return;

            var migrationPlan = new MigrationPlan("Shopify");

            migrationPlan.From(string.Empty)
                .To<MigrateProductPickerToJsonStorage>("shopify-productpicker-json-storage");

            var upgrader = new Upgrader(migrationPlan);

            await upgrader.ExecuteAsync(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        }
    }
}
