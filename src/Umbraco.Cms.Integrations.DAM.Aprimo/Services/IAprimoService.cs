using Umbraco.Cms.Integrations.DAM.Aprimo.Models;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Services
{
    public interface IAprimoService
    {
        Task<AprimoResponse<SearchedRecordsPaged>> SearchRecords(string page);

        Task<AprimoResponse<Record>> GetRecordById(Guid id);
    }
}
