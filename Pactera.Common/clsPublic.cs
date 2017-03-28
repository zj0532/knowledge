using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace Pactera.Common
{
    public static class clsPublic
    {
        #region //弹出客户端消息类

        /// <summary>
        /// 客户端提示消息
        /// </summary>
        /// <param name="sMsg">提示信息</param>
        public static void MsgBox(string sMsg)
        {
            MsgBox("msgbox", sMsg, false);
        }

        /// <summary>
        /// 客户端提示消息(包含是否返回上一个页面)
        /// </summary>
        /// <param name="ScriptKey">脚本名称</param>
        /// <param name="sMsg">提示信息</param>
        /// <param name="bBack">是否返回上一个页面</param>
        public static void MsgBox(string ScriptKey, string sMsg, bool bBack)
        {
            string sScript;
            sScript = "<script>alert('" + sMsg + "');";
            if (bBack == true)
            {
                sScript = sScript + "history.back(-1);";
            }
            sScript = sScript + "</script>";
            //((Page)System.Web.HttpContext.Current.Handler).RegisterStartupScript(ScriptKey, sScript);
            //RegisterStartupScript("notpass", sScript);
        }

        /// <summary>
        /// 客户端提示消息，不返回上一个页面
        /// </summary>
        /// <param name="ScriptKey"></param>
        /// <param name="sMsg"></param>
        public static void MsgBox(string ScriptKey, string sMsg)
        {
            MsgBox(ScriptKey, sMsg, false);
        }

        /// <summary>
        /// 客户端提示消息(包含是否关闭页面)
        /// </summary>
        /// <param name="sMsg">提示信息</param>
        /// <param name="isCloseWindow">是否关闭页面</param>
        public static void MsgBox(string sMsg, bool isCloseWindow)
        {
            if (isCloseWindow) sMsg = "alert('" + sMsg + "'); window.top.close();";
            else sMsg = "alert('" + sMsg + "');";

            JScript("jsCode", sMsg);
        }

        /// <summary>
        /// 执行js脚步
        /// </summary>
        /// <param name="JsCode">js脚本</param>
        public static void JScript(string JsCode)
        {
            JScript("jsCode", JsCode);
        }

        /// <summary>
        /// 执行js脚步
        /// </summary>
        /// <param name="ScriptKey"></param>
        /// <param name="JsCode">js脚本</param>
        public static void JScript(string ScriptKey, string JsCode)
        {
            string sScript;
            sScript = "<script>" + JsCode + "</script>";

            ((Page)System.Web.HttpContext.Current.Handler).ClientScript.RegisterStartupScript(HttpContext.Current.GetType(), ScriptKey, JsCode, true);
        }
        #endregion


        /// <summary>
        /// 获取Post方法传递的参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string RequstForm(string name)
        {
            return (HttpContext.Current.Request.Form[name] == null ? string.Empty : HttpContext.Current.Request.Form[name].ToString().Trim());
        }

        public static HttpPostedFile RequstFiles(string name)
        {
            return HttpContext.Current.Request.Files[name];
        }
        /// <summary>
        /// 获取Get方法传递的参数
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public static string RequstString(string sParam)
        {
            return (HttpContext.Current.Request[sParam] == null ? string.Empty : HttpContext.Current.Request[sParam].ToString().Trim());
        }

        /// <summary>
        /// 获取传递的整形参数
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public static int RequstInt(string sParam)
        {
            int iValue = 0;
            try
            {
                iValue = int.Parse(RequstString(sParam));
            }
            catch { }
            return iValue;
        }

        /// <summary>
        /// 过滤get方法传递的参数，防止SQL注入--string flag = clsPublic.FilterRequestString("flag"); 
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public static string FilterRequestString(string sParam)
        {
            return ProcessRequest(RequstString(sParam));
        }

        /// <summary>
        /// 过滤Post方法传递的参数，防止SQL注入
        /// </summary>
        /// <param name="sParam"></param>
        /// <returns></returns>
        public static string FilterRequstForm(string sParam)
        {
            return ProcessRequest(RequstForm(sParam));
        }

        /// <summary>
        /// 过滤字符串，防止SQL注入
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string ProcessRequest(string Htmlstring)
        {
            if (Htmlstring == null)
            {
                return "";
            }
            else
            {
                //删除脚本
                Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
                //删除HTML
                Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

                Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
                //Htmlstring = Regex.Replace(Htmlstring, @"\\", "", RegexOptions.IgnoreCase);

                //删除与数据库相关的词
                Htmlstring = Regex.Replace(Htmlstring, "select", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "insert", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "delete from", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "count''", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "drop table", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "truncate", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "asc", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "mid", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "char", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "xp_cmdshell", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "exec master", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "net localgroup administrators", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "and", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "net user", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "or", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "net", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "delete", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "drop", "", RegexOptions.IgnoreCase);
                Htmlstring = Regex.Replace(Htmlstring, "script", "", RegexOptions.IgnoreCase);

                //特殊的字符
                Htmlstring = Htmlstring.Replace("%20", "");
                Htmlstring = Htmlstring.Replace("%", "");
                Htmlstring = Htmlstring.Replace("'", "");
                //Htmlstring = Htmlstring.Replace("/", "");
                Htmlstring = Htmlstring.Replace("\\", "");
                Htmlstring = Htmlstring.Replace("|", "");
                Htmlstring = Htmlstring.Replace("<", "");
                Htmlstring = Htmlstring.Replace(">", "");
                //Htmlstring = Htmlstring.Replace("*", "");
                //Htmlstring = Htmlstring.Replace("?", "");
                Htmlstring = Htmlstring.Replace("\r\n", "");
                Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

                return Htmlstring;
            }
        }

        /// <summary>
        /// 获取用户权限机组
        /// </summary>
        /// <returns></returns>
        //public static string GetUserUnitIds()
        //{
        //    string reStr = "1";
        //    if (HttpContext.Current.Session["powerroleid"] == null && HttpContext.Current.Session["powerunitid"] == null) return reStr;
        //    string rolepower = "," + HttpContext.Current.Session["powerroleid"].ToString().ToLower() + ",";
        //    if (rolepower.IndexOf(",r01,") < 0)
        //    {
        //        string unitids = HttpContext.Current.Session["powerunitid"].ToString().TrimEnd(',');
        //        unitids = unitids.Replace(",", "','");
        //        reStr = "'" + unitids + "'";
        //    }
        //    else
        //        reStr = "1";
        //    return reStr;
        //}
    }
}
