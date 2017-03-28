using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Script.Serialization;
using System.Collections;
using Pactera.Data;
using Pactera.Common.Serialization;
using System.Text;
using Pactera.Model;
using Pactera.Model.Json;
using System.Data.SqlClient;

namespace Pactera.Handler
{
    /// <summary>
    /// PublicKjcase 的摘要说明
    /// </summary>
    public class PublicKjcase : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string json = string.Empty;

            Hashtable htCondition = new Hashtable();
            var dbm = DataBaseFactory.Instance.Create();//链接数据库
            var Kjcase = dbm.GetDataTable("*", "Cus_kjCase", htCondition, "id");

            if ((Kjcase.Rows.Count) > 0) { }
            
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

    }
}