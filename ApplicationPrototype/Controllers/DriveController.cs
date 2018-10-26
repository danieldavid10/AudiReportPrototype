using ApplicationPrototype.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ApplicationPrototype.Controllers
{
    public class DriveController : Controller
    {
        DriveRepository driveRepository = new DriveRepository();

        // GET: Drive
        public ActionResult Index()
        {
            var files = driveRepository.getFilesInFolder();
            return View(files);
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            driveRepository.FileUpload(file);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public void DownloadFile(string id)
        {
            string FilePath = driveRepository.DownloadGoogleFile(id);

            Response.ContentType = "application/zip";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
            Response.WriteFile(System.Web.HttpContext.Current.Server.MapPath("~/GoogleDriveFiles/" + Path.GetFileName(FilePath)));
            Response.End();
            Response.Flush();
        }

        [HttpGet]
        public ActionResult DeleteFile(string id)
        {
            driveRepository.DeleteFile(id);
            return RedirectToAction("Index");
        }

    }
}
