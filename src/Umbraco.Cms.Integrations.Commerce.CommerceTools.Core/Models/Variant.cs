using System.Collections.Generic;
using System.Runtime.Serialization;
using Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Responses;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models
{
    [DataContract]
    public class Variant : BaseModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "sku")]
        public string SKU { get; set; }

        [DataMember(Name = "inStock")]
        public bool InStock { get; set; }

        [DataMember(Name = "availableQuantity")]
        public long AvailableQuantity { get; set; }

        [DataMember(Name = "images")]
        public IEnumerable<Responses.Image> Images { get; }

        /// <remarks>Represents the "cent amount".</remarks>
        [DataMember(Name = "prices")]
        public IEnumerable<Price> Prices { get; set; }

        internal Variant(Responses.Variant variant, string languageCode)
        {
            Id = variant.Id;
            Key = variant.Key;
            SKU = variant.SKU;
            InStock = variant.Availability?.IsOnStock == true;
            AvailableQuantity = variant.Availability?.AvailableQuantity ?? 0;
            Images = variant.Images;
            Prices = variant.Prices;
        }
    }
}
