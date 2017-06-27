using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DSWeb.Models;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using DBLib;

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

        /// <summary>
        /// 获得树形json
        /// </summary>
        /// <param name="ModGUID"></param>
        /// <returns></returns>
        public string GetDstreeJson(string ModGUID)
        {
            if (string.IsNullOrEmpty(ModGUID))
            {
                return "";
            }
            DataTable dt = new DataTable();
            SQLHelper sqdb = new SQLHelper(db.Database.Connection.ConnectionString);
            dt = sqdb.GetTable("SELECT id,case when pid='' then '0' else pid end  as pId,DescribeCn+ CASE WHEN ResultCn <> '' THEN '则'+ResultCn ELSE '' END AS name FROM dbo.DSTree  WHERE ModGUID = '" + ModGUID + "' ORDER BY ID");
            if (dt.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dt);
            }
            return "";
        }


        /// <summary>
        /// 获取图形json
        /// </summary>
        /// <param name="ModGUID"></param>
        /// <returns></returns>
        public string GetGraphicJson(string ModGUID)
        {
            if (string.IsNullOrEmpty(ModGUID))
            {
                return "";
            }
            DataTable dt = new DataTable();
            SQLHelper sqdb = new SQLHelper(db.Database.Connection.ConnectionString);
            string strsql = @"SELECT  CASE WHEN dsp.PID = '' THEN dsp.FactorNameCn ELSE dsp.FactorNameCn + '&' + REPLACE(dsp.PID,'.','') END AS [source] ,
                            dsc.FactorNameCn + '&' + REPLACE(dsp.ID,'.','') AS [target] ,
                            'resolved' AS [type] ,
                            dsp.DescribeCn AS [rela]
                            FROM    dbo.DSTree dsp
                            INNER JOIN dbo.DSTree dsc ON dsp.ID = dsc.PID
                            AND dsp.ModGUID = dsc.ModGUID
                            AND dsp.ModGUID = 'd21bfbc8-9761-4e73-aa5e-133bf9a12f06'
                            UNION ALL
                            SELECT CASE WHEN PID = '' THEN FactorNameCn ELSE  FactorNameCn + '&' + REPLACE(PID,'.','') END AS [source] ,
                            ResultCn + '&' + REPLACE(PID,'.','') AS [target] ,
                            'resolved' AS [type] ,
                            DescribeCn AS [rela]
                            FROM    dbo.DSTree
                            WHERE   Result <> ''
                            AND ModGUID = 'd21bfbc8-9761-4e73-aa5e-133bf9a12f06'
                            ORDER BY [source]";
            dt = sqdb.GetTable(strsql);
            if (dt.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dt);
            }
            return "";
        }

        public string GetJson()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            DataTable dt = new DataTable();
            SQLHelper sqdb = new SQLHelper(db.Database.Connection.ConnectionString);
            dt = sqdb.GetTable("SELECT id,case when pid='' then '0' else pid end  as pId,DescribeCn+ CASE WHEN ResultCn <> '' THEN '则'+ResultCn ELSE '' END AS name FROM dbo.DSTree  WHERE ModGUID = 'D21BFBC8-9761-4E73-AA5E-133BF9A12F06' ORDER BY ID");

            return JsonConvert.SerializeObject(dt);
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
