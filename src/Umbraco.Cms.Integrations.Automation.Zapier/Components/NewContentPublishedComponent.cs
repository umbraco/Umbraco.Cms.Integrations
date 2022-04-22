#if NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Integrations.Automation.Zapier.Helpers;
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
        private readonly ZapierSubscriptionHookService _zapierSubscriptionHookService;

        private readonly ZapierService _zapierService;

        private readonly ILogger _logger;

        public NewContentPublishedComponent(ZapierSubscriptionHookService zapierSubscriptionHookService, ZapierService zapierService, ILogger logger)
        {
            _zapierSubscriptionHookService = zapierSubscriptionHookService;

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
            var triggerHelper = new TriggerHelper(_zapierService);

            foreach (var node in e.PublishedEntities)
            {
                if (_zapierSubscriptionHookService.TryGetByAlias(node.ContentType.Alias, out var zapContentConfigList))
                {
                    foreach (var zapContentConfig in zapContentConfigList)
                    {
                        var result = triggerHelper.Execute(zapContentConfig.HookUrl, node.Id.ToString(), node.Name);

                        if (!string.IsNullOrEmpty(result))
                            _logger.Error<NewContentPublishedComponent>(result);
                    }
                }
            }
        }
    }
}
#endif
