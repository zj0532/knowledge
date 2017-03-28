using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pactera.Model;

namespace Pactera.Web.UI
{
    public class BasePage : System.Web.UI.Page
    {
        #region ..属性..
        /// <summary>
        /// 当前登录的用户
        /// </summary>
        protected UserInfo CurrentSigninUser
        {
            get
            {
                if (Session["CURRENT_SIGNIN_USER"] == null)
                {
                    string script = "alert('登录超时或未登录，请重新登录');window.top.location.href='/login.aspx';";
                    Response.Write("<script type=\"text/javascript\">" + script + "</script>");
                    Response.End();
                }
                return Session["CURRENT_SIGNIN_USER"] as UserInfo;
            }

            set { Session["CURRENT_SIGNIN_USER"] = value; }
        }
        /// <summary>
        /// 获取或设置会员登录成功后跳转的页面
        /// </summary>
        protected string MemberLoginPostBack
        {
            get
            {
                if (Session["MEMBER_LOGIN_POST_BACK"] == null) { return null; }
                string url = Session["MEMBER_LOGIN_POST_BACK"].ToString();
                Session.Remove("MEMBER_LOGIN_POST_BACK");
                return url;
            }

            set { Session["MEMBER_LOGIN_POST_BACK"] = value; }
        }
        /// <summary>
        /// 获取页面标题名称
        /// </summary>
        protected string PageTitleName
        {
            get
            {
                return string.Empty;
                //return BaseBiz.GetFieldValue("Cus_Keywords", "title", "1");
            }
        }
        /// <summary>
        /// 获取页面关键字
        /// </summary>
        protected string PageKeyWords
        {
            get
            {
                return string.Empty;
                //string keywords = BaseBiz.GetFieldValue("Cus_Keywords", "keywords", "1");
                //return string.Format("<meta name=\"keywords\" content=\"{0}\"/>", keywords);
            }
        }
        /// <summary>
        /// 获取页面描述
        /// </summary>
        protected string PageDescription
        {
            get
            {
                return string.Empty;
                //string description = BaseBiz.GetFieldValue("Cus_Keywords", "description", "1");
                //return string.Format("<meta name=\"description\" content=\"{0}\"/>", description);
            }
        }
        #endregion

        #region >>禁用当前页面缓存<<
        /// <summary>
        /// 禁用当前页面缓存
        /// </summary>
        protected void DisableCurrentPageCache()
        {
            Response.Buffer = true;
            Response.Expires = 0;
            Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1);
            Response.CacheControl = "no-cache";
        }
        #endregion
    }
}
