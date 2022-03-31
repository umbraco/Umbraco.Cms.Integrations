using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public class ZapierService
    {
        // Using a static HttpClient (see: https://www.aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
        private readonly static HttpClient s_client = new HttpClient();

        // Access to the client within the class is via ClientFactory(), allowing us to mock the responses in tests.
        public static Func<HttpClient> ClientFactory = () => s_client;

        public async Task<string> TriggerAsync(string requestUri, Dictionary<string, string> content)
        {
            try
            {
                var result = await ClientFactory().PostAsync(requestUri, new FormUrlEncodedContent(content));

                return result.IsSuccessStatusCode ? string.Empty : result.ReasonPhrase;
            }
            catch (HttpRequestException)
            {
                return "Could not access the requested URL: " + requestUri;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
