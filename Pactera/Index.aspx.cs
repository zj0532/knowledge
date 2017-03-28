using Pactera.Data;
using Pactera.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pactera
{
    public partial class Index : Pactera.Web.UI.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			
        }

        /// <summary>
        /// 生成菜单
        /// </summary>
        /// <returns></returns>
        protected string GenerateMenu()
        {
            var dbm = DataBaseFactory.Instance.Create();
			Hashtable htCondition = new Hashtable();
			htCondition.Add("IsEnable", true);

            // 找到所有菜单,根据ID排序
			var dtMenu = dbm.GetDataTable("*", "Sys_Menu", htCondition, "Id");

            string menuItem = GenerateFirstMenu(dtMenu);
            return menuItem;
        }

        private string GenerateFirstMenu(DataTable dtMenu)
        {
            StringBuilder sbMenu = new StringBuilder();

			var rowMenuItems = from p in dtMenu.AsEnumerable() where Convert.ToInt32(p["ParentId"]) == 0 orderby p["DisplayOrder"] select p;
			foreach (DataRow row in rowMenuItems)
            {
                int id = int.Parse(row["Id"].ToString());
                string imgSrc = row["MenuImgSrc"].ToString();
                string menuText = row["MenuText"].ToString();
				string menuUrl = row["Url"].ToString();
				int parentId = int.Parse(row["ParentId"].ToString());

                // 已经生成好的二级菜单
                string menuItems = GenerateSecondMenu(id, dtMenu);

                if (!string.IsNullOrEmpty(menuItems))
                {
                    // 生成一级菜单
                    sbMenu.AppendFormat("<div title=\"{0}\" data-options=\"iconCls:'icon-sys'\">", menuText);
                    sbMenu.Append("<ul>");
                    sbMenu.Append(menuItems);
                    sbMenu.Append("</ul>");
                    sbMenu.Append("</div>");
                }
            }

            return sbMenu.ToString();
        }

        private string GenerateSecondMenu(int id, DataTable dtMenu)
        {
            StringBuilder sbItem = new StringBuilder();
			var rowMenuItems = from p in dtMenu.AsEnumerable() where Convert.ToInt32(p["ParentId"]) == id orderby p["DisplayOrder"] select p;

			foreach (DataRow row in rowMenuItems)
            {
                int menuId = int.Parse(row["Id"].ToString());
                int parentId = int.Parse(row["ParentId"].ToString());
                string menuText = row["MenuText"].ToString();
                string menuUrl = row["Url"].ToString();

                foreach (RoleInfo role in CurrentSigninUser.Role)
                {
                    var per = role.Permission.FirstOrDefault(p => p.MenuId == menuId);
                    if (per != null)
                    {
                        // 有这个菜单的权限
                        sbItem.AppendFormat("<li><a href=\"javascript:void(0)\" onclick=\"addTab('{0}', '{1}');\">{0}</a></li>", menuText, menuUrl);
                    }
                }
            }

            return sbItem.ToString();
        }
    }
}
