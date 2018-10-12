using ApplicationPrototype.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApplicationPrototype.Controllers
{
    public class AuditController : ApiController
    {
        DocRepository Repository = new DocRepository();
        // GET: api/Audit
        public async Task<Audit> GetAudit()
        {
            Audit audit;
            using (var client = new HttpClient())
            {

                var response = await client.GetStringAsync("http://www.mocky.io/v2/5bbe0ac43100003800711390");
                audit = JsonConvert.DeserializeObject<Audit>(response);
            }
            return audit;
        }

        // GET: api/Audit/GetModify
        public Audit GetModify()
        {
            Audit audit = Repository.GetDataFromDoc();
            return audit;
        }

        // GET: api/Audit/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Audit
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Audit/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Audit/5
        public void Delete(int id)
        {
        }
    }
}
