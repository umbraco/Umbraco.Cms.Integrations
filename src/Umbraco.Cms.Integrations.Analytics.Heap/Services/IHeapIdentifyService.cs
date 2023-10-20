using Umbraco.Cms.Integrations.Analytics.Heap.Models;

namespace Umbraco.Cms.Integrations.Analytics.Heap.Services;

public interface IHeapIdentifyService
{
    Task<User?> Identify();
}
