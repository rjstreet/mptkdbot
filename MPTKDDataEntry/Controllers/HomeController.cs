using MPTKDDataEntry.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MPTKDDataEntry.Controllers
{
    public class HomeController : Controller
    {
        private MTKDataEntities db = new MTKDataEntities();
        public ActionResult Index()
        {
            BotInfo inf = new BotInfo();
            var infos = this.db.Infos;
            return View(infos.First());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "ID,Monday,Tuesday,Wedensday,Thursday,Friday,Saturday,Testing,Promotion,Holidays")]BotInfo info)
        {
            if(ModelState.IsValid)
            {
                db.Entry(info).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(info);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}