
using Umbraco.Cms.Integrations.Automation.Zapier.Migrations;

#if NETCOREAPP
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
#else
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Components
{
#if NETCOREAPP
    public class ZapContentConfigMigration : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        
        private readonly IScopeProvider _scopeProvider;
        
        private readonly IKeyValueService _keyValueService;
        
        private readonly IRuntimeState _runtimeState;

        public ZapContentConfigMigration(
            IScopeProvider scopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState)
        {
            _migrationPlanExecutor = migrationPlanExecutor;

            _scopeProvider = scopeProvider;

            _keyValueService = keyValueService;

            _runtimeState = runtimeState;
        }

        public void Handle(UmbracoApplicationStartingNotification notification)
        {
            if(_runtimeState.Level < RuntimeLevel.Run) return;

            var migrationPlan = new MigrationPlan(Constants.MigrationPlanName);

            migrationPlan.From(string.Empty)
                .To<ZapContentConfigTable>(Constants.TargetStateName);

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
        }
    }
#else
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class ZapContentConfigComposer : ComponentComposer<ZapContentConfigComponent>
    {

    }

    public class ZapContentConfigComponent : IComponent
    {
        private IScopeProvider _scopeProvider;

        private IMigrationBuilder _migrationBuilder;

        private IKeyValueService _keyValueService;

        private ILogger _logger;

        public ZapContentConfigComponent(IScopeProvider scopeProvider, IMigrationBuilder migrationBuilder, IKeyValueService keyValueService, ILogger logger)
        {
            _scopeProvider = scopeProvider;

            _migrationBuilder = migrationBuilder;

            _keyValueService = keyValueService;

            _logger = logger;
        }

        public void Initialize()
        {
            var migrationPlan = new MigrationPlan(Constants.MigrationPlanName);

            migrationPlan.From(string.Empty)
                .To<ZapContentConfigTable>(Constants.TargetStateName);

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
        }

        public void Terminate()
        {
        }
    }
#endif
}
