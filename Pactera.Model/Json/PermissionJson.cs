using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Pactera.Model.Json
{
    /// <summary>
    /// 权限模型
    /// </summary>
    public class PermissionJson : BaseTreeItemJson
    {
        /// <summary>
        /// 权限编码，所有权限不允许出现重复的编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// 菜单Id
        /// </summary>
        public int MenuId { get; set; }
        /// <summary>
        /// 权限的描述
        /// </summary>
        public string Description { get; set; }
    }
}
