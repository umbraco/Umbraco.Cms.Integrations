﻿using NPoco;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

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

        protected override void Migrate()
        {
            Logger.LogDebug(MigrationLoggingMessage);

            if (TableExists(Constants.DynamicsOAuthConfigurationTable) == false)
            {
                Create.Table<DynamicsOAuthConfigurationTable>().Do();
            }
            else
            {
                Logger.LogDebug(DbTableExistsMessage);
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
        [Length(Constants.AccessTokenFieldSize)]
        public string AccessToken { get; set; }

        [Column("UserId")]
        public string UserId { get; set; }

        [Column("FullName")]
        public string FullName { get; set; }
    }
}
