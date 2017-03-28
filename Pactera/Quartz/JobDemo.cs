using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;

namespace Pactera.Quartz
{
	public class JobDemo : IJob
	{
		public void Execute(IJobExecutionContext context)
		{
			int count = 0;
			if (!int.TryParse(context.MergedJobDataMap["Shuju1"].ToString(), out count))
			{
				Pactera.Common.Logger.Info("参数异常，停止作业");
				return;
			}

			Pactera.Common.Logger.Info("JobDemo启动，执行第" + count + "次作业！");
			Pactera.Common.Logger.Info("此次作业携带了参数 Shuju1 : [" + count + "]");

			// 当执行到一定次数或自定义的一些条件后，停止作业
			count++;
			if (count > 3)
			{
				Pactera.Common.Logger.Info("作业已执行超过3次，停止作业");
				return;
			}

			// 1 定义作业
			IJobDetail jobDetail = JobBuilder.Create<Quartz.JobDemo>()
				// 这里是作业的名称和分组
				//.WithIdentity("DefaultJob", "DefaultGroup")
				.Build();
			jobDetail.JobDataMap.Add("Shuju1", count);

			// 2 定义触发器
			ITrigger trigger = TriggerBuilder.Create()
				// 这里是触发器的名称和分组
				//.WithIdentity("DefaultTrigger", "DefaultTriggerGroup")
				// 根据Utc时间来执行
				.StartAt(DateTimeOffset.UtcNow.AddSeconds(30))
				.Build();

			// 3 调度作业
			Pactera.Core.Scheduler.Instance.ScheduleJob(jobDetail, trigger);
		}
	}
}