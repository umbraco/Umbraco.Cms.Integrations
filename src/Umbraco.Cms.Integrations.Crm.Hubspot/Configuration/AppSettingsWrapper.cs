using System.Collections.Specialized;
using System.Configuration;

namespace Umbraco.Cms.Integrations.Crm.Hubspot.Configuration
{
    public class AppSettingsWrapper : IAppSettings
    {
        private readonly NameValueCollection _appSettings;

        public AppSettingsWrapper()
        {
            _appSettings = ConfigurationManager.AppSettings;
        }

        public string this[string key] => _appSettings[key];
    }
}
