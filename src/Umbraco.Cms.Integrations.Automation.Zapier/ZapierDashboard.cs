using System;

#if NETCOREAPP
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Dashboards;
#else
using Umbraco.Core.Composing;
using Umbraco.Core.Dashboards;
#endif

namespace Umbraco.Cms.Integrations.Automation.Zapier
{
    [Weight(30)]
    public class ZapierDashboard : IDashboard
    {
        public string Alias => "zapierDashboard";

        public string View => "/App_Plugins/UmbracoCms.Integrations/Automation/Zapier/dashboard.html";

        public string[] Sections => new[] { Core.Constants.Applications.Content };

        public IAccessRule[] AccessRules => Array.Empty<IAccessRule>();
    }
}
