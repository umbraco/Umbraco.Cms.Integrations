using NPoco;
using Umbraco.Cms.Integrations.Automation.Zapier.Helpers;

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

        public string FormDbTableExistsMessage =
            $"The database table {Constants.ZapierFormSubscriptionHookTable} already exists, skipping";

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

            if (FormsHelper.IsFormsExtensionInstalled)
            {
                if (TableExists(Constants.ZapierFormSubscriptionHookTable) == false)
                {
                    Create.Table<ZapierFormSubscriptionHookTable>().Do();
                }
                else
                {
#if NETCOREAPP
                Logger.LogDebug(FormDbTableExistsMessage);
#else
                    Logger.Debug<ZapierMigration>(FormDbTableExistsMessage);
#endif
                }
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

            [Column("ContentTypeAlias")]
            [Index(IndexTypes.UniqueNonClustered, Name = "IX_ZapierSubscriptionHook_ContentTypeAlias")]
            public string ContentTypeAlias { get; set; }

            [Column("HookUrl")]
            [Index(IndexTypes.UniqueNonClustered, Name = "IX_ZapierSubscriptionHook_HookUrl")]
            public string HookUrl { get; set; }
        }

        [TableName(Constants.ZapierFormSubscriptionHookTable)]
        [PrimaryKey("Id", AutoIncrement = true)]
        [ExplicitColumns]
        public class ZapierFormSubscriptionHookTable
        {
            [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
            [Column("Id")]
            public int Id { get; set; }

            [Column("FormName")]
            [Index(IndexTypes.UniqueNonClustered, Name = "IX_ZapierFormSubscriptionHook_FormName")]
            public string FormName { get; set; }

            [Column("HookUrl")]
            [Index(IndexTypes.UniqueNonClustered, Name = "IX_ZapierFormSubscriptionHook_HookUrl")]
            public string HookUrl { get; set; }
        }
    }
}
