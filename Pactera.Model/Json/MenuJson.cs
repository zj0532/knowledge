using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Pactera.Model.Json
{
    /// <summary>
    /// 菜单模型
    /// </summary>
    public class MenuJson : BaseTreeItemJson
    {
        /// <summary>
        /// 连接地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 菜单类型
        /// 0 [菜单，无连接地址]
        /// 1 [功能，有实际的连接地址]
        /// </summary
        public int MenuType { get; set; }
        /// <summary>
        /// 父级菜单Id
        /// </summary>
        public int ParentId { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
