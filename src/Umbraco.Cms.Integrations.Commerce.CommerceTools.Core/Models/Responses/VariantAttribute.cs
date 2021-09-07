using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Responses
{
    /// <remarks>Not related to <see cref="Attribute"/>.</remarks>
    public class VariantAttribute
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "value")]
        public Dictionary<string, string> Value { get; set; }

        [DataMember(Name = "key")]
        public string Key => Value["key"];

        [DataMember(Name = "label")]
        public string Label => Value["label"];
    }
}
