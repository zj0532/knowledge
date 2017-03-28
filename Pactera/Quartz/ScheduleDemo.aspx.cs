using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Quartz;

namespace Pactera.Quartz
{
	public partial class ScheduleDemo : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            //// 作业
            //IJobDetail jobDetail = JobBuilder.Create<Quartz.JobDemo>()
            //    //.WithIdentity("DefaultJob", "DefaultGroup")
            //    .Build();
            //jobDetail.JobDataMap.Add("Shuju1", "1");

            //// 触发器
            //ITrigger trigger = TriggerBuilder.Create()
            //    //.WithIdentity("DefaultTrigger", "DefaultTriggerGroup")
            //    .StartAt(DateTimeOffset.UtcNow.AddSeconds(10))
            //    .Build();

            //Pactera.Core.Scheduler.Instance.ScheduleJob(jobDetail, trigger);
            //Pactera.Common.Logger.Info("设置并启动定时器，在10秒后执行！");

            // 作业
            IJobDetail jobDetail = JobBuilder.Create<Quartz.JobTenderRemind>()
                //.WithIdentity("DefaultJob", "DefaultGroup")
                .Build();
            jobDetail.JobDataMap.Add("RemindCount", "1");

            // 触发器
            ITrigger trigger = TriggerBuilder.Create()
                //.WithIdentity("DefaultTrigger", "DefaultTriggerGroup")
                .StartAt(DateTimeOffset.UtcNow.AddMinutes(3))
                .Build();

            Pactera.Core.Scheduler.Instance.ScheduleJob(jobDetail, trigger);
            Pactera.Common.Logger.Info("设置并启动定时器，在3分钟后执行！");

			TOIService.TOIServiceClient client = new TOIService.TOIServiceClient();

			LitJson.JsonData data = new LitJson.JsonData();

			data["dmakedate"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			data["abc"] = "123";
			data["abc"] = 123;

			LitJson.JsonData dataChi = new LitJson.JsonData();
			dataChi["abaa"] = "123";

			data["Object"] = dataChi;

			data["Object"].Add(dataChi);

			LitJson.JsonMapper.ToJson(data);

		}
	}
}