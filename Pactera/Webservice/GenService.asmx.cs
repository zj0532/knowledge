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
using System.Web.Services;

namespace Pactera.Webservice
{
    /// <summary>
    /// GenService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class GenService : System.Web.Services.WebService
    {
        /// <summary>
        /// 这样就做好了
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public List<Pactera.Model.GenService.Knowledge> KnowledgeList()
        {
            var list = new List<Pactera.Model.GenService.Knowledge>();

            var dbm = DataBaseFactory.Instance.Create();
            Hashtable htCondition = new Hashtable();
            htCondition.Add("Checked", "1");

            DataTable dtKnow = dbm.GetDataTable("*", "Cus_Knowledge", htCondition, "Id");

            foreach (DataRow row in dtKnow.Rows)
            {
                var ki = new Pactera.Model.GenService.Knowledge()
                {//在这里
                    Id = row["Id"].ToString(),
                    OneValue =dbm.GetFieldValue("Cus_CaseType","Text", row["OneValue"].ToString(),"Id"),
                    TwoValue = dbm.GetFieldValue("Cus_CaseType", "Text",row["TwoValue"].ToString(), "Id"),
                    ThreeValue = dbm.GetFieldValue("Cus_CaseType", "Text", row["ThreeValue"].ToString(), "Id"),
                    FourValue = dbm.GetFieldValue("Cus_CaseType", "Text", row["FourValue"].ToString(), "Id"),
                    MainReason = row["MainReason"].ToString(),
                    MalDescription = row["MalDescription"].ToString(),
                    Solution = row["Solution"].ToString(),
                };

                list.Add(ki);
            }


            return list;
        }

        [WebMethod]
        public string InsertOrUpdateKnowledge(string OneValue, string TwoValue, string ThreeValue, string FourValue, string Solution, string MalDescription, string MainReason, string Created, string CreateDate)
        {

            var dbm = DataBaseFactory.Instance.Create();
            Hashtable htCondition = new Hashtable();

            Hashtable htField = new Hashtable();
            htField.Add("OneValue", OneValue);
            htField.Add("TwoValue", TwoValue);
            htField.Add("ThreeValue", ThreeValue);
            htField.Add("FourValue", FourValue);
            htField.Add("Solution", Solution);
            htField.Add("MalDescription", MalDescription);
            htField.Add("MainReason", MainReason);
            htField.Add("Created", Created);
            htField.Add("CreateDate", CreateDate);
            htField.Add("Checked", "0");
            int result = dbm.Insert("Cus_Knowledge", htField);
            if (result > 0)
            {
                // 所有的操作已经完成
                return "知识点添加成功！";
            }
            else
            {
                return "知识点添加失败！";
            }
        }
    }
}
