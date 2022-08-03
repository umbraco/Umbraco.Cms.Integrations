#if NETCOREAPP
using Umbraco.Cms.Infrastructure.Migrations;
#else
using Umbraco.Core.Migrations;
#endif

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Migrations
{
    public class DynamicsMigrationPlan : MigrationPlan
    {
        public DynamicsMigrationPlan() : base(Constants.MigrationPlanName)
        {
            From(string.Empty)
                .To<DynamicsMigration>(Constants.TargetStateName);
        }
    }
}
