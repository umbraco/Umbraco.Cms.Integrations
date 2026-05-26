using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Migrations
{
    public class EnsureAccessTokenColumnLength : AsyncMigrationBase
    {
        public EnsureAccessTokenColumnLength(IMigrationContext context) : base(context)
        {
        }

        protected override Task MigrateAsync()
        {
            Logger.LogDebug("Running migration {0}", nameof(EnsureAccessTokenColumnLength));

            Alter.Table(Constants.DynamicsOAuthConfigurationTable)
                .AlterColumn(nameof(DynamicsOAuthConfigurationTable.AccessToken))
                .AsString(Constants.AccessTokenFieldSize)
                .NotNullable()
                .Do();

            return Task.CompletedTask;
        }
    }
}
