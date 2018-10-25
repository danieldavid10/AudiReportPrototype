using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPrototype.Models
{
    public class Issue
    {
        [JsonProperty("issueId")]
        public int IssueId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        public int AuditId { get; set; }

        [JsonProperty("recommendations")]
        public List<Recommendation> Recommendations { get; set; }
    }
}