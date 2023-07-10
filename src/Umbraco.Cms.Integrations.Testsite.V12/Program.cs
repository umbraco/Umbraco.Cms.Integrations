namespace Umbraco.Cms.Integrations.Testsite.V12
{
    public class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args)
                .Build()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
#if DEBUG
                .ConfigureAppConfiguration(config
                    => config.AddJsonFile(
                        "appsettings.Local.json",
                        optional: true,
                        reloadOnChange: true))
#endif
                .ConfigureUmbracoDefaults()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStaticWebAssets();
                    webBuilder.UseStartup<Startup>();
                });
    }
}
