using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Crm.ActiveCampaign
{
    [DataEditor(alias: Constants.PropertyEditorAlias)]
    public class ActiveCampaignFormPickerPropertyEditor : DataEditor
    {
        public ActiveCampaignFormPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory)
        {
        }
    }
}
