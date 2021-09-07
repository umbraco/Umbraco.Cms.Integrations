using System.Runtime.Serialization;
#if NETCOREAPP
using Umbraco.Cms.Core.PropertyEditors;
#else
using Umbraco.Core.PropertyEditors;
#endif

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Editors
{
    public class CommerceToolsPickerConfiguration
    {
        public CommerceToolsPickerConfiguration()
        {
            // initialize defaults
            PageSize = 20;
            OrderBy = "Name";
            OrderDirection = "asc";
        }

        [ConfigurationField("entityType", "Entity Type", Constants.AppPluginFolderPath + "/PropertyEditors/EntityTypePicker.html",
            Description = "Select which entity type to pick")]
        public string EntityType { get; set; } = Models.EntityType.Product.ToString();

        [ConfigurationField("validationLimit", "Amount", "numberrange", Description = "Set a required range of items selected")]
        public NumberRange ValidationLimit { get; set; } = new NumberRange();

        [ConfigurationField("pageSize", "Page Size", "number", Description = "Number of items per page in picker")]
        public int PageSize { get; set; }

        [ConfigurationField("orderBy", "Order By", Constants.AppPluginFolderPath + "/PropertyEditors/OrderByPicker.html",
            Description = "The default sort order for the list in the picker")]
        public string OrderBy { get; set; }

        [ConfigurationField("orderDirection", "Order Direction", "views/propertyeditors/listview/orderdirection.prevalues.html")]
        public string OrderDirection { get; set; }

        [DataContract]
        public class NumberRange
        {
            [DataMember(Name = "min")]
            public int? Min { get; set; }

            [DataMember(Name = "max")]
            public int? Max { get; set; }
        }
    }
}
