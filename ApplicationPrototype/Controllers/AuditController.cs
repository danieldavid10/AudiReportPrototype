using ApplicationPrototype.Models;
using ApplicationPrototype.Repository;
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
        //DocRepository Repository = new DocRepository();
        AuditRepository auditRepository = new AuditRepository();

        // GET: api/Audit
        public async Task<IEnumerable<Audit>> GetAudits()
        {
            var audits = await auditRepository.GetAudits();
            return audits;
        }

        // GET: api/Audit/GetAudit/{id}
        public Audit GetAudit(int id)
        {
            var audit = auditRepository.FindAuditById(id);
            return audit;
        }
    }
}
