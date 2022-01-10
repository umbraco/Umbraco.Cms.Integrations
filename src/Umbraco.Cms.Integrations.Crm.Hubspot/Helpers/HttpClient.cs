namespace Umbraco.Cms.Integrations.Crm.Hubspot.Helpers
{
    public static class Http
    {
        private static System.Net.Http.HttpClient _httpClient;

        public static System.Net.Http.HttpClient GetHttpClient()
        {
            return _httpClient ?? (_httpClient = new System.Net.Http.HttpClient());
        }
    }
}
