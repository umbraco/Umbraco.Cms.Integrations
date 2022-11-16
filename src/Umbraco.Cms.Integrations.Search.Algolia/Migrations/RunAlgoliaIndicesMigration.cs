using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;

namespace Umbraco.Cms.Integrations.Search.Algolia.Migrations
{
    public class RunAlgoliaIndicesMigration : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public RunAlgoliaIndicesMigration(
            ICoreScopeProvider coreScopeProvider, 
            IMigrationPlanExecutor migrationPlanExecutor, 
            IKeyValueService keyValueService, 
            IRuntimeState runtimeState)
        {
            _coreScopeProvider = coreScopeProvider;

            _migrationPlanExecutor = migrationPlanExecutor;

            _keyValueService = keyValueService;

            _runtimeState = runtimeState;
        }

        public void Handle(UmbracoApplicationStartingNotification notification)
        {
            if (_runtimeState.Level < Core.RuntimeLevel.Run) return;

            var migrationPlan = new MigrationPlan("AlgoliaIndices");

            migrationPlan.From(string.Empty)
                .To<AddAlgoliaIndicesTable>("algoliaindices-db");

            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        }
    }
}
