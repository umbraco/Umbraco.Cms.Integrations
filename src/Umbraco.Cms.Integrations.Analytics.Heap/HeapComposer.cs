using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Integrations.Analytics.Heap.Manifests;
using Umbraco.Cms.Integrations.Analytics.Heap.Services;
using Umbraco.Cms.Integrations.Analytics.Heap.Services.Implement;

namespace Umbraco.Cms.Integrations.Analytics.Heap;

internal class HeapComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.ManifestFilters().Append<HeapManifestFilter>();

        builder.Services.AddScoped<IHeapIdentifyService, HeapIdentifyService>();
    }
}
