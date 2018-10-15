using ApplicationPrototype.Models;
using System;
using System.Collections.Generic;
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
            string documentName = docRepository.GenerateDocument(audit);
            driveRepository.UploadFile(documentName);
            return RedirectToAction("Index");
        }
    }
}
