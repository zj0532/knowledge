using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Impl;

namespace Pactera.Core
{
	public class Scheduler
	{
		private static IScheduler _scheduler;

		private Scheduler() { }

		public static IScheduler Instance
		{
			get
			{
				if(_scheduler == null)
				{
					ISchedulerFactory factory = new StdSchedulerFactory();
					_scheduler = factory.GetScheduler();
				}

				return _scheduler;
			}
		}
	}
}
