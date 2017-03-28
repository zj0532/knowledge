using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;  //添加引用：System.Web.Extensions
using System.Runtime.Serialization;
using System.IO;
namespace Pactera.Common.DBUtility
{
    public class Common<T>where T:new()
    {
        /// <summary>
        /// 将DataTable转换成Model实体
        /// </summary>
        /// <param name="obj">Model名称</param>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static T Dt2Model(T obj, DataTable dt)
        {
            if (dt.Rows.Count <= 0) return (T)obj;
            Type type = obj.GetType();
            PropertyInfo[] propertys = type.GetProperties();
            foreach (PropertyInfo p in propertys)
            {
                if (dt.Rows[0][p.Name.ToString()] != DBNull.Value)
                {
                    if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(obj, dt.Rows[0][p.Name], null);
                    }
                    else if (p.PropertyType == typeof(int))
                    {
                        p.SetValue(obj, int.Parse(dt.Rows[0][p.Name].ToString()), null);
                    }
                    else if (p.PropertyType == typeof(DateTime))
                    {
                        p.SetValue(obj, DateTime.Parse(dt.Rows[0][p.Name].ToString()), null);
                    }
                    else if (p.PropertyType == typeof(float))
                    {
                        p.SetValue(obj, float.Parse(dt.Rows[0][p.Name].ToString()), null);
                    }
                    else if (p.PropertyType == typeof(decimal))
                    {
                        p.SetValue(obj, decimal.Parse(dt.Rows[0][p.Name].ToString()), null);
                    }
                    else if (p.PropertyType == typeof(double))
                    {
                        p.SetValue(obj, double.Parse(dt.Rows[0][p.Name].ToString()), null);
                    }
                }
                else
                {
                    p.SetValue(obj, null, null);
                }
            }
            return (T)obj;
        }

        /// <summary>
        /// 将DataTable转换成IList
        /// </summary>
        /// <param name="dt">要转换的DataTable</param>
        /// <returns>返回IList</returns>
        public static IList<T> Dt2List(DataTable dt)
        {
            IList<T> list = new List<T>();
            Type type = typeof(T);
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo p in propertys)
                {

                    object value = dr[p.Name];
                    if (value != DBNull.Value)
                    {
                        if (p.PropertyType == typeof(string))
                        {
                            p.SetValue(t, dr[p.Name].ToString(), null);
                        }
                        else if (p.PropertyType == typeof(int))
                        {
                            p.SetValue(t, int.Parse(dr[p.Name].ToString()), null);
                        }
                        else if (p.PropertyType == typeof(Int64))
                        {
                            p.SetValue(t, Int64.Parse(dr[p.Name].ToString()), null);
                        }
                        else if (p.PropertyType == typeof(DateTime))
                        {
                            p.SetValue(t, DateTime.Parse(dr[p.Name].ToString()), null);
                        }
                        else if (p.PropertyType == typeof(float))
                        {
                            p.SetValue(t, float.Parse(dr[p.Name].ToString()), null);
                        }
                        else if (p.PropertyType == typeof(decimal))
                        {
                            p.SetValue(t, decimal.Parse(dr[p.Name].ToString()), null);
                        }
                        else if (p.PropertyType == typeof(double))
                        {
                            p.SetValue(t, double.Parse(dr[p.Name].ToString()), null);
                        }
                    }
                    else
                    {
                        p.SetValue(t, null, null);
                    }
                }
                list.Add(t);
            }
            return list;
        }

        /// <summary>
        /// 通过反射获得对象名称和自动增长ID
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>返回string[0]类名，string[1]自增ID</returns>
        public static string[] GetModelInfo(T obj)
        {
            string[] str = new string[2];
            Type T = obj.GetType();
            MethodInfo method = T.GetMethod("ReturnAutoID",
                                        BindingFlags.NonPublic
                                        | BindingFlags.Instance,
                                        null, new Type[] { }, null);
            //通过反射执行ReturnAutoID方法，返回AutoID值

            string AutoID = "";
            if (method != null) AutoID = method.Invoke(obj, null).ToString();
            str[0] = T.Name.ToString();
            str[1] = AutoID;
            //返回该Obj的名称以及ReturnAutoID的值
            return str;
        }

        /// <summary>
        /// 填充DataGrid的Json数据格式  
        /// </summary>
        /// <param name="IL">IList数据集</param>
        /// <param name="page">页数</param>
        /// <param name="rows">每页显示的行数</param>
        /// <returns>JSON数据格式</returns>
        public static string onDataGrid<T>(IList<T> IL, int page, int rows)
        {
            page = (page == 0) ? 1 : page;
            rows = (rows == 0) ? 10 : rows;
            int start = (page - 1) * rows;
            int end = page * rows;
            end = (end > IL.Count) ? IL.Count : end;
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"total\":" + IL.Count + ",\"rows\":[");
            for (int i = start; i < end; i++)
            {
                T obj = Activator.CreateInstance<T>();
                Type type = obj.GetType();
                PropertyInfo[] pis = type.GetProperties();
                jsonBuilder.Append("{");
                for (int j = 0; j < pis.Length; j++)
                {
                    if (Convert.ToString(pis[j].GetValue(IL[0], null)) == "0001-1-1 0:00:00")
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToString() + "\":\"\",");
                    }
                    else if (Convert.ToString(pis[j].GetValue(IL[0], null)).Contains(" 0:00:00") && (!Convert.ToString(pis[j].GetValue(IL[0], null)).Contains("♂")))
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToString() + "\":\"" + Convert.ToString(pis[j].GetValue(IL[0], null)).Split(' ')[0] + "\",");
                    }
                    else if (IsDate(Convert.ToString(pis[j].GetValue(IL[0], null))))
                    {
                        jsonBuilder.Append(DateTime.Parse(Convert.ToString(pis[j].GetValue(IL[0], null))).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToString() + "\":\"" + pis[j].GetValue(IL[0], null).ToString().Trim().Replace("\n", "") + "\",");
                    }
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            if (IL.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("]}");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 填充DataGrid的Json数据格式  
        /// </summary>
        /// <param name="IL">IList数据集</param>
        /// <param name="page">页数</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="footer">footer统计数据</param>
        //{"total":28,"rows":[
        //     {"productid":"FI-SW-01","unitcost":10.00,"status":"P","listprice":36.50,"attr1":"Large","itemid":"EST-1"},
        //     {"productid":"K9-DL-01","unitcost":12.00,"status":"P","listprice":18.50,"attr1":"Spotted Adult Female","itemid":"EST-10"},
        //     {"productid":"RP-SN-01","unitcost":12.00,"status":"P","listprice":28.50,"attr1":"Venomless","itemid":"EST-11"},
        //     {"productid":"RP-SN-01","unitcost":12.00,"status":"P","listprice":26.50,"attr1":"Rattleless","itemid":"EST-12"},
        //     {"productid":"RP-LI-02","unitcost":12.00,"status":"P","listprice":35.50,"attr1":"Green Adult","itemid":"EST-13"},
        //     {"productid":"FL-DSH-01","unitcost":12.00,"status":"P","listprice":158.50,"attr1":"Tailless","itemid":"EST-14"},
        //     {"productid":"FL-DSH-01","unitcost":12.00,"status":"P","listprice":83.50,"attr1":"With tail","itemid":"EST-15"},
        //     {"productid":"FL-DLH-02","unitcost":12.00,"status":"P","listprice":63.50,"attr1":"Adult Female","itemid":"EST-16"},
        //     {"productid":"FL-DLH-02","unitcost":12.00,"status":"P","listprice":89.50,"attr1":"Adult Male","itemid":"EST-17"},
        //     {"productid":"AV-CB-01","unitcost":92.00,"status":"P","listprice":63.50,"attr1":"Adult Male","itemid":"EST-18"}
        // ],"footer":[
        //     {"unitcost":19.80,"listprice":60.40,"productid":"Average:"},
        //     {"unitcost":198.00,"listprice":604.00,"productid":"Total:"}
        // ]}
        /// <returns>JSON数据格式</returns>
        public static string onDataGrid<T>(IList<T> IL, int page, int rows, DataTable dtFooter)
        {
            page = (page == 0) ? 1 : page;
            rows = (rows == 0) ? 10 : rows;
            int start = (page - 1) * rows;
            int end = page * rows;
            end = (end > IL.Count) ? IL.Count : end;
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"total\":" + IL.Count + ",\"rows\":[");
            for (int i = start; i < end; i++)
            {
                T obj = Activator.CreateInstance<T>();
                Type type = obj.GetType();
                PropertyInfo[] pis = type.GetProperties();
                jsonBuilder.Append("{");
                for (int j = 0; j < pis.Length; j++)
                {
                    if (Convert.ToString(pis[j].GetValue(IL[0], null)) == "0001-1-1 0:00:00")
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToString() + "\":\"\",");
                    }
                    else if (Convert.ToString(pis[j].GetValue(IL[0], null)).Contains(" 0:00:00") && (!Convert.ToString(pis[j].GetValue(IL[0], null)).Contains("♂")))
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToString() + "\":\"" + Convert.ToString(pis[j].GetValue(IL[0], null)).Split(' ')[0] + "\",");
                    }
                    else if (IsDate(Convert.ToString(pis[j].GetValue(IL[0], null))))
                    {
                        jsonBuilder.Append(DateTime.Parse(Convert.ToString(pis[j].GetValue(IL[0], null))).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToString() + "\":\"" + pis[j].GetValue(IL[0], null).ToString().Trim().Replace("\n", "") + "\",");
                    }
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            if (IL.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("],\"footer\":[");
            jsonBuilder.Append("{");
            for (int j = 0; j < dtFooter.Columns.Count; j++)
            {
                jsonBuilder.Append("\"");
                jsonBuilder.Append(dtFooter.Columns[j].ColumnName.ToLower());
                jsonBuilder.Append("\":\"");
                jsonBuilder.Append(dtFooter.Rows[0][j].ToString().Trim());
                jsonBuilder.Append("\",");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("}");
            jsonBuilder.Append("]}");
            return jsonBuilder.ToString();
        }

        /// <summary>  
        /// 填充DataGrid的Json数据格式  
        /// </summary>  
        /// <param name="dt">数据集</param>  
        /// <param name="page">页数</param>  
        /// <param name="rows">每页行数</param>  
        /// <returns>JSON数据格式</returns>  
        public static string onDataGrid(DataTable dt, int page, int rows)
        {

            page = (page == 0) ? 1 : page;
            rows = (rows == 0) ? 10 : rows;
            int start = (page - 1) * rows;
            int end = page * rows;
            end = (end > dt.Rows.Count) ? dt.Rows.Count : end;
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"total\":" + dt.Rows.Count + ",\"rows\":[");
            for (int i = start; i < end; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName.ToLower());
                    jsonBuilder.Append("\":\"");
                    if (dt.Rows[i - start][j].ToString() == "0001-1-1 0:00:00")
                    {
                        jsonBuilder.Append("");
                    }
                    else if (dt.Rows[i][j].ToString().Contains(" 0:00:00") && (!dt.Rows[i][j].ToString().Contains("♂")))
                    {
                        jsonBuilder.Append(dt.Rows[i][j].ToString().Split(' ')[0]);
                    }
                    else if (IsDate(dt.Rows[i][j].ToString()))
                    {
                        jsonBuilder.Append(DateTime.Parse(dt.Rows[i][j].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        jsonBuilder.Append(dt.Rows[i][j].ToString().Trim().Replace("\r\n", "<br>"));
                    }
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("]}");
            return jsonBuilder.ToString();
        }
        /// <summary>  
        /// 填充DataGrid的Json数据格式  
        /// </summary>  
        /// <param name="dt">数据集</param>  
        /// <param name="page">页数</param>  
        /// <param name="rows">每页行数</param>  
        /// <param name="footer">footer统计数据</param>
        //{"total":28,"rows":[
        //     {"productid":"FI-SW-01","unitcost":10.00,"status":"P","listprice":36.50,"attr1":"Large","itemid":"EST-1"},
        //     {"productid":"K9-DL-01","unitcost":12.00,"status":"P","listprice":18.50,"attr1":"Spotted Adult Female","itemid":"EST-10"},
        //     {"productid":"RP-SN-01","unitcost":12.00,"status":"P","listprice":28.50,"attr1":"Venomless","itemid":"EST-11"},
        //     {"productid":"RP-SN-01","unitcost":12.00,"status":"P","listprice":26.50,"attr1":"Rattleless","itemid":"EST-12"},
        //     {"productid":"RP-LI-02","unitcost":12.00,"status":"P","listprice":35.50,"attr1":"Green Adult","itemid":"EST-13"},
        //     {"productid":"FL-DSH-01","unitcost":12.00,"status":"P","listprice":158.50,"attr1":"Tailless","itemid":"EST-14"},
        //     {"productid":"FL-DSH-01","unitcost":12.00,"status":"P","listprice":83.50,"attr1":"With tail","itemid":"EST-15"},
        //     {"productid":"FL-DLH-02","unitcost":12.00,"status":"P","listprice":63.50,"attr1":"Adult Female","itemid":"EST-16"},
        //     {"productid":"FL-DLH-02","unitcost":12.00,"status":"P","listprice":89.50,"attr1":"Adult Male","itemid":"EST-17"},
        //     {"productid":"AV-CB-01","unitcost":92.00,"status":"P","listprice":63.50,"attr1":"Adult Male","itemid":"EST-18"}
        // ],"footer":[
        //     {"unitcost":19.80,"listprice":60.40,"productid":"Average:"},
        //     {"unitcost":198.00,"listprice":604.00,"productid":"Total:"}
        // ]}
        /// <returns>JSON数据格式</returns>  
        public static string onDataGrid(DataTable dt, int page, int rows, DataTable dtFooter)
        {

            page = (page == 0) ? 1 : page;
            rows = (rows == 0) ? 10 : rows;
            int start = (page - 1) * rows;
            int end = page * rows;
            end = (end > dt.Rows.Count) ? dt.Rows.Count : end;
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"total\":" + dt.Rows.Count + ",\"rows\":[");
            for (int i = start; i < end; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName.ToLower());
                    jsonBuilder.Append("\":\"");
                    if (dt.Rows[i - start][j].ToString() == "0001-1-1 0:00:00")
                    {
                        jsonBuilder.Append("");
                    }
                    else if (dt.Rows[i][j].ToString().Contains(" 0:00:00") && (!dt.Rows[i][j].ToString().Contains("♂")))
                    {
                        jsonBuilder.Append(dt.Rows[i][j].ToString().Split(' ')[0]);
                    }
                    else if (IsDate(dt.Rows[i][j].ToString()))
                    {
                        jsonBuilder.Append(DateTime.Parse(dt.Rows[i][j].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        jsonBuilder.Append(dt.Rows[i][j].ToString().Trim().Replace("\r\n", "<br>"));
                    }
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("],\"footer\":[");
            jsonBuilder.Append("{");
            for (int j = 0; j < dtFooter.Columns.Count; j++)
            {
                jsonBuilder.Append("\"");
                jsonBuilder.Append(dtFooter.Columns[j].ColumnName.ToLower());
                jsonBuilder.Append("\":\"");
                jsonBuilder.Append(dtFooter.Rows[0][j].ToString().Trim());
                jsonBuilder.Append("\",");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("}");
            jsonBuilder.Append("]}");
            jsonBuilder.Append("]}");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 填充DataGrid的Json数据格式，单页方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IL"></param>
        /// <returns></returns>
        public static string onDataGrid<T>(IList<T> IL)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            string tmpvalue;
            jsonBuilder.Append("[");
            for (int i = 0; i < IL.Count; i++)
            {
                T obj = Activator.CreateInstance<T>();
                Type type = obj.GetType();
                PropertyInfo[] pis = type.GetProperties();
                jsonBuilder.Append("{");
                for (int j = 0; j < pis.Length; j++)
                {
                    if (Convert.ToString(pis[j].GetValue(IL[0], null)) == "0001-1-1 0:00:00")
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToLower() + "\":\"\",");
                    }
                    else if (Convert.ToString(pis[j].GetValue(IL[0], null)).Contains(" 0:00:00") && (!Convert.ToString(pis[j].GetValue(IL[0], null)).Contains("♂")))
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToLower() + "\":\"" + DateTime.Parse(Convert.ToString(pis[j].GetValue(IL[i], null)).Split(' ')[0]).ToString("yyyy-MM-dd") + "\",");
                    }
                    else if (IsDate(Convert.ToString(pis[j].GetValue(IL[0], null))))
                    {
                        jsonBuilder.Append(DateTime.Parse(Convert.ToString(pis[j].GetValue(IL[0], null))).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        tmpvalue = (pis[j].GetValue(IL[i], null) == null) ? "" : pis[j].GetValue(IL[i], null).ToString().Trim();
                        jsonBuilder.Append("\"" + pis[j].Name.ToLower() + "\":\"" + tmpvalue.Replace("\n", "") + "\",");
                    }
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            if (IL.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 填充DataGrid的Json数据格式(单页数据加载，提高速度）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IL">IList数据集</param>
        /// <param name="total">数据总个数</param>
        /// <param name="page">页数</param>
        /// <param name="rows">每页显示的行数</param>
        /// <returns>JSON数据格式</returns>
        public static string onDataGrid<T>(IList<T> IL, int total, int page, int rows)
        {
            page = (page == 0) ? 1 : page;
            rows = (rows == 0) ? 10 : rows;
            int start = (page - 1) * rows;
            int end = page * rows;
            end = (end > total) ? total : end;
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"total\":" + total.ToString() + ",\"rows\":[");
            for (int i = start; i < end; i++)
            {
                T obj = Activator.CreateInstance<T>();
                Type type = obj.GetType();
                PropertyInfo[] pis = type.GetProperties();
                jsonBuilder.Append("{");
                for (int j = 0; j < pis.Length; j++)
                {
                    if (Convert.ToString(pis[j].GetValue(IL[0], null)) == "0001-1-1 0:00:00")
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToLower() + "\":\"\",");
                    }
                    else if (Convert.ToString(pis[j].GetValue(IL[0], null)).Contains(" 0:00:00") && (!Convert.ToString(pis[j].GetValue(IL[0], null)).Contains("♂")))
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToLower() + "\":\"" + DateTime.Parse(Convert.ToString(pis[j].GetValue(IL[i - start], null)).Split(' ')[0]).ToString("yyyy-MM-dd") + "\",");
                    }
                    else if (IsDate(Convert.ToString(pis[j].GetValue(IL[0], null))))
                    {
                        jsonBuilder.Append(DateTime.Parse(Convert.ToString(pis[j].GetValue(IL[0], null))).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToLower() + "\":\"" + pis[j].GetValue(IL[i - start], null) + "\",");
                    }
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            if (IL.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("]}");
            return jsonBuilder.ToString();
        }
        /// <summary>
        /// 填充DataGrid的Json数据格式(单页数据加载，提高速度）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="IL">IList数据集</param>
        /// <param name="total">数据总个数</param>
        /// <param name="page">页数</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="footer">footer统计数据</param>
        //{"total":28,"rows":[
        //     {"productid":"FI-SW-01","unitcost":10.00,"status":"P","listprice":36.50,"attr1":"Large","itemid":"EST-1"},
        //     {"productid":"K9-DL-01","unitcost":12.00,"status":"P","listprice":18.50,"attr1":"Spotted Adult Female","itemid":"EST-10"},
        //     {"productid":"RP-SN-01","unitcost":12.00,"status":"P","listprice":28.50,"attr1":"Venomless","itemid":"EST-11"},
        //     {"productid":"RP-SN-01","unitcost":12.00,"status":"P","listprice":26.50,"attr1":"Rattleless","itemid":"EST-12"},
        //     {"productid":"RP-LI-02","unitcost":12.00,"status":"P","listprice":35.50,"attr1":"Green Adult","itemid":"EST-13"},
        //     {"productid":"FL-DSH-01","unitcost":12.00,"status":"P","listprice":158.50,"attr1":"Tailless","itemid":"EST-14"},
        //     {"productid":"FL-DSH-01","unitcost":12.00,"status":"P","listprice":83.50,"attr1":"With tail","itemid":"EST-15"},
        //     {"productid":"FL-DLH-02","unitcost":12.00,"status":"P","listprice":63.50,"attr1":"Adult Female","itemid":"EST-16"},
        //     {"productid":"FL-DLH-02","unitcost":12.00,"status":"P","listprice":89.50,"attr1":"Adult Male","itemid":"EST-17"},
        //     {"productid":"AV-CB-01","unitcost":92.00,"status":"P","listprice":63.50,"attr1":"Adult Male","itemid":"EST-18"}
        // ],"footer":[
        //     {"unitcost":19.80,"listprice":60.40,"productid":"Average:"},
        //     {"unitcost":198.00,"listprice":604.00,"productid":"Total:"}
        // ]}
        /// <returns>JSON数据格式</returns>
        public static string onDataGrid<T>(IList<T> IL, int total, int page, int rows, DataTable dtFooter)
        {
            page = (page == 0) ? 1 : page;
            rows = (rows == 0) ? 10 : rows;
            int start = (page - 1) * rows;
            int end = page * rows;
            end = (end > total) ? total : end;
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"total\":" + total.ToString() + ",\"rows\":[");
            for (int i = start; i < end; i++)
            {
                T obj = Activator.CreateInstance<T>();
                Type type = obj.GetType();
                PropertyInfo[] pis = type.GetProperties();
                jsonBuilder.Append("{");
                for (int j = 0; j < pis.Length; j++)
                {
                    if (Convert.ToString(pis[j].GetValue(IL[0], null)) == "0001-1-1 0:00:00")
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToLower() + "\":\"\",");
                    }
                    else if (Convert.ToString(pis[j].GetValue(IL[0], null)).Contains(" 0:00:00") && (!Convert.ToString(pis[j].GetValue(IL[0], null)).Contains("♂")))
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToLower() + "\":\"" + DateTime.Parse(Convert.ToString(pis[j].GetValue(IL[i - start], null)).Split(' ')[0]).ToString("yyyy-MM-dd") + "\",");
                    }
                    else if (IsDate(Convert.ToString(pis[j].GetValue(IL[0], null))))
                    {
                        jsonBuilder.Append(DateTime.Parse(Convert.ToString(pis[j].GetValue(IL[0], null))).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        jsonBuilder.Append("\"" + pis[j].Name.ToLower() + "\":\"" + pis[j].GetValue(IL[i - start], null).ToString().Trim().Replace("\n", "") + "\",");
                    }
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            if (IL.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("],\"footer\":[");
            jsonBuilder.Append("{");
            for (int j = 0; j < dtFooter.Columns.Count; j++)
            {
                jsonBuilder.Append("\"");
                jsonBuilder.Append(dtFooter.Columns[j].ColumnName.ToLower());
                jsonBuilder.Append("\":\"");
                jsonBuilder.Append(dtFooter.Rows[0][j].ToString().Trim());
                jsonBuilder.Append("\",");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("}");
            jsonBuilder.Append("]}");
            return jsonBuilder.ToString();
        }
        /// <summary>
        /// 填充DataGrid的Json数据格式(单页数据加载，提高速度）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string onDataGrid(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    if (dt.Rows[i][j].ToString() == "0001-1-1 0:00:00")
                    {
                        jsonBuilder.Append("");
                    }
                    else if (dt.Rows[i][j].ToString().Contains(" 0:00:00") && (!dt.Rows[i][j].ToString().Contains("♂")))
                    {
                        jsonBuilder.Append(DateTime.Parse(dt.Rows[i][j].ToString().Split(' ')[0]).ToString("yyyy-MM-dd"));
                    }
                    else if (IsDate(dt.Rows[i][j].ToString()))
                    {
                        jsonBuilder.Append(DateTime.Parse(dt.Rows[i][j].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        jsonBuilder.Append(dt.Rows[i][j].ToString().Trim().Replace("\r\n", "<br>"));
                    }
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("]");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 填充DataGrid的Json数据格式(单页数据加载，提高速度）
        /// </summary>
        /// <param name="dt">数据集</param>
        /// <param name="total">数据总个数</param>
        /// <param name="page">页数</param>
        /// <param name="rows">每页显示的行数</param>
        /// <returns>JSON数据格式</returns>
        public static string onDataGrid(DataTable dt, int total, int page, int rows)
        {
            page = (page == 0) ? 1 : page;
            rows = (rows == 0) ? 10 : rows;
            int start = (page - 1) * rows;
            int end = page * rows;
            end = (end > total) ? total : end;
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"total\":" + total + ",\"rows\":[");
            for (int i = start; i < end; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName.ToLower());
                    jsonBuilder.Append("\":\"");
                    if (dt.Rows[i - start][j].ToString() == "0001-1-1 0:00:00")
                    {
                        jsonBuilder.Append("");
                    }
                    else if (dt.Rows[i - start][j].ToString().Contains(" 0:00:00") && (!dt.Rows[i - start][j].ToString().Contains("♂")))
                    {
                        jsonBuilder.Append(DateTime.Parse(dt.Rows[i - start][j].ToString().Split(' ')[0]).ToString("yyyy-MM-dd"));
                    }
                    else if (IsDate(dt.Rows[i - start][j].ToString()))
                    {
                        jsonBuilder.Append(DateTime.Parse(dt.Rows[i - start][j].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        jsonBuilder.Append(dt.Rows[i - start][j].ToString().Trim().Replace("\r\n", " ").Replace("\n", " "));
                    }
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("]}");
            return jsonBuilder.ToString();
        }
        /// <summary>
        /// 填充DataGrid的Json数据格式(单页数据加载，提高速度）
        /// </summary>
        /// <param name="dt">数据集</param>
        /// <param name="total">数据总个数</param>
        /// <param name="page">页数</param>
        /// <param name="rows">每页显示的行数</param>
        /// <param name="footer">footer统计数据</param>
        //{"total":28,"rows":[
        //     {"productid":"FI-SW-01","unitcost":10.00,"status":"P","listprice":36.50,"attr1":"Large","itemid":"EST-1"},
        //     {"productid":"K9-DL-01","unitcost":12.00,"status":"P","listprice":18.50,"attr1":"Spotted Adult Female","itemid":"EST-10"},
        //     {"productid":"RP-SN-01","unitcost":12.00,"status":"P","listprice":28.50,"attr1":"Venomless","itemid":"EST-11"},
        //     {"productid":"RP-SN-01","unitcost":12.00,"status":"P","listprice":26.50,"attr1":"Rattleless","itemid":"EST-12"},
        //     {"productid":"RP-LI-02","unitcost":12.00,"status":"P","listprice":35.50,"attr1":"Green Adult","itemid":"EST-13"},
        //     {"productid":"FL-DSH-01","unitcost":12.00,"status":"P","listprice":158.50,"attr1":"Tailless","itemid":"EST-14"},
        //     {"productid":"FL-DSH-01","unitcost":12.00,"status":"P","listprice":83.50,"attr1":"With tail","itemid":"EST-15"},
        //     {"productid":"FL-DLH-02","unitcost":12.00,"status":"P","listprice":63.50,"attr1":"Adult Female","itemid":"EST-16"},
        //     {"productid":"FL-DLH-02","unitcost":12.00,"status":"P","listprice":89.50,"attr1":"Adult Male","itemid":"EST-17"},
        //     {"productid":"AV-CB-01","unitcost":92.00,"status":"P","listprice":63.50,"attr1":"Adult Male","itemid":"EST-18"}
        // ],"footer":[
        //     {"unitcost":19.80,"listprice":60.40,"productid":"Average:"},
        //     {"unitcost":198.00,"listprice":604.00,"productid":"Total:"}
        // ]}
        /// <returns>JSON数据格式</returns>
        public static string onDataGrid(DataTable dt, int total, int page, int rows, DataTable dtFooter)
        {
            page = (page == 0) ? 1 : page;
            rows = (rows == 0) ? 10 : rows;
            int start = (page - 1) * rows;
            int end = page * rows;
            end = (end > total) ? total : end;
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"total\":" + total + ",\"rows\":[");
            for (int i = start; i < end; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName.ToLower());
                    jsonBuilder.Append("\":\"");
                    if (dt.Rows[i - start][j].ToString() == "0001-1-1 0:00:00")
                    {
                        jsonBuilder.Append("");
                    }
                    else if (dt.Rows[i - start][j].ToString().Contains(" 0:00:00") && (!dt.Rows[i - start][j].ToString().Contains("♂")))
                    {
                        jsonBuilder.Append(DateTime.Parse(dt.Rows[i - start][j].ToString().Split(' ')[0]).ToString("yyyy-MM-dd"));
                    }
                    else if (IsDate(dt.Rows[i - start][j].ToString()))
                    {
                        jsonBuilder.Append(DateTime.Parse(dt.Rows[i - start][j].ToString()).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        jsonBuilder.Append(dt.Rows[i - start][j].ToString().Trim().Replace("\r\n", "<br>"));
                    }
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            if (dt.Rows.Count > 0)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }

            jsonBuilder.Append("],\"footer\":[");
            jsonBuilder.Append("{");
            for (int j = 0; j < dtFooter.Columns.Count; j++)
            {
                jsonBuilder.Append("\"");
                jsonBuilder.Append(dtFooter.Columns[j].ColumnName.ToLower());
                jsonBuilder.Append("\":\"");
                jsonBuilder.Append(dtFooter.Rows[0][j].ToString().Trim());
                jsonBuilder.Append("\",");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("}");
            jsonBuilder.Append("]}");
            return jsonBuilder.ToString();
        }

        /// <summary>
        /// 验证是否是日期
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDate(string str)
        {
            bool Result = true;
            if (str.Length < 10)
            {
                Result = false;
            }
            else
            {
                try
                {
                    System.Convert.ToDateTime(str);
                }
                catch
                {
                    Result = false;
                }
            }
            return Result;
        }
    }
}


