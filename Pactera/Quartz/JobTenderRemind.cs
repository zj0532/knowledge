using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pactera.Core.Configuration;
using Pactera.Data;
using Quartz;

namespace Pactera.Quartz
{
    /// <summary>
    /// 封标审批之后进行开标提示处理
    /// </summary>
    public class JobTenderRemind : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            IDataBaseMgr dataBaseMgr = DataBaseFactory.Instance.Create();//数据库访问对象
            //var TenderFirstNotificationDays = WebConfiguration.TenderFirstNotificationDays;//第一次发送时间
            var TenderFollowNotificationDays = WebConfiguration.TenderFollowNotificationDays;//第二次之后发送间隔
            var TenderNotificationUsers = WebConfiguration.TenderNotificationUsers;//除发起人之外的OCS信息接收者

            int remindCount = 0;//提醒次数
            var createUser = "";//发起人
            var formTitle = "";//表单标题
            var projectId = "";//对应市场机会Id
            if (!int.TryParse(context.MergedJobDataMap["RemindCount"].ToString(), out remindCount))
            {
                Pactera.Common.Logger.Info("参数异常，停止作业");
                return;
            }
            createUser = context.MergedJobDataMap["createUser"].ToString();
            formTitle = context.MergedJobDataMap["formTitle"].ToString();
            projectId = context.MergedJobDataMap["projectId"].ToString();
            var title = "开标结果未填写提示";
            var content = string.Format("您的【{0}】的开标结果还没有提交，请及时提交！", formTitle);
            //获取封标审批之后是否进行开标结果填写
            var tender = dataBaseMgr.ExecuteScalar(string.Format("select count(ProjectChanceId) from BPM_BID_TenderOpening where ProjectChanceId='{0}'", projectId));

            //验证是否需要继续发送消息
            if (Convert.ToInt32(tender) > 0)
            {
                //Pactera.Common.Logger.Info("JobTenderRemind已经跳出执行，" + content);
                return;
            }
            //发送OCS消息
            //给发起人发送
            if (!TenderNotificationUsers.Contains(createUser))
            {
                //NotificationEngine.Instance.SendOcsNotification(createUser, title, content);
                //Pactera.Common.Logger.Info("JobTenderRemind，执行第" + remindCount + "次作业！" + createUser + projectId);
            }
            //给配置需要收到的人员发送 这个类，你后期用的到
            for (int i = 0; i < TenderNotificationUsers.Length; i++)
            {
                //NotificationEngine.Instance.SendOcsNotification(TenderNotificationUsers[i], title, content);
                //Pactera.Common.Logger.Info("JobTenderRemind，执行第" + remindCount + "次作业！" + TenderNotificationUsers[i]);
            }
            // 1 定义作业
            IJobDetail jobDetail = JobBuilder.Create<Quartz.JobTenderRemind>().Build();
            jobDetail.JobDataMap.Add("RemindCount", ++remindCount);
            jobDetail.JobDataMap.Add("createUser", createUser);
            jobDetail.JobDataMap.Add("formTitle", formTitle);
            jobDetail.JobDataMap.Add("projectId", projectId);

            // 2 定义触发器
            ITrigger trigger = TriggerBuilder.Create()
                // 根据Utc时间来执行
                .StartAt(DateTimeOffset.UtcNow.AddDays(TenderFollowNotificationDays))
                //.StartAt(DateTimeOffset.UtcNow.AddMinutes(TenderFollowNotificationDays))
                .Build();

            // 3 调度作业
            Pactera.Core.Scheduler.Instance.ScheduleJob(jobDetail, trigger);
        }
    }
}