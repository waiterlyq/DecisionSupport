using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DSWeb.Models;

namespace DSWeb.Controllers
{
    public class DSTreeCEMapsController : Controller
    {
        private DSEntities db = new DSEntities();

        // GET: DSTreeCEMaps
        public ActionResult Index()
        {
            string str = Request.QueryString["id"];
            ;
            return View(db.Database.SqlQuery<DSTreeCEMap>("SELECT * FROM dbo.DSTreeCEMap WHERE ModGUID = '" + str + "'").ToList());
        }

        // GET: DSTreeCEMaps/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSTreeCEMap dSTreeCEMap = db.DSTreeCEMap.Find(id);
            if (dSTreeCEMap == null)
            {
                return HttpNotFound();
            }
            return View(dSTreeCEMap);
        }

        // GET: DSTreeCEMaps/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DSTreeCEMaps/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CEMapGUID,ModGUID,ECellName,CCellName,IsResultFactor")] DSTreeCEMap dSTreeCEMap)
        {
            if (ModelState.IsValid)
            {
                dSTreeCEMap.CEMapGUID = Guid.NewGuid();
                db.DSTreeCEMap.Add(dSTreeCEMap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dSTreeCEMap);
        }

        // GET: DSTreeCEMaps/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSTreeCEMap dSTreeCEMap = db.DSTreeCEMap.Find(id);
            if (dSTreeCEMap == null)
            {
                return HttpNotFound();
            }
            return View(dSTreeCEMap);
        }

        // POST: DSTreeCEMaps/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CEMapGUID,ModGUID,ECellName,CCellName,IsResultFactor")] DSTreeCEMap dSTreeCEMap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dSTreeCEMap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dSTreeCEMap);
        }

        // GET: DSTreeCEMaps/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSTreeCEMap dSTreeCEMap = db.DSTreeCEMap.Find(id);
            if (dSTreeCEMap == null)
            {
                return HttpNotFound();
            }
            return View(dSTreeCEMap);
        }

        // POST: DSTreeCEMaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DSTreeCEMap dSTreeCEMap = db.DSTreeCEMap.Find(id);
            db.DSTreeCEMap.Remove(dSTreeCEMap);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
