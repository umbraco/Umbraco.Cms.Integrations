#if NETCOREAPP
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
#else
using Umbraco.Core.PropertyEditors;
#endif

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Editors
{
    public class CommerceToolsPickerConfigurationEditor : ConfigurationEditor<CommerceToolsPickerConfiguration>
    {
#if NETCOREAPP
        public CommerceToolsPickerConfigurationEditor(IIOHelper ioHelper) : base(ioHelper)
        {

        }
#endif
    }
}
