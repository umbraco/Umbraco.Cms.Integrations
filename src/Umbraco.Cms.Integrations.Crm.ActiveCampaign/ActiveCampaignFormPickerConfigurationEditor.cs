using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign.Core
{
    public class ActiveCampaignFormPickerConfigurationEditor : ConfigurationEditor<ActiveCampaignFormPickerConfiguration>
    {
        public ActiveCampaignFormPickerConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser)
            : base(ioHelper, editorConfigurationParser)
        {

        }
    }
}
