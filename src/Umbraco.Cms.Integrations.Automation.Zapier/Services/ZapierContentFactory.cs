using Umbraco.Cms.Integrations.Automation.Zapier.Services.Parsers;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public class ZapierContentFactory : IZapierContentFactory
    {
        public IZapierContentParser Create(string editorAlias)
        {
            switch (editorAlias)
            {
                case Core.Constants.PropertyEditors.Aliases.MediaPicker:
                case Core.Constants.PropertyEditors.Aliases.MediaPicker3:
                    return new MediaParser();
                case Core.Constants.PropertyEditors.Aliases.DropDownListFlexible:
                case Core.Constants.PropertyEditors.Aliases.CheckBoxList:
                    return new ItemsListParser();
                case Core.Constants.PropertyEditors.Aliases.MultiNodeTreePicker:
                    return new MultiNodeTreePickerParser();
                default: return new DefaultParser();
            }
        }
    }
}
