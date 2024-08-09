using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Crm.Dynamics.Editors
{
    [DataEditor(
        "Umbraco.Cms.Integrations.Crm.Dynamics.FormPicker",
        ValueEditorIsReusable = true)]
    public class DynamicsFormPickerPropertyEditor : DataEditor
    {
        public DynamicsFormPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory)
        {
        }
    }
}
