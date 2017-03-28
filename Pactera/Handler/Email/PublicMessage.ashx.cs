using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pactera.Data;
using Pactera.Common.Serialization;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Pactera.Model;
using Pactera.Web;

namespace Pactera.Handler.Email
{
    /// <summary>
    /// PublicMessage 的摘要说明
    /// </summary>
    public class PublicMessage : Pactera.Web.BaseHttpHandler, IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string json = string.Empty;
            string Action = context.Request["Action"] ?? "";
            switch (Action)
            {
                case "GetEngineer":
                    json = getGetEngineer(context);
                    break;
                case "SenderMessage":
                    json = AddMessage(context);
                    break;
                case "SMSList":
                    json = SMSList(context);
                    break;
                case "delete_SMS":
                    json = delete_SMS(context);
                    break;
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(json);
        }

        private string getGetEngineer(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();
            DataTable dt = dbm.GetDataTable("*","Sys_User",new Hashtable(),"Id");
            return Pactera.Common.Serialization.JSONHelper.DataTable2Json(dt);
        }

        private string AddMessage(HttpContext context)
        {
            string Id = context.Request["Id"] ?? "";
            string txtContext = context.Request["txtContext"] ?? "";

            var dbm = DataBaseFactory.Instance.Create();
            /////根据工程师Id获取工程师手机号码
            DataTable dt = dbm.GetDataTable("*", "Sys_User", Id, "Id");
            if (dt.Rows[0]["Mobile"].ToString() == "" || dt.Rows[0]["Mobile"].ToString() == null)
            {
                return new HandlerResponse(1, "该工程师手机号码暂未添加！").ToJson();
            }
            string SysUserId = dt.Rows[0]["Name"].ToString();//工程师ID
            Hashtable htField = new Hashtable();
            htField.Add("Receiver", SysUserId);//接受短信的工程师
            htField.Add("Sender", CurrentSigninUser.Name);//当前登录用户ID
            htField.Add("MessageContent", txtContext);
            htField.Add("SendingTime", DateTime.Now);

            int addMessage = dbm.Insert("Cus_Email", htField);
            if (addMessage > 0) return new HandlerResponse(0, "操作完成！").ToJson();

            return new HandlerResponse(1, "短信发送成功").ToJson();
        }

        private string SMSList(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();

            int rows = Convert.ToInt32(context.Request["rows"]);//页大小
            int page = Convert.ToInt32(context.Request["page"]);//当前页
            int pageCount = 0;//页总数
            int total = 0;//数据总数

            var dt = dbm.GetDataTable("Id", "*", "Cus_Email",  new Hashtable(), "Id desc", page, rows, out total, out pageCount);
            
            return "{\"total\":" + total + ",\"rows\":" + JSONHelper.DataTable2Json(dt) + "}";

        }

        private string delete_SMS(HttpContext context)
        {
            string Id = context.Request["Id"] ?? "";
            if (Id == "") { return "参数异常！"; }

            var dbm = DataBaseFactory.Instance.Create();
            int dtDeleteCustom = dbm.Delete("Cus_Email", Id, "Id");
            if (dtDeleteCustom > 0)
            {
                return "操作成功！";
            }
            else { return new HandlerResponse(1, "操作失败！").ToJson(); }
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