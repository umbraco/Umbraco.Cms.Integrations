using Umbraco.Cms.Integrations.Crm.Dynamics.Models;
using Umbraco.Cms.Integrations.Crm.Dynamics.Models.Dtos;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Services
{
    public interface IDynamicsService
    {
        Task<IdentityDto> GetIdentity(string accessToken);

        Task<string> GetEmbedCode(string formId);

        Task<IEnumerable<FormDto>> GetForms(DynamicsModule module);

        Task<FormDto> GetRealTimeForm(string id);
    }
}
