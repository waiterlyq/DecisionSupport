using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace loglib
{
    public enum logtype{ Info, Debug, Warn, Fatal, Error }
    public class myLog
    {
        /// <summary>
        /// 实例化log4net
        /// </summary>
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 分类型记录日志
        /// </summary>
        /// <param name="Type">类型</param>
        /// <param name="text"></param>
        public static void writeLog(logtype Type, string text)
        {
            switch (Type)
            {
                case logtype.Info:
                    log.Info(text);
                    break;
                case logtype.Debug:
                    log.Debug(text);
                    break;
                case logtype.Warn:
                    log.Warn(text);
                    break;
                case logtype.Fatal:
                    log.Fatal(text);
                    break;
                default:
                    log.Error(text);
                    break;
            }
        }
        /// <summary>
        /// 异常日志记录
        /// </summary>
        /// <param name="ex"></param>
        public static void writeLog(Exception ex)
        {
            log.Error(ex.Message, ex);
        }
    }
}
