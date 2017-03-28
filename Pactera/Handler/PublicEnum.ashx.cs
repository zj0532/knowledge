using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pactera.Common.Serialization;
using Pactera.Data;
using System.Data;

namespace Pactera.Handler
{
    /// <summary>
    /// PublicEnum 的摘要说明
    /// 枚举数据字典查询处理程序
    /// </summary>
    public class PublicEnum : IHttpHandler
    {
        string json = "{}";
        string action = "";
        IDataBaseMgr dataBaseMgr = DataBaseFactory.Instance.Create();//数据库访问对象
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            HttpRequest Request = context.Request;
            action = Request["action"] ?? "";
            string field = "*";
            //组合查询条件
            Hashtable htCondition = new Hashtable();
            //int result = 0;//执行方法返回值
            switch (action)
            {
                case "enum_auto"://查询系统中新增枚举值

                    //获取必要参数
                    var ParentValue = Request["pvalue"] ?? "0";
					var InsertAll = Request["insert_select_all_item"] ?? "";

                    htCondition.Clear();
                    htCondition.Add("IsDelete", "0");//未删除
                    htCondition.Add("IsEnable", "1");//启用
                    if (!string.IsNullOrEmpty(ParentValue))//父级值
                    {
                        htCondition.Add("ParentValue", ParentValue);
                    }
                    field = "EnumValue AS [Key],EnumText as [Value]";
                    var dtenum = dataBaseMgr.GetDataTable(field, "BPM_Sys_Enum", htCondition, "EnumOrder ASC");

					if (InsertAll != "")
					{
						DataRow row = dtenum.NewRow();
						row["Key"] = "0";
						row["Value"] = "全部";
						dtenum.Rows.InsertAt(row, 0);
					}

                    //获取json字符串
                    json = JSONHelper.DataTable2Json(dtenum);
                    break;
                case "enum_auto_q"://查询系统中新增枚举值--带全部
                    //获取必要参数
                    var ParentValueq = Request["pvalue"] ?? "0";

                    htCondition.Clear();
                    htCondition.Add("IsDelete", "0");//未删除
                    htCondition.Add("IsEnable", "1");//启用
                    if (!string.IsNullOrEmpty(ParentValueq))//父级值
                    {
                        htCondition.Add("ParentValue", ParentValueq);
                    }
                    field = "EnumValue AS [Key],EnumText as [Value]";
                    var dtenumq = dataBaseMgr.GetDataTable(field, "BPM_Sys_Enum", htCondition, "EnumOrder ASC");
                    //添加一行空数据
                    var row1 = dtenumq.NewRow();
                    row1[0] = "0";
                    row1[1] = "全部";
                    dtenumq.Rows.InsertAt(row1, 0);
                    //获取json字符串
                    json = JSONHelper.DataTable2Json(dtenumq);
                    break;
                case "enum_Text"://根据key，获取text
                    //获取必要参数
                    var Value = Request["value"] ?? "0";

                    string sql = "select EnumText as [Value] from BPM_Sys_Enum where IsDelete='0' and IsEnable='1' and EnumValue='" + Value + "' order by EnumOrder";
                    json = (string)dataBaseMgr.ExecuteScalar(sql);
                    break;
                case "enum_getPid"://根据key，获取pid
                    //获取必要参数
                    Value = Request["value"] ?? "0";
                    sql = "";
                    sql = "select ParentValue as [Value] from BPM_Sys_Enum where IsDelete='0' and IsEnable='1' and EnumValue='" + Value + "' order by EnumOrder";
                    json = (string)dataBaseMgr.ExecuteScalar(sql);
                    break;
                default:
                    break;
            }

            context.Response.Write(json);
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