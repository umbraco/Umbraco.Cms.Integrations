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
            UserGroup = appSettings[Constants.UmbracoCmsIntegrationsAutomationZapierUserGroup];

            ApiKey = appSettings[Constants.UmbracoCmsIntegrationsAutomationZapierApiKey];
        }

        public string UserGroup { get; set; }

        public string ApiKey { get; set; }
    }
}
