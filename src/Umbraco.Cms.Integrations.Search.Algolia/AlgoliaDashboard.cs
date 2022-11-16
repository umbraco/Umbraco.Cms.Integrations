using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Dashboards;

namespace Umbraco.Cms.Integrations.Search.Algolia
{
    [Weight(100)]
    public class AlgoliaDashboard : IDashboard
    {
        public string[] Sections => new[] { Umbraco.Cms.Core.Constants.Applications.Settings };

        public IAccessRule[] AccessRules => Array.Empty<IAccessRule>();

        public string Alias => "algoliaSearchManagement";

        public string View => "/App_Plugins/UmbracoCms.Integrations/Search/Algolia/views/dashboard.html";
    }
}
