#if NETCOREAPP
using Umbraco.Cms.Infrastructure.Migrations;
#else
using Umbraco.Core.Migrations;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Migrations
{
    public class ZapierMigrationPlan : MigrationPlan
    {
        public ZapierMigrationPlan() : base(Constants.MigrationPlanName)
        {
            From(string.Empty)
                .To<ZapierMigration>(Constants.TargetStateName);
        }
    }
}
