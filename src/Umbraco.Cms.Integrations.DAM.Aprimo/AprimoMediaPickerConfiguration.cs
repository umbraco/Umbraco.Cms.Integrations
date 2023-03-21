using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Integrations.DAM.Aprimo.Configuration;

namespace Umbraco.Cms.Integrations.DAM.Aprimo
{
    public class AprimoMediaPickerConfiguration
    {
        [ConfigurationField(
            key: "configuration",
            name: "Configuration",
            view: "~/App_Plugins/UmbracoCms.Integrations/DAM/Aprimo/views/configuration.html",
            Description = "Media Picker configuration.")]
        public EditorConfiguration Configuration { get; set; }

        public AprimoMediaPickerConfiguration()
        {
            Configuration= new EditorConfiguration();
        }
    }
}
