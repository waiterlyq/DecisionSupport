using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using loglib;
using RDotNet;

namespace RLib
{
    /// <summary>
    /// C50树类
    /// </summary>
    public class RC50Tree
    {
        /// <summary>
        /// 树字符串
        /// </summary>
        private string _dstree;
        /// <summary>
        /// 树深度
        /// </summary>
        private int _treesize;

        /// <summary>
        /// 因素贡献度
        /// </summary>
        private DataTable _dtFactors;

        /// <summary>
        /// 决策树表
        /// </summary>
        private DataTable _dtDstree;

        public RC50Tree()
        {
            setDtDstreeStruct();
            setDtFactorsStruct();
        }

        public RC50Tree(string strtree, string strfactors, int isize, DataTable dtcnz, DataTable dtpy, string strFormula)
        {
            Treesize = isize;
            setDtFactors(strfactors, dtcnz);
            setDtDstree(strtree, dtcnz, dtpy, strFormula);
        }

        /// <summary>
        /// 设置决策支持树dt结构
        /// </summary>
        public void setDtDstreeStruct()
        {
            DtDstree = new DataTable();
            DtDstree.Columns.Add("ID");
            DtDstree.Columns.Add("ParentId");
            DtDstree.Columns.Add("FactorName");
            DtDstree.Columns.Add("FactorNameCn");
            DtDstree.Columns.Add("Operator");
            DtDstree.Columns.Add("OperatorCn");
            DtDstree.Columns.Add("FactorValue");
            DtDstree.Columns.Add("FactorValueCn");
            DtDstree.Columns.Add("Describe");
            DtDstree.Columns.Add("DescribeCn");
            DtDstree.Columns.Add("Result");
            DtDstree.Columns.Add("ResultCn");
            DtDstree.Columns.Add("CoverCount");
            DtDstree.Columns.Add("ErroCount");
            DtDstree.Columns.Add("refGUID");
        }

        /// <summary>
        /// 根据决策树字符串生成决策树表
        /// </summary>
        public void setDtDstree(string strtree, DataTable dtcnz, DataTable dtpy, string strFormula)
        {
            setDtDstreeStruct();
            strtree = strtree.Replace(":...", "    ");

            int[] icj = new int[Treesize];
            for (int i = 0; i < Treesize; i++)
            {
                icj[i] = 0;
            }
            string[] arrtree = strtree.Split(new string[] { "\n" }, StringSplitOptions.None);
            int ir = arrtree.Length;
            if (ir == 0 || ir == 1)
            {
                return;
            }
            for (int i = 0; i < ir; i++)
            {
                int iSpaceCount = 0;
                int icjcount = 0;
                string strid = "";
                string strpid = "";
                string strFactorName = "";
                string strFactorNameCn = "";
                string strOperatorCn = "";
                string strOperator = "";
                string strFactorValue = "";
                string strFactorValueCn = "";
                string strDescribe = "";
                string strDescribeCn = "";
                string strResult = "";
                string strResultCn = "";
                int iCoverCount = 0;
                int iErroCount = 0;
                ///通过计算有多少个4个空格来获取层级
                iSpaceCount = getSpaceCount(arrtree[i]);
                icjcount = iSpaceCount / 4;
                ///层级对应id自增长
                icj[icjcount]++;
                for (int j = 0; j < icjcount + 1; j++)
                {
                    strid += "." + icj[j];
                    if (j < icjcount)
                    {
                        strpid += "." + icj[j];
                    }
                }
                strid = strid.Substring(1);
                if (strpid != "")
                {
                    strpid = strpid.Substring(1);
                }

                arrtree[i] = arrtree[i].Substring(iSpaceCount);
                ///分割结果和规则
                string[] arrstr1 = arrtree[i].Split(new string[] { ":" }, StringSplitOptions.None);
                arrstr1[0] = arrstr1[0].Trim();

                strDescribe = arrstr1[0];
                ///如果为末级则设置结果和覆盖率和错误率
                if (arrstr1.Length == 2)
                {
                    if (!string.IsNullOrEmpty(arrstr1[1]))
                    {
                        arrstr1[1] = arrstr1[1].Trim();
                        string[] arrstr2 = arrstr1[1].Split(' ');
                        strResult = arrstr2[0];
                        strResultCn = dtpy.Select("cn='" + strFormula + "' AND cvpy='" + arrstr2[0] + "'")[0][1].ToString();
                        arrstr2[1] = arrstr2[1].Replace("(", "").Replace(")", "");
                        string[] arrstr3 = arrstr2[1].Split('/');
                        iCoverCount = int.Parse(arrstr3[0]);
                        if (arrstr3.Length == 2)
                        {
                            iErroCount = int.Parse(arrstr3[1]);
                        }
                        else
                        {
                            iErroCount = 0;
                        }
                    }
                }
                string[] arrstr4 = arrstr1[0].Split(' ');
                ///设置因素
                strFactorName = arrstr4[0];
                strFactorNameCn = dtcnz.Select("cn='" + arrstr4[0] + "'")[0][1].ToString();
                //设置操作符
                strOperator = arrstr4[1];
                strOperatorCn = arrstr4[1];
                if (arrstr4[1] == "in")
                {
                    strOperatorCn = "属于";
                }
                strFactorValue = arrstr4[2];
                if (isNumberic(arrstr4[2]))
                {
                    strFactorValueCn = arrstr4[2];
                }
                else
                {
                    ///处理因素集合的中文显示
                    if (arrstr4[2].IndexOf('{') > -1)
                    {
                        string strfc = arrstr4[2].Replace("{", "").Replace("}", "");
                        string strt = "";
                        string[] arrfc = strfc.Split(',');
                        int ifcr = arrfc.Length;
                        strFactorValueCn = "(";
                        for (int j = 0; j < ifcr; j++)
                        {
                            strt += "," + dtpy.Select("cn='" + strFactorName + "' AND cvpy='" + arrfc[j] + "'")[0][1].ToString();
                        }
                        strFactorValueCn = strt.Substring(1) + ")";
                    }
                    else
                    {
                        strFactorValueCn = dtpy.Select("cn='" + strFactorName + "' AND cvpy='" + arrstr4[2] + "'")[0][1].ToString();
                    }
                }
                strDescribeCn = strFactorNameCn + " " + strOperatorCn + " " + strFactorValueCn;
                DtDstree.Rows.Add(strid, strpid, strFactorName, strFactorNameCn, strOperator, strOperatorCn, strFactorValue, strFactorValueCn, strDescribe, strDescribeCn, strResult, strResultCn, iCoverCount, iErroCount, "");
            }
        }

        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected bool isNumberic(string message)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(message, @"^[+-]?\d*[.]?\d*$");
        }



        /// <summary>
        /// 获得字符串前面的空格数量
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int getSpaceCount(string str)
        {
            int iSpace, iLength;
            iLength = str.Length;
            iSpace = 0;
            for (int i = 0; i < iLength; i++)
            {
                if (str.Substring(i, 1) == " ")
                {
                    iSpace++;
                }
                else
                {
                    break;
                }
            }

            return iSpace;
        }

        /// <summary>
        /// 设置因素dt结构
        /// </summary>
        public void setDtFactorsStruct()
        {
            DtFactors = new DataTable();
            ///因素名称
            DtFactors.Columns.Add("Factorname");
            ///因素中文名称
            DtFactors.Columns.Add("FactornameCn");
            ///贡献度
            DtFactors.Columns.Add("Useage");
        }
        /// <summary>
        /// 根据因素字符串生成因素表
        /// </summary>
        public void setDtFactors(string strfactors, DataTable dtcnz)
        {
            setDtFactorsStruct();
            string[] arrfactors = strfactors.Split(new string[] { "\n\t" }, StringSplitOptions.None);
            int ir = arrfactors.Length;
            if (ir > 0)
            {
                for (int i = 0; i < ir; i++)
                {
                    string[] strtemp = arrfactors[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                    DtFactors.Rows.Add(strtemp[1], dtcnz.Select("cn='" + strtemp[1] + "'")[0][1], strtemp[0].Replace(@"%", ""));
                }
            }
        }

        public string Dstree
        {
            get
            {
                return _dstree;
            }

            set
            {
                _dstree = value;
            }
        }

        public int Treesize
        {
            get
            {
                return _treesize;
            }

            set
            {
                _treesize = value;
            }
        }

        public DataTable DtFactors
        {
            get
            {
                return _dtFactors;
            }

            set
            {
                _dtFactors = value;
            }
        }

        public DataTable DtDstree
        {
            get
            {
                return _dtDstree;
            }

            set
            {
                _dtDstree = value;
            }
        }
    }

    /// <summary>
    /// C50控制器
    /// </summary>
    public class RC50Control
    {
        private string _subset;
        private string _bands;
        private string _winnow;
        private string _noGlobalPruning;
        /// <summary>
        /// 最小置信度
        /// </summary>
        private double _CF;
        /// <summary>
        /// 最小数量
        /// </summary>
        private int _minCases;
        private string _fuzzyThresHold;
        private int _sample;
        private int _seed;
        private string _earlyStopping;
        private string _label;
        private string _ctName;

        public double CF
        {
            get
            {
                return _CF;
            }

            set
            {
                _CF = value;
            }
        }

        public int MinCases
        {
            get
            {
                return _minCases;
            }

            set
            {
                _minCases = value;
            }
        }

        public string CtName
        {
            get
            {
                return _ctName;
            }

            set
            {
                _ctName = value;
            }
        }

        public RC50Control(double dcf = 0.25, int imc = 10, string strCtName = "tc")
        {
            CF = dcf;
            MinCases = imc;
            CtName = strCtName;
        }


        /// <summary>
        /// 获得C5.0control格式：tc<-C5.0Control(CF=0.01,minCases =1)
        /// </summary>
        /// <returns></returns>
        public string getCtString()
        {
            if (CF == 0 && MinCases == 0)
            {
                return "";
            }
            string str1 = "";
            str1 = CtName + "<-C5.0Control(";
            if (CF != 0)
            {
                str1 += "CF=" + CF + ",";
            }
            if (MinCases != 0)
            {
                str1 += "minCases=" + MinCases;
            }
            str1 += ")";
            return str1;
        }
    }

    /// <summary>
    /// C50算法类
    /// </summary>
    public class RC50 : RClass
    {
        /// <summary>
        /// 模型名称
        /// </summary>
        private string _c5Name;
        /// <summary>
        /// 迭代次数次数
        /// </summary>
        private int _trials;
        /// <summary>
        /// 是否以规则形式输出
        /// </summary>
        private string _rules;
        /// <summary>
        /// 特定权重
        /// </summary>
        private string _weights;
        /// <summary>
        /// 损失矩阵
        /// </summary>
        private string _costs;
        /// <summary>
        /// 结果字段
        /// </summary>
        private string _formula;
        /// <summary>
        /// 数据集名称
        /// </summary>
        private string _data;
        /// <summary>
        /// 缺省值动作
        /// </summary>
        private string _naaction;
        /// <summary>
        /// c50控制器
        /// </summary>
        private RC50Control _rct;

        public string Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        public string Formula
        {
            get
            {
                return _formula;
            }

            set
            {
                _formula = value;
            }
        }

        public int Trials
        {
            get
            {
                return _trials;
            }

            set
            {
                _trials = value;
            }
        }

        public RC50Control Rct
        {
            get
            {
                return _rct;
            }

            set
            {
                _rct = value;
            }
        }

        public string C5Name
        {
            get
            {
                return _c5Name;
            }

            set
            {
                _c5Name = value;
            }
        }

        public string Rules
        {
            get
            {
                return _rules;
            }

            set
            {
                _rules = value;
            }
        }

        public RC50(string strData, string strFormula, string strC5Name = "module", string strRules = "F", int itrials = 10, double dcf = 0.25, int imc = 10, string strCtName = "ct")
        {
            Data = strData;
            Formula = strFormula;
            Trials = itrials;
            C5Name = strC5Name;
            Rules = strRules;
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();
            Rct = new RC50Control(dcf, imc, strCtName);
            EvaluateByR("library(C50)");
        }

        /// <summary>
        /// 获得C50字符串
        /// </summary>
        /// <returns></returns>
        public string getC50String()
        {
            string strct = Rct.getCtString();
            string strC50 = "";
            if (Data == "" && Formula == "")
            {
                return strC50;
            }
            if (strct == "")
            {
                strC50 = C5Name + "<- C5.0(formula=" + Formula + " ~ ., data = " + Data + ",rules=" + Rules + ")";
            }
            else
            {
                if (EvaluateByR(strct))
                {
                    strC50 = C5Name + "<- C5.0(formula=" + Formula + " ~ ., data = " + Data + ",rules=" + Rules + ",control =" + Rct.CtName + ")";
                }
            }
            return strC50;
        }

        /// <summary>
        /// 获得决策支持树
        /// </summary>
        /// <param name="dtcnz">字段中文对照表</param>
        /// <param name="dtpy">内容拼音对照表</param>
        /// <returns></returns>
        public RC50Tree getC50Tree(DataTable dtcnz, DataTable dtpy)
        {
            try
            {
                string strResult = "";
                int iResult = -1;
                string strTree = "";
                string strFactors = "";

                string strC50 = getC50String();
                string strR = "";
                if (!string.IsNullOrEmpty(strC50))
                {
                    if (EvaluateByR(strC50))
                    {
                        strR = C5Name + "$output";
                        strResult = getStringByR(strR);
                        strR = C5Name + "$size";
                        iResult = getIntByR(strR);
                        if (!string.IsNullOrEmpty(strResult) && iResult != -1)
                        {
                            string strtree = getSpecifiedString(strResult, "tree");
                            string strfactors = getSpecifiedString(strResult, "factors");
                            RC50Tree Rctree = new RC50Tree(strtree, strfactors, iResult, dtcnz, dtpy, Formula);
                            return Rctree;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                loglib.myLog.writeLog(e);
                return null;
            }
            return null;
        }

        /// <summary>
        /// 截取特定字符串字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string getSpecifiedString(string str, string sType)
        {
            string strIndex1 = "";
            string strIndex2 = "";
            int istart, iLength;
            switch (sType)
            {
                ///获取树字符串
                case "tree":
                    strIndex1 = "tree:\n\n";
                    strIndex2 = "\n\n\nEvaluation";
                    break;
                ///获取因素贡献度字符串
                case "factors":
                    strIndex1 = "usage:\n\n\t";
                    strIndex2 = "\n\n\nTime:";
                    break;
            }
            istart = str.IndexOf(strIndex1) + strIndex1.Length;
            iLength = str.IndexOf(strIndex2) - istart;
            return str.Substring(istart, iLength);
        }
    }
}
