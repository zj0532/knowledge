using Pactera.Common.Serialization;
using Pactera.Data;
using Pactera.Model;
using Pactera.Model.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Pactera.Handler
{
    /// <summary>
    /// Menu 的摘要说明
    /// </summary>
    public class Menu : Pactera.Web.BaseHttpHandler, IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string roleId = context.Request["role_id"] ?? "";

            string json = GetMenuPermissionTree(roleId);
            context.Response.Write(json);
        }

        #region >>获取菜单权限树<<
        private string GetMenuPermissionTree(string roleId)
        {
            string json = string.Empty;

            // 1 获取全部菜单和对应的菜单权限
            var dbm = DataBaseFactory.Instance.Create();

            Hashtable htCondition = new Hashtable();
            htCondition.Add("IsEnable", true);

            // 1.1 获取权限
            string permission = dbm.GetFieldValue("Sys_Role", "Permission", roleId, "Id");

            var dtMenu = dbm.GetDataTable("*", "Sys_Menu", htCondition, "Id");
            List<MenuJson> list = GenerateMenuInfo(permission, dtMenu);

            // 2 转换成Json数据
			json = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            return json.Replace("\"Checked\":", "\"checked\":");
        }
        private List<MenuJson> GenerateMenuInfo(string rolePermission, DataTable dtMenu)
        {
            List<MenuJson> list = new List<MenuJson>();

            foreach(DataRow row in dtMenu.Rows)
            {
                int menuType = int.Parse(row["MenuType"].ToString());
                if (menuType != 0) continue;

                MenuJson menu = new MenuJson();
                menu.id = int.Parse(row["Id"].ToString());
                menu.text = row["MenuText"].ToString();
                menu.Url = row["Url"].ToString();
                menu.MenuType = menuType;
                menu.ParentId = int.Parse(row["ParentId"].ToString());
                menu.IsEnable = Convert.ToBoolean(row["IsEnable"]);

                GenerateChildren(menu, dtMenu, rolePermission.Split(','));

                list.Add(menu);
            }

            return list;
        }
        private void GenerateChildren(MenuJson menuInfo, DataTable dtMenu, string[] permission)
        {
            foreach(DataRow row in dtMenu.Rows)
            {
                int menuType = int.Parse(row["MenuType"].ToString());
                int parentId = int.Parse(row["ParentId"].ToString());

                if (menuInfo.id != parentId) continue;

                MenuJson menu = new MenuJson();
                menu.id = int.Parse(row["Id"].ToString());
                menu.text = row["MenuText"].ToString();
                menu.Url = row["Url"].ToString();
                menu.MenuType = menuType;
                menu.iconCls = "tree-file";
                menu.ParentId = parentId;
                menu.IsEnable = Convert.ToBoolean(row["IsEnable"]);

                // 获取出所有的权限
                Hashtable htCondition = new Hashtable();
                htCondition.Add("MenuId", menu.id);
                var dbm = DataBaseFactory.Instance.Create();

                var dtPerm = dbm.GetDataTable("*", "Sys_Permission", htCondition, "Id");
                foreach (DataRow permissionRow in dtPerm.Rows)
                {
                    PermissionJson permissionJson = new PermissionJson();
                    permissionJson.id = int.Parse(permissionRow["Id"].ToString());
                    permissionJson.Code = permissionRow["Code"].ToString();
                    permissionJson.MenuId = int.Parse(permissionRow["MenuId"].ToString());
                    permissionJson.text = permissionRow["Text"].ToString();
                    permissionJson.Description = permissionRow["Description"].ToString();
                    permissionJson.iconCls = "icon-man";
                    // TODO 张春雨 - 权限菜单 - 获取真实用户权限
                    permissionJson.Checked = permission.Contains(permissionRow["Code"].ToString());

                    if (menu.children == null) menu.children = new List<BaseTreeItemJson>();
                    menu.children.Add(permissionJson);
                }

                if (menuInfo.children == null) menuInfo.children = new List<BaseTreeItemJson>();
                menuInfo.children.Add(menu);

                GenerateChildren(menu, dtMenu, permission);
            }
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