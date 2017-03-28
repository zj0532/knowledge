using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core.Configuration.Descriptor
{
	/// <summary>
	/// 封标审批，通知用户配置文件
	/// </summary>
	public class TenderNotificationUsersDescriptor
	{
		/// <summary>
		/// 配置文件路径
		/// </summary>
		public static string FilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Xml\\Settings\\TenderNotificationUsers.xml";

		public static string BaseNodeNameDefine = "TenderNotificationUsers";
		public static string UserNodeNameDefine = "User";

		public static List<string> User = new List<string>();
	}
}
