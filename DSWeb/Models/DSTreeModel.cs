//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace DSWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DSTreeModel
    {
        public System.Guid ModGUID { get; set; }
        public string ModName { get; set; }
        public string ModRemark { get; set; }
        public string ModStatus { get; set; }
        public Nullable<System.DateTime> ModGenerateTime { get; set; }
        public Nullable<bool> IsFile { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ModServer { get; set; }
        public string ModDataBase { get; set; }
        public string ModUid { get; set; }
        public string ModPassword { get; set; }
        public string ModDataSource { get; set; }

        public string GetConnString()
        {
            string strconn = @"data source=" + ModServer + ";initial catalog=" + ModDataBase + ";user id=" + ModUid + ";password=" + ModPassword;
            return strconn;
        }

    }
}
