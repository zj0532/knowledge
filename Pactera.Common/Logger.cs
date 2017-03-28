using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pactera.Common
{
	public class Logger
	{
		// log4net日志专用
		private static readonly log4net.ILog _Logger = log4net.LogManager.GetLogger("Logger");

		public static void SetConfig()
		{
			log4net.Config.XmlConfigurator.Configure();
		}

		public static void SetConfig(FileInfo configFile)
		{
			log4net.Config.XmlConfigurator.Configure(configFile);
		}

		/// <summary>
		/// 普通的文件记录日志
		/// </summary>
		/// <param name="info"></param>
		public static void Info(string info)
		{
			if (_Logger.IsInfoEnabled)
			{
				_Logger.Info(info);
			}
		}

		/// <summary>
		/// 错误日志
		/// </summary>
		/// <param name="info"></param>
		public static void Error(string info)
		{
			if (_Logger.IsErrorEnabled)
			{
				_Logger.Error(info);
			}
		}

		/// <summary>
		/// 错误日志
		/// </summary>
		/// <param name="info"></param>
		/// <param name="se"></param>
		public static void Error(string info, Exception se)
		{
			if (_Logger.IsErrorEnabled)
			{
				_Logger.Error(info, se);
			}
		}
	}
}
