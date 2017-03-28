using Pactera.Core.Configuration.Descriptor;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Pactera.Core.Configuration
{
	/// <summary>
	/// 用户权限配置文件服务
	/// </summary>
	public class UserPermissionConfigurationService
	{
		// 用户权限定义缓存
		static UserPermissionDescriptor UserPermissionDescriptor = new UserPermissionDescriptor();

		#region >>构造函数<<
		/// <summary>
		/// 构造函数
		/// </summary>
		static UserPermissionConfigurationService()
		{
			// 从流程配置文件中加载流程
			UserPermissionDescriptor = Load();
		}
		#endregion

		/// <summary>
		/// 从XML用户权限中加用户权限定义对象
		/// </summary>
		/// <returns></returns>
		private static UserPermissionDescriptor Load()
		{
			if (!File.Exists(UserPermissionDescriptor.UserPermissionFilePath))
				throw new ArgumentNullException("用户权限配置文件不存在！");

			XElement root = XElement.Load(UserPermissionDescriptor.UserPermissionFilePath);

			UserPermissionDescriptor desc = new UserPermissionDescriptor();
			var items = root.Elements().Where(p => p.Name.LocalName == UserPermissionDescriptor.Current.PermissionDefine.NodeNameDefine).ToArray();
			if (items.Length == 0) return desc;

			for (int i = 0; i < items.Length; i++)
			{
				var permission = new PermissionDescriptor();

				// 解析流程步骤
				permission.Key = items[i].GetAttributeValue<string>(UserPermissionDescriptor.Current.PermissionDefine.KeyDefine);
				permission.Group = items[i].GetAttributeValue<string>(UserPermissionDescriptor.Current.PermissionDefine.GroupDefine);
				permission.Text = items[i].GetAttributeValue<string>(UserPermissionDescriptor.Current.PermissionDefine.Text);

				desc.Permission.Add(permission);
			}

			return desc;
		}
	}

}
