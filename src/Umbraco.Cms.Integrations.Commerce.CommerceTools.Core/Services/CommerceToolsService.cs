using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Configuration;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Extensions;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Search.Filters;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Search.Sorting;

#if NETCOREAPP
using System.Text.Json;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

#else
using System.Configuration;
using Umbraco.Core;
using Umbraco.Core.Persistence.DatabaseModelDefinitions;
using Newtonsoft.Json;
#endif

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Services
{
    public class CommerceToolsService : ICommerceToolsService
    {
        private CommerceToolsSettings _options;

        private string Scope { get; }

        private string AccessToken { get; set; }

        private DateTime? Expiration { get; set; }

#if NETCOREAPP
        public CommerceToolsService(IOptions<CommerceToolsSettings> options)
        {
            _options = options.Value;
            Scope = $"manage_project:{_options.ProjectKey}";
        }
#else
        public CommerceToolsService()
        {
            _options = new CommerceToolsSettings(ConfigurationManager.AppSettings);
            Scope = $"manage_project:{_options.ProjectKey}";
        }
#endif

        public async Task<Product> GetProductByIdAsync(Guid id, string languageCode = null)
        {
            languageCode = languageCode ?? _options.DefaultLanguage;
            var products = await GetProductsAsync(pageSize: 1, languageCode: languageCode, filters: new IdFilter(id)).ConfigureAwait(false);

            return products.Results.FirstOrDefault();
        }

        public async Task<IEnumerable<Product>> GetProductsByIdsAsync(Guid[] ids, string languageCode = null)
        {
            languageCode = languageCode ?? _options.DefaultLanguage;
            if (ids.Length == 0)
                return Enumerable.Empty<Product>();

            var filter = new CustomFilter($"id in ({string.Join(", ", ids.Select(id => $"\"{id}\""))})");

            var products = await GetProductsAsync(pageSize: ids.Length, languageCode: languageCode, filters: filter).ConfigureAwait(false);

            return products.Results;
        }

        public async Task<Category> GetCategoryByIdAsync(Guid id, string languageCode = null)
        {
            languageCode = languageCode ?? _options.DefaultLanguage;
            var categories = await GetCategoriesAsync(pageSize: 1, languageCode: languageCode, filters: new IdFilter(id)).ConfigureAwait(false);
            return categories.Results.FirstOrDefault();
        }

        public async Task<IEnumerable<Category>> GetCategoriesByIdsAsync(Guid[] ids, string languageCode = null)
        {
            languageCode = languageCode ?? _options.DefaultLanguage;
            if (ids.Length == 0)
                return Enumerable.Empty<Category>();

            var filter = new CustomFilter($"id in ({string.Join(", ", ids.Select(id => $"\"{id}\""))})");

            var categories = await GetCategoriesAsync(pageSize: ids.Length, languageCode: languageCode, filters: filter).ConfigureAwait(false);

            return categories.Results;
        }

        public async Task<PagedResults<Category>> GetPagedCategoriesAsync(
            int pageNumber,
            int pageSize,
            CategorySortingProperty orderBy = CategorySortingProperty.None,
            Direction orderDirection = Direction.Ascending,
            string languageCode = null,
            string terms = "")
        {
            languageCode = languageCode ?? _options.DefaultLanguage;
            var filters = new List<BaseFilter>();

            if (!terms.IsNullOrWhiteSpace())
            {
                if (Guid.TryParse(terms, out Guid termsGuid))
                    filters.Add(new IdFilter(termsGuid));
                else
                    filters.Add(new CategoryNameFilter(terms, languageCode));
            }

            var categories = await GetCategoriesAsync(pageNumber - 1, pageSize, languageCode: languageCode, sorting: new CategorySorting(orderBy, orderDirection.ToSortingDirection(), languageCode), filters: filters.ToArray()).ConfigureAwait(false);

            return categories;
        }

        public async Task<PagedResults<Product>> GetPagedProductsAsync(
            int pageNumber,
            int pageSize,
            ProductSortingProperty orderBy = ProductSortingProperty.None,
            Direction orderDirection = Direction.Ascending,
            string languageCode = null,
            string terms = ""
            )
        {
            languageCode = languageCode ?? _options.DefaultLanguage;
            var filters = new List<BaseFilter>();

            if (!terms.IsNullOrWhiteSpace())
            {
                if (Guid.TryParse(terms, out Guid termsGuid))
                    filters.Add(new IdFilter(termsGuid));
                else
                    filters.Add(new ProductNameFilter(terms, languageCode));
            }

            var products = await GetProductsAsync(pageNumber - 1, pageSize, languageCode: languageCode, sorting: new ProductSorting(orderBy, orderDirection.ToSortingDirection(), languageCode), filters: filters.ToArray()).ConfigureAwait(false);

            return products;
        }

        #region Commercetools API methods

        public async Task<PagedResults<Category>> GetCategoriesAsync(int? pageIndex = null, int? pageSize = null, string languageCode = null, CategorySorting sorting = null, params BaseFilter[] filters)
        {
            languageCode = languageCode ?? _options.DefaultLanguage;
            var response = await GetResponse<Models.Responses.Response<Models.Responses.Category>>("categories", pageIndex, pageSize, sorting, filters);
            var items = response.Results.Select(x => new Category(x, languageCode));
            return new PagedResults<Category>(items, pageIndex, pageSize, response.Total);
        }

        public async Task<PagedResults<Product>> GetProductsAsync(int? pageIndex = null, int? pageSize = null, string languageCode = null, ProductSorting sorting = null, params BaseFilter[] filters)
        {
            languageCode = languageCode ?? _options.DefaultLanguage;
            var response = await GetRawProductsAsync(pageIndex, pageSize, sorting, filters).ConfigureAwait(false);
            var items = response.Results.Select(x => new Product(x, languageCode));
            return new PagedResults<Product>(items, pageIndex, pageSize, response.Total);
        }

        /// <summary>
        /// Return simple deserialized API responses for products.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sorting"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        private async Task<Models.Responses.Response<Models.Responses.Product>> GetRawProductsAsync(int? pageIndex = null, int? pageSize = null, ProductSorting sorting = null, params BaseFilter[] filters) =>
            await GetResponse<Models.Responses.Response<Models.Responses.Product>>("products", pageIndex, pageSize, sorting, filters).ConfigureAwait(false);

        #endregion

        #region General API querying methods

        /// <summary>
        /// Create, send, receive, and deserialize a web response.
        /// </summary>
        /// <typeparam name="T">The type that the request result should be serialized as, from JSON.</typeparam>
        /// <param name="relativeEndpoint">The path of the API endpoint, relative to the base API endpoint.</param>
        /// <param name="pageIndex">The index of the current page. Will be omitted from web request if null.</param>
        /// <param name="pageSize">The size of the result pages. Will be omitted from web request if null</param>
        /// <param name="sorting">Sorting order of the results. Will use API default if null.</param>
        /// <param name="filters">Filter queries for the web request.</param>
        /// <returns></returns>
        private async Task<T> GetResponse<T>(string relativeEndpoint, int? pageIndex = null, int? pageSize = null, ISorting sorting = null, params BaseFilter[] filters)
        {
            List<(string key, object value)> queries = new List<(string, object)>
            {
                // Add sorting.
                ("sort", sorting?.Stringify() ?? string.Empty)
            };

            // Add filters.

            if (filters.Any(x => x != null))
            {
                queries.Add(("where", string.Join(" and ", filters.Select(x => x.Stringify()))));
            }

            // Add paging query parameters, if necessary.
            if (pageIndex.HasValue && pageIndex > 0)
            {
                queries.Add(("offset", pageIndex * pageSize));
            }
            if (pageSize.HasValue && pageSize > 0)
            {
                queries.Add(("limit", pageSize));
            }

            // Format query string.
            var queryString = FormatQueryString(queries
                .Where(x => !string.IsNullOrWhiteSpace(x.value?.ToString()))
                .ToList());

            // Make web request.
            var requestUrl = $"{_options.ApiUrl}/{_options.ProjectKey}/{relativeEndpoint}{queryString}";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            using (var client = await GetClient().ConfigureAwait(false))
            {
                var request = await client.SendAsync(requestMessage).ConfigureAwait(false);
                request.EnsureSuccessStatusCode();
                return DeserializeJson<T>(await request.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
        }

        private async Task<HttpClient> GetClient()
        {
            if (!AccessToken.IsNullOrWhiteSpace() && Expiration != null && Expiration > DateTime.UtcNow)
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                return client;
            }
            await Authenticate().ConfigureAwait(false);
            return await GetClient().ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a valid bearer token from the authentication API, as well as the token's expiration time.
        /// </summary>
        /// <returns></returns>
        private async Task Authenticate()
        {
            // Basic variables.
            var grantType = $"grant_type=client_credentials&scope={HttpUtility.UrlEncode(Scope)}";
            var credentialsBytes = Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}");
            var base64Credentials = Convert.ToBase64String(credentialsBytes);
            var mediaType = "application/x-www-form-urlencoded";

            // Apply basic authorization request header.
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);

            var request = new HttpRequestMessage(HttpMethod.Post, _options.OAuthUrl)
            {
                Content = new StringContent(grantType, Encoding.UTF8, mediaType)
            };

            // Get and deserialize authentication response.
            var response = await client.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var deserializedResponse = DeserializeJson<AuthenticationResponse>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            AccessToken = deserializedResponse.AccessToken;
            Expiration = DateTime.UtcNow.AddSeconds(deserializedResponse.ExpiresIn - 60); // Decrease expiration time by 60 seconds, to make up for any potential delays during execution time.
        }

        #endregion

        /// <summary>
        /// Format <paramref name="queries"/> as a URI query string.
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        private string FormatQueryString(List<(string key, object value)> queries)
        {
            var queryString = string.Empty;
            for (var i = 0; i < queries?.Count(); i++)
            {
                var querySeparator = i == 0 ? '?' : '&';
                var queryKey = queries.ElementAt(i).key;
                var queryValue = HttpUtility.UrlEncode(queries.ElementAt(i).value.ToString());

                if (!string.IsNullOrWhiteSpace(queryValue))
                {
                    queryString += $"{querySeparator}{queryKey}={queryValue}";
                }
            }
            return queryString;
        }

        /// <summary>
        /// Deserialize JSON strings as <typeparamref name="T"/>
        /// </summary>
        /// <remarks>Property names are parsed case insensitively.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private T DeserializeJson<T>(string jsonString)
        {
#if NETCOREAPP
            return JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
#else
            return JsonConvert.DeserializeObject<T>(jsonString);
#endif
        }
    }
}
