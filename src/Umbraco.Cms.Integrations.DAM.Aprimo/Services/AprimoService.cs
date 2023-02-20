using System.Dynamic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using Umbraco.Cms.Integrations.DAM.Aprimo.Models;

namespace Umbraco.Cms.Integrations.DAM.Aprimo.Services
{
    public class AprimoService : IAprimoService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly OAuthConfigurationStorage _oauthConfigurationStorage;

        public AprimoService(IHttpClientFactory httpClientFactory, OAuthConfigurationStorage oauthConfigurationStorage)
        {
            _httpClientFactory = httpClientFactory;

            _oauthConfigurationStorage = oauthConfigurationStorage;
        }

        public async Task<AprimoResponse<Record>> SearchRecord(Guid id)
        {
            var client = _httpClientFactory.CreateClient(Constants.AprimoClient);

            var oauthConfiguration = _oauthConfigurationStorage.Get();
            if (oauthConfiguration == null) return AprimoResponse<Record>.Fail(Constants.ErrorResources.Unauthorized, false);

            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Umbraco.Cms.Integrations.DAM.Aprimo", "1.0.0"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthConfiguration.AccessToken);
            client.DefaultRequestHeaders.Add("select-record", "title,tag,thumbnail");

            var response = await client.GetAsync($"record/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if(response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<Record>(content);

                return AprimoResponse<Record>.Ok(data ?? new Record()); 
            }

            return AprimoResponse<Record>.Fail(content, response.StatusCode != System.Net.HttpStatusCode.Unauthorized);
        }

        public async Task<AprimoResponse<SearchedRecordsPaged>> SearchRecords(string page)
        {
            var client = _httpClientFactory.CreateClient(Constants.AprimoClient);

            var oauthConfiguration = _oauthConfigurationStorage.Get();
            if (oauthConfiguration == null) return AprimoResponse<SearchedRecordsPaged>.Fail(Constants.ErrorResources.Unauthorized, false);

            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Umbraco.Cms.Integrations.DAM.Aprimo", "1.0.0"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oauthConfiguration.AccessToken);
            client.DefaultRequestHeaders.Add("select-record", "title,tag,thumbnail");
            client.DefaultRequestHeaders.Add("page", page);
            client.DefaultRequestHeaders.Add("pagesize", "10");

            var request = new AprimoRequest
            {
                SearchExpression = new AprimoSearchExpression
                {
                    Expression = "ContentStatus = 'Released'"
                }
            };

            var response = await client.PostAsync($"search/records", 
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<SearchedRecordsPaged>(content);

                return AprimoResponse<SearchedRecordsPaged>.Ok(data ?? new SearchedRecordsPaged());
            }

            return AprimoResponse<SearchedRecordsPaged>.Fail(content, response.StatusCode != System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
