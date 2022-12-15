using Umbraco.Cms.Integrations.PIM.Inriver.Models;

namespace Umbraco.Cms.Integrations.PIM.Inriver.Services
{
    public interface IInriverService
    {
        Task<ServiceResponse<IEnumerable<EntityType>>> GetEntityTypes();

        Task<ServiceResponse<QueryResponse>> Query(QueryRequest request);

        Task<ServiceResponse<Entity>> GetEntitySummary(int id);

        Task<ServiceResponse<IEnumerable<FieldValue>>> GetEntityFieldValues(int id);
    }
}
