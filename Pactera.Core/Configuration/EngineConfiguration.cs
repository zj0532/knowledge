using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core.Configuration
{
    public sealed class EngineConfiguration
    {
        /// <summary>
        /// 工作流定义的文件夹路径
        /// </summary>
        public static string WorkflowDefinitionFolder = AppDomain.CurrentDomain.BaseDirectory + "\\Xml\\WorkFlows\\";
    }
}
