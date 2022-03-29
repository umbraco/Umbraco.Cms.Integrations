using NPoco;

using Umbraco.Core.Migrations;
using Umbraco.Core.Persistence.DatabaseAnnotations;
using Umbraco.Core.Logging;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Migrations
{
    public class ZapContentConfigTable : MigrationBase
    {
        public ZapContentConfigTable(IMigrationContext context) : base(context)
        {
        }

        public override void Migrate()
        {
            Logger.Debug<ZapContentConfigTable>("Running migration {MigrationStep}", "ZapContentConfigTable");

            if (TableExists(Constants.ZapContentConfigTable) == false)
            {
                Create.Table<ZapContentConfig>().Do();
            }
            else
            {
                Logger.Debug<ZapContentConfigTable>("The database table {DbTable} already exists, skipping", "ZapContentConfigTable");
            }
        }

        [TableName(Constants.ZapContentConfigTable)]
        [PrimaryKey("Id", AutoIncrement = true)]
        [ExplicitColumns]
        public class ZapContentConfig
        {
            [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
            [Column("Id")]
            public int Id { get; set; }

            [Column("ContentTypeAlias")]
            public string ContentTypeAlias { get; set; }

            [Column("WebHookUrl")] 
            public string WebHookUrl { get; set; }
        }
    }
}
