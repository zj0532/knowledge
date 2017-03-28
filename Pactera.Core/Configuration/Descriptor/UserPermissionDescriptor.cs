using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core.Configuration.Descriptor
{
    /// <summary>
    /// 配置文件描述 - 用户权限
    /// </summary>
    public sealed class UserPermissionDescriptor
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string UserPermissionFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\Xml\\Permission\\UserPermission.xml";

        /// <summary>
        /// 得到或设置当前的用户权限定义
        /// </summary>
        public static UserPermissionDescriptor Current = new UserPermissionDescriptor();

        /// <summary>
        /// 权限列表节点名称
        /// </summary>
        public string NodeNameDefine = "PermissionList";

        public PermissionDescriptor PermissionDefine { get; set; }

        public List<PermissionDescriptor> Permission = new List<PermissionDescriptor>();
    }
}
