using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DSWeb.Models;
using DSWeb.RWS;
using Newtonsoft.Json;
using DBLib;
using Pylib;
using Loglib;

namespace DSWeb.Controllers
{
    public class DSTreeModelsController : Controller
    {
        private DSEntities db = new DSEntities();

        // GET: DSTreeModels
        public ActionResult Index()
        {
            return View(db.DSTreeModel.ToList());
        }

        // GET: DSTreeModels/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSTreeModel dSTreeModel = db.DSTreeModel.Find(id);
            if (dSTreeModel == null)
            {
                return HttpNotFound();
            }
            return View(dSTreeModel);
        }

        public string Generate(string ModGUID)
        {
            RServiceClient client = new RServiceClient();
            client.AddRq(ModGUID);
            MyLog.writeLog("执行", logtype.Info);
            return "success";
        }

        public JsonResult GetDSTreeModelList(int limit, int offset)
        {
            var total = db.DSTreeModel.ToList().Count;
            var rows = db.DSTreeModel.ToList().Skip(offset).Take(limit).ToList();
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }

        // GET: DSTreeModels/Create
        public ActionResult Create()
        {
            
            return View();
        }

        /// <summary>
        /// 待完善1、表头获取 2、表头校验
        /// </summary>
        /// <param name="dSTreeModel"></param>
        /// <returns></returns>
        // POST: DSTreeModels/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ModGUID,ModName,ModUid,ModPassword,ModDataBase,ModServer,ModDataSource,ModRemark")] DSTreeModel dSTreeModel)
        {
            if (ModelState.IsValid)
            {
                dSTreeModel.ModGUID = Guid.NewGuid();
                db.DSTreeModel.Add(dSTreeModel);
                db.SaveChanges();
                SQLHelper sqdb = new SQLHelper(dSTreeModel.GetConnString());
                DataTable dt = sqdb.GetTable(dSTreeModel.ModDataSource);
                int ic = dt.Columns.Count;
                DataTable dtCNPy =  NPy.getDtCNPy(dt, ic);
                List<DSTreeCEMap> ldstcm = new List<DSTreeCEMap>();
                for (int i = 0; i < ic; i++)
                {
                    DSTreeCEMap dstcm = new DSTreeCEMap();
                    dstcm.CEMapGUID = Guid.NewGuid();
                    dstcm.ModGUID = dSTreeModel.ModGUID;
                    dstcm.CCellName = dt.Columns[i].ColumnName;
                    dstcm.ECellName = dtCNPy.Select("cnC='" + dstcm.CCellName + "'")[0][1].ToString();
                    db.DSTreeCEMap.Add(dstcm);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dSTreeModel);
        }

        /// <summary>
        /// 获取模型下拉列表
        /// </summary>
        /// <returns></returns>
        public string GetModSelectJson()
        {
            DataTable dt = new DataTable();
            SQLHelper sqdb = new SQLHelper(db.Database.Connection.ConnectionString);
            dt = sqdb.GetTable("SELECT ModGUID,ModName FROM dbo.DSTreeModel");
            if (dt.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dt);
            }
            return "";
        }
        // GET: DSTreeModels/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSTreeModel dSTreeModel = db.DSTreeModel.Find(id);
            if (dSTreeModel == null)
            {
                return HttpNotFound();
            }
            return View(dSTreeModel);
        }

        // POST: DSTreeModels/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ModGUID,ModName,ModUid,ModPassword,ModDataBase,ModServer,ModDataSource,ModRemark")] DSTreeModel dSTreeModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dSTreeModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dSTreeModel);
        }

        // GET: DSTreeModels/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSTreeModel dSTreeModel = db.DSTreeModel.Find(id);
            if (dSTreeModel == null)
            {
                return HttpNotFound();
            }
            return View(dSTreeModel);
        }

        // POST: DSTreeModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            DSTreeModel dSTreeModel = db.DSTreeModel.Find(id);
            db.DSTreeModel.Remove(dSTreeModel);
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
