using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pactera.Web;
using Pactera.Model;

namespace Pactera.Handler
{
    /// <summary>
    /// PublicPermission 的摘要说明
    /// </summary>
    public class PublicPermission : BaseHttpHandler, IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string json = string.Empty;
            string action = context.Request["Action"] ?? "";

            switch (action)
            {
                case "CurrentSigninUserHasPermission":
                    json = ValidationPermission(context);
                    break;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(json);
        }

        public string ValidationPermission(HttpContext context)
        {
            string permission = context.Request["Permission"] ?? "";
            return CurrentSigninUser.HasPermission(permission) ? "0" : "1";
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}