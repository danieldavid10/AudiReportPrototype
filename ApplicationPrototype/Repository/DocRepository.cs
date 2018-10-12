using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ApplicationPrototype.Models
{
    public class DocRepository
    {
        public Audit GetDataFromDoc()
        {
            // Cargar un Documento
            Document document = new Document();
            document.LoadFromFile(@"D:\WDocuments\WordDocumentGenerated.docx");

            Audit audit = new Audit();
            List<Recommendation> recommendations = new List<Recommendation>();
            List<Issue> issues = new List<Issue>();

            var paragraphs = document.Sections[0].Paragraphs;
            int i = 1;

            audit.AuditId = 1;
            audit.Title = paragraphs[0].Text;

            if (paragraphs[i].Text == "RECOMENDATIONS:")
            {
                i++;
                while (paragraphs[i].Text != "ISSUES:")
                {
                    Recommendation recom = new Recommendation();
                    recom.RecommendationId = 1;
                    recom.Title = "Title of Recomendation";
                    recom.Description = paragraphs[i].Text;
                    i++;

                    recommendations.Add(recom);
                }
                i++;
                while (i < paragraphs.Count)
                {
                    Issue issue = new Issue();
                    issue.IssueId = 1;
                    issue.Title = "Title of Issue";
                    issue.Description = paragraphs[i].Text;
                    i++;

                    issues.Add(issue);
                }
            }

            audit.Recommendations = recommendations;
            audit.Issues = issues;

            return audit;
        }

    }
}