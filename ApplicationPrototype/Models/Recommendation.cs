using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPrototype.Models
{
    public class Recommendation
    {
        public int RecommendationId { get; set; }
        public string Description { get; set; }
        public int IssueId { get; set; }

        public Recommendation()
        {
            RecommendationId = 0;
            Description = "";
            IssueId = 0;
        }
    }
}