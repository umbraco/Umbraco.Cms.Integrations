using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Migrations
{
    public class AprimoMigrationPlan : MigrationPlan
    {
        public AprimoMigrationPlan() : base(Constants.Migration.Name)
        {
            From(string.Empty)
                .To<AprimoMigration>(Constants.Migration.TargetStateName);
        }
    }
}
