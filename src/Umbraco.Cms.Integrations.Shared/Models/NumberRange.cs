﻿using System.Runtime.Serialization;

namespace Umbraco.Cms.Integrations.Shared.Models
{
    [DataContract]
    public class NumberRange
    {
        [DataMember(Name = "min")]
        public int? Min { get; set; }

        [DataMember(Name = "max")]
        public int? Max { get; set; }
    }
}
