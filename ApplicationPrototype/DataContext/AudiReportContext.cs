using ApplicationPrototype.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ApplicationPrototype.DataContext
{
    public class AudiReportContext : DbContext
    {
        public AudiReportContext()
            : base("name=DefaultConnection")
        {

        }

        public DbSet<Recommendation> Recommendations { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Audit> Audits { get; set; }

    }
}