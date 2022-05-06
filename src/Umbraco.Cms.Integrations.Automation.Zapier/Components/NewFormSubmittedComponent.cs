#if NETFRAMEWORK
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Web;
using Umbraco.Cms.Integrations.Automation.Zapier.Helpers;
using Umbraco.Cms.Integrations.Automation.Zapier.Services;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Components
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class NewFormSubmittedComposer : ComponentComposer<NewFormSubmittedComponent>
    { }

    public class NewFormSubmittedComponent : IComponent
    {
        private readonly ZapierFormSubscriptionHookService _zapierFormSubscriptionHookService;

        private readonly ZapierFormService _zapierFormService;

        private readonly ZapierService _zapierService;

        private readonly ILogger _logger;

        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public NewFormSubmittedComponent(ZapierFormSubscriptionHookService zapierFormSubscriptionHookService, 
            ZapierFormService zapierFormService, 
            ZapierService zapierService, 
            ILogger logger,
            IUmbracoContextAccessor umbracoContextAccessor)
        {
            _zapierFormSubscriptionHookService = zapierFormSubscriptionHookService;

            _zapierFormService = zapierFormService;

            _zapierService = zapierService;

            _logger = logger;

            _umbracoContextAccessor = umbracoContextAccessor;
        }

        public void Initialize()
        {
            if (FormsHelper.IsFormsExtensionInstalled && _zapierFormService.TryResolveRecordStorage(out var recordStorage))
            {
                _zapierFormService.AddEventHandler(recordStorage, RecordInserting);
            }
        }

        public void Terminate()
        {
        }

        private void RecordInserting(object sender, object args)
        {
            var record = args.GetType().GetProperty("Record");
            var form = args.GetType().GetProperty("Form");

            if (form == null) throw new ArgumentNullException("Invalid form reference");

            var formObjValue = form.GetValue(args);
            var recordObjValue = record.GetValue(args);

            var formId = formObjValue.GetType().GetProperty("Id").GetValue(formObjValue).ToString();
            var formName = formObjValue.GetType().GetProperty("Name").GetValue(formObjValue).ToString();
            var umbracoPageId = recordObjValue.GetType().GetProperty("UmbracoPageId").GetValue(recordObjValue).ToString();

            var triggerHelper = new TriggerHelper(_zapierService);

            if (_zapierFormSubscriptionHookService.TryGetByName(formName, out var zapFormConfigList))
            {
                UmbracoContext umbracoContext = _umbracoContextAccessor.UmbracoContext;
                var pageUrl = umbracoContext.UrlProvider.GetUrl(int.Parse(umbracoPageId), UrlMode.Absolute);

                foreach (var zapFormConfig in zapFormConfigList)
                {

                    var result = triggerHelper.FormExecute(zapFormConfig.HookUrl, formId, formName,
                        HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));

                    if(!string.IsNullOrEmpty(result))
                        _logger.Error<NewFormSubmittedComponent>(result);
                }
            }
        }
    }
}

#endif