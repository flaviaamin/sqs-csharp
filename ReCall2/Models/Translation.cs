using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ReCall2.Models
{
    public class Translation
    {
        [JsonPropertyName("en-us")]
        public Country EnUs { get; set; }
    }

}
