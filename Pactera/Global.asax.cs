using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using Quartz;
using Quartz.Job;
using Quartz.Impl;

namespace Pactera
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
			RegisterRoutes(RouteTable.Routes);

			// 设置log4net配置
			Pactera.Common.Logger.SetConfig();

			// 启动调度器
			Pactera.Core.Scheduler.Instance.Start();

			// TODO 张春雨 - 是否应该在应用启动时检查是否有未完成的调度
		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{

		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{
			Pactera.Core.Scheduler.Instance.Shutdown();
		}

		/// <summary>
		/// 注册路由
		/// </summary>
		/// <param name="routes"></param>
		protected void RegisterRoutes(RouteCollection routes)
		{
			// 默认页
			//routes.MapPageRoute("Defautl", "abc", "~/Index.aspx");

			// 对{folder}/{webform}形式的URL进行路由
			//routes.MapPageRoute("WebForm1", "{folder}/{webform}", "~/{folder}/{webform}.aspx");

			// 对{folder}/{page}形式的URL进行路由(带参数)
			//routes.MapPageRoute("oarequest", "{floder}/{webform}/{parameter}", "~/Route/WorkflowRouting.aspx");
            routes.MapPageRoute("OaRequest", "oarequest", "~/Transfer.aspx");
			//routes.MapPageRoute("OaRequestParameter", "oarequest/{parameter}", "~/Route/WorkflowRouting.aspx");
		}
	}
}