using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;

namespace ApplicationPrototype.Models
{
    public class DriveRepository
    {
        // Folder of Files
        string pathFiles = HttpContext.Current.Server.MapPath("~/Files/");
        // Defined scope.
        public static string[] Scopes = { DriveService.Scope.Drive };

        public static DriveService GetService()
        {
            //get Credentials from client_secret.json file 
            UserCredential credential;
            string path = HttpContext.Current.Server.MapPath("~/Credentials");
            using (var stream = new FileStream(path + "/client_secret.json", FileMode.Open, FileAccess.Read))
            {
                String FolderPath = @"D:\";
                String FilePath = Path.Combine(FolderPath, "DriveServiceCredentials.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }

            //create Drive API service.
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleDriveRestAPI-v3",
            });
            return service;
        }

        public List<Audit> LoadAudits()
        {
            List<Audit> audits;
            using (var client = new HttpClient())
            {
                var response = client.GetStringAsync("http://localhost:59449/api/Audit/GetAudits").Result;
                audits = JsonConvert.DeserializeObject<List<Audit>>(response);
            }
            return audits;
        }

        public Audit FindById(int id)
        {
            foreach (var audit in LoadAudits())
            {
                if (audit.AuditId == id)
                {
                    return audit;
                }
            }
            return null;
        }

        public void UploadFile(string name)
        {
            DriveService service = GetService();

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = name
                //MimeType = "application/vnd.google-apps.document"
            };
            FilesResource.CreateMediaUpload request;
            string path = pathFiles + name + ".docx";
            using (var stream = new System.IO.FileStream(@"D:\Info-Arch Projects\Audi-Reports Prototype\AudiReportPrototype\ApplicationPrototype\Files\"+name+".docx", System.IO.FileMode.Open))
            {
                request = service.Files.Create(
                    fileMetadata, stream, "application/vnd.google-apps.document");
                request.Fields = "id";
                request.Upload();
            }
            var file = request.ResponseBody;
            Console.WriteLine("File ID: " + file.Id);
        }


    }
}