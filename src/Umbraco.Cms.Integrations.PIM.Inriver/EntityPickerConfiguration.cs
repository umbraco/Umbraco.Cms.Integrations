using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Integrations.PIM.Inriver.Configuration;

namespace Umbraco.Cms.Integrations.PIM.Inriver
{
    public class EntityPickerConfiguration
    {
        [ConfigurationField(
            key: "configuration",
            name: "Configuration",
            view: "~/App_Plugins/UmbracoCms.Integrations/PIM/Inriver/views/configuration.html",
            Description = "Entity configuration.")]
        public EditorSettings Configuration { get; set; }

        public EntityPickerConfiguration()
        {
            Configuration = new EditorSettings();
        }
    }
}
