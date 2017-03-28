using System;
using System.Data;
using System.Linq;
using System.Text;

namespace Pactera.Core.Workflow
{
    /// <summary>
    /// 工作流程引擎
    /// </summary>
	public partial class WorkflowEngine
	{
        private static WorkflowEngine _instance;

        public static WorkflowEngine Instance
        {
            get
            {
                if (_instance == null) _instance = new WorkflowEngine();
                return _instance;
            }
        }
	}
}
