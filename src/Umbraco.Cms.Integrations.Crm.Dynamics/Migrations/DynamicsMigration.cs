using NPoco;

#if NETCOREAPP
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
#else
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Persistence.DatabaseAnnotations;
#endif

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Migrations
{
    public class DynamicsMigration : MigrationBase
    {
        public string MigrationLoggingMessage = $"Running migration {Constants.MigrationPlanName}";

        public string DbTableExistsMessage =
            $"The database table {Constants.DynamicsOAuthConfigurationTable} already exists, skipping";

        public DynamicsMigration(IMigrationContext context) : base(context)
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
            Logger.Debug<DynamicsMigration>(MigrationLoggingMessage);
#endif

            if (TableExists(Constants.DynamicsOAuthConfigurationTable) == false)
            {
                Create.Table<DynamicsOAuthConfigurationTable>().Do();
            }
            else
            {
#if NETCOREAPP
                Logger.LogDebug(DbTableExistsMessage);
#else
                Logger.Debug<DynamicsMigration>(DbTableExistsMessage);
#endif
            }
        }
    }

    [TableName(Constants.DynamicsOAuthConfigurationTable)]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class DynamicsOAuthConfigurationTable
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("AccessToken")]
        [SpecialDbType(SpecialDbTypes.NTEXT)]
        public string AccessToken { get; set; }

        [Column("UserId")]
        public string UserId { get; set; }

        [Column("FullName")]
        public string FullName { get; set; }
    }
}
