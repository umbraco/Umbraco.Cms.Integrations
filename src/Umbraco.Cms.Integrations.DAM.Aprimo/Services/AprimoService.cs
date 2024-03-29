﻿using System.Net.Http.Headers;
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

        public AprimoResponse<SearchItemsPaged<Language>> GetLanguages() => 
            GetLanguagesAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<AprimoResponse<SearchItemsPaged<Language>>> GetLanguagesAsync()
        {
            var client = _httpClientFactory.CreateClient(Constants.AprimoClient);

            if (!OAuthConfigurationIsValid(out string accessToken))
                return AprimoResponse<SearchItemsPaged<Language>>.Fail(Constants.ErrorResources.Unauthorized, false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync("languages");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<SearchItemsPaged<Language>>(content);

                return AprimoResponse<SearchItemsPaged<Language>>.Ok(data ?? new SearchItemsPaged<Language>());
            }

            var errorResponse = JsonSerializer.Deserialize<AprimoErrorResponse>(content);

            return AprimoResponse<SearchItemsPaged<Language>>.Fail(
                errorResponse != null
                    ? errorResponse.ToString()
                    : content,
                response.StatusCode != System.Net.HttpStatusCode.Unauthorized);
        }

        public AprimoResponse<Record> GetRecordById(Guid id) => 
            GetRecordByIdAsync(id).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<AprimoResponse<Record>> GetRecordByIdAsync(Guid id)
        {
            var client = _httpClientFactory.CreateClient(Constants.AprimoClient);

            if (!OAuthConfigurationIsValid(out string accessToken))
                return AprimoResponse<Record>.Fail(Constants.ErrorResources.Unauthorized, false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var request = new HttpRequestMessage(HttpMethod.Get, $"record/{id}");
            request.Headers.Add("select-record", "title,thumbnail,fields,masterfilelatestversion");
            request.Headers.Add("select-fileversion", "renditions");
            request.Headers.Add("select-rendition", "publiclinks");

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<Record>(content);

                return AprimoResponse<Record>.Ok(data ?? new Record());
            }

            var errorResponse = JsonSerializer.Deserialize<AprimoErrorResponse>(content);

            return AprimoResponse<Record>.Fail(
                errorResponse != null
                    ? errorResponse.ToString()
                    : content,
                response.StatusCode != System.Net.HttpStatusCode.Unauthorized);
        }

        public AprimoResponse<SearchItemsPaged<Record>> SearchRecords(string page, string searchTerm) =>
            SearchRecordsAsync(page, searchTerm).ConfigureAwait(false).GetAwaiter().GetResult();

        public async Task<AprimoResponse<SearchItemsPaged<Record>>> SearchRecordsAsync(string page, string searchTerm)
        {
            var client = _httpClientFactory.CreateClient(Constants.AprimoClient);

            if (!OAuthConfigurationIsValid(out string accessToken))
                return AprimoResponse<SearchItemsPaged<Record>>.Fail(Constants.ErrorResources.Unauthorized, false);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var request = new HttpRequestMessage(HttpMethod.Post, $"search/records");
            request.Headers.Add("page", page);
            request.Headers.Add("pagesize", "10");
            request.Headers.Add("select-record", "title,thumbnail");

            var aprimoRequest = new AprimoRequest
            {
                SearchExpression = new AprimoSearchExpression
                {
                    Expression = string.IsNullOrEmpty(searchTerm)
                        ? "ContentStatus = 'Released'"
                        : searchTerm
                }
            };
            request.Content = new StringContent(JsonSerializer.Serialize(aprimoRequest), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
                
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<SearchItemsPaged<Record>>(content);

                return AprimoResponse<SearchItemsPaged<Record>>.Ok(data ?? new SearchItemsPaged<Record>());
            }

            var errorResponse = JsonSerializer.Deserialize<AprimoErrorResponse>(content);

            return AprimoResponse<SearchItemsPaged<Record>>.Fail(
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

            accessToken = oauthConfiguration.AccessToken;

            return true;
        }
    }
}
