using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Configuration
{
    public class ZapierFormsSettings : ZapierSettings
    {
        public ZapierFormsSettings()
        {

        }

        public ZapierFormsSettings(NameValueCollection appSettings)
        {
            UserGroupAlias = appSettings[Constants.UmbracoFormsIntegrationsAutomationZapierUserGroupAlias];

            ApiKey = appSettings[Constants.UmbracoFormsIntegrationsAutomationZapierApiKey];
        }
    }
}
