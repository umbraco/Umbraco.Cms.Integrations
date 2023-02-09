using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.DAM.Aprimo
{
    public class AprimoMediaPickerConfiguration
    {
        [ConfigurationField(
            key: "configuration",
            name: "Configuration",
            view: "~/App_Plugins/UmbracoCms.Integrations/DAM/Aprimo/views/configuration.html",
            Description = "Media Picker configuration.")]
        public string Configuration { get; set; }
    }
}
