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
    public class ZapContentConfigTable : MigrationBase
    {
        public string MigrationLoggingMessage = $"Running migration {Constants.MigrationPlanName}";

        public string DbTableExistsMessage =
            $"The database table {Constants.ZapContentConfigTable} already exists, skipping";

        public ZapContentConfigTable(IMigrationContext context) : base(context)
        {
        }

#if NETCOREAPP
        protected override void Migrate()
        {
            Logger.LogDebug(MigrationLoggingMessage);

            if (TableExists(Constants.ZapContentConfigTable) == false)
            {
                Create.Table<ZapContentConfig>().Do();
            }
            else
            {
                
                Logger.LogDebug(DbTableExistsMessage);
            }
        }
#else
        public override void Migrate()
        {

            Logger.Debug<ZapContentConfigTable>(MigrationLoggingMessage);

            if (TableExists(Constants.ZapContentConfigTable) == false)
            {
                Create.Table<ZapContentConfig>().Do();
            }
            else
            {
                Logger.Debug<ZapContentConfigTable>(DbTableExistsMessage);
            }
        }
#endif



        [TableName(Constants.ZapContentConfigTable)]
        [PrimaryKey("Id", AutoIncrement = true)]
        [ExplicitColumns]
        public class ZapContentConfig
        {
            [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
            [Column("Id")]
            public int Id { get; set; }

            [Column("ContentTypeName")]
            public string ContentTypeName { get; set; }

            [Column("WebHookUrl")] 
            public string WebHookUrl { get; set; }
        }
    }
}
