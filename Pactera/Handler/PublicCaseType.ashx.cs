using System;
using System.Collections;
using System.Linq;
using System.Web;
using Pactera.Data;
using System.Data;

namespace Pactera.Handler
{
    /// <summary>
    /// PublicCaseType 的摘要说明
    /// </summary>
    public class PublicCaseType : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string json = string.Empty;
            string action = context.Request["Action"] ?? "";

            switch (action)
            {
                case "GetCaseType":
                    json = GetCaseType(context);
                    break;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(json);
        }

        /// <summary>
        /// Case类型
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetCaseType(HttpContext context)
        {
            // 获取参数
            string r = context.Request["r"] ?? "";
            string parentId = context.Request["ParentId"] ?? "";
            string insertSelectAll = context.Request["InsertSelectAll"] ?? "";

            Hashtable htCondition = new Hashtable();
            htCondition.Add("ParentId", parentId);

            var dtCaseType = DataBaseFactory.Instance.Create().GetDataTable("*", "Cus_CaseType", htCondition, "Text");
            if (insertSelectAll != "")
            {
                DataRow row = dtCaseType.NewRow();
                // 这里一会你把对应的字段都写好写好了
                // 其他的呢？不用么？mei了就这些
                row["Id"] = "0";
                row["Text"] = "全部";

                // 把新创建的这个数据插入到第一行
                dtCaseType.Rows.InsertAt(row, 0);

                // 提交自上次调用 System.Data.DataTable.AcceptChanges() 以来对该表进行的所有更改。
                dtCaseType.AcceptChanges();
            }

            return Pactera.Common.Serialization.JSONHelper.DataTable2Json(dtCaseType).Replace("\\", "\\\\");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}