using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core
{
    public class EngineResponse
	{
		#region >>构造函数，获取指定的引擎状态<<
		public EngineResponse(int state)
		{
			State = state;

			switch(state)
			{
				case Complete:
					Message = "完成！";
					break;
                case Engine_Workflow_CreateWorkflowFail:
                    Message = "工作流创建失败！";
                    break;
                case Engine_Workflow_CreateWorkflowLogFail:
                    Message = "工作流创建失败！";
                    break;
				default:
					State = -1;
					Message = "未知的状态！";
					break;
			}
		}
		#endregion

		#region >>状态<<
		public const int Complete = 0;
        private const int Engine_Workflow_Base = 10000;
        private const int Engine_WorkflowLog_Base = 20000;

		/// <summary>
		/// 工作流创建失败
		/// </summary>
        public const int Engine_Workflow_CreateWorkflowFail = Engine_Workflow_Base + 1000 + 1;
        /// <summary>
        /// 工作流创建失败
        /// </summary>
        public const int Engine_Workflow_CreateWorkflowLogFail = Engine_WorkflowLog_Base + 1000 + 1;
		#endregion

		#region >>属性<<
		/// <summary>
        /// 获取或设置工作流引擎响应状态
        /// 0 : 正常
        /// 1 : 不正常（从Message中获取异常信息）
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 获取或设置工作流引擎响应信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 获取或设置携带的对象
        /// </summary>
        public object Obj { get; set; }
		#endregion
	}
}
