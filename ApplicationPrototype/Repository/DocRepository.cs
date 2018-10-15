using Newtonsoft.Json;
using Spire.Doc;
using Spire.Doc.Documents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ApplicationPrototype.Models
{
    public class DocRepository
    {
        string pathFiles = HttpContext.Current.Server.MapPath("~/Files/");

        public async Task<List<Audit>> GetAudits()
        {
            List<Audit> audits;
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync("http://www.mocky.io/v2/5bc0fe85320000700021abcb");
                audits = JsonConvert.DeserializeObject<List<Audit>>(response);
            }
            return audits;
        }

        public string GenerateDocument(Audit audit)
        {
            // New Document
            Document document = new Document();
            // Add Section
            Section section = document.AddSection();

            //----------------------- TITLE --------------------
            // Add new paragraph
            Paragraph pTitle = section.AddParagraph();
            // Add Title
            pTitle.AppendText(audit.Title);
            pTitle.Format.HorizontalAlignment = HorizontalAlignment.Center;

            //-------------------- RECOMMENDATIONS --------------  
            string titleRecommendations = "RECOMENDATIONS:";
            generateParagraphList(section, titleRecommendations, audit, true);

            //----------------------- ISSUES --------------------
            string titleIssues = "ISSUES:";
            generateParagraphList(section, titleIssues, audit, false); // optimizate
            // Save Word Document
            string path = pathFiles + audit.Title + ".docx";
            document.SaveToFile(path, FileFormat.Docx2013);
            // Open Document
            //Process.Start(path);

            return audit.Title;
        }

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

        private static void generateParagraphList(Section section, string title, Audit audit, bool action)
        {
            Paragraph paragraph = section.AddParagraph();
            paragraph.AppendText(title);
            if (action)
            {
                foreach (var re in audit.Recommendations)
                {
                    // Add new paragraphs
                    Paragraph p = section.AddParagraph();
                    p.AppendText(re.Title + ": " + re.Description);
                    p.ListFormat.ApplyBulletStyle();
                    p.ListFormat.CurrentListLevel.NumberPosition = -10;
                }
            }
            else
            {
                foreach (var re in audit.Issues)
                {
                    // Add new paragraphs
                    Paragraph p = section.AddParagraph();
                    p.AppendText(re.Title + ": " + re.Description);
                    p.ListFormat.ApplyBulletStyle();
                    p.ListFormat.CurrentListLevel.NumberPosition = -10;
                }
            }
        }
    }
}