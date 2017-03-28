using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core.Workflow
{
    /// <summary>
    /// 日志动作
    /// </summary>
    public enum WorkflowLogAction
    {
        /// <summary>
        /// 拟单人发起工作流
        /// </summary>
        Created,
        /// <summary>
        /// 工程师处理工单
        /// </summary>
        Handle,
        /// <summary>
        /// 已处理
        /// </summary>
        Handled,
        /// <summary>
        ///工程师将任务转办
        /// </summary>
        Transfer,
        /// <summary>
        /// 工程师无法处理时，升级工单级别
        /// </summary>
        LevelUp,
        /// <summary>
        /// 退回
        /// </summary>
        GoBack,
        /// <summary>
        /// 拟单人回访客户，对处理结果审核
        /// </summary>
        Approve
    }
}
