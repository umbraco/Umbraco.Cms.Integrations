using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Crm.Dynamics
{
    [DataEditor(alias: Constants.PropertyEditorAlias)]
    public class DynamicsFormPickerPropertyEditor : DataEditor
    {
        public DynamicsFormPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory)
        {
        }
    }
}
