using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Pactera.Core.Configuration
{
    public sealed class WebConfiguration
    {
        /// <summary>
        /// 工作流定义的文件夹路径
        /// </summary>
        public static string WorkflowDefinitionFolder = AppDomain.CurrentDomain.BaseDirectory + "\\Xml\\WorkFlows\\";

        /// <summary>
        /// 获取 审批流中 市场机会 冲突协调 所在步骤
        /// </summary>
        public static int WorkflowConflictCoordinationStep
        {
            get
            {
                int step = -1;
                string stepText = ConfigurationManager.AppSettings["WorkflowConflictCoordinationStep"];
                int.TryParse(stepText, out step);

                return step;
            }
        }

		/// <summary>
		/// 获取 审批流程 日志节点显示文本
		/// </summary>
		public static string WorkflowLogNodeDisplayText
		{
			get { return ConfigurationManager.AppSettings["WorkflowLogNodeDisplayText"]; }
		}

        /// <summary>
        /// 获取 审批流中 市场机会 分公司领导审批 所在步骤
        /// 用户获取上报时间
        /// </summary>
        public static int WorkflowCorpLeaderStep
        {
            get
            {
                int step = -1;
                string stepText = ConfigurationManager.AppSettings["WorkflowCorpLeaderStep"];
                int.TryParse(stepText, out step);

                return step;
            }
        }

		/// <summary>
		/// 封标审批 第一次通知时间，单位（天），默认三天
		/// 如果风标审批完成后，在这个时间内没有提交开标结果，则发送通知
		/// </summary>
		public static int TenderFirstNotificationDays
		{
			get
			{
				int days = 0;
				string daysText = ConfigurationManager.AppSettings["TenderFirstNotificationDays"];
				if (!int.TryParse(daysText, out days)) days = 3;

				return days;
			}
		}

		/// <summary>
		/// 封标审批 第一次通知后每次间隔时间，单位（天），默认一天
		/// 第一次发送通知完成后在这个时间内没有提交开标结果，则重复发送通知
		/// </summary>
		public static int TenderFollowNotificationDays
		{
			get
			{
				int days = 0;
				string daysText = ConfigurationManager.AppSettings["TenderFollowNotificationDays"];
				if (!int.TryParse(daysText, out days)) days = 1;

				return days;
			}
		}

		/// <summary>
		/// 封标审批 发送通知时要通知的人
		/// </summary>
		public static string[] TenderNotificationUsers
		{
			get
			{
				return Configuration.TenderNotificationConfigurationService.GetNotificationUsers().ToArray();
			}
		}
        #region >>是否为测试模式<<
        /// <summary>
        /// 获取 是否为测试模式
        /// </summary>
        public static bool IsTestMode
        {
            get
            {
                string testMode = ConfigurationManager.ConnectionStrings["TestMode"].ConnectionString;
                return testMode == "1" ? true : false;
            }
        }
        #endregion
    }
}
