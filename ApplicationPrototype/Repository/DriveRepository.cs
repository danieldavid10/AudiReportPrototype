using ApplicationPrototype.Repository;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
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
        AuditRepository auditRepository = new AuditRepository();

        // Defined scope.
        public static string[] Scopes = new string[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile };

        public static DriveService GetService()
        {
            // Get Credentials from client_secret.json file 
            UserCredential credential;
            string path = HttpContext.Current.Server.MapPath("~/Credentials");
            using (var stream = new FileStream(path + "/client_secret.json", FileMode.Open, FileAccess.Read))
            {
                String FolderPath = HttpContext.Current.Server.MapPath("~/Credentials");
                //String FilePath = Path.Combine(FolderPath, "DriveServiceCredentials.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FolderPath, true)).Result;
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
            //List<Audit> audits;
            //using (var client = new HttpClient())
            //{
            //    var response = client.GetStringAsync("http://localhost:59449/api/Audit/GetAudits").Result;
            //    audits = JsonConvert.DeserializeObject<List<Audit>>(response);
            //}
            return auditRepository.GetAudits(); ;
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

        public List<GoogleDriveFiles> getFilesInFolder()
        {
            DriveService service = GetService();

            // define parameters of request.
            FilesResource.ListRequest FileListRequest = service.Files.List();

            //listRequest.PageSize = 10;
            //listRequest.PageToken = 10;
            FileListRequest.Fields = "nextPageToken, files(id, name, size, version, createdTime, webViewLink, iconLink)";
            FileListRequest.Q = "'1RKjoybSiXZlSMlE-vINZ-y2rd9QNU9A4' in parents"; // Files in folder id

            //get file list.
            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFiles> FileList = new List<GoogleDriveFiles>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    GoogleDriveFiles File = new GoogleDriveFiles
                    {
                        Id = file.Id,
                        Name = file.Name,
                        Size = file.Size,
                        WebViewLink = file.WebViewLink,
                        IconLink = file.IconLink
                    };
                    FileList.Add(File);
                }
            }
            return FileList;
        }

        public void FileUpload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                DriveService service = GetService();
                string folderID = "1RKjoybSiXZlSMlE-vINZ-y2rd9QNU9A4";

                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/GoogleDriveFiles"),
                Path.GetFileName(file.FileName));
                file.SaveAs(path);


                var FileMetaData = new Google.Apis.Drive.v3.Data.File();
                FileMetaData.Name = Path.GetFileName(file.FileName);
                //FileMetaData.MimeType = MimeMapping.GetMimeMapping(path); // Uploading any file
                FileMetaData.MimeType = "application/vnd.google-apps.document"; // Upload as Google Document
                FileMetaData.Parents = new List<string> { folderID };

                FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    request.Fields = "id";
                    request.Upload();
                }
                Console.WriteLine("File ID: " + request.ResponseBody.Id);
            }
        }

        public string FileUpload(string FileName)
        {
            if (FileName != null && FileName.Length > 0)
            {
                DriveService service = GetService();
                string folderID = "1RKjoybSiXZlSMlE-vINZ-y2rd9QNU9A4";

                string path = HttpContext.Current.Server.MapPath("~/Files/" + FileName + ".docx");

                var FileMetaData = new Google.Apis.Drive.v3.Data.File();
                FileMetaData.Name = Path.GetFileName(FileName + ".docx");
                //FileMetaData.MimeType = MimeMapping.GetMimeMapping(path); // Uploading any file
                FileMetaData.MimeType = "application/vnd.google-apps.document"; // Upload as Google Document
                FileMetaData.Parents = new List<string> { folderID };

                FilesResource.CreateMediaUpload request;
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    request.Fields = "id";
                    request.Upload();
                }
                return request.ResponseBody.Id;
            }
            else
            {
                return null;
            }
        }

        public string DownloadGoogleFile(string fileId)
        {
            DriveService service = GetService();

            string FolderPath = HttpContext.Current.Server.MapPath("/GoogleDriveFiles/");

            FilesResource.GetRequest FileRequest = service.Files.Get(fileId);

            string FileName = FileRequest.Execute().Name;
            string FilePath = Path.Combine(FolderPath, FileName);

            MemoryStream stream1 = new MemoryStream();

            // Convert Google Document to Word Document
            FilesResource.ExportRequest request = service.Files.Export(fileId, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            SaveStream(stream1, FilePath);
                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
            request.Download(stream1);
            return FilePath;
        }

        // file save to server path
        private static void SaveStream(MemoryStream stream, string FilePath)
        {
            using (System.IO.FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.WriteTo(file);
            }
        }

        //Delete file from the Google drive
        public void DeleteFile(string id)
        {
            DriveService service = GetService();
            try
            {
                // Initial validation.
                if (service == null)
                    throw new ArgumentNullException("service");

                if (id == null)
                    throw new ArgumentNullException(id);

                // Make the request.
                service.Files.Delete(id).Execute();
            }
            catch (Exception ex)
            {
                throw new Exception("Request Files.Delete failed.", ex);
            }
        }

        public string GetGoogleDocument(string title)
        {
            DriveService service = GetService();
            FilesResource.ListRequest FileListRequest = service.Files.List();
            FileListRequest.Fields = "nextPageToken, files(id, name)";
            FileListRequest.Q = "'1RKjoybSiXZlSMlE-vINZ-y2rd9QNU9A4' in parents";

            IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
            List<GoogleDriveFiles> FileList = new List<GoogleDriveFiles>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files.OrderBy(x => "modifiedTime"))
                {
                    if (title == file.Name)
                    {
                        return file.Id;
                    }
                }
            }
            else
            {
                return null;
            }
            return null;
        }

        public void DownloadGoogleDoc(string fileId)
        {
            DriveService service = GetService();
            MemoryStream stream = new MemoryStream();
            FilesResource.GetRequest FileRequest = service.Files.Get(fileId);

            string FileName = FileRequest.Execute().Name;

            string FilePath = HttpContext.Current.Server.MapPath("~/Files/" + FileName);


            // Convert Google Document to Word Document
            FilesResource.ExportRequest request = service.Files.Export(fileId, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            SaveStream(stream, FilePath);
                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
            request.Download(stream);
        }
    }
}