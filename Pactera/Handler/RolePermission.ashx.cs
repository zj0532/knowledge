using LitJson;
using Pactera.Common.Serialization;
using Pactera.Core;
using Pactera.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Pactera.Handler
{
    /// <summary>
    /// RolePermission 的摘要说明
    /// </summary>
    public class RolePermission : Pactera.Web.BaseHttpHandler, IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string json = string.Empty;

            string action = context.Request["action"];
            switch (action)
            {
                // 获取角色列表
                case "role_list":
                    {
                        string userName = context.Request["user_name"];
                        string name = context.Request["name"];

                        json = GetRoleList(context).Replace("\"True\"", "true").Replace("\"False\"", "false");
                    }
                    break;
                case "save_permission":
                    {
                        string id = context.Request["role_id"];
                        string permission = context.Request["permission"];
                        json = SaveRolePermission(id, permission);
                    }
                    break;
                case "insert_role":
                    {
                        string roleName = context.Request["role_name"] ?? "";
                        if(roleName == "")
                        {
                            json = "请输入角色名称！";
                            break;
						}
						if (roleName.Length > 30)
						{
							json = "角色名称长度不能超过30位！";
							break;
						}

                        json = InsertRole(roleName);
                    }
                    break;
                case "update_role":
                    {
                        int roleId = 0;
                        int.TryParse(context.Request["role_id"], out roleId);
                        if(roleId < 1)
                        {
                            json = "请正常操作！";
                            break;
                        }

                        string roleName = context.Request["role_name"] ?? "";
                        if (roleName == "")
                        {
                            json = "请输入角色名称！";
                            break;
						}
						if (roleName.Length > 30)
						{
							json = "角色名称长度不能超过30位！";
							break;
						}

                        json = UpdateRole(roleId.ToString(), roleName);
                    }
                    break;
                case "delete_role":
                    {
                        int roleId = 0;
                        int.TryParse(context.Request["role_id"], out roleId);
                        if (roleId < 1)
                        {
                            json = "请正常操作！";
                            break;
                        }

						json = DeleteRole(roleId.ToString());
						break;
                    }
				case "insert_role_users":
					{
						// 获取请求对象
						var jsonData = context.Request.GetJsonData("data");

						var dbm = DataBaseFactory.Instance.Create();

						string roleId = jsonData["RoleId"].ToString();
						if(jsonData["UserIds"].IsArray)
						{
							for (int i=0;i < jsonData["UserIds"].Count;i++)
							{
								// 1 检测当前用户是否已存在，如果已存在，不执行任何操作
								Hashtable htParams = new Hashtable();
								htParams.Add("UserId", jsonData["UserIds"][i]["Id"].ToString());
								htParams.Add("RoleId", roleId);

								if (dbm.Exists("Sys_UserRole", htParams)) continue;

								// 2 如果不存在，执行新增
								dbm.Insert("Sys_UserRole", htParams);
							}

							json = "操作成功！";
							break;
						}

						json = "操作失败！";
						break;
					}
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(json);
        }

        #region >>获取角色列表<<
        private string GetRoleList(HttpContext context)
        {
            Hashtable htCondition = new Hashtable();
            var dbm = DataBaseFactory.Instance.Create();

            // 先获取角色列表
            var dtRoleList = dbm.GetDataTable("Id,RoleName", "Sys_Role", htCondition, "Id DESC");

            // 是否需要根据用户选中指定角色
            int userId = 0;
            int.TryParse(context.Request["user_id"], out userId);
            if(userId > 0)
            {
                htCondition["UserId"] = userId;
                var dtUserRoleList = dbm.GetDataTable("RoleId", "Sys_UserRole", htCondition, "Id DESC");
                // 设置主键，用于查找
                //dtUserRoleList.PrimaryKey = new DataColumn[] { dtRoleList.Columns["RoleId"] };

                // 需要根据用户来选中行，新增列
                dtRoleList.Columns.Add(new DataColumn("checked", typeof(bool)));
                foreach(DataRow roleRow in dtRoleList.Rows)
                {
                    bool isExists = false;
                    foreach(DataRow userRoleRow in dtUserRoleList.Rows)
                    {
                        if (userRoleRow["RoleId"].ToString() == roleRow["Id"].ToString())
                        {
                            isExists = true;
                            break;
                        }
                    }

                    roleRow["checked"] = isExists;
                }
            }

            return JSONHelper.DataTable2Json(dtRoleList);
        }
        #endregion

        #region >>保存角色权限<<
        private string SaveRolePermission(string id, string permission)
        {
            string json = string.Empty;

            Hashtable htField = new Hashtable();
            htField.Add("Permission", permission);

            var dbm = DataBaseFactory.Instance.Create();
            dbm.Update("Sys_Role", htField, id, "Id");

            return "操作完成！";
        }
        #endregion

        #region >>添加角色<<
        private string InsertRole(string roleName)
        {
            var dbm = DataBaseFactory.Instance.Create();

            Hashtable htField = new Hashtable();
            htField.Add("RoleName", roleName);

            // 检测是否存在
            if (dbm.Exists("Sys_Role", htField)) return "角色已存在！";

            htField.Add("IsEnable", true);
            if (dbm.Insert("Sys_Role", htField) > 0) return "添加完成！";

            return "操作没有完成，请稍后再试！";
        }
        #endregion

        #region >>更新角色<<
        private string UpdateRole(string id, string roleName)
        {
            var dbm = DataBaseFactory.Instance.Create();

            Hashtable htField = new Hashtable();
            htField.Add("RoleName", roleName);

            // 检测是否存在
            if (dbm.Exists("Sys_Role", htField)) return "角色已存在！";

            htField.Add("IsEnable", true);
            if (dbm.Update("Sys_Role", htField, id, "Id") > 0) return "更新完成！";

            return "操作没有完成，请稍后再试！";
        }
        #endregion

        #region >>更新角色<<
        private string DeleteRole(string id)
        {
            var dbm = DataBaseFactory.Instance.Create();

            // 删除角色
            dbm.Delete("Sys_Role", id, "Id");

            // 删除用户角色中间表
            Hashtable htCondition = new Hashtable();
            htCondition.Add("RoleId", id);
            dbm.Delete("Sys_UserRole", htCondition);

            return "操作完成！";
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