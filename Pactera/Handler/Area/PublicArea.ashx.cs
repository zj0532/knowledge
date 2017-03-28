using Pactera.Common.Serialization;
using Pactera.Data;
using Pactera.Model.Json;
using Pactera.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace Pactera.Handler.Area
{
    /// <summary>
    /// PublicArea 的摘要说明
    /// </summary>
    public class PublicArea : Pactera.Web.BaseHttpHandler, IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string Action = context.Request["Action"] ?? "";
            string json = "";
            switch (Action)
            {
                case "InsertNodeAndReturn":
                    json = InsertNodeAndReturn(context);
                    break;
                case "GetAreaListJson":
                    json = GetAreaListJson(context);
                    break;
                case "UpdateNode":
                    json = UpdateNode(context);
                    break;
                case "DeleteNode":
                    json = DeleteNode(context);
                    break;
                case "InsertTopNode":
                    json = InsertTopNode(context);
                    break;

                case "AreaJilian":
                    json = GetCaseType(context);
                    break;
                case "Engineer":
                    json = getEngineer(context);
                    break;

            }
            context.Response.ContentType = "text/plain";
            context.Response.Write(json);
        }

        #region >>更新节点<<
        private string UpdateNode(HttpContext context)
        {
            var jsonData = new LitJson.JsonData();

            // 获取父级节点ID
            int id;
            if (!int.TryParse(context.Request["Id"], out id))
            {
                jsonData["StateCode"] = 1;
                jsonData["Message"] = "参数异常！";
                return LitJson.JsonMapper.ToJson(jsonData);
            }
            string text = context.Request["Text"] ?? "";

            var dbm = DataBaseFactory.Instance.Create();

            Hashtable htField = new Hashtable();
            htField.Add("Text", text);

            int result = dbm.Update("Cus_Area", htField, id.ToString(), "Id");

            // 插入成功，返回结果
            if (result > 0)
            {
                jsonData["StateCode"] = 0;
                jsonData["Message"] = "操作完成！";
                return LitJson.JsonMapper.ToJson(jsonData);
            }

            jsonData["StateCode"] = 2;
            jsonData["Message"] = "新增节点时失败，请稍后再试！";
            return LitJson.JsonMapper.ToJson(jsonData);
        }
        #endregion

        private string InsertTopNode(HttpContext context)
        {
            var jsonData = new LitJson.JsonData();

            // 获取父级节点ID
            int parentId, level;
            if (!int.TryParse(context.Request["ParentId"], out parentId) || !int.TryParse(context.Request["Level"], out level))
            {
                jsonData["StateCode"] = 1;
                jsonData["Message"] = "参数异常！";
                return LitJson.JsonMapper.ToJson(jsonData);
            }

            var dbm = DataBaseFactory.Instance.Create();

            Hashtable htField = new Hashtable();
            htField.Add("Text", "新增顶级节点");
            htField.Add("ParentId", parentId);
            htField.Add("Level", level);
            htField.Add("IsDelete", "1");

            int nodeId = 0;
            dbm.Insert("Cus_Area", htField, out nodeId);

            // 插入成功，返回结果
            if (nodeId > 0)
            {
                jsonData["StateCode"] = 0;
                jsonData["Message"] = "操作完成！";
                jsonData["Id"] = nodeId;
                jsonData["ParentId"] = parentId;
                jsonData["Text"] = "新增节点";
                jsonData["Level"] = level;
                jsonData["IsDelete"] = "1";

                return LitJson.JsonMapper.ToJson(jsonData);
            }

            jsonData["StateCode"] = 2;
            jsonData["Message"] = "新增节点时失败，请稍后再试！";
            return LitJson.JsonMapper.ToJson(jsonData);
        }

        #region >>插入并返回节点<<
        private string InsertNodeAndReturn(HttpContext context)
        {
            var jsonData = new LitJson.JsonData();

            // 获取父级节点ID
            int parentId, level;
            if (!int.TryParse(context.Request["ParentId"], out parentId) || !int.TryParse(context.Request["Level"], out level))
            {
                jsonData["StateCode"] = 1;
                jsonData["Message"] = "参数异常！";
                return LitJson.JsonMapper.ToJson(jsonData);
            }

            var dbm = DataBaseFactory.Instance.Create();

            Hashtable htField = new Hashtable();
            htField.Add("Text", "新增节点");
            htField.Add("ParentId", parentId);
            htField.Add("Level", level);
            htField.Add("IsDelete", "1");

            int nodeId = 0;
            dbm.Insert("Cus_Area", htField, out nodeId);

            // 插入成功，返回结果
            if (nodeId > 0)
            {
                jsonData["StateCode"] = 0;
                jsonData["Message"] = "操作完成！";
                jsonData["Id"] = nodeId;
                jsonData["ParentId"] = parentId;
                jsonData["Text"] = "新增节点";
                jsonData["Level"] = level;
                jsonData["IsDelete"] = "1";

                return LitJson.JsonMapper.ToJson(jsonData);
            }

            jsonData["StateCode"] = 2;
            jsonData["Message"] = "新增节点时失败，请稍后再试！";
            return LitJson.JsonMapper.ToJson(jsonData);
        }
        #endregion

        #region >>获取区域列表<<
        /// <summary>
        /// 省份列表
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private string GetAreaListJson(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();

            Hashtable htCondition = new Hashtable();
            htCondition.Add("ParentId", "0");
            htCondition.Add("IsDelete", "1");
            var dtProvince = dbm.GetDataTable("*", "Cus_Area", htCondition, "Id");

            // 首先我们看data它本身是个集合，所以我们需要创建一个集合,我移动也没动不赖我
            List<AreaTreeJson> data = new List<AreaTreeJson>();
            foreach (DataRow provRow in dtProvince.Rows)
            {
                var province = new AreaTreeJson();
                province.id = int.Parse(provRow["Id"].ToString());
                province.text = provRow["Text"].ToString();
                province.Level = int.Parse(provRow["Level"].ToString());
                province.ParentId = int.Parse(provRow["ParentId"].ToString());

                // 取出所有城市
                htCondition["ParentId"] = province.id;
                var dtCity = dbm.GetDataTable("*", "Cus_Area", htCondition, "Id");
                foreach (DataRow cityRow in dtCity.Rows)
                {
                    var city = new AreaTreeJson();
                    city.id = int.Parse(cityRow["Id"].ToString());
                    city.text = cityRow["Text"].ToString();
                    city.Level = int.Parse(cityRow["Level"].ToString());
                    city.ParentId = int.Parse(cityRow["ParentId"].ToString());

                    if (province.children == null) province.children = new List<AreaTreeJson>();
                    province.children.Add(city);

                    // 取出所有区县
                    htCondition["ParentId"] = city.id;
                    var dtDistrict = dbm.GetDataTable("*", "Cus_Area", htCondition, "Id");
                    foreach (DataRow districtRow in dtDistrict.Rows)
                    {
                        var district = new AreaTreeJson();
                        district.id = int.Parse(districtRow["Id"].ToString());
                        district.text = districtRow["Text"].ToString();
                        district.Level = int.Parse(districtRow["Level"].ToString());
                        district.ParentId = int.Parse(districtRow["ParentId"].ToString());

                        if (city.children == null) city.children = new List<AreaTreeJson>();
                        city.children.Add(district);
                    }
                }

                data.Add(province);
            }

            return Pactera.Common.Serialization.JsonSerializer.ToJsonString(data);
        }
        #endregion

        private string DeleteNode(HttpContext context)
        {

            var jsonData = new LitJson.JsonData();

            string Id = context.Request["Id"] ?? "";
            string Level = context.Request["Level"] ?? "";
            int result = 0;

            var dbm = DataBaseFactory.Instance.Create();
            ///根据ID获取ParentID
            Hashtable htCondition = new Hashtable();

            Hashtable htField = new Hashtable();
            htField.Add("IsDelete", "0");
            result = dbm.Update("Cus_Area", htField, Id, "Id");
            if (Level != "")
            {

                string ParentID = dbm.GetFieldValue("Cus_Area", "ParentId", Id, "ID");

                if (ParentID == "0")
                {
                    htCondition.Clear();
                    htCondition.Add("ParentId", Id);
                    DataTable dt= dbm.GetDataTable("*","Cus_Area",htCondition,"Id");
                    foreach (DataRow row in dt.Rows)
                    {

                       result =  dbm.Update("Cus_Area", htField, htCondition);
                        //获取Parent=ID的所有ID(二级ID)
                        string Id02 = row["Id"].ToString();
                        htCondition.Clear();
                        htCondition.Add("ParentId", Id02);
                        DataTable dt02 = dbm.GetDataTable("*", "Cus_Area", htCondition, "Id");
                        foreach (DataRow row02 in dt02.Rows)
                        {
                          result =  dbm.Update("Cus_Area", htField, htCondition);
                        }
                    }
                }
                if(ParentID!="0")
                {
                    htCondition.Add("ParentId", Id);
                    DataTable dt = dbm.GetDataTable("*", "Cus_Area", htCondition, "Id");
                    foreach (DataRow row in dt.Rows)
                    {
                       result = dbm.Update("Cus_Area", htField, htCondition);
                    }
                }
            }
            else
            {
                result = dbm.Update("Cus_Area", htField, Id, "Id");
            }

            // 插入成功，返回结果
            if (result > 0)
            {
                jsonData["StateCode"] = 0;
                jsonData["Message"] = "操作完成！";
                return LitJson.JsonMapper.ToJson(jsonData);
            }

            jsonData["StateCode"] = 2;
            jsonData["Message"] = "新增节点时失败，请稍后再试！";
            return LitJson.JsonMapper.ToJson(jsonData);
        }

        private string GetCaseType(HttpContext context)
        {
            // 获取参数
            string r = context.Request["r"] ?? "";
            string parentId = context.Request["ParentId"] ?? "";

            Hashtable htCondition = new Hashtable();
            htCondition.Add("IsDelete", "1");
            htCondition.Add("ParentId", parentId);

            var dtCaseType = DataBaseFactory.Instance.Create().GetDataTable("*", "Cus_Area", htCondition, "Text");

            return Pactera.Common.Serialization.JSONHelper.DataTable2Json(dtCaseType).Replace("\\", "\\\\");
        }

        private string getEngineer(HttpContext context)
        {
            var dbm = DataBaseFactory.Instance.Create();
            
            Hashtable htCondition = new Hashtable();
            htCondition.Add("ParentId", "0");
            htCondition.Add("IsDelete", "1");
            var dtProvince = dbm.GetDataTable("*", "Cus_Area", htCondition, "Id");

            // 首先我们看data它本身是个集合，所以我们需要创建一个集合,我移动也没动不赖我
            List<AreaTreeJson> data = new List<AreaTreeJson>();
            foreach (DataRow provRow in dtProvince.Rows)
            {
                var province = new AreaTreeJson();
                province.id = int.Parse(provRow["Id"].ToString());
                province.text = provRow["Text"].ToString();
                province.Level = int.Parse(provRow["Level"].ToString());
                province.ParentId = int.Parse(provRow["ParentId"].ToString());

                // 取出所有城市
                htCondition["ParentId"] = province.id;
                var dtCity = dbm.GetDataTable("*", "Cus_Area", htCondition, "Id");
                foreach (DataRow cityRow in dtCity.Rows)
                {
                    var city = new AreaTreeJson();
                    city.id = int.Parse(cityRow["Id"].ToString());
                    city.text = cityRow["Text"].ToString();
                    city.Level = int.Parse(cityRow["Level"].ToString());
                    city.ParentId = int.Parse(cityRow["ParentId"].ToString());

                    if (province.children == null) province.children = new List<AreaTreeJson>();
                    province.children.Add(city);

                    // 取出所有区县
                    htCondition["ParentId"] = city.id;
                    var dtDistrict = dbm.GetDataTable("*", "Cus_Area", htCondition, "Id");
                    foreach (DataRow districtRow in dtDistrict.Rows)
                    {
                        var district = new AreaTreeJson();
                        district.id = int.Parse(districtRow["Id"].ToString());
                        district.text = districtRow["Text"].ToString();
                        district.Level = int.Parse(districtRow["Level"].ToString());
                        district.ParentId = int.Parse(districtRow["ParentId"].ToString());

                        if (city.children == null) city.children = new List<AreaTreeJson>();
                        city.children.Add(district);
                        
                    }
                }

                data.Add(province);
            }

            return Pactera.Common.Serialization.JsonSerializer.ToJsonString(data);
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