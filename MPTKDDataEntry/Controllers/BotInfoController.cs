using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using MPTKDDataEntry.Models;

namespace MPTKDDataEntry.Controllers
{
    public class BotInfoController : ApiController
    {
        private MTKDataEntities db = new MTKDataEntities();

        // GET: api/BotInfo
        public IQueryable<BotInfo> GetInfos()
        {
            return db.Infos;
        }

        // GET: api/BotInfo/5
        [ResponseType(typeof(BotInfo))]
        public IHttpActionResult GetBotInfo(int id)
        {
            BotInfo botInfo = db.Infos.Find(id);
            if (botInfo == null)
            {
                return NotFound();
            }

            return Ok(botInfo);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BotInfoExists(int id)
        {
            return db.Infos.Count(e => e.ID == id) > 0;
        }
    }
}