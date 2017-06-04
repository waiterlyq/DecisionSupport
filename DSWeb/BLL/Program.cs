using System;
using System.Text;
using RLib;
using System.Data;
using System.IO;

namespace DSTreeLib
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
            string strfile = @"D:\4321.csv";
            DataTable dtxxx = OpenCSV(strfile);
            RDataFramePy rpy = new RDataFramePy();
            rpy.setDataFrameInRByDt(dtxxx);
            dtxxx.Clear();
            using (RC50 rc = new RC50(rpy.DfName, "Status"))
            {
                if (rc.EvaluateByR(rpy.DfR))
                {
                    rpy.DfR = "";
                }
                DataTable dt = new DataTable();
                dt.Columns.Add("cn");
                dt.Columns.Add("cnz");
                dt.Rows.Add("OppSource", "机会来源");
                dt.Rows.Add("Process", "进度");
                dt.Rows.Add("Status", "状态");
                dt.Rows.Add("CognizeAve", "认知途径");
                dt.Rows.Add("CstType", "客户类型");
                dt.Rows.Add("Gender", "性别");
                dt.Rows.Add("Marriage", "婚姻状态");
                dt.Rows.Add("EduLevel", "教育水平");
                dt.Rows.Add("Family", "家庭状态");
                dt.Rows.Add("Work", "工作");
                dt.Rows.Add("Xqah", "兴趣爱好");
                dt.Rows.Add("BuyersUse", "购房用途");
                RC50Tree rct = rc.getC50Tree(dt, rpy.DtPy);
                DataTable dt1 = rct.DtDstree;
                rc.Dispose();
            }
            Console.ReadLine();
        }

        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCSV(string filePath)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.Default);
            //StreamReader sr = new StreamReader(fs, encoding);
            //string fileContent = sr.ReadToEnd();
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        tableHead[i] = tableHead[i].Replace("\"", "");
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j].Replace("\"", "");
                    }
                    dt.Rows.Add(dr);
                }
            }
            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[2] + " " + "DESC";
            }
            sr.Close();
            fs.Close();
            return dt;
        }

        /// <summary>
        /// 将DataTable中数据写入到CSV文件中
        /// </summary>
        /// <param name="dt">提供保存数据的DataTable</param>
        /// <param name="fileName">CSV的文件路径</param>
        public static bool SaveCSV(DataTable dt, string fullPath)
        {
            try
            {
                FileInfo fi = new FileInfo(fullPath);
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                string data = "";
                //写出列名称
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    data += "\"" + dt.Columns[i].ColumnName.ToString() + "\"";
                    if (i < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
                //写出各行数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string str = dt.Rows[i][j].ToString();
                        str = string.Format("\"{0}\"", str);
                        data += str;
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                    sw.WriteLine(data);
                }
                sw.Close();
                fs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
