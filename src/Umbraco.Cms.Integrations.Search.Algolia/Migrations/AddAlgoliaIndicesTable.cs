using Microsoft.Extensions.Logging;

using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco.Cms.Integrations.Search.Algolia.Migrations
{
    public class AddAlgoliaIndicesTable : AsyncMigrationBase
    {
        public AddAlgoliaIndicesTable(IMigrationContext context) : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            Logger.LogDebug("Running migration {MigrationStep}", nameof(AddAlgoliaIndicesTable));

            if (TableExists(Constants.AlgoliaIndicesTableName))
                Logger.LogDebug("The database table {DbTable} already exists, skipping.", Constants.AlgoliaIndicesTableName);
            else
                Create.Table<AlgoliaIndex>().Do();

            return Task.CompletedTask;
        }
    }
}
