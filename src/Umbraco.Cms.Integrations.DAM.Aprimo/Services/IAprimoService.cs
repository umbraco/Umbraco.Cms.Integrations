using Umbraco.Cms.Integrations.DAM.Aprimo.Models;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Services
{
    public interface IAprimoService
    {
        Task<AprimoResponse<SearchItemsPaged<Language>>> GetLanguages();

        Task<AprimoResponse<SearchItemsPaged<Record>>> SearchRecords(string page, string searchTerm);

        Task<AprimoResponse<Record>> GetRecordById(Guid id);
    }
}
