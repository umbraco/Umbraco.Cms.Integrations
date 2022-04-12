#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Core.Services.Implement;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Components
{

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
                if (zapContentConfig == null || !zapContentConfig.IsEnabled) continue;

                var content = new Dictionary<string, string>
                {
                    { Constants.Content.Id, node.Id.ToString() },
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
}
#endif
