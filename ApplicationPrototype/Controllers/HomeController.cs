using ApplicationPrototype.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApplicationPrototype.Controllers
{
    public class HomeController : Controller
    {
        DriveRepository driveRepository = new DriveRepository();
        DocRepository docRepository = new DocRepository();

        // GET: Home
        public ActionResult Index()
        {
            return View(driveRepository.LoadAudits());
        }

        [HttpGet]
        public ActionResult EditDocumentInGoogleDocs(int id)
        {
            Audit audit = driveRepository.FindById(id);
            string documentName = docRepository.GenerateDocument(audit); // Word Dcument
            string documentId = driveRepository.FileUpload(documentName); // Google Docs Document
            Process.Start("https://docs.google.com/document/d/" + documentId + "/edit"); // Open Document
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult UpdateFileData(int id)
        {
            Audit audit = driveRepository.FindById(id);
            string documentId = driveRepository.GetGoogleDocument(audit.Title + ".docx"); // Google Docs
            driveRepository.DownloadGoogleDoc(documentId);
            docRepository.UpdateFileChanges(audit);
            return RedirectToAction("Index");
        }
    }
}
