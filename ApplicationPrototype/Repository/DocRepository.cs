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
        string pathFiles = @"D:\WDocuments\";

        public async Task<List<Audit>> GetAudits()
        {
            List<Audit> audits;
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync("http://www.mocky.io/v2/5bc74ed73200002c000b07d3");
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

            #region Test Insert HTML code
            //section.AddParagraph().AppendHTML("<ol><li id='1'><b>Version 1</b></li><li>Version 2</li><li>Version 3</li></ol>");
            #endregion
            //----------------------- TITLE --------------------
            // Add new paragraph
            Paragraph pTitle = section.AddParagraph();
            // Add Title
            pTitle.AppendHTML("<b>" + audit.Title + "</b>");
            //pTitle.AppendText(audit.Title);
            pTitle.Format.HorizontalAlignment = HorizontalAlignment.Center;
            pTitle.Format.AfterSpacing = 10;

            //----------------------- ISSUES --------------------
            int i = 1;
            foreach (var issue in audit.Issues)
            {
                Paragraph pIssueTitle = section.AddParagraph();
                pIssueTitle.Format.BeforeSpacing = 10;
                pIssueTitle.AppendHTML("<b>ISSUE #" + i + "</b>");
                //pIssueTitle.AppendText("ISSUES:");
                i++;
                Paragraph issueTitle = section.AddParagraph();
                issueTitle.AppendHTML("<b>" + issue.Title + "</b>");
                //issueTitle.AppendText(issue.Title);
                Paragraph issueDescription = section.AddParagraph();
                issueDescription.AppendText(issue.Description);
                issueDescription.Format.AfterSpacing = 10;

                // --------------- RECOMMENDATIONS ----------------
                Paragraph pRecomTitle = section.AddParagraph();
                pRecomTitle.AppendHTML("<b>Recommendations:</b>");
                //pRecomTitle.AppendText("Recommendations:");
                foreach (var re in issue.Recommendations)
                {
                    Paragraph recomDescription = section.AddParagraph();
                    recomDescription.AppendText(re.Description);
                    recomDescription.ListFormat.ApplyBulletStyle();
                    recomDescription.ListFormat.CurrentListLevel.NumberPosition = -10;
                }
            }
            // Save Word Document
            string path = pathFiles + audit.Title + ".docx";
            document.SaveToFile(path, FileFormat.Docx2013);
            // Open Document
            Process.Start(path);

            return audit.Title;
        }

        //public Audit GetDataFromDoc()
        //{
        //    // Cargar un Documento
        //    Document document = new Document();
        //    document.LoadFromFile(@"D:\WDocuments\WordDocumentGenerated.docx");

        //    Audit audit = new Audit();
        //    List<Recommendation> recommendations = new List<Recommendation>();
        //    List<Issue> issues = new List<Issue>();

        //    var paragraphs = document.Sections[0].Paragraphs;
        //    int i = 1;

        //    audit.AuditId = 1;
        //    audit.Title = paragraphs[0].Text;

        //    if (paragraphs[i].Text == "RECOMENDATIONS:")
        //    {
        //        i++;
        //        while (paragraphs[i].Text != "ISSUES:")
        //        {
        //            Recommendation recom = new Recommendation();
        //            recom.RecommendationId = 1;
        //            recom.Title = "Title of Recomendation";
        //            recom.Description = paragraphs[i].Text;
        //            i++;

        //            recommendations.Add(recom);
        //        }
        //        i++;
        //        while (i < paragraphs.Count)
        //        {
        //            Issue issue = new Issue();
        //            issue.IssueId = 1;
        //            issue.Title = "Title of Issue";
        //            issue.Description = paragraphs[i].Text;
        //            i++;

        //            issues.Add(issue);
        //        }
        //    }

        //    audit.Recommendations = recommendations;
        //    audit.Issues = issues;

        //    return audit;
        //}
    }
}