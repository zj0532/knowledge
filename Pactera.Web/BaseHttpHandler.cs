using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pactera.Model;
using System.Web;
using System.Web.SessionState;

namespace Pactera.Web
{
    public class BaseHttpHandler : IRequiresSessionState
    {
		/// <summary>
		/// 获取当前登录的用户
		/// </summary>
		protected UserInfo CurrentSigninUser
		{
			get
			{
				if (HttpContext.Current.Session["CURRENT_SIGNIN_USER"] == null)
				{
					return null;
				}
				return HttpContext.Current.Session["CURRENT_SIGNIN_USER"] as UserInfo;
			}
		}
    }

    #region >>处理程序响应对象<<
    /// <summary>
    /// 处理程序响应对象
    /// </summary>
    public class HandlerResponse
    {
        public HandlerResponse(int stateCode, string message)
        {
            StateCode = stateCode;
            Message = message;
        }

        /// <summary>
        /// 获取或设置 处理程序响应的状态号码
        /// </summary>
        public int StateCode { get; set; }
        /// <summary>
        /// 获取或设置 处理程序响应的信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 将对象转化为Json字符串
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return "{ \"StateCode\": " + StateCode + ", \"Message\": \"" + Message + "\" }";
        }
    }
    #endregion
}
