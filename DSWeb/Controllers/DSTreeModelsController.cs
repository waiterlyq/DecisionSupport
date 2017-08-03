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
using DBlib;
using Pylib;
using Loglib;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Csvlib;
using Excellib;

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
            MyLog.writeLog("执行");
            return "success";
        }

        /// <summary>
        /// 获取列名
        /// </summary>
        /// <returns></returns>
        public string GetFieldName()
        {
            string strModGUID = Request.QueryString["ModGUID"];
            string isFile = Request.QueryString["isFile"];
            DataTable dt = new DataTable();
            if(string.IsNullOrEmpty(isFile)||string.IsNullOrEmpty(strModGUID))
            {
                return "";
            }
            if(isFile=="1")
            {
                string dirPath = HttpContext.Server.MapPath("/Uploads/" + strModGUID + "/");
                string strFilePath = Directory.GetFiles(dirPath)[0];
                DirectoryInfo di = new DirectoryInfo(dirPath);
                FileInfo fi = di.GetFiles()[0];
                if(fi.FullName != "")
                {
                    if(fi.Extension == ".csv")
                    {
                        dt = CsvHelper.GetFieldName(fi.FullName);
                    }
                    else
                    {
                        ExcelHelper exc = new ExcelHelper(fi.FullName);
                        dt = exc.GetExcelFieldName();
                    }
                }
            }
            else
            {
                string strModDataSource = Request.QueryString["dbs"];
                if(string.IsNullOrEmpty(strModDataSource))
                {
                    return null;
                }
                else
                {
                    DSTreeModel dbs = JsonConvert.DeserializeObject<DSTreeModel>(strModDataSource);
                    SQLHelper targetdb = new SQLHelper(dbs.GetConnString());
                    string strSql = "select top 1 * from (" + dbs.ModDataSource + ") tmp";
                    DataTable dtTemp = targetdb.GetTable(strSql);
                    if(dtTemp ==null)
                    {
                        return null;
                    }
                    int ic = dtTemp.Columns.Count;
                    dt.Columns.Add("FieldName");
                    for(int i = 0; i < ic;i++)
                    {
                        DataRow dr = dt.NewRow();
                        dr[0] = dtTemp.Columns[i].ColumnName;
                        dt.Rows.Add(dr);
                    }
                }
            }
            return JsonConvert.SerializeObject(dt);
        }

        public string CheckDbString(string Server, string DataBase, string Uid, string PassWord)
        {
            string strconn = @"data source=" + Server + ";initial catalog=" + DataBase + ";user id=" + Uid + ";password=" + PassWord;
            SqlConnection con = new SqlConnection(strconn);
            string result = "success";
            try
            {
                con.Open();
                if (con.State == ConnectionState.Open)
                {
                    result = "success";
                }
                else
                {
                    result = "failure";
                }
            }
            catch (Exception)
            {
                result = "failure";
            }
            finally
            {
                con.Close();
            }
            return result;
        }

        public string UpdateResultField(string ModGUID, string ceMapGUID)
        {
            string result = "success";
            string sql = "";
            sql = string.Format("update dstreecemap set IsResultFactor = null where ModGUID = '{0}';", ModGUID);
            sql += string.Format("update dstreecemap set IsResultFactor = 1 where CEMapGUID = '{0}';", ceMapGUID);
            SQLHelper sqdb = new SQLHelper(db.Database.Connection.ConnectionString);
            try
            {
                if (sqdb.ExcuteSQL(sql) >= 0)
                {
                    result = "success";
                }
                else
                {
                    result = "failure";
                }
            }
            catch (Exception)
            {
                result = "failure";
            }
            return result;
        }

        public JsonResult GetDSTreeModelList(int limit, int offset)
        {
            var total = db.DSTreeModel.ToList().Count;
            var rows = db.DSTreeModel.ToList().Skip(offset).Take(limit).ToList();
            return Json(new { total = total, rows = rows }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        public JsonResult UpLoader()
        {
            HttpPostedFileBase file = Request.Files[0];
            string strModGUID = Request.QueryString["ModGUID"];
            try
            {
                string dirPath = HttpContext.Server.MapPath("/Uploads/" + strModGUID + "/");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                string filePath = Path.Combine(dirPath, file.FileName);
                file.SaveAs(filePath);
                StreamReader st = new StreamReader(filePath, Encoding.GetEncoding("UTF-8"));
                RServiceClient client = new RServiceClient();
                client.saveFile(strModGUID + Path.GetExtension(file.FileName), st.ReadToEnd());
                st.Dispose();
                st.Close();
            }
            catch(Exception e)
            {
                MyLog.writeLog("ERROR",e);
            }
           
            //foreach (HttpPostedFileBase file in FilesInput)
            //{
            //    string filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/"), Path.GetExtension(file.FileName));
            //    file.SaveAs(filePath);
            //}
            return Json(new { });
        }

        // GET: DSTreeModels/Create
        public ActionResult Create()
        {
            DSTreeModel ds = new DSTreeModel();
            ds.ModGUID = Guid.NewGuid();
            return View(ds);
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
                dSTreeModel.ModStatus = "等待执行";
                db.DSTreeModel.Add(dSTreeModel);
                db.SaveChanges();
                SQLHelper sqdb = new SQLHelper(dSTreeModel.GetConnString());
                string sDataSource = "select top 1 * from (" + dSTreeModel.ModDataSource + ") tmp";
                DataTable dt = sqdb.GetTable(sDataSource);
                if (dt != null)
                {
                    int ic = dt.Columns.Count;
                    DataTable dtCNPy = NPy.getDtCNPy(dt, ic);
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
                }
                return RedirectToAction("Index", new { ModGUID = dSTreeModel.ModGUID });
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

        /// <summary>
        /// 获取饼图数据
        /// </summary>
        /// <returns></returns>
        public string GetModSelectPieJson(string ID)
        {
            DataTable dt = new DataTable();
            SQLHelper sqdb = new SQLHelper(db.Database.Connection.ConnectionString);
            dt = sqdb.GetTable("SELECT CoverCount,ErrorCount FROM dbo.DSTree WHERE DSTreeGUID='" + ID + "'");
            if (dt.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dt);
            }
            return "";
        }

        /// <summary>
        /// 获取柱状图数据
        /// </summary>
        /// <returns></returns>
        public string GetModSelectGramJson(string ID)
        {
            DataTable dt = new DataTable();
            SQLHelper sqdb = new SQLHelper(db.Database.Connection.ConnectionString);
            dt = sqdb.GetTable("SELECT FactornameCn,Useage FROM dbo.DSTreeFactors WHERE ModGUID='" + ID + "'");
            if (dt.Rows.Count > 0)
            {
                return JsonConvert.SerializeObject(dt);
            }
            return "";
        }

        public string GetResultFieldJson(string ModGUID)
        {
            DataTable dt = new DataTable();
            SQLHelper sqdb = new SQLHelper(db.Database.Connection.ConnectionString);
            dt = sqdb.GetTable("select CEMapGUID,CCellName from DSTreeCEMap WHERE ModGUID = '" + ModGUID + "'");
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
