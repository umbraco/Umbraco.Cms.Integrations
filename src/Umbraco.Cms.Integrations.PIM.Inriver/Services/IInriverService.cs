using Umbraco.Cms.Integrations.PIM.Inriver.Models;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Services
{
    public interface IInriverService
    {
        Task<ServiceResponse<IEnumerable<EntityType>>> GetEntityTypes();

        Task<ServiceResponse<IEnumerable<EntityData>>> FetchData(FetchDataRequest request);

        Task<ServiceResponse<QueryResponse>> Query(QueryRequest request);
    }
}
