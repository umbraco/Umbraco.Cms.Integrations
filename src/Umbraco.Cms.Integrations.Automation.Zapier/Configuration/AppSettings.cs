namespace Umbraco.Cms.Integrations.Automation.Zapier.Configuration
{
    public abstract class AppSettings
    {
        public string UserGroupAlias { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;
    }

    public class ZapierSettings : AppSettings
    {
    }

    public class ZapierFormsSettings : ZapierSettings
    {
    }
}
