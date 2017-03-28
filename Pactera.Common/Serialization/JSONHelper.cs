using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Data;
using System.Runtime.Serialization;
using System.IO;
namespace Pactera.Common.Serialization
{
    public static class JSONHelper
    {
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer serialzer = new JavaScriptSerializer();
            return serialzer.Serialize(obj);
        }
        public static string ToJSON(this object obj, int recursionDepth)
        {
            JavaScriptSerializer serialzer = new JavaScriptSerializer();
            serialzer.RecursionLimit = recursionDepth;
            return serialzer.Serialize(obj);
        }
        /// <summary>
        /// DataTable转化为JSON
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTable2Json(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                return "[]";
            }
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");//转换成多个model的形式
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    if (dt.Columns[j].DataType == typeof(int) && dt.Rows[i][j] != DBNull.Value)
                    {
                        string StrValue = dt.Rows[i][j].ToString();
                        //StrValue = StrValue.Replace("<", "< ");
                        //StrValue = StrValue.Replace("\r\n", "<br>");
                        //StrValue = StrValue.Replace("\n", "<br>");
                        jsonBuilder.Append("\":");
                        jsonBuilder.Append(StrValue);
                        jsonBuilder.Append(",");
                    }
                    else
                    {
                        string StrValue = dt.Rows[i][j].ToString().Replace("\"", "\\\"");
                        StrValue = StrValue.Replace("\r\n", "\\r\\n");
                        StrValue = StrValue.Replace("\n", "\\n");
                        StrValue = StrValue.Replace("<", "< ");
                        jsonBuilder.Append("\":\"");
                        jsonBuilder.Append(StrValue);
                        jsonBuilder.Append("\",");
                    }
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// DataTable转化为JSON
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string DataTable2Json(DataTable dt, string Str1)
        {
            if (dt.Rows.Count == 0)
            {
                return "[]";
            }
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");//转换成多个model的形式
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{\"Param\":\"" + Str1 + "\",");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    if (dt.Columns[j].DataType == typeof(int) && dt.Rows[i][j] != DBNull.Value)
                    {
                        string StrValue = dt.Rows[i][j].ToString();
                        StrValue = StrValue.Replace("<", "< ");
                        StrValue = StrValue.Replace("\r\n", "<br>");
                        StrValue = StrValue.Replace("\n", "<br>");
                        jsonBuilder.Append("\":");
                        jsonBuilder.Append(StrValue);
                        jsonBuilder.Append(",");
                    }
                    else
                    {
                        string StrValue = dt.Rows[i][j].ToString();
                        StrValue = StrValue.Replace("<", "< ");
                        StrValue = StrValue.Replace("\r\n", "<br>");
                        StrValue = StrValue.Replace("\n", "<br>");
                        jsonBuilder.Append("\":\"");
                        jsonBuilder.Append(StrValue);
                        jsonBuilder.Append("\",");
                    }
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }
        /// <summary>
        /// JSON转化为List
        /// </summary>
        /// <param name="json"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Object Json2Obj(String json, Type t)
        {
            try
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(t);
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    return serializer.ReadObject(ms);
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        ///List 转化为 JSON
        /// </summary>
        /// <param name="json"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string Obj2Json<T>(T data)
        {
            try
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(data.GetType());
                using (MemoryStream ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, data);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 主子表DataTable转化为JSON
        /// </summary>
        /// <param name="td_father">主表</param>
        /// <param name="td_Child">子表</param>
        ///<param name="KeyID">主表关联列</param>
        /// <param name="FKeyCol">子表关联键</param>
        /// <returns></returns>
        public static string DataSetToJson(DataTable td_father, DataTable td_Child, string KeyID, string FKeyCol)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < td_father.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < td_father.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(td_father.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(td_father.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append(",");
                if (td_father.Rows[i][KeyID] != string.Empty && td_father.Rows[i][KeyID].ToString() != "")
                {
                    //DataRow[] drs = td_Child.Select(FKeyCol + "='" + td_father.Rows[i][KeyID].ToString() + "'");
                    for (int j = 0; j < td_Child.Columns.Count; j++)
                    {
                        if (td_Child.Columns[j].ColumnName != FKeyCol)
                        {
                            jsonBuilder.Append("\"");
                            jsonBuilder.Append(td_Child.Columns[j].ColumnName);
                            jsonBuilder.Append("\":\"");
                            DataView dv = td_Child.AsDataView();
                            dv.RowFilter = FKeyCol + "='" + td_father.Rows[i][KeyID].ToString() + "'";
                            DataTable temp = dv.ToTable();
                            if (temp.Rows.Count > 0)
                            {
                                foreach (DataRow dr in temp.Rows)
                                {
                                    if (dr[j].ToString() != "" && dr[j] != string.Empty)
                                    {
                                        jsonBuilder.Append(dr[j].ToString());
                                        jsonBuilder.Append("@@");
                                    }
                                }
                                jsonBuilder.Remove(jsonBuilder.Length - 2, 2);
                            }
                            jsonBuilder.Append("\",");
                        }
                    }
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            if (jsonBuilder.Length > 1)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }
    }
}