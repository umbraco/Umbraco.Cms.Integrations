using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Responses
{
    [DataContract]
    public class Price
    {
        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "value")]
        public Dictionary<string, object> Value { get; set; }

        [DataMember(Name = "centAmount")]
        public long CentAmount => long.TryParse(Value["centAmount"]?.ToString(), out var value) ? value : -1;

        [DataMember(Name = "currencyCode")]
        public string CurrencyCode => Value["currencyCode"]?.ToString();

        [DataMember(Name = "fractionDigits")]
        public int FractionDigits => int.TryParse(Value["fractionDigits"]?.ToString(), out var value) ? value : -1;

        [DataMember(Name = "type")]
        public string Type => Value["type"].ToString();
    }
}
