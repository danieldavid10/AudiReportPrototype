using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ApplicationPrototype.Models
{
    public class DriveRepository
    {
        public List<Audit> LoadAudits()
        {
            List<Audit> audits;
            using (var client = new HttpClient())
            {
                var response = client.GetStringAsync("http://www.mocky.io/v2/5bc0fe85320000700021abcb").Result;
                audits = JsonConvert.DeserializeObject<List<Audit>>(response);
            }
            return audits;
        }
    }
}