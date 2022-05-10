#if NETCOREAPP
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Automation.Zapier.Helpers;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Components
{
    public class NewContentPublishedNotification : INotificationHandler<ContentPublishedNotification>
    {
        private readonly ZapierSubscriptionHookService _zapierSubscriptionHookService;

        private readonly ZapierService _zapierService;

        private readonly ILogger<NewContentPublishedNotification> _logger;

        public NewContentPublishedNotification(ZapierSubscriptionHookService zapierSubscriptionHookService, ZapierService zapierService, ILogger<NewContentPublishedNotification> logger)
        {
            _zapierSubscriptionHookService = zapierSubscriptionHookService;

            _zapierService = zapierService;

            _logger = logger;
        }

        public void Handle(ContentPublishedNotification notification)
        {
            var triggerHelper = new TriggerHelper(_zapierService);

            foreach (var node in notification.PublishedEntities)
            {
                if (_zapierSubscriptionHookService.TryGetByAlias(node.ContentType.Alias, out var zapContentConfigList))
                {
                    var content = new Dictionary<string, string>
                    {
                        {Constants.Content.Id, node.Id.ToString() },
                        {Constants.Content.Name, node.Name },
                        {Constants.Content.PublishDate, DateTime.UtcNow.ToString("s") }
                    };

                    foreach (var zapContentConfig in zapContentConfigList)
                    {
                        var result = triggerHelper.Execute(zapContentConfig.HookUrl, content);

                        if (!string.IsNullOrEmpty(result))
                            _logger.LogError(result);
                    }
                }
            }
        }
    }
}
#endif
