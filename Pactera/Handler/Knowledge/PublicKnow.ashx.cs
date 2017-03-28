using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pactera.Data;
using Pactera.Common.Serialization;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Pactera.Model;
using Pactera.Web;

namespace Pactera.Handler.Knowledge
{
    /// <summary>
    /// PublicKnow 的摘要说明
    /// </summary>
    public class PublicKnow : Pactera.Web.BaseHttpHandler, IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string Action = context.Request["action"] ?? "";
            string json = string.Empty;
            switch (Action)
            {
                case "getKnowlist":
                    json = getKnowList(context);
                    break;
                case "AddKnow":
                    json = AddKnow(context);
                    break;
                case "getKnowInfo":
                    json = getKnowInfo(context);
                    break;
                case "getKnowDetailInfo":
                    json = getKnowDetailInfo(context);
                    break;
                case "UpdateKnow":
                    json = UpdateKnowInfo(context);
                    break;
                case "DeleteKnow":
                    json = DeleteKnow(context);
                    break;
                case "getMyKnowlist":
                    json = getMyKnowlist(context);
                    break;
                case "getCheckKnowlist":
                    json = getCheckKnowlist(context);
                    break;
                case "CheckKnowledge":
                    json = CheckKnowledge(context);
                    break;
                case "NoCheckKnowledge":
                    json = NoCheckKnowledge(context);
                    break;
            }


            context.Response.ContentType = "text/plain";
            context.Response.Write(json);
        }
        /// <summary>
        /// 知识库列表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>

        private string getKnowList(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();

            int rows = Convert.ToInt32(context.Request["rows"]);//页大小
            int page = Convert.ToInt32(context.Request["page"]);//当前页
            int pageCount = 0;//页总数
            int total = 0;//数据总数

            string OneValue = context.Request["OneValue"] ?? "";
            string TwoValue = context.Request["TwoValue"] ?? "";
            string ThreeValue = context.Request["ThreeValue"]?? "";
            string FourValue = context.Request["FourValue"]?? "";
            string MalDescription = context.Request["MalDescription"] ?? "";
            string CreateDateStart = context.Request["CreateDateStart"]?? "";
            string CreateDateEnd = context.Request["CreateDateEnd"]?? "";
            string Created = context.Request["Created"]?? "";

            Hashtable htCondition = new Hashtable();
            htCondition.Add("Checked", "1");
            if (OneValue != "")
            {
                htCondition.Add("OneValue", OneValue);
            }
            if (TwoValue != "")
            {
                htCondition.Add("TwoValue", TwoValue);
            }
            if (ThreeValue != "")
            {
                htCondition.Add("ThreeValue", ThreeValue);
            }
            if (FourValue != "")
            {
                htCondition.Add("FourValue", FourValue);
            }
            if (MalDescription != "")
            {
                htCondition.Add("MalDescription like @MalDescription", new SqlParameter("@MalDescription", "%" + MalDescription + "%"));
            }
            if (CreateDateStart != "")
            {
                htCondition.Add("CreateDate >= @CreateDateStart", new SqlParameter("@CreateDateStart",CreateDateStart));
            }
            if (CreateDateEnd != "")
            {
                htCondition.Add("CreateDate <= @CreateDateEnd", new SqlParameter("@CreateDateEnd", CreateDateEnd));
            }
            if (Created != "")
            {
                htCondition.Add("Created", Created);
            }

            string field = "Id,(select Text from Cus_CaseType where Id = OneValue) as OneCaseType,OneValue,(select Text from Cus_CaseType where Id = TwoValue) as TwoCaseType,TwoValue,(select Text from Cus_CaseType where Id = ThreeValue) as ThreeCaseType,ThreeValue,(select Text from Cus_CaseType where Id = FourValue) as FourCaseType,FourValue,MalDescription,Solution,Created,CreateDate,Checked,MainReason";
            string table = "Cus_Knowledge";

            // 获取全部用户
            var dtKnowLedgeList = dbm.GetDataTable("Id", field, table, htCondition, "Id DESC", page, rows, out total, out pageCount);

            return "{\"total\":" + total + ",\"rows\":" + JSONHelper.DataTable2Json(dtKnowLedgeList) + "}";
        }

        /// <summary>
        /// 添加知识点
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string AddKnow(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();
            string OneValue = context.Request["OneValue"] ?? "";
            string TwoValue = context.Request["TwoValue"] ?? "";
            string ThreeValue = context.Request["ThreeValue"] ?? "";
            string FourValue = context.Request["FourValue"] ?? "";
            string Solution = context.Request["Solution"] ?? "";
            string MalDescription = context.Request["MalDescription"] ?? "";
            string MainReason = context.Request["MainReason"] ?? "";

            if (Solution == "") { return new HandlerResponse(1, "解决方案不允许为空！").ToJson(); }
            if (Solution.Contains('\\'))
            {
                Solution=Solution.Replace("\\","/");
            }
            if (MalDescription == "") { return new HandlerResponse(1, "故障描述不允许为空！").ToJson(); }
            if (MainReason == "") { return new HandlerResponse(1, "根本原因不允许为空！").ToJson(); }

            Hashtable htField = new Hashtable();
            htField.Add("OneValue", OneValue);
            htField.Add("TwoValue", TwoValue);
            htField.Add("ThreeValue", ThreeValue);
            htField.Add("FourValue", FourValue);
            htField.Add("Solution", Solution);
            htField.Add("MalDescription", MalDescription);
            htField.Add("MainReason", MainReason);
            htField.Add("Created", CurrentSigninUser.Name);
            htField.Add("CreateDate", DateTime.Now);
            htField.Add("Checked", "0");
            int result = dbm.Insert("Cus_Knowledge",htField);
            if (result > 0)
            {
                // 所有的操作已经完成
                return new HandlerResponse(0, "添加成功！").ToJson();
            }
            else
            {
                return new HandlerResponse(1, "添加失败！").ToJson();
            }
        }

        /// <summary>
        /// 获取知识点信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string getKnowInfo(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();
            string Id = context.Request["Id"] ?? "";
            if (Id == "") { return "参数异常"; }
            var dtKnow = dbm.GetDataTable("*", "Cus_Knowledge", Id, "Id");
            if (dtKnow.Rows.Count == 0) return "没有找到指定的用户";
            return JSONHelper.DataTable2Json(dtKnow).Replace("[", "").Replace("]", "");
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string UpdateKnowInfo(HttpContext context)
        {
            string OneValue = context.Request["OneValue"] ?? "";
            string TwoValue = context.Request["TwoValue"] ?? "";
            string ThreeValue = context.Request["ThreeValue"] ?? "";
            string FourValue = context.Request["FourValue"] ?? "";
            string MalDescription = context.Request["MalDescription"] ?? "";
            string Solution = context.Request["Solution"] ?? "";
            string MainReason = context.Request["MainReason"] ?? "";
            string Id = context.Request["Id"] ?? "";

            if (Solution == "") { return new HandlerResponse(1, "解决方案不允许为空！").ToJson(); }
            if (MalDescription == "") { return new HandlerResponse(1, "故障描述不允许为空！").ToJson(); }
            if (MainReason == "") { return new HandlerResponse(1, "根本原因不允许为空！").ToJson(); }

            var dbm = DataBaseFactory.Instance.Create();

            Hashtable htField = new Hashtable();
            htField.Add("OneValue", OneValue);
            htField.Add("TwoValue", TwoValue);
            htField.Add("ThreeValue", ThreeValue);
            htField.Add("FourValue", FourValue);
            htField.Add("MalDescription", MalDescription);
            htField.Add("Solution", Solution);
            htField.Add("MainReason", MainReason);

            int result = dbm.Update("Cus_Knowledge", htField,Id,"Id");
            if (result > 0) { return  new HandlerResponse(0, "修改成功！").ToJson();}
            else { return new HandlerResponse(0, "修改失败！").ToJson(); }
        }
        /// <summary>
        /// 知识点详细内容
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string getKnowDetailInfo(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();
            string Id = context.Request["Id"] ?? "";
            if (Id == "") { return "参数异常"; }
            string field = "Id,(select Text from Cus_CaseType where Id = OneValue) as OneValue,(select Text from Cus_CaseType where Id = TwoValue) as TwoValue,(select Text from Cus_CaseType where Id = ThreeValue) as ThreeValue,(select Text from Cus_CaseType where Id = FourValue) as FourValue,MalDescription,Solution,Created,CreateDate,Checked,MainReason";
            string table = "Cus_Knowledge";

            var dtKnow = dbm.GetDataTable(field, table, Id, "Id");
            if (dtKnow.Rows.Count == 0) return "没有找到指定的用户";
            return JSONHelper.DataTable2Json(dtKnow).Replace("[", "").Replace("]", "");
        }
        /// <summary>
        /// 删除知识点
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string DeleteKnow(HttpContext context)
        {
            string Id = context.Request["Id"] ?? "";
            var dbm = DataBaseFactory.Instance.Create();
            int result = dbm.Delete("Cus_Knowledge", Id, "Id");

            if (result > 0) { return new HandlerResponse(0, "删除成功！").ToJson(); }
            else { return new HandlerResponse(1, "删除失败！").ToJson(); }
        }

        private string getMyKnowlist(HttpContext context)
        {

            var dbm = DataBaseFactory.Instance.Create();

            int rows = Convert.ToInt32(context.Request["rows"]);//页大小
            int page = Convert.ToInt32(context.Request["page"]);//当前页
            int pageCount = 0;//页总数
            int total = 0;//数据总数



            string OneValue = context.Request["OneValue"] ?? "";
            string TwoValue = context.Request["TwoValue"] ?? "";
            string ThreeValue = context.Request["ThreeValue"] ?? "";
            string FourValue = context.Request["FourValue"] ?? "";
            string MalDescription = context.Request["MalDescription"] ?? "";
            string CreateDateStart = context.Request["CreateDateStart"] ?? "";
            string CreateDateEnd = context.Request["CreateDateEnd"] ?? "";
            string Created = context.Request["Created"] ?? "";

            Hashtable htCondition = new Hashtable();
            htCondition.Add("Created", CurrentSigninUser.Name);
            if (OneValue != "")
            {
                htCondition.Add("OneValue", OneValue);
            }
            if (TwoValue != "")
            {
                htCondition.Add("TwoValue", TwoValue);
            }
            if (ThreeValue != "")
            {
                htCondition.Add("ThreeValue", ThreeValue);
            }
            if (FourValue != "")
            {
                htCondition.Add("FourValue", FourValue);
            }
            if (MalDescription != "")
            {
                htCondition.Add("MalDescription like @MalDescription", new SqlParameter("@MalDescription", "%" + MalDescription + "%"));
            }
            if (CreateDateStart != "")
            {
                htCondition.Add("CreateDate >= @CreateDateStart", new SqlParameter("@CreateDateStart", CreateDateStart));
            }
            if (CreateDateEnd != "")
            {
                htCondition.Add("CreateDate <= @CreateDateEnd", new SqlParameter("@CreateDateEnd", CreateDateEnd));
            }
            if (Created != "")
            {
                htCondition.Add("Created", Created);
            }

            string field = "Id,(select Text from Cus_CaseType where Id = OneValue) as OneCaseType,OneValue,(select Text from Cus_CaseType where Id = TwoValue) as TwoCaseType,TwoValue,(select Text from Cus_CaseType where Id = ThreeValue) as ThreeCaseType,ThreeValue,(select Text from Cus_CaseType where Id = FourValue) as FourCaseType,FourValue,MalDescription,Solution,Created,CreateDate,Checked,MainReason";
            string table = "Cus_Knowledge";

            // 获取全部用户
            var dtKnowLedgeList = dbm.GetDataTable("Id", field, table, htCondition, "Id DESC", page, rows, out total, out pageCount);

            return "{\"total\":" + total + ",\"rows\":" + JSONHelper.DataTable2Json(dtKnowLedgeList) + "}";
        }

        /// <summary>
        /// 知识点列表待审核
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string getCheckKnowlist(HttpContext context)
        {

            var dbm = DataBaseFactory.Instance.Create();

            int rows = Convert.ToInt32(context.Request["rows"]);//页大小
            int page = Convert.ToInt32(context.Request["page"]);//当前页
            int pageCount = 0;//页总数
            int total = 0;//数据总数


            Hashtable htCondition = new Hashtable();
            htCondition.Add("Checked","0");

            string field = "Id,(select Text from Cus_CaseType where Id = OneValue) as OneCaseType,OneValue,(select Text from Cus_CaseType where Id = TwoValue) as TwoCaseType,TwoValue,(select Text from Cus_CaseType where Id = ThreeValue) as ThreeCaseType,ThreeValue,(select Text from Cus_CaseType where Id = FourValue) as FourCaseType,FourValue,MalDescription,Solution,Created,CreateDate,Checked,MainReason";
            string table = "Cus_Knowledge";

            // 获取全部用户
            var dtKnowLedgeList = dbm.GetDataTable("Id", field, table, htCondition, "Id DESC", page, rows, out total, out pageCount);
            Console.Write(dtKnowLedgeList);
            return "{\"total\":" + total + ",\"rows\":" + JSONHelper.DataTable2Json(dtKnowLedgeList) + "}";

        }


        /// <summary>
        /// 审核知识点
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string CheckKnowledge(HttpContext context)
        {
            string Id = context.Request["Id"] ?? "";
            var dbm = DataBaseFactory.Instance.Create();

            Hashtable htField = new Hashtable();
            htField.Add("Checked","1");

            int result = dbm.Update("Cus_Knowledge", htField, Id, "Id");
            if (result > 0)
            {
                return new HandlerResponse(0, "审核成功！").ToJson();
            }
            else
            {
                return new HandlerResponse(0, "审核失败！").ToJson();
            }
        }

        /// <summary>
        /// 审核未通过
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string NoCheckKnowledge(HttpContext context)
        {
            string Id = context.Request["Id"] ?? "";
            var dbm = DataBaseFactory.Instance.Create();

            Hashtable htCondition = new Hashtable();

            int result = dbm.Delete("Cus_Knowledge", Id, "Id");
            if (result > 0)
            {
                return new HandlerResponse(0, "审核未通过！").ToJson();
            }
            else
            {
                return new HandlerResponse(1, "未通过失败！").ToJson();
            }
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