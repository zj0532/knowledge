using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core.PacteraException
{
    /// <summary>
    /// 工作流引擎异常
    /// </summary>
    [Serializable]
    public class WorkflowException : Exception
    {
        public WorkflowException(string message) : base(message) { }
        public WorkflowException(string message, Exception innerException) : base(message, innerException) { }
    }
}
