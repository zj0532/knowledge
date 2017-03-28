using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Model.GenService
{
    public class Knowledge
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Case类别一级
        /// </summary>
        public string OneValue { get; set; }

        /// <summary>
        /// Case类别二级
        /// </summary>
        public string TwoValue { get; set; }

        /// <summary>
        /// case类别三级
        /// </summary>
        public string ThreeValue { get; set; }

        /// <summary>
        /// Case类别四级
        /// </summary>
        public string FourValue { get; set; }

        /// <summary>
        /// 故障描述
        /// </summary>
        public string MalDescription { get; set; }

        /// <summary>
        /// 解决方案
        /// </summary>
        public string Solution { get; set; }

        /// <summary>
        /// 主要原因
        /// </summary>
        public string MainReason { get; set; }
    }
}
