using Umbraco.Cms.Core.Dashboards;
using Umbraco.Cms.Core.Manifest;

namespace Umbraco.Cms.Integrations.Analytics.Heap.Manifests;

internal class HeapManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        manifests.Add(new PackageManifest
        {
            PackageName = "Umbraco.Cms.Integrations.Analytics.Heap",
            Scripts = new[]
            {
                "/App_Plugins/UmbracoCms.Integrations/Analytics/Heap/js/content-app.controller.js",
                "/App_Plugins/UmbracoCms.Integrations/Analytics/Heap/js/dashboard.controller.js"
            },
            AllowPackageTelemetry = true,
            Version = "1.0.0",
            ContentApps = new[]
            {
                new ManifestContentAppDefinition
                {
                    Alias = "heap",
                    Name = "Heap",
                    Icon = "icon-activity",
                    View = "/App_Plugins/UmbracoCms.Integrations/Analytics/Heap/views/content-app.html",
                    Weight = 1
                }
            },
            Dashboards = new[]
            {
                new ManifestDashboard
                {
                    Sections = new[] { Core.Constants.Applications.Content },
                    AccessRules = Array.Empty<IAccessRule>(),
                    Alias = "heapAnalyticsManagement",
                    View = "/App_Plugins/UmbracoCms.Integrations/Analytics/Heap/views/dashboard.html",
                    Weight = 100
                }
            }
        });
    }
}
