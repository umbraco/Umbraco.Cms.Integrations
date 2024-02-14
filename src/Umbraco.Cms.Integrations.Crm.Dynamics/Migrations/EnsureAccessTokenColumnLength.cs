#if NETCOREAPP
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Persistence;
#else
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
#endif

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Migrations
{
    public class EnsureAccessTokenColumnLength : MigrationBase
    {
        public EnsureAccessTokenColumnLength(IMigrationContext context) : base(context)
        {
        }

#if NETCOREAPP
        protected override void Migrate()
#else
        public override void Migrate()
#endif
        {
#if NETCOREAPP
            Logger.LogDebug("Running migration {0}", nameof(EnsureAccessTokenColumnLength));

#else
            Logger.Debug<EnsureAccessTokenColumnLength>("Running migration {0}", nameof(EnsureAccessTokenColumnLength));
#endif

            Alter.Table(Constants.DynamicsOAuthConfigurationTable)
                .AlterColumn(nameof(DynamicsOAuthConfigurationTable.AccessToken))
                .AsString(Constants.AccessTokenFieldSize)
                .NotNullable()
                .Do();
            
        }
    }
}
