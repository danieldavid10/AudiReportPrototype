using ApplicationPrototype.DataContext;
using ApplicationPrototype.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ApplicationPrototype.Repository
{
    public class AuditRepository
    {
        private AudiReportContext Context;

        public async Task<IEnumerable<Audit>> GetAudits()
        {
            using (Context = new AudiReportContext())
            {
                var audits = await Context.Audits.Include("Issues").Include("Issues.Recommendations").ToListAsync();
                return audits;
            }
        }

        public void UpdateAudit(Audit audit)
        {
            Audit model = FindAuditById(audit.AuditId);
            using (Context = new AudiReportContext())
            {
                model.Title = audit.Title;
                Context.Entry(model).State = EntityState.Modified;
                Context.SaveChanges();
                // Update or add Issues
                AddOrUpdateIssue(model.AuditId, audit, Context);
                // Update or add Recommendations

                foreach (Issue issue in audit.Issues)
                {
                    if (issue.Recommendations != null)
                    {
                        AddOrUpdateRecommendation(issue.Recommendations, issue.IssueId, Context);
                    }
                }
            }
        }

        private void AddOrUpdateRecommendation(List<Recommendation> recomendations, int issueId, AudiReportContext context)
        {
            foreach (var recom in recomendations)
            {
                recom.IssueId = issueId;
                if (recom.RecommendationId == 0)
                {
                    CreateRecommendation(recom, context);
                }
                else
                {
                    UpdateRecommendation(recom, context);
                }
            }
        }

        private void AddOrUpdateIssue(int auditId, Audit audit, AudiReportContext context)
        {
            foreach (Issue issue in audit.Issues)
            {
                issue.AuditId = auditId;
                if (issue.IssueId == 0)
                {
                    CreateIssue(issue, context);
                }
                else if (issue.Title != "{delete}")
                {
                    UpdateIssue(issue, context);
                }
                else
                {
                    DeleteIssue(issue.IssueId,context);
                }
            }
        }

        private void DeleteIssue(int issueId, AudiReportContext context)
        {
            var issue = context.Issues.Where(x => x.IssueId == issueId).FirstOrDefault();
            context.Issues.Attach(issue);
            context.Issues.Remove(issue);
            context.SaveChanges();
        }

        private void CreateIssue(Issue issue, AudiReportContext context)
        {
            context.Issues.Add(issue);
            context.SaveChanges();
        }

        private void UpdateIssue(Issue issue, AudiReportContext context)
        {
            // Find Issue by Id
            Issue issueModel = context.Issues.Include("Recommendations").Where(i => i.IssueId == issue.IssueId).FirstOrDefaultAsync().Result;
            // Update Data
            issueModel.Title = issue.Title;
            issueModel.Description = issue.Description;
            Context.Entry(issueModel).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public Audit FindAuditById(int id)
        {
            using (Context = new AudiReportContext())
            {
                var result = Context.Audits.Include("Issues").Include("Issues.Recommendations").Where(a => a.AuditId == id).FirstOrDefaultAsync();
                return result.Result;
            }
        }

        private void CreateRecommendation(Recommendation recommendation, AudiReportContext context)
        {
            context.Recommendations.Add(recommendation);
            context.SaveChanges();
        }

        private void UpdateRecommendation(Recommendation recommendation, AudiReportContext context)
        {
            // Find Issue by Id
            Recommendation RecommendationModel = context.Recommendations.Where(r => r.RecommendationId == recommendation.RecommendationId).FirstOrDefaultAsync().Result;
            // Update Data
            RecommendationModel.Description = recommendation.Description;
            Context.Entry(RecommendationModel).State = EntityState.Modified;
            Context.SaveChanges();
        }
    }
}