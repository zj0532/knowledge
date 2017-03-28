using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Configuration;
namespace Pactera.Common
{
    public static class AppConfig
    {
        /// <summary>
        /// 获取数据库连接配置信息
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static string getConnStr(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        /// <summary>
        /// 获取数据库连接提供程序名
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static string getConnProviderName(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ProviderName;
        }

        /// <summary>
        /// 获取WebConfig中的APP信息
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static string getAppStr(string key)
        {
            return ConfigurationManager.AppSettings[key].ToString();
        }

        /// <summary>
        /// 获取WebConfig中的APP信息,转换成Int
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static int getAppInt(string key)
        {
            int iValue = 0;
            try
            {
                iValue = int.Parse(getAppStr(key));
            }
            catch { }
            return iValue;
        }

        /// <summary>
        /// 获取WebConfig中的版本信息
        /// </summary>
        /// <returns></returns>
        public static string getVersion()
        {
            return getAppStr("oaversion");
        }
    }
}
