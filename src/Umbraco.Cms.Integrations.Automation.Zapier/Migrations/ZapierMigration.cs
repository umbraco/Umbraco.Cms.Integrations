using NPoco;

#if NETCOREAPP
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
#else
using Umbraco.Core.Migrations;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Umbraco.Core.Logging;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Migrations
{
    public class ZapierMigration : MigrationBase
    {
        public string MigrationLoggingMessage = $"Running migration {Constants.MigrationPlanName}";

        public string ContentDbTableExistsMessage =
            $"The database table {Constants.ZapierSubscriptionHookTable} already exists, skipping";

        public ZapierMigration(IMigrationContext context) : base(context)
        {
        }

#if NETCOREAPP
        protected override void Migrate()
#else
        public override void Migrate()
#endif
        {
#if NETCOREAPP
            Logger.LogDebug(MigrationLoggingMessage);
            
#else
            Logger.Debug<ZapierMigration>(MigrationLoggingMessage);
#endif

            if (TableExists(Constants.ZapierSubscriptionHookTable) == false)
            {
                Create.Table<ZapierSubscriptionHookTable>().Do();
            }
            else
            {
#if NETCOREAPP
                Logger.LogDebug(ContentDbTableExistsMessage);
#else
                Logger.Debug<ZapierMigration>(ContentDbTableExistsMessage);
#endif
            }
        }

        [TableName(Constants.ZapierSubscriptionHookTable)]
        [PrimaryKey("Id", AutoIncrement = true)]
        [ExplicitColumns]
        public class ZapierSubscriptionHookTable
        {
            [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
            [Column("Id")]
            public int Id { get; set; }

            /// <summary>
            /// Column stores two types of references:
            /// 1. content type alias for content triggers
            /// 2. form ID for form triggers
            /// </summary>
            [Column("EntityId")]
            [Index(IndexTypes.UniqueNonClustered, Name = "IX_ZapierSubscriptionHook_EntityId")]
            public string EntityId { get; set; }

            [Column("Type")]
            [Index(IndexTypes.UniqueNonClustered, Name = "IX_ZapierSubscriptionHook_Type", ForColumns = "EntityId,Type")]
            public int Type { get; set; }

            [Column("HookUrl")]
            [Index(IndexTypes.UniqueNonClustered, Name = "IX_ZapierSubscriptionHook_HookUrl")]
            public string HookUrl { get; set; }
        }
    }
}
