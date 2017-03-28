using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pactera.Handler
{
    /// <summary>
    /// Session 的摘要说明
    /// </summary>
    public class Session : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string name = context.Request["name"];
            context.Session.Add("User", name);

            Console.WriteLine("123");

            context.Response.ContentType = "text/plain";
            context.Response.Write(context.Session.SessionID);
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