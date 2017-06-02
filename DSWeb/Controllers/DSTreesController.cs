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
    public class DSTreesController : Controller
    {
        private DSEntities db = new DSEntities();

        // GET: DSTrees
        public ActionResult Index()
        {
            return View(db.DSTree.ToList());
        }

        // GET: DSTrees/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSTree dSTree = db.DSTree.Find(id);
            if (dSTree == null)
            {
                return HttpNotFound();
            }
            return View(dSTree);
        }

        // GET: DSTrees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DSTrees/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DSTreeGUID,ModGUID,ID,PID,FactorName,FactorNameCn,Operator,OperatorCn,FactorValue,FactorValueCn,Describe,DescribeCn,Result,ResultCn,CoverCount,ErroCount")] DSTree dSTree)
        {
            if (ModelState.IsValid)
            {
                dSTree.DSTreeGUID = Guid.NewGuid();
                db.DSTree.Add(dSTree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dSTree);
        }

        // GET: DSTrees/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSTree dSTree = db.DSTree.Find(id);
            if (dSTree == null)
            {
                return HttpNotFound();
            }
            return View(dSTree);
        }

        // POST: DSTrees/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DSTreeGUID,ModGUID,ID,PID,FactorName,FactorNameCn,Operator,OperatorCn,FactorValue,FactorValueCn,Describe,DescribeCn,Result,ResultCn,CoverCount,ErroCount")] DSTree dSTree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dSTree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dSTree);
        }

        // GET: DSTrees/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSTree dSTree = db.DSTree.Find(id);
            if (dSTree == null)
            {
                return HttpNotFound();
            }
            return View(dSTree);
        }

        // POST: DSTrees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DSTree dSTree = db.DSTree.Find(id);
            db.DSTree.Remove(dSTree);
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
