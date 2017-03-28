using Pactera.Core;
using Pactera.Data;
using Pactera.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pactera
{
	public partial class Transfer : Pactera.Web.UI.BasePage
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			// 获取Token
			string token = Request["token"];

			DataTable dtTransfer = TransferLinkService.GetTransferByToken(token);
			if (dtTransfer.Rows.Count < 1)
			{
				//Response.Redirect("/Signin.aspx");
				Response.Write("此链接已过期，请登录系统进行操作！");
				Response.End();
				return;
			}

			// 接收跳转后清理当前的跳转入口
			TransferLinkService.DeleteTransferByToken(token);

			// 获取跳转信息
			string userId = dtTransfer.Rows[0]["UserId"].ToString();
			string targetUrl = dtTransfer.Rows[0]["TargetUrl"].ToString();

			// 登录当前账户
			var dbm = DataBaseFactory.Instance.Create();
			Hashtable htCondition = new Hashtable();

			var dtUserInfo = dbm.GetDataTable("*", "Sys_User", userId, "Id");
			if (dtUserInfo.Rows.Count < 1)
			{
				Response.Write("您不是此系统的有效用户");
				Response.End();
				return;
			}

			// 检测用户是否已被删除
			if(dtUserInfo.Rows[0]["IsDelete"].ToString() == "1")
			{
				Response.Write("此账户已被删除，请联系管理员！");
				Response.End();
				return;
			}

			// 获取用户角色
			htCondition.Clear();
			htCondition.Add("a.UserId = @UserId", new SqlParameter("@UserId", dtUserInfo.Rows[0]["Id"]));

			string field = "b.*";
			string table = "Sys_UserRole AS a LEFT OUTER JOIN Sys_Role AS b ON a.RoleId=b.Id";
			var dtUserRole = dbm.GetDataTable(field, table, htCondition, "Id");

			// 获取用户公司
			field = "b.*";
			table = "Sys_UserCorp AS a LEFT OUTER JOIN BPM_Corp AS b ON a.PK_CORP=b.PK_CORP";
			var dtUserCorp = dbm.GetDataTable(field, table, htCondition, "Id");

			// 获取权限
			var dtPermission = dbm.GetDataTable("*", "Sys_Permission", new Hashtable(), "Id");

			CurrentSigninUser = UserInfo.GetInstance(dtUserInfo, dtUserRole, dtPermission);

			Response.Redirect(targetUrl);
		}
	}
}