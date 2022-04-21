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
    public class ZapierSubscriptionHookTable : MigrationBase
    {
        public string MigrationLoggingMessage = $"Running migration {Constants.MigrationPlanName}";

        public string DbTableExistsMessage =
            $"The database table {Constants.ZapierSubscriptionHookTable} already exists, skipping";

        public ZapierSubscriptionHookTable(IMigrationContext context) : base(context)
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
            Logger.Debug<ZapierSubscriptionHookTable>(MigrationLoggingMessage);
#endif

            if (TableExists(Constants.ZapierSubscriptionHookTable) == false)
            {
                Create.Table<ZapierSubscriptionHook>().Do();
            }
            else
            {
#if NETCOREAPP
                Logger.LogDebug(DbTableExistsMessage);
#else
                Logger.Debug<ZapierSubscriptionHookTable>(DbTableExistsMessage);
#endif
            }
        }



        [TableName(Constants.ZapierSubscriptionHookTable)]
        [PrimaryKey("Id", AutoIncrement = true)]
        [ExplicitColumns]
        public class ZapierSubscriptionHook
        {
            [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
            [Column("Id")]
            public int Id { get; set; }

            [Column("ContentTypeAlias")]
            [Index(IndexTypes.UniqueNonClustered, Name = "IX_ZapierSubscriptionHook_ContentTypeAlias")]
            public string ContentTypeAlias { get; set; }

            [Column("HookUrl")] 
            [Index(IndexTypes.UniqueNonClustered, Name = "IX_ZapierSubscriptionHook_HookUrl")]
            public string HookUrl { get; set; }
        }
    }
}
