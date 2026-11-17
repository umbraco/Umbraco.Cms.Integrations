using Umbraco.Cms.Integrations.Automation.Zapier.Services.Parsers;

namespace Umbraco.Cms.Integrations.Automation.Zapier.Services
{
    public class ZapierContentFactory : IZapierContentFactory
    {
        public IZapierContentParser Create(string editorAlias)
        {
            switch (editorAlias)
            {
                case Core.Constants.PropertyEditors.Aliases.MediaPicker3:
                case Core.Constants.PropertyEditors.Aliases.SingleMediaPicker:
                    return new MediaParser();
                case Core.Constants.PropertyEditors.Aliases.MultipleDropDown:
                case Core.Constants.PropertyEditors.Aliases.SingleDropDown:
                case Core.Constants.PropertyEditors.Aliases.CheckBoxList:
                    return new ItemsListParser();
                case Core.Constants.PropertyEditors.Aliases.MultiNodeTreePicker:
                    return new MultiNodeTreePickerParser();
                default: return new DefaultParser();
            }
        }
    }
}
