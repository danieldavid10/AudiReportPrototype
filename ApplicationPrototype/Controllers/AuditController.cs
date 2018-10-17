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
        public async Task<List<Audit>> GetAudits()
        {
            List<Audit> audits = await Repository.GetAudits();
            return audits;
        }

        // GET: api/Audit/GetModify
        //public Audit GetModify()
        //{
        //    Audit audit = Repository.GetDataFromDoc();
        //    return audit;
        //}
    }
}
