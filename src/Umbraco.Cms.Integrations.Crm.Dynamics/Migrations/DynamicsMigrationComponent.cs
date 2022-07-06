#if NETFRAMEWORK
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Migrations
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class DynamicsMigrationComposer : ComponentComposer<DynamicsMigrationComponent>
    {

    }

    public class DynamicsMigrationComponent : IComponent
    {
        private IScopeProvider _scopeProvider;

        private IMigrationBuilder _migrationBuilder;

        private IKeyValueService _keyValueService;

        private ILogger _logger;

        public DynamicsMigrationComponent(IScopeProvider scopeProvider, IMigrationBuilder migrationBuilder, IKeyValueService keyValueService, ILogger logger)
        {
            _scopeProvider = scopeProvider;

            _migrationBuilder = migrationBuilder;

            _keyValueService = keyValueService;

            _logger = logger;
        }

        public void Initialize()
        {
            var upgrader = new Upgrader(new DynamicsMigrationPlan());
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
        }

        public void Terminate()
        {
        }
    }
}
#endif
