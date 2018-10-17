using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPrototype.Models
{
    public class Recommendation
    {
        [JsonProperty("recomendationId")]
        public int RecommendationId { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}