using Pactera.Data;
using Pactera.Model;
using System;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.Security;

namespace Pactera
{
    public partial class Login : Pactera.Web.UI.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void btnSignin_Click(object sender, EventArgs e)
        {
            // 验证输入
            if (UserName.Value == "")
            {
                Message.InnerText = "请输入用户名";
                return;
            }
            if (UserPwd.Value == "")
            {
                Message.InnerText = "请输入密码";
                return;
            }

            var dbm = DataBaseFactory.Instance.Create();

            Hashtable htCondition = new Hashtable();
            htCondition.Add("UserName", UserName.Value);
            htCondition.Add("IsDelete", "0");
            htCondition.Add("Enable", "0");
            htCondition.Add("Password", UserPwd.Value);
            //htCondition.Add("Password", FormsAuthentication.HashPasswordForStoringInConfigFile(UserPwd.Value, "MD5"));

            var dtUserInfo = dbm.GetDataTable("*", "Sys_User", htCondition, "Id");
            if (dtUserInfo.Rows.Count < 1)
            {
                Message.InnerText = "用户名或密码错误！";
                return;
            }

            // 获取用户角色
            htCondition.Clear();
            htCondition.Add("a.UserId = @UserId", new SqlParameter("@UserId", dtUserInfo.Rows[0]["Id"]));

            // 
            string field = "b.*";
            string table = "Sys_UserRole AS a LEFT OUTER JOIN Sys_Role AS b ON a.RoleId=b.Id";
            var dtUserRole = dbm.GetDataTable(field, table, htCondition, "Id");

            // 获取权限
            var dtPermission = dbm.GetDataTable("*", "Sys_Permission", new Hashtable(), "Id");

            CurrentSigninUser = UserInfo.GetInstance(dtUserInfo, dtUserRole, dtPermission);

            Response.Redirect("/Index.aspx");
        }
    }
}