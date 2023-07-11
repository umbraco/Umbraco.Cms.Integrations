using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Configuration
{
    public class ZapierSettings : AppSettings
    {
        public ZapierSettings()
        {

        }

        public ZapierSettings(NameValueCollection appSettings)
        {
            UserGroupAlias = appSettings[Constants.UmbracoCmsIntegrationsAutomationZapierUserGroupAlias];

            ApiKey = appSettings[Constants.UmbracoCmsIntegrationsAutomationZapierApiKey];
        }
    }
}
