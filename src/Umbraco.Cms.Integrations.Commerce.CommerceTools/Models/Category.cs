using System;
using System.Runtime.Serialization;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models
{
    [DataContract]
    public class Category : BaseModel
    {
        [DataMember(Name = "id")]
        public Guid Id { get; }

        [DataMember(Name = "key")]
        public string Key { get; }

        [DataMember(Name = "orderHint")]
        public string OrderHint { get; }

        [DataMember(Name = "name")]
        public string Name { get; }

        [DataMember(Name = "description")]
        public string Description { get; }

        [DataMember(Name = "slug")]
        public string Slug { get; }

        internal Category(Responses.Category category, string languageCode)
        {
            Id = category.Id;
            Key = category.Key;
            OrderHint = category.OrderHint;
            Name = SafelyGetDictionaryValue(category.Name, languageCode);
            Description = SafelyGetDictionaryValue(category.Description, languageCode);
            Slug = SafelyGetDictionaryValue(category.Slug, languageCode);
        }
    }
}
