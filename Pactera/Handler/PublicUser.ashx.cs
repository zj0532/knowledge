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

namespace Pactera.Handler
{
    /// <summary>
    /// PublicUser 的摘要说明
    /// </summary>
    public class PublicUser : Pactera.Web.BaseHttpHandler, IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            string json = string.Empty;
            string action = context.Request["Action"];

            switch (action)
            {
                // 以后你的项目里，所有有关参数的都用这种规则 GetUserInfo开头字母大写，骆驼命名法
                case "GetUserInfo":
                    json = GetUserInfo(context);
                    break;
                case "get_simple_user_list":
                    json = GetSimpleUserList(context);
                    break;
                // 获取用户列表
                case "get_user_list":
                    json = GetUserListJson(context);
                    break;
                case "get_approve_user_list_json":
                    json = GetApproveUserListJson(context);
                    break;
                // 安全退出
                case "safe_quit":
                    {
                        context.Session.Clear();
                        json = "{\"message\":\"成功！\"}";
                        break;
                    }
                // 更改用户信息
                case "UpdateUserInfo":
                    json = UpdateUserInfo(context);
                    break;
                // 更新用户角色
                case "update_role":
                    {
                        var dbm = DataBaseFactory.Instance.Create();

                        int userId = 0;

                        int.TryParse(context.Request["user_id"], out userId);
                        string roleIds = context.Request["role_ids"] ?? "";

                        if (userId == 0)
                        {
                            json = "请求参数不正确！";
                            break;
                        }

                        Hashtable htParam = new Hashtable();

                        // 删除角色
                        htParam.Add("UserId", userId);
                        dbm.Delete("Sys_UserRole", htParam);

                        // 如果没有传递任何角色，只做清除操作
                        if (roleIds == "")
                        {
                            json = "操作完成！";
                            break;
                        }

                        // 新增角色
                        string[] ids = roleIds.Split(',');
                        foreach (string roleId in ids)
                        {
                            htParam["RoleId"] = roleId;
                            dbm.Insert("Sys_UserRole", htParam);
                        }

                        json = "操作完成！";
                        break;
                    }
                // 新增用户
                case "CreateNewUser":
                    json = CreateUser(context);
                    break;
                // 删除用户
                case "delete_user":
                    json = DeleteUser(context);
                    break;
                // 获取用户角色名称
                case "get_user_role_text":
                    json = CreateUser(context);
                    break;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(json);
        }

        #region >>获取简单的用户列表<<
        /// <summary>
        /// 获取简单的用户列表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetSimpleUserList(HttpContext context)
        {
            string userName = context.Request["user_name"] ?? "";
            string name = context.Request["name"] ?? "";

            string order = context.Request["order"] ?? "desc";//排序
            int rows = Convert.ToInt32(context.Request["rows"]);//页大小
            int page = Convert.ToInt32(context.Request["page"]);//当前页
            int pageCount = 0;//页总数
            int total = 0;//数据总数

            var dbm = DataBaseFactory.Instance.Create();
            Hashtable htCondition = new Hashtable();
            htCondition.Add("IsDelete", 0);

            if (userName != "") htCondition.Add("UserName LIKE @UserName", new SqlParameter("@UserName", "%" + userName + "%"));
            if (name != "") htCondition.Add("Name LIKE @Name", new SqlParameter("@Name", "%" + name + "%"));

            string field = "Id,UserName,Name";
            string table = "Sys_User";

            // 获取全部用户
            var dtUserList = dbm.GetDataTable("Id", field, table, htCondition, "Id DESC", page, rows, out total, out pageCount);

            return "{\"total\":" + total + ",\"rows\":" + JSONHelper.DataTable2Json(dtUserList) + "}";
        }
        #endregion

        #region >>获取选择审批人用户列表JSON<<
        public string GetApproveUserListJson(HttpContext context)
        {
            string pkCorp = context.Request["pk_corp"];
            string roleId = context.Request["role_id"] ?? "";

            var dbm = DataBaseFactory.Instance.Create();

            // 根据角色ID找到角色下所有用户
            Hashtable htCondition = new Hashtable();
            GetUserCorpCondition(CurrentSigninUser, pkCorp, htCondition);
            htCondition.Add("a.RoleId = @RoleId", new SqlParameter("@RoleId", roleId));

            string field = "DISTINCT b.Id,b.Name,d.RoleName";
            string table = @"Sys_UserRole AS a 
				LEFT OUTER JOIN Sys_User AS b ON a.UserId=b.Id 
				LEFT OUTER JOIN Sys_UserCorp AS c ON a.UserId=c.UserId 
				LEFT OUTER JOIN Sys_Role AS d ON a.RoleId=d.Id";
            var dtUsers = dbm.GetDataTable(field, table, htCondition, "b.Id");

            return JSONHelper.DataTable2Json(dtUsers);
        }
        #endregion

        #region >>获取指定公司的所有上级公司<<
        /// <summary>
        /// 获取当前公司的上级公司
        /// </summary>
        /// <param name="corpList"></param>
        /// <param name="pkCorp"></param>
        private void GetHigherUpsCorp(List<string> corpList, string pkCorp)
        {
            // 1 将查询结果添加到集合
            corpList.Add(pkCorp);

            // 2 根据结果查询父级ID
            var dbm = DataBaseFactory.Instance.Create();
            Hashtable htCondition = new Hashtable();
            htCondition.Add("PK_CORP", pkCorp);

            // 2.1 获取父级ID
            string fatherCorp = dbm.GetFieldValue("BPM_Corp", "FATHERCORP", htCondition, null);
            if (string.IsNullOrEmpty(fatherCorp)) return;

            GetHigherUpsCorp(corpList, fatherCorp);
        }
        #endregion

        #region >>获取公司查询级联查询条件<<
        public void GetUserCorpCondition(UserInfo user, string pkCorp, Hashtable htCondition)
        {
            // 1 找出用户所在公司以及父级公司
            string defaultCorp = string.Empty;
            List<string> corpList = new List<string>();

            // 1.1 判断是根据公司还是人来读取公司角色
            if (string.IsNullOrEmpty(pkCorp))
            {

            }
            else
            {
                // 1.2 递归找到所有父级公司
                GetHigherUpsCorp(corpList, pkCorp);
            }

            // 去除重复的公司主键
            corpList = corpList.Distinct().ToList();

            // 2.1 拼接公司查询条件
            StringBuilder sbCorpCondition = new StringBuilder();
            if (corpList.Count > 1)
            {
                foreach (string corp in corpList)
                {
                    if (sbCorpCondition.Length < 1)
                    {
                        sbCorpCondition.AppendFormat("(c.PK_CORP = '{0}'", corp);
                        continue;
                    }

                    sbCorpCondition.AppendFormat(" OR c.PK_CORP = '{0}'", corp);
                }
                sbCorpCondition.Append(")");
            }
            else
            {
                sbCorpCondition.AppendFormat("(c.PK_CORP = '{0}')", corpList[0]);
            }

            htCondition.Add(sbCorpCondition, null);
        }
        #endregion

        #region >>获取用户列表JSON<<

        public string GetUserListJson(HttpContext context)
        {
            string userName = context.Request["user_name"] ?? "";
            string name = context.Request["name"] ?? "";
            string role = context.Request["role"] ?? "";
            string oneArea = context.Request["oneArea"] ?? "";
            string twoArea = context.Request["twoArea"] ?? "";
            string threeArea = context.Request["threeArea"] ?? "";

            string order = context.Request["order"] ?? "desc";//排序
            int rows = Convert.ToInt32(context.Request["rows"]);//页大小
            int page = Convert.ToInt32(context.Request["page"]);//当前页
            int pageCount = 0;//页总数
            int total = 0;//数据总数

            var dbm = DataBaseFactory.Instance.Create();
            Hashtable htCondition = new Hashtable();
            htCondition.Add("t.IsDelete = @IsDelete", new SqlParameter("@IsDelete", "0"));

            if (userName != "") htCondition.Add("t.UserName LIKE @UserName", new SqlParameter("@UserName", "%" + userName + "%"));
            if (name != "") htCondition.Add("t.Name LIKE @Name", new SqlParameter("@Name", "%" + name + "%"));
            if (role != "") htCondition.Add("t.RoleNames LIKE @RoleName", new SqlParameter("@RoleName", "%" + role + "%"));
            if (oneArea != "") htCondition.Add("OneArea", oneArea);
            if (twoArea != "") htCondition.Add("TwoArea", twoArea);
            if (threeArea != "") htCondition.Add("ThreeArea", threeArea);

            string field = "t.Id,t.UserName,t.Name,t.Phone,t.Email,t.Address,t.Enable,t.OneArea,t.TwoArea,t.ThreeArea,t.RoleIds,t.RoleNames";
            //string table = "Sys_User";
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("(select Id,UserName,Name,Phone,Email,Address,Enable,OneArea,TwoArea,ThreeArea,IsDelete,");
            sbTable.Append("(select cast(ro.Id as varchar(15)) + ',' from Sys_UserRole ur left join Sys_Role ro on ur.RoleId=ro.Id where ur.UserId=u.Id for xml path('')) RoleIds,");
            sbTable.Append("(select ro.RoleName + ',' from Sys_UserRole ur left join Sys_Role ro on ur.RoleId=ro.Id where ur.UserId=u.Id for xml path('')) RoleNames ");
            sbTable.Append("from Sys_User u) t");

            // 获取全部用户
            var dtUserList = dbm.GetDataTable("t.Id", field, sbTable.ToString(), htCondition, "t.Id DESC", page, rows, out total, out pageCount);

            // 处理掉过长的数据
            dtUserList.Columns.Add(new DataColumn("ShortRoleName"));
            foreach (DataRow row in dtUserList.Rows)
            {
                string roleNames = row["RoleNames"].ToString();

                // 先去掉后面的逗号
                roleNames = roleNames.TrimEnd(',');

                roleNames = roleNames.Length > 35 ? roleNames.Substring(0, 35) + "..." : roleNames;

                row["ShortRoleName"] = roleNames;
                string Enable = row["Enable"].ToString();
                if (Enable == "0") { row["Enable"] = "使用"; }
                else { row["Enable"] = "过期"; }
            }

            return "{\"total\":" + total + ",\"rows\":" + JSONHelper.DataTable2Json(dtUserList) + "}";
        }
        #endregion

        #region >>新增用户<<
        private string CreateUser(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();

            string userName = context.Request["UserName"] ?? "";
            string password = context.Request["Password"] ?? "";
            string name = context.Request["Name"] ?? "";
            string phone = context.Request["Phone"] ?? "";
            string Mobile = context.Request["Mobile"]??"";
            string Repassword = context.Request["Repassword"];
            string Email = context.Request["Email"] ?? "";
            string Address = context.Request["Address"] ?? "";
            string Enable = context.Request["Enable"] ?? "";
            string OneArea = context.Request["OneArea"] ?? "";
            string TwoArea = context.Request["TwoArea"] ?? "";
            string ThreeArea = context.Request["ThreeArea"] ?? "";
            if (OneArea == "") { return new HandlerResponse(1, "省份为必填项").ToJson(); }
            if (TwoArea == "") { return new HandlerResponse(1, "城市为必填项").ToJson(); }
            if (ThreeArea == "") { return new HandlerResponse(1, "地区必填项").ToJson(); }
            if (userName == "") { return new HandlerResponse(1, "用户名为必填项").ToJson(); }
            if (password == "") { return new HandlerResponse(1, "密码为必填项").ToJson(); }
            if (Repassword == "") { return new HandlerResponse(1, "重复密码为必填项").ToJson(); }
            if (!(password.Equals(Repassword)))
            {
                return new HandlerResponse(1, "两次输入密码不一致").ToJson();
            }

            // 检测用户名是否已存在
            Hashtable htCondition = new Hashtable();
            htCondition.Add("UserName", userName);
            if (dbm.Exists("Sys_User", htCondition)) return new HandlerResponse(1, "用户名已存在！").ToJson();
            if (phone == "") { return new HandlerResponse(1, "电话为必填项").ToJson(); }
            if (Mobile == "") { return new HandlerResponse(1, "手机为必填项").ToJson(); }
            if (Email == "") { return new HandlerResponse(1, "电子邮件为必填项").ToJson(); }
            if (Address == "") { return new HandlerResponse(1, "工程师办公地址为必填项").ToJson(); }
            // 新增用户 你写
            Hashtable htField = new Hashtable();
            htField.Add("Name", name);
            htField.Add("UserName", userName);
            htField.Add("Password", password);
            htField.Add("Phone", phone);
            htField.Add("Email", Email);
            htField.Add("Address", Address);
            htField.Add("Enable", Enable);
            htField.Add("OneArea", OneArea);
            htField.Add("TwoArea", TwoArea);
            htField.Add("ThreeArea", ThreeArea);
            htField.Add("CreateDate", DateTime.Now);

            int result = dbm.Insert("Sys_User", htField);
            if (result > 0) return new HandlerResponse(0, "操作完成！").ToJson();

            return new HandlerResponse(1, "新增用户时失败，请稍后再试或联系管理员！").ToJson();
        }
        #endregion

        #region >>删除用户<<
        private string DeleteUser(HttpContext context)
        {
            int id = 0;
            int.TryParse(context.Request["id"], out id);
            if (id < 1) return "请正常操作";

            var dbm = DataBaseFactory.Instance.Create();

            // 删除用户
            Hashtable htField = new Hashtable();
            htField.Add("IsDelete", true);
            dbm.Update("Sys_User", htField, id.ToString(), "Id");

            // 删除用户对应的角色
            Hashtable htCondition = new Hashtable();
            htCondition.Add("UserId", id);
            dbm.Delete("Sys_UserRole", htCondition);


            return "操作完成！";
        }
        #endregion

        #region >>更新用户信息<<
        /// <summary>
        /// 更改用户信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string UpdateUserInfo(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();

            string userId = context.Request["Id"] ?? "";
            string name = context.Request["Name"] ?? "";
            string phone = context.Request["Phone"] ?? "";
            string Email = context.Request["Email"] ?? "";
            string Address = context.Request["Address"] ?? "";
            string Enable = context.Request["Enable"] ?? "";
            string OneArea = context.Request["OneArea"] ?? "";
            string TwoArea = context.Request["TwoArea"] ?? "";
            string ThreeArea = context.Request["ThreeArea"] ?? "";
            string Mobile = context.Request["Mobile"] ?? "";

            if (phone == "" && Mobile == "") { return new HandlerResponse(1, "联系电话和手机至少要填写一项！").ToJson(); }

            Hashtable htField = new Hashtable();
            htField.Add("Name", name);
            htField.Add("Phone", phone);
            htField.Add("Mobile", Mobile);
            htField.Add("Email", Email);
            htField.Add("Address", Address);
            htField.Add("Enable", Enable);
            htField.Add("OneArea", OneArea);
            htField.Add("TwoArea", TwoArea);
            htField.Add("ThreeArea", ThreeArea);

            if (dbm.Update("Sys_User", htField, userId, "id") > 0)
            {
                return new HandlerResponse(0, "操作成功！").ToJson();
            }
            else { return new HandlerResponse(1, "操作失败！").ToJson(); }

            
        }
        #endregion

        #region >>获取用户信息<<
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetUserInfo(HttpContext context)
        {
            string userId = context.Request["Id"] ?? "";
            if (userId == "") return "参数异常！";

            //组合查询条件
            var dbm = DataBaseFactory.Instance.Create();
            var dtOwner = dbm.GetDataTable("*", "Sys_User", userId, "Id");
            if (dtOwner.Rows.Count < 1) return "没有找到指定的用户！";

            //获取json字符串
            return JSONHelper.DataTable2Json(dtOwner).Replace("[", "").Replace("]", "");
        }
        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}