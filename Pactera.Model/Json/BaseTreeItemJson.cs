using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Pactera.Model.Json
{
    [DataContract]
    [KnownType(typeof(MenuJson))]
    [KnownType(typeof(PermissionJson))]
    public class BaseTreeItemJson
    {
        /// <summary>
        /// id
        /// </summary>
        [DataMember]
        public int id { get; set; }
        /// <summary>
        /// 显示的文本
        /// </summary>
        [DataMember]
        public string text { get; set; }
        /// <summary>
        /// 显示的图标
        /// </summary>
        [DataMember]
        public string iconCls { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public string state { get; set; }
        [DataMember]
        public bool Checked { get; set; }
        /// <summary>
        /// 子菜单集合
        /// </summary>
        [DataMember]
        public List<BaseTreeItemJson> children { get; set; }
    }
}
