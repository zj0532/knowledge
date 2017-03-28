using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Pactera.Model
{
    [Serializable]
    public class UserInfo
    {
        public override string ToString()
        {
            var jsonData = new LitJson.JsonData();
            jsonData["Id"] = Id;
            jsonData["Name"] = Name;
            jsonData["UserName"] = UserName;
            jsonData["Enable"] = Enable;
            jsonData["IsDelete"] = IsDelete;
            jsonData["CreateDate"] = CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
            jsonData["OneArea"] = OneArea;
            jsonData["TwoArea"] = TwoArea;
            jsonData["ThreeArea"] = ThreeArea;

            return LitJson.JsonMapper.ToJson(jsonData);
        }

        public UserInfo()
        {
            Role = new List<RoleInfo>();
        }

        private static UserInfo _userInfo;

        public int Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public List<RoleInfo> Role { get; set; }

        public int IsDelete { get; set; }

        public int Enable { get; set; }

        public DateTime CreateDate { get; set; }

        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string OneArea { get; set; }
        public string TwoArea { get; set; }
        public string ThreeArea { get; set; }

        /// <summary>
        /// 获取用户实例
        /// </summary>
        /// <param name="dtUserInfo"></param>
        /// <param name="dtUserRoleList"></param>
        /// <param name="dtPermission"></param>
        /// <returns></returns>
		public static UserInfo GetInstance(DataTable dtUserInfo, DataTable dtUserRoleList, DataTable dtPermission)
		{
			DataRow row = dtUserInfo.Rows[0];
			_userInfo = new UserInfo();
            _userInfo.Role = new List<RoleInfo>();

			_userInfo.Id = int.Parse(row["Id"].ToString());
			_userInfo.Name = row["Name"].ToString();
			_userInfo.UserName = row["UserName"].ToString();
			_userInfo.Password = row["Password"].ToString();
			_userInfo.IsDelete = int.Parse(row["IsDelete"].ToString());
			_userInfo.Enable = int.Parse(row["Enable"].ToString());
			_userInfo.CreateDate = DateTime.Parse(row["CreateDate"].ToString());
            _userInfo.OneArea = row["OneArea"].ToString();
            _userInfo.TwoArea = row["TwoArea"].ToString();
            _userInfo.ThreeArea = row["ThreeArea"].ToString();

			// 获取角色信息
			foreach (DataRow r in dtUserRoleList.Rows)
			{
				int roleId = 0;
				if (!int.TryParse(r["Id"].ToString(), out roleId)) continue;

				RoleInfo role = new RoleInfo();
				role.Id = roleId;
				role.RoleName = r["RoleName"].ToString();

				// 获取权限信息
				string[] permissionArray = r["Permission"].ToString().Split(',');
				foreach (DataRow rowPer in dtPermission.Rows)
				{
					if (permissionArray.Contains(rowPer["Code"].ToString()))
					{
						PermissionInfo permissionInfo = new PermissionInfo();
						permissionInfo.Id = int.Parse(rowPer["Id"].ToString());
						permissionInfo.Code = rowPer["Code"].ToString();
						permissionInfo.MenuId = int.Parse(rowPer["MenuId"].ToString());
						permissionInfo.Text = rowPer["Text"].ToString();
						permissionInfo.Description = rowPer["Description"].ToString();

						// 如果权限已存在，不重复添加
						if (role.Permission.FirstOrDefault<PermissionInfo>(p => p.Id == permissionInfo.Id) != null) continue;

						role.Permission.Add(permissionInfo);
					}
				}

				role.IsEnable = bool.Parse(r["IsEnable"].ToString());
				_userInfo.Role.Add(role);
			}

			return _userInfo;
		}

        #region >>是否拥有指定权限<<
        /// <summary>
        /// 是否拥有指定权限
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasPermission(string permission)
        {
            foreach (RoleInfo role in _userInfo.Role)
            {
                var per = role.Permission.FirstOrDefault(p => p.Code == permission);
                if (per != null) return true;
            }

            return false;
        }
        #endregion
    }
}
