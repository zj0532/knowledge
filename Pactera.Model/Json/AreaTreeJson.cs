using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Pactera.Model.Json
{
	/// <summary>
	/// 区域管理树菜单结构对象
	/// </summary>
	[DataContract]
	public class AreaTreeJson
	{
		/// <summary>
		/// 主键
		/// </summary>
		[DataMember]
		public int id { get; set; }
		/// <summary>
		/// 显示的图标 - 后期扩展用，先留着
		/// </summary>
		[DataMember]
		public string iconCls { get; set; }
		/// <summary>
		/// 枚举名称
		/// </summary>
		[DataMember]
        public string text { get; set; }
        /// <summary>
        /// ParentId
        /// </summary>
        [DataMember]
        public int ParentId { get; set; }
		/// <summary>
		/// 子菜单集合
        /// 这个就是用于显示子节点，其实子节点还是它自己
		/// </summary>
		[DataMember]
		public List<AreaTreeJson> children { get; set; }
        /// <summary>
        /// 获取或设置所在级别
        /// </summary>
        [DataMember]
        public int Level { get; set; }
	}
}
