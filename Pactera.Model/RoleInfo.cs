using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Pactera.Model
{
    [Serializable]
    public class RoleInfo
    {
        public RoleInfo()
        {
            Permission = new List<PermissionInfo>();
        }
        public int Id { get; set; }

        public string RoleName { get; set; }

        public List<PermissionInfo> Permission { get; set; }

        public bool IsEnable { get; set; }
    }
}
