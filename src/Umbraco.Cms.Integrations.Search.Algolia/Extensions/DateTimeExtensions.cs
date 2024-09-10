namespace Umbraco.Cms.Integrations.Search.Algolia.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            TimeSpan elapsedTime = dateTime - unixEpoch;
            
            return (long)elapsedTime.TotalSeconds;
        }
    }
}
