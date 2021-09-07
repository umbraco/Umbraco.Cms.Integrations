using System.Collections.Specialized;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Configuration
{
    public class CommerceToolsSettings
    {
        public CommerceToolsSettings()
        {
        }

        public CommerceToolsSettings(NameValueCollection appSettings)
        {
            ApiUrl = appSettings["Umbraco.Cms.Integrations.Commerce.CommerceTools.ApiUrl"];
            ClientId = appSettings["Umbraco.Cms.Integrations.Commerce.CommerceTools.ClientId"];
            ClientSecret = appSettings["Umbraco.Cms.Integrations.Commerce.CommerceTools.ClientSecret"];
            OAuthUrl = appSettings["Umbraco.Cms.Integrations.Commerce.CommerceTools.OAuthUrl"];
            ProjectKey = appSettings["Umbraco.Cms.Integrations.Commerce.CommerceTools.ProjectKey"];
            DefaultLanguage = appSettings["Umbraco.Cms.Integrations.Commerce.CommerceTools.DefaultLanguage"] ?? "en-US";
        }

        public string OAuthUrl { get; set; }
        
        public string ApiUrl { get; set; }
        
        public string ProjectKey { get; set; }
        
        public string ClientId { get; set; }
        
        public string ClientSecret { get; set; }
        
        public string DefaultLanguage { get; set; } = "en-US";
    }
}
