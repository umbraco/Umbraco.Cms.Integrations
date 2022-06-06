using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Configuration
{
    public class ZapierSettings
    {
        public ZapierSettings()
        {

        }

        public ZapierSettings(NameValueCollection appSettings)
        {
            UserGroupAlias = appSettings[Constants.UmbracoCmsIntegrationsAutomationZapierUserGroupAlias];

            ApiKey = appSettings[Constants.UmbracoCmsIntegrationsAutomationZapierApiKey];
        }

        public string UserGroupAlias { get; set; }

        public string ApiKey { get; set; }
    }
}
