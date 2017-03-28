using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Pactera.Model
{
    [Serializable]
    public class PermissionInfo
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int MenuId { get; set; }

        public string Text { get; set; }

        public string Description { get; set; }
    }
}
