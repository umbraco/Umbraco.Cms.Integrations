using Umbraco.Cms.Integrations.Automation.Zapier.Migrations;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Migrations.Upgrade;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Components
{

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
            var migrationPlan = new MigrationPlan("ZapContentConfig");

            migrationPlan.From(string.Empty)
                .To<ZapContentConfigTable>("zapiercontentconfigurations-db");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_scopeProvider, _migrationBuilder, _keyValueService, _logger);
        }

        public void Terminate()
        {
        }
    }
}
