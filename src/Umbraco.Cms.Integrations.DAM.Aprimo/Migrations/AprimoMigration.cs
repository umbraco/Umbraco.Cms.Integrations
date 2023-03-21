using Microsoft.Extensions.Logging;

using NPoco;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Migrations
{
    public class AprimoMigration : MigrationBase
    {
        public AprimoMigration(IMigrationContext context) : base(context) { }

        protected override void Migrate()
        {
            Logger.LogDebug($"Running migration {Constants.Migration.Name}");

            if(TableExists(Constants.Migration.TableName))
                Logger.LogDebug($"The database table {Constants.Migration.TableName} already exists, skipping");
            else 
                Create.Table<AprimoOAuthConfiguration>().Do();
        }
    }

    [TableName(Constants.Migration.TableName)]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class AprimoOAuthConfiguration
    {
        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("AccessToken")]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string AccessToken { get; set; }

        [Column("RefreshToken")]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string RefreshToken { get; set; }

        [Column("CodeVerifier")]
        public string CodeVerifier { get; set; }

        [Column("CodeChallenge")]
        public string CodeChallenge { get; set; }
    }
}
