using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPrototype.Models
{
    public class Issue
    {
        public int IssueId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AuditId { get; set; }

        public List<Recommendation> Recommendations { get; set; }

        public Issue()
        {
            IssueId = 0;
            Title = "";
            Description = "";
            AuditId = 0;
            Recommendations = new List<Recommendation>();
        }
    }
}