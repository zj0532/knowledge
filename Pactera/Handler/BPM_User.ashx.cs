using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pactera.Common.Serialization;
using Pactera.Data;
using System.Data.SqlClient;
using System.Data;
using Pactera.Model;
namespace Pactera.Handler
{
    /// <summary>
    /// Sys_User 用户表操作
    /// </summary>
    public class BPM_User : Pactera.Web.BaseHttpHandler, IHttpHandler
    {
        public void ProcessRequest (HttpContext context) {
            string json = string.Empty;
            string action = context.Request["action"] ?? "";

            Hashtable htCondition = new Hashtable();
            var dbm = DataBaseFactory.Instance.Create();

            switch (action)
            {
                case "auto_complete":  //用户表--全部-市场
                    {
                        var id = context.Request["id"] ?? "";
                        htCondition.Clear();
                        htCondition.Add("IsDelete", "0");
                        string name=context.Request["q"] ?? "";
                        name = name.TrimStart().TrimEnd();
                        htCondition.Add("Name LIKE '" + string.Format("%{0}%", name) + "'", new SqlParameter("@m", "1"));
                        if (!string.IsNullOrEmpty(id))
                        {
                            htCondition.Add("Id", id);
                        }
                        //获取权限
                        string rolestring = "";
                        UserInfo currentUser = CurrentSigninUser;
                        if(rolestring!="")
                        {
                            htCondition.Add(" id in( select distinct userid from Sys_UserCorp where ("+rolestring+"))", new SqlParameter("@n", "1"));
                        }

                        var dtInfo = dbm.GetDataTable("Id as [Key],Name as [Value]", "Sys_User", htCondition, "Id");
                        //添加一行空数据
                        if (name == "")
                        {
                            var row1 = dtInfo.NewRow();
                            row1[0] = "0";
                            row1[1] = "全部";
                            dtInfo.Rows.InsertAt(row1, 0);
                        }
                        json = JSONHelper.DataTable2Json(dtInfo);
                    }
                    break;
                case "auto_complete1":  //用户表-全部-协调
                    {
                        var id = context.Request["id"] ?? "";
                        htCondition.Clear();
                        htCondition.Add("IsDelete", "0");
                        string name = context.Request["q"] ?? "";
                        name = name.TrimStart().TrimEnd();
                        htCondition.Add("Name LIKE '" + string.Format("%{0}%", name) + "'", new SqlParameter("@m", "1"));

                        if (!string.IsNullOrEmpty(id))
                        {
                            htCondition.Add("Id", id);
                        }

                        var dtInfo = dbm.GetDataTable("Id as [Key],Name as [Value]", "Sys_User", htCondition, "Id");
                        //添加一行空数据
                        if (name == "")
                        {
                            var row1 = dtInfo.NewRow();
                            row1[0] = "0";
                            row1[1] = "全部";
                            dtInfo.Rows.InsertAt(row1, 0);
                        }
                        json = JSONHelper.DataTable2Json(dtInfo);
                    }
                    break;
                case "auto_xmjmcomplete":  //项目经理表--全部
                    {
                        var id = context.Request["id"] ?? "";
                        htCondition.Clear();
                        htCondition.Add("PersonName LIKE '" + string.Format("%{0}%", context.Request["q"] ?? "") + "'", new SqlParameter("@m", "1"));

                        if (!string.IsNullOrEmpty(id))
                        {
                            htCondition.Add("Id", id);
                        }
                        var dtInfo = dbm.GetDataTable("Id as [Key],PersonName as [Value]", "BPM_ProjectManager", htCondition, "Id");
                        //添加一行空数据
                        var row1 = dtInfo.NewRow();
                        row1[0] = "0";
                        row1[1] = "全部";
                        dtInfo.Rows.InsertAt(row1, 0);
                        json = JSONHelper.DataTable2Json(dtInfo);
                    }
                    break;
                case "auto_xmjmcomplete1":  //项目经理表
                    {
                        var id = context.Request["id"] ?? "";
                        htCondition.Clear();
                        htCondition.Add("PersonName LIKE '" + string.Format("%{0}%", context.Request["q"] ?? "") + "'", new SqlParameter("@m", "1"));

                        if (!string.IsNullOrEmpty(id))
                        {
                            htCondition.Add("Id", id);
                        }
                        var dtInfo = dbm.GetDataTable("Id as [Key],PersonName as [Value]", "BPM_ProjectManager", htCondition, "Id");
                        json = JSONHelper.DataTable2Json(dtInfo);
                    }
                    break;
                default:
                    break;
            }

            // 输出结果
            context.Response.ContentType = "text/plain";
            context.Response.Write(json);
        }

     
        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}