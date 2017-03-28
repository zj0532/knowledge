using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core.Configuration.Descriptor
{
	/// <summary>
	/// 权限定义
	/// </summary>
	public class PermissionDescriptor
	{
		public string NodeNameDefine = "Permission";
		public string KeyDefine = "Key";
		public string GroupDefine = "Group";
		public string TextDefine = "Text";

		public string Key { get; set; }
		public string Group { get; set; }
		public string Text { get; set; }
	}
}
