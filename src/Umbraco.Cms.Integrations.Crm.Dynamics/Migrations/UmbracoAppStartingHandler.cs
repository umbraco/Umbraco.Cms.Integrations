﻿using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Migrations
{
    public class UmbracoAppStartingHandler : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;

        private readonly IScopeProvider _scopeProvider;

        private readonly IKeyValueService _keyValueService;

        private readonly IRuntimeState _runtimeState;

        public UmbracoAppStartingHandler(
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
            if (_runtimeState.Level < RuntimeLevel.Run) return;

            var upgrader = new Upgrader(new DynamicsMigrationPlan());
            upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
        }
    }
}