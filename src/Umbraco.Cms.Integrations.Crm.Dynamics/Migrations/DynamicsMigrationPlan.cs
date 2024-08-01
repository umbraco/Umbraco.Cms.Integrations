using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Migrations
{
    public class DynamicsMigrationPlan : MigrationPlan
    {
        public DynamicsMigrationPlan() : base(Constants.MigrationPlanName)
        {
            From(string.Empty)
                .To<DynamicsMigration>(Constants.TargetStateName)
                .To<EnsureAccessTokenColumnLength>(Constants.AlterAccessTokenColumnLengthTargetStateName);
        }
    }
}
