using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Pactera.Core.PacteraException;
using Pactera.Core.Configuration.Descriptor;

namespace Pactera.Core.Configuration
{
	/// <summary>
	/// 用户权限配置文件服务
	/// </summary>
	public class TenderNotificationConfigurationService
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static List<string> GetNotificationUsers()
		{
			if (!File.Exists(TenderNotificationUsersDescriptor.FilePath))
				throw new ArgumentNullException("配置文件不存在！");

			List<string> users = new List<string>();
			XElement root = XElement.Load(TenderNotificationUsersDescriptor.FilePath);

			var items = root.Elements().Where(p => p.Name.LocalName == TenderNotificationUsersDescriptor.UserNodeNameDefine).ToArray();
			if (items.Length == 0) return users;

			foreach (var item in items)
			{
				//Pactera.Common.Logger.Info(item.Value);
				users.Add(item.Value);
			}

			return users;
		}

	}

}
