using Umbraco.Cms.Integrations.DAM.Aprimo.Models;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Services
{
    public interface IAprimoService
    {
        AprimoResponse<SearchItemsPaged<Language>> GetLanguages();

        Task<AprimoResponse<SearchItemsPaged<Language>>> GetLanguagesAsync();

        AprimoResponse<SearchItemsPaged<Record>> SearchRecords(string page, string searchTerm);

        Task<AprimoResponse<SearchItemsPaged<Record>>> SearchRecordsAsync(string page, string searchTerm);

        AprimoResponse<Record> GetRecordById(Guid id);

        Task<AprimoResponse<Record>> GetRecordByIdAsync(Guid id);
    }
}
