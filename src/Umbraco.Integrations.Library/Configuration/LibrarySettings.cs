using System.Collections.Specialized;

namespace Umbraco.Integrations.Library.Configuration
{
    public class LibrarySettings
    {
        public LibrarySettings()
        {

        }

        public LibrarySettings(NameValueCollection appSettings)
        {
            UserGroupAlias = appSettings[Constants.UmbracoIntegrationsLibraryUserGroupAlias];

            ApiKey = appSettings[Constants.UmbracoIntegrationsLibraryApiKey];
        }

        public string UserGroupAlias { get; set; }

        public string ApiKey { get; set; }
    }
}
