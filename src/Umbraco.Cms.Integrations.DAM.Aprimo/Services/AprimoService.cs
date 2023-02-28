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

        public async Task<AprimoResponse<Record>> GetRecordById(Guid id)
        {
            var client = _httpClientFactory.CreateClient(Constants.AprimoClient);

            if(!OAuthConfigurationIsValid(out string accessToken)) 
                return AprimoResponse<Record>.Fail(Constants.ErrorResources.Unauthorized, false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync($"record/{id}");
            var content = await response.Content.ReadAsStringAsync();

            if(response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<Record>(content);

                return AprimoResponse<Record>.Ok(data ?? new Record()); 
            }

            var errorResponse = JsonSerializer.Deserialize<AprimoErrorResponse>(content);

            return AprimoResponse<Record>.Fail(
                errorResponse != null 
                    ?errorResponse.ToString()
                    : content, 
                response.StatusCode != System.Net.HttpStatusCode.Unauthorized);
        }

        public async Task<AprimoResponse<SearchedRecordsPaged>> SearchRecords(string page)
        {
            var client = _httpClientFactory.CreateClient(Constants.AprimoClient);

            if(!OAuthConfigurationIsValid(out string accessToken)) 
                return AprimoResponse<SearchedRecordsPaged>.Fail(Constants.ErrorResources.Unauthorized, false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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

            var errorResponse = JsonSerializer.Deserialize<AprimoErrorResponse>(content);

            return AprimoResponse<SearchedRecordsPaged>.Fail(
                errorResponse != null
                    ? errorResponse.ToString()
                    : content,
                response.StatusCode != System.Net.HttpStatusCode.Unauthorized);
        }

        private bool OAuthConfigurationIsValid(out string accessToken)
        {
            var oauthConfiguration = _oauthConfigurationStorage.Get();

            if (oauthConfiguration == null || string.IsNullOrEmpty(oauthConfiguration.AccessToken))
            {
                accessToken = string.Empty;
                return false;
            }

            accessToken= oauthConfiguration.AccessToken;

            return true;
        }
    }
}
