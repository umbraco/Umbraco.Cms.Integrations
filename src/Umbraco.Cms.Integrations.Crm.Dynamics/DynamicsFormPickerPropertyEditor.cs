//#if NETCOREAPP
//using Umbraco.Cms.Core.IO;
//using Umbraco.Cms.Core.PropertyEditors;
//#else
//using Umbraco.Core.Logging;
//using Umbraco.Core.PropertyEditors;
//#endif

//namespace Umbraco.Cms.Integrations.Crm.Dynamics
//{
//    [DataEditor(
//        alias: "Umbraco.Cms.Integrations.Crm.Dynamics.FormPicker",
//        name: "Dynamics Form Picker",
//        view: "~/App_Plugins/UmbracoCms.Integrations/Crm/Dynamics/views/formpicker.html",
//        Group = "Pickers",
//        Icon = "icon-handshake", 
//        ValueType = ValueTypes.Json
//        )]
//    public class DynamicsFormPickerPropertyEditor : DataEditor
//    {
//#if NETCOREAPP
//        private readonly IIOHelper _iioHelper;

//        public DynamicsFormPickerPropertyEditor(IIOHelper iioHelper, IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory)
//        {
//            _iioHelper = iioHelper;
//        }
//#else
//        public DynamicsFormPickerPropertyEditor(ILogger logger) : base(logger)
//        {
//        }


//#endif

//        protected override IConfigurationEditor CreateConfigurationEditor()
//        {
//#if NETCOREAPP
//            return new DynamicsFormPickerConfigurationEditor(_iioHelper);
//#else
//            return new DynamicsFormPickerConfigurationEditor();
//#endif
//        }
//    }
//}
