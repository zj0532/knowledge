using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core.Attribute
{
	/// <summary>
	/// 版本属性
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public class VersionAttribute : System.Attribute
	{
		/// <summary>
		/// 作者
		/// </summary>
		public string Author { get; set; }
		/// <summary>
		/// 日期
		/// </summary>
		public string Date { get; set; }
		/// <summary>
		/// 当前版本
		/// </summary>
		public string Version { get; set; }
		/// <summary>
		/// 有效版本，指在哪些版本中有效
		/// </summary>
		public string ValidVersion { get; set; }
		/// <summary>
		/// 描述
		/// </summary>
		public string Description { get; set; }
	}
}
