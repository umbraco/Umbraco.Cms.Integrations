﻿using Umbraco.Cms.Core.PropertyEditors;

namespace Umbraco.Cms.Integrations.Crm.Hubspot
{
    [DataEditor(alias: Constants.PropertyEditorAlias)]
    public class HubspotFormPickerPropertyEditor : DataEditor
    {
        public HubspotFormPickerPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory)
        {
        }
    }
}
