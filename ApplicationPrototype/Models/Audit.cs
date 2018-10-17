using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPrototype.Models
{
    public class Audit
    {
        [JsonProperty("auditId")]
        public int AuditId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("issues")]
        public List<Issue> Issues { get; set; }
    }
}