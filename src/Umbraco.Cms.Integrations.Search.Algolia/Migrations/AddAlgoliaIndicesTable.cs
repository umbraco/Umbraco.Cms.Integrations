using Microsoft.Extensions.Logging;

using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco.Cms.Integrations.Search.Algolia.Migrations
{
    public class AddAlgoliaIndicesTable : MigrationBase
    {
        public AddAlgoliaIndicesTable(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {MigrationStep}", nameof(AddAlgoliaIndicesTable));

            if (TableExists(Constants.AlgoliaIndicesTableName))
                Logger.LogDebug("The database table {DbTable} already exists, skipping.", Constants.AlgoliaIndicesTableName);
            else
                Create.Table<AlgoliaIndex>().Do();
        }
    }
}
