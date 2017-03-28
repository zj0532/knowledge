using System;
using System.IO;
using System.Web;
using Pactera.Model;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web.SessionState;
using System.Linq;
using System.Web.Routing;

namespace Pactera.Core
{
    public class PacteraHttpModule : IHttpModule, IRequiresSessionState
    {
        /// <summary>
        /// 获取或设置当前在线的用户
        /// </summary>
        public static List<UserInfo> OnlineUsers = new List<UserInfo>();

        /// <summary>
        /// 要忽略权限验证的页面
        /// 只限于 .net 内置的页面 .aspx .ashx 等...
        /// </summary>
        private string[] _ignorePermissionPath = new string[] {
            "/ImportCaseTypeFromExcel.aspx",
			"/DataList.aspx",
            "/Handler/ExportExcel.aspx",
            "/login.aspx",
			"/Transfer.aspx",
			"/Workflow/WorkflowConfigure.aspx"
        };

        public void Init(HttpApplication context)
        {
            context.AcquireRequestState += context_AcquireRequestState;
            context.BeginRequest += context_BeginRequest;
			context.Error += context_Error;
        }

		#region >>应用程序异常<<
		void context_Error(object sender, EventArgs e)
		{
			Exception exception = HttpContext.Current.Server.GetLastError().InnerException;
			HttpContext.Current.Response.Write(exception.ToString().Replace("\r\n", "<br />"));
			HttpContext.Current.Response.Write("<br />");
			HttpContext.Current.Response.Write("Message : " + exception.Message + "<br />");
			HttpContext.Current.Server.ClearError();

			if (exception.GetType() == typeof(PacteraException.DataValidationException))
			{
				HttpContext.Current.Response.Write("验证异常！");
			}
			
			/*//在出现未处理的错误时运行的代码
        Exception objExp = HttpContext.Current.Server.GetLastError();
        string username = "";
        string userid = "";
        if (Session["ulogin"] != null)
        { 
            string[] uinfo=Session["ulogin"].ToString().Split('|');
            userid = uinfo[0];
            username = uinfo[1];
        }
        Aotain114.Public.LogHelper.WriteLog("\r\n用户ID:"+userid+"\r\n用户名:"+username+"\r\n客户机IP:" + Request.UserHostAddress + "\r\n错误地址:" + Request.Url + "\r\n异常信息:" + Server.GetLastError().Message, objExp);*/

		}
		#endregion

		void context_BeginRequest(object sender, EventArgs e)
        {
            
        }

        private void context_AcquireRequestState(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            HttpContext context = application.Context;
			HttpRequest request = context.Request;

            // 获取URL目录数组
            string[] url = context.Request.Url.Segments;
            string ext = Path.GetExtension(context.Request.Url.LocalPath);
            string name = Path.GetFileName(context.Request.Url.LocalPath);

            // 只验证特定的文件
            if (ext != ".aspx") return;

            // 放行的页面
            if (_ignorePermissionPath.Contains(context.Request.Url.AbsolutePath)) return;

            // 获取登录的用户，如果未登录，直接跳转至登录页面
            UserInfo userInfo = context.Session["CURRENT_SIGNIN_USER"] as UserInfo;
            if(userInfo == null)
            {
                context.Response.ContentType = "text/html; charset=utf-8";

                context.Response.Clear();
                context.Response.Write("<script type=\"text/javascript\">");
                context.Response.Write("alert('登录超时或未登录，请重新登录');");
                context.Response.Write("window.parent.parent.parent.location.href = '/login.aspx';");
                context.Response.Write("</script>");
                context.Response.End();
                return;
            }

        }

        public void Dispose()
        {

        }
    }
}
