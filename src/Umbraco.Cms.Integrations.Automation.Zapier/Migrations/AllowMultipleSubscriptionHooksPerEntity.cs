using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Migrations
{
    /// <summary>
    /// Replaces the unique index on <c>EntityId, Type</c> with a non-unique one, so more than one
    /// Zap can subscribe to the same content type or form. <c>HookUrl</c> stays unique.
    /// </summary>
    public class AllowMultipleSubscriptionHooksPerEntity : AsyncMigrationBase
    {
        private const string LegacyUniqueIndexName = "IX_ZapierSubscriptionHook_Type";

        public AllowMultipleSubscriptionHooksPerEntity(IMigrationContext context) : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            Logger.LogDebug("Running migration {Migration}", nameof(AllowMultipleSubscriptionHooksPerEntity));

            if (IndexExists(LegacyUniqueIndexName))
            {
                Delete.Index(LegacyUniqueIndexName).OnTable(Constants.ZapierSubscriptionHookTable).Do();
            }

            // A fresh install already gets this index from the table definition.
            if (IndexExists(Constants.SubscriptionHookEntityIndexName) == false)
            {
                CreateIndex<ZapierMigration.ZapierSubscriptionHookTable>(Constants.SubscriptionHookEntityIndexName);
            }

            return Task.CompletedTask;
        }
    }
}
