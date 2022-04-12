#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Components
{
    public class NewContentPublishedNotification : INotificationHandler<ContentPublishedNotification>
    {
        private readonly ZapConfigService _zapConfigService;

        private readonly ZapierService _zapierService;

        private readonly ILogger<NewContentPublishedNotification> _logger;

        public NewContentPublishedNotification(ZapConfigService zapConfigService, ZapierService zapierService, ILogger<NewContentPublishedNotification> logger)
        {
            _zapConfigService = zapConfigService;

            _zapierService = zapierService;

            _logger = logger;
        }

        public void Handle(ContentPublishedNotification notification)
        {
            foreach (var node in notification.PublishedEntities)
            {
                var zapContentConfig = _zapConfigService.GetByName(node.ContentType.Name);
                if (zapContentConfig == null || !zapContentConfig.IsEnabled) continue;

                var content = new Dictionary<string, string>
                {
                    { Constants.Content.Id, node.Id.ToString() },
                    { Constants.Content.Name, node.Name },
                    { Constants.Content.PublishDate, DateTime.UtcNow.ToString() }
                };

                var t = Task.Run(async () => await _zapierService.TriggerAsync(zapContentConfig.WebHookUrl, content));

                var result = t.Result;

                if (!string.IsNullOrEmpty(result))
                    _logger.LogError(result);
            }
        }
    }
}
#endif
