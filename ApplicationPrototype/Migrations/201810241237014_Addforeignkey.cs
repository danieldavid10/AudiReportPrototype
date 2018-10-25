namespace ApplicationPrototype.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addforeignkey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Issues", "Audit_AuditId", "dbo.Audits");
            DropForeignKey("dbo.Recommendations", "Issue_IssueId", "dbo.Issues");
            DropIndex("dbo.Issues", new[] { "Audit_AuditId" });
            DropIndex("dbo.Recommendations", new[] { "Issue_IssueId" });
            RenameColumn(table: "dbo.Issues", name: "Audit_AuditId", newName: "AuditId");
            RenameColumn(table: "dbo.Recommendations", name: "Issue_IssueId", newName: "IssueId");
            AlterColumn("dbo.Issues", "AuditId", c => c.Int(nullable: false));
            AlterColumn("dbo.Recommendations", "IssueId", c => c.Int(nullable: false));
            CreateIndex("dbo.Issues", "AuditId");
            CreateIndex("dbo.Recommendations", "IssueId");
            AddForeignKey("dbo.Issues", "AuditId", "dbo.Audits", "AuditId", cascadeDelete: true);
            AddForeignKey("dbo.Recommendations", "IssueId", "dbo.Issues", "IssueId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Recommendations", "IssueId", "dbo.Issues");
            DropForeignKey("dbo.Issues", "AuditId", "dbo.Audits");
            DropIndex("dbo.Recommendations", new[] { "IssueId" });
            DropIndex("dbo.Issues", new[] { "AuditId" });
            AlterColumn("dbo.Recommendations", "IssueId", c => c.Int());
            AlterColumn("dbo.Issues", "AuditId", c => c.Int());
            RenameColumn(table: "dbo.Recommendations", name: "IssueId", newName: "Issue_IssueId");
            RenameColumn(table: "dbo.Issues", name: "AuditId", newName: "Audit_AuditId");
            CreateIndex("dbo.Recommendations", "Issue_IssueId");
            CreateIndex("dbo.Issues", "Audit_AuditId");
            AddForeignKey("dbo.Recommendations", "Issue_IssueId", "dbo.Issues", "IssueId");
            AddForeignKey("dbo.Issues", "Audit_AuditId", "dbo.Audits", "AuditId");
        }
    }
}
