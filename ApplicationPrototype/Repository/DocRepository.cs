using ApplicationPrototype.DataContext;
using ApplicationPrototype.Repository;
using Newtonsoft.Json;
using Spire.Doc;
using Spire.Doc.Collections;
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
        AuditRepository auditRepository = new AuditRepository();

        public string GenerateDocument(Audit audit)
        {
            // New Document
            Document document = new Document();

            document.LoadFromFile(HttpContext.Current.Server.MapPath("~/Files/DocumentTemplate.docx"));
            // Add Section
            Section section = document.Sections[0];

            //----------------------- TITLE --------------------
            Paragraph pTitle = section.Paragraphs[0];
            // Add Title
            pTitle.AppendText(audit.Title);
            pTitle.ApplyStyle(BuiltinStyle.Title);
            pTitle.Format.HorizontalAlignment = HorizontalAlignment.Center;

            //----------------------- ISSUES --------------------
            int i = 1;
            foreach (var issue in audit.Issues)
            {
                Paragraph pIssueTitle = section.AddParagraph();
                pIssueTitle.AppendText("ISSUE " + i);
                pIssueTitle.ApplyStyle(BuiltinStyle.Heading1);
                i++;

                Paragraph issueTitle = section.AddParagraph();
                issueTitle.AppendText(issue.Title);
                issueTitle.ApplyStyle(BuiltinStyle.Heading2);

                Paragraph issueDescription = section.AddParagraph();
                issueDescription.AppendText(issue.Description);

                // --------------- RECOMMENDATIONS ----------------
                Paragraph pRecomTitle = section.AddParagraph();
                pRecomTitle.AppendText("Recommendations:");
                pRecomTitle.ApplyStyle(BuiltinStyle.Heading3);
                foreach (var re in issue.Recommendations)
                {
                    Paragraph recomDescription = section.AddParagraph();
                    recomDescription.AppendText(re.Description);
                    recomDescription.ListFormat.ApplyBulletStyle();
                }
            }
            // Save Word Document
            string path = HttpContext.Current.Server.MapPath("~/Files/" + audit.Title + ".docx");
            document.SaveToFile(path, FileFormat.Docx2013);
            // Open Document
            //Process.Start(path);

            return audit.Title;
        }

        public string UpdateFileChanges(Audit audit, string docId)
        {
            Document document = new Document();
            string FilePath = HttpContext.Current.Server.MapPath("~/Files/");
            string message = "";
            document.LoadFromFile(FilePath + audit.Title + ".docx");
            RemoveEmptyParagraphs(document.Sections);
            var paragraphs = document.Sections[0].Paragraphs;
            bool repeat;
            int i, fI;
            try
            {
                do
                {
                    repeat = false;
                    i = 0; // paragraph pointer

                    if (paragraphs[i].StyleName == "Title")
                    {
                        // Update Title
                        audit.Title = paragraphs[i].Text;
                        i++;
                    }
                    fI = 0;
                    foreach (var issue in audit.Issues)
                    {
                        do
                        {
                            i++;
                            //---------- ISSUES ----------------
                            if (i < paragraphs.Count && paragraphs[i].StyleName == "Heading2")
                            {
                                string txt = "";
                                if (fI + 1 < audit.Issues.Count)
                                {
                                    txt = audit.Issues[fI + 1].Title;
                                }

                                if (issue.Title == paragraphs[i].Text || paragraphs[i].Text != txt)
                                {
                                    // Update Issue
                                    issue.Title = paragraphs[i].Text;
                                    i++;
                                    string description = "";
                                    do
                                    {
                                        description += paragraphs[i].Text + "\n";
                                        i++;
                                    } while (i < paragraphs.Count && paragraphs[i].StyleName == "Normal");
                                    description = description.TrimEnd('\n');
                                    issue.Description = description;
                                }
                                else if (paragraphs[i].Text == audit.Issues[fI + 1].Title)
                                {
                                    // Delete Issue
                                    issue.Title = "{delete}";
                                    issue.Recommendations = null;
                                    i--;
                                    break;
                                }
                            }
                            //---------- RECOMMENDATIONS ----------------
                            if (i < paragraphs.Count && paragraphs[i].StyleName == "Heading3")
                            {
                                i++;
                                if (issue.Recommendations != null)
                                {
                                    // Update Recommendation
                                    foreach (var recom in issue.Recommendations)
                                    {
                                        if (i < paragraphs.Count && paragraphs[i].StyleName == "Normal")
                                        {
                                            recom.Description = paragraphs[i].Text;
                                            i++;
                                        }
                                        else
                                        {
                                            recom.Description = "{delete}";
                                        }
                                    }
                                }
                                while (i < paragraphs.Count && paragraphs[i].StyleName != "Heading1")
                                {
                                    // Insert Recommendation
                                    Recommendation recommendation = new Recommendation()
                                    {
                                        RecommendationId = 0,
                                        IssueId = issue.IssueId,
                                        Description = paragraphs[i].Text
                                    };
                                    issue.Recommendations.Add(recommendation);
                                    i++;
                                }
                            }

                        } while (i < paragraphs.Count && paragraphs[i].StyleName != "Heading1");

                        fI++;
                    }

                    if (i < paragraphs.Count && paragraphs[i].StyleName == "Heading1")
                    {
                        i++;

                        if (i < paragraphs.Count && paragraphs[i].StyleName == "Heading2")
                        {
                            // Insert Issue
                            Issue issue = new Issue();
                            issue.AuditId = audit.AuditId;
                            issue.IssueId = 0;
                            issue.Title = paragraphs[i].Text;
                            i++;
                            string description = "";
                            while (i < paragraphs.Count && paragraphs[i].StyleName == "Normal")
                            {
                                description += paragraphs[i].Text + "\n";
                                i++;
                            }
                            description = description.TrimEnd('\n');
                            issue.Description = description;
                            issue.Recommendations = new List<Recommendation>();
                            audit.Issues.Add(issue);

                            auditRepository.UpdateAudit(audit);
                            DriveRepository driveRepository = new DriveRepository();
                            driveRepository.DownloadGoogleDoc(docId);
                            repeat = true;
                        }
                    }

                } while (repeat);

                message = "Success";
            }
            catch (Exception)
            {
                message = "Error";
                //throw;
            }
            if (message != "Error")
            {
                auditRepository.UpdateAudit(audit);
            }
            return message;
        }

        private void RemoveEmptyParagraphs(SectionCollection sections)
        {
            foreach (Section section in sections)
            {
                for (int i = 0; i < section.Body.ChildObjects.Count; i++)
                {
                    if (section.Body.ChildObjects[i].DocumentObjectType == DocumentObjectType.Paragraph)
                    {
                        if (String.IsNullOrEmpty((section.Body.ChildObjects[i] as Paragraph).Text.Trim()))
                        {
                            section.Body.ChildObjects.Remove(section.Body.ChildObjects[i]);
                            i--;
                        }
                    }
                }
            }
        }
    }
}