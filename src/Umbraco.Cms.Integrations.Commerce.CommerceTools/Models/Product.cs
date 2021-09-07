using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Models
{
    [DataContract]
    public class Product : BaseModel
    {
        [DataMember(Name = "id")]
        public Guid Id { get; }

        [DataMember(Name = "key")]
        public string Key { get; }

        [DataMember(Name = "name")]
        public string Name { get; }

        [DataMember(Name = "slug")]
        public string Slug { get; }

        [DataMember(Name = "metaTitle")]
        public string MetaTitle { get; }

        [DataMember(Name = "metaDescription")]
        public string MetaDescription { get; }

        [DataMember(Name = "images")]
        public IEnumerable<Responses.Image> Images { get; }

        [DataMember(Name = "variants")]
        public IEnumerable<Variant> Variants { get; private set; }

        internal Product(Responses.Product product, string languageCode)
        {
            Id = product.Id;
            Key = product.Key;
            Name = SafelyGetDictionaryValue(product.MasterData.Current.Name, languageCode);
            Slug = SafelyGetDictionaryValue(product.MasterData.Current.Slug, languageCode);
            MetaTitle = SafelyGetDictionaryValue(product.MasterData.Current.MetaTitle, languageCode);
            MetaDescription = SafelyGetDictionaryValue(product.MasterData.Current.MetaDescription, languageCode);
            Images = product.MasterData.Current.MasterVariant?.Images;
            Variants = product.MasterData.Current.Variants.Prepend(product.MasterData.Current.MasterVariant).Select(x => new Variant(x, languageCode));
        }
    }
}
