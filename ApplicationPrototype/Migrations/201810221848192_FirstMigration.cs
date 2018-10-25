namespace ApplicationPrototype.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Audits",
                c => new
                    {
                        AuditId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.AuditId);
            
            CreateTable(
                "dbo.Issues",
                c => new
                    {
                        IssueId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Audit_AuditId = c.Int(),
                    })
                .PrimaryKey(t => t.IssueId)
                .ForeignKey("dbo.Audits", t => t.Audit_AuditId)
                .Index(t => t.Audit_AuditId);
            
            CreateTable(
                "dbo.Recommendations",
                c => new
                    {
                        RecommendationId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Issue_IssueId = c.Int(),
                    })
                .PrimaryKey(t => t.RecommendationId)
                .ForeignKey("dbo.Issues", t => t.Issue_IssueId)
                .Index(t => t.Issue_IssueId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Issues", "Audit_AuditId", "dbo.Audits");
            DropForeignKey("dbo.Recommendations", "Issue_IssueId", "dbo.Issues");
            DropIndex("dbo.Recommendations", new[] { "Issue_IssueId" });
            DropIndex("dbo.Issues", new[] { "Audit_AuditId" });
            DropTable("dbo.Recommendations");
            DropTable("dbo.Issues");
            DropTable("dbo.Audits");
        }
    }
}
