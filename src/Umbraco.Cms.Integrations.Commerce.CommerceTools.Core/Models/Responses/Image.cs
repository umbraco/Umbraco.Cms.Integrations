using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Umbraco.Cms.Integrations.Commerce.CommerceTools.Core.Models.Responses
{
    [DataContract]
    public class Image
    {
        [DataMember(Name = "dimensions")]
        public Dictionary<string, int> Dimensions { get; set; }

        [DataMember(Name = "width")]
        public int Width => Dimensions["w"];

        [DataMember(Name = "height")]
        public int Height => Dimensions["h"];

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
