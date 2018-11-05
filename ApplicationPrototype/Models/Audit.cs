using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApplicationPrototype.Models
{
    public class Audit
    {
        public int AuditId { get; set; }
        public string Title { get; set; }
        public string GoogleDocId { get; set; }
        public List<Issue> Issues { get; set; }

        public Audit()
        {
            AuditId = 0;
            Title = "";
            GoogleDocId = "";
            Issues = new List<Issue>();
        }
    }
}