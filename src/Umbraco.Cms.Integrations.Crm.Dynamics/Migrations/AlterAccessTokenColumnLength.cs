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
    public class AlterAccessTokenColumnLength : MigrationBase
    {
        public AlterAccessTokenColumnLength(IMigrationContext context) : base(context)
        {
        }

#if NETCOREAPP
        protected override void Migrate()
#else
        public override void Migrate()
#endif
        {
#if NETCOREAPP
            Logger.LogDebug("Running migration {0}", nameof(AlterAccessTokenColumnLength));

#else
            Logger.Debug<AlterAccessTokenColumnLength>("Running migration {0}", nameof(AlterAccessTokenColumnLength));
#endif

            Alter.Table(Constants.DynamicsOAuthConfigurationTable)
                .AlterColumn(nameof(DynamicsOAuthConfigurationTable.AccessToken))
                .AsString(4000)
                .NotNullable()
                .Do();
            
        }
    }
}
