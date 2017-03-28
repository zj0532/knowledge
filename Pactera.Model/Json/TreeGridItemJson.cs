using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Pactera.Model.Json
{
    /// <summary>
    /// TreeGrid数据源对象
    /// </summary>
    [DataContract]
    public class TreeGridItemJson
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }
        /// <summary>
        /// 显示的图标
        /// </summary>
        [DataMember]
        public string iconCls { get; set; }
        /// <summary>
        /// 枚举名称
        /// </summary>
        [DataMember]
        public string EnumText { get; set; }
        /// <summary>
        /// 枚举值
        /// </summary>
        [DataMember]
        public string EnumValue { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
		public string EnumDesc { get; set; }
		/// <summary>
		/// 排序
		/// </summary>
		[DataMember]
		public string EnumOrder { get; set; }
        /// <summary>
        /// 父级值
        /// </summary>
        [DataMember]
        public string ParentValue { get; set; }
        /// <summary>
        /// 父级名称
        /// </summary>
        [DataMember]
        public string ParentText { get; set; }  
        /// <summary>
        /// 子菜单集合
        /// </summary>
        [DataMember]
        public List<TreeGridItemJson> children { get; set; }
    }
}
