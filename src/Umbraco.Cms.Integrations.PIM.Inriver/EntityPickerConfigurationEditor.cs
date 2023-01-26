using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace Umbraco.Cms.Integrations.PIM.Inriver
{
    public class EntityPickerConfigurationEditor : ConfigurationEditor<EntityPickerConfiguration>
    {
        public EntityPickerConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) 
            : base(ioHelper, editorConfigurationParser)
        {
        }
    }
}
