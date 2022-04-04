using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Umbraco.Core;

#if NETCOREAPP
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
#else
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier.Components
{
#if NETCOREAPP
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
                if (zapContentConfig == null) continue;

                var content = new Dictionary<string, string>
                {
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
#else
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class NewContentPublishedComposer : ComponentComposer<NewContentPublishedComponent>
    { }

    public class NewContentPublishedComponent : IComponent
    {
        private readonly ZapConfigService _zapConfigService;

        private readonly ZapierService _zapierService;

        private readonly ILogger _logger;

        public NewContentPublishedComponent(ZapConfigService zapConfigService, ZapierService zapierService, ILogger logger)
        {
            _zapConfigService = zapConfigService;

            _zapierService = zapierService;

            _logger = logger;
        }

        public void Initialize()
        {
            ContentService.Published += ContentServiceOnPublished;
        }

        public void Terminate()
        {
            ContentService.Published -= ContentServiceOnPublished;
        }

        private void ContentServiceOnPublished(IContentService sender, ContentPublishedEventArgs e)
        {
            foreach (var node in e.PublishedEntities)
            {
                var zapContentConfig = _zapConfigService.GetByName(node.ContentType.Name);
                if (zapContentConfig == null) continue;

                var content = new Dictionary<string, string>
                {
                    { Constants.Content.Name, node.Name },
                    { Constants.Content.PublishDate, DateTime.UtcNow.ToString() }
                };

                var t = Task.Run(async () => await _zapierService.TriggerAsync(zapContentConfig.WebHookUrl, content));

                var result = t.Result;

                if(!string.IsNullOrEmpty(result))
                    _logger.Error<NewContentPublishedComponent>(result);
            }
        }
    }
#endif
}
