using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pactera.Route
{
	public partial class WorkflowRouting : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//Response.Write("Page.RouteData.Values : " + Page.RouteData.Values["parameter"] as string + "<br />");
			Response.Write("Request.Params : " + Request["requestid"] + "<br />");
			Response.Write("Request.Params : " + Request["token"] + "<br />");

			// 1 验证身份

			// 2 获取参数


			// 3 转到审批页面
		}
	}
}