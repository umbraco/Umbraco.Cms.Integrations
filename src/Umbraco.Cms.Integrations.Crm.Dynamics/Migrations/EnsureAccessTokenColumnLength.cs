using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Migrations
{
    public class EnsureAccessTokenColumnLength : MigrationBase
    {
        public EnsureAccessTokenColumnLength(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            Logger.LogDebug("Running migration {0}", nameof(EnsureAccessTokenColumnLength));

            // SQLite has no ALTER COLUMN, and does not need one: it types values dynamically, so
            // VARCHAR(n) never enforced a length there and an access token of any size already
            // stored fine. Without this guard the step throws on every startup, which also stops
            // the plan ever recording its final state.
            if (DatabaseType == DatabaseType.SQLite)
            {
                return;
            }

            Alter.Table(Constants.DynamicsOAuthConfigurationTable)
                .AlterColumn(nameof(DynamicsOAuthConfigurationTable.AccessToken))
                .AsString(Constants.AccessTokenFieldSize)
                .NotNullable()
                .Do();
        }
    }
}
