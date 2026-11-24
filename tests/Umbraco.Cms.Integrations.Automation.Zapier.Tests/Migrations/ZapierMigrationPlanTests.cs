using System.Reflection;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;
using Umbraco.Cms.Integrations.Automation.Zapier.Migrations;
using Xunit;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Tests.Migrations;

public class ZapierMigrationPlanTests
{
    private static IndexAttribute[] TableIndexes() =>
        typeof(ZapierMigration.ZapierSubscriptionHookTable)
            .GetProperties()
            .SelectMany(p => p.GetCustomAttributes<IndexAttribute>())
            .ToArray();

    [Fact]
    public void Plan_EndsWithTheMultipleHooksPerEntityState()
    {
        var plan = new ZapierMigrationPlan();

        plan.Validate();
        Assert.Equal(Constants.AllowMultipleHooksPerEntityTargetStateName, plan.FinalState);
    }

    [Fact]
    public void Plan_MovesExistingInstallsOnToTheIndexMigration()
    {
        var plan = new ZapierMigrationPlan();

        var transition = plan.Transitions[Constants.TargetStateName];

        Assert.NotNull(transition);
        Assert.Equal(typeof(AllowMultipleSubscriptionHooksPerEntity), transition!.MigrationType);
    }

    [Fact]
    public void Table_HasNoUniqueIndexOnEntityIdAndType()
    {
        Assert.DoesNotContain(
            TableIndexes(),
            i => i.IndexType == IndexTypes.UniqueNonClustered && i.ForColumns == "EntityId,Type");
    }

    [Fact]
    public void Table_DefinesTheNonUniqueEntityIndexTheMigrationCreates()
    {
        var index = Assert.Single(TableIndexes(), i => i.Name == Constants.SubscriptionHookEntityIndexName);

        Assert.Equal(IndexTypes.NonClustered, index.IndexType);
        Assert.Equal("EntityId,Type", index.ForColumns);
    }

    [Fact]
    public void Table_KeepsHookUrlUnique()
    {
        var hookUrlIndex = typeof(ZapierMigration.ZapierSubscriptionHookTable)
            .GetProperty(nameof(ZapierMigration.ZapierSubscriptionHookTable.HookUrl))!
            .GetCustomAttribute<IndexAttribute>();

        Assert.NotNull(hookUrlIndex);
        Assert.Equal(IndexTypes.UniqueNonClustered, hookUrlIndex!.IndexType);
    }
}
