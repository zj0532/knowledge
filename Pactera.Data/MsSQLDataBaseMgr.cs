using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pactera.Data
{
    public class MsSQLDataBaseMgr : BaseDataBaseMgr, IDataBaseMgr
    {
        private readonly string connString = ConfigurationManager.ConnectionStrings["MsSQLServer"].ConnectionString.ToString();
        //private readonly string connString = "Data Source=10.20.0.22\\SQL2012;Initial Catalog=CnqcMarketManagement;Persist Security Info=True;User ID=sa;Password=sql123!;Pooling=False";

        /// <summary>
        /// 执行删除
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="ID">主键值</param>
        /// <param name="pk">主键字段名称</param>
        /// <returns>删除结果</returns>
        public int Delete(string table, string id, string pk)
        {
            string sql = "DELETE FROM " + table + " where " + pk + "=@Id";
            ArrayList args = new ArrayList();
            args.Add(new SqlParameter("Id", id));

            return ExecuteSql(sql, args.ToArray());
        }

        /// <summary>
        /// 根据查询条件删除
        /// </summary>
        /// <param name="table"></param>
        /// <param name="htCondition"></param>
        /// <returns></returns>
        public int Delete(string table, Hashtable htCondition)
        {
            Array args;
            string conditionStr = GetConditionString(htCondition, out args);
            string sql = string.Format("DELETE FROM {0} WHERE 1=1{1}", table, conditionStr);
            return ExecuteSql(sql, args);
        }

        /// <summary>
        /// 执行插入
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="htFilds">字段和值</param>
        /// <returns>结果</returns>
        public int Insert(string table, Hashtable htField)
        {
            StringBuilder sbField = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();
            ArrayList args = new ArrayList();

            foreach (DictionaryEntry de in htField)
            {
                // 参数集合
                args.Add(new SqlParameter(de.Key.ToString(), de.Value));
                // 字段和值
                sbField.Append("[" + de.Key.ToString() + "],");
                sbValue.Append("@" + de.Key.ToString() + ",");
            }

            // 去除最后的逗号
            sbField.Remove(sbField.Length - 1, 1);
            sbValue.Remove(sbValue.Length - 1, 1);

            string sql = string.Format
            (
                "INSERT INTO {0}({1})VALUES({2})",
                table,
                sbField.ToString(),
                sbValue.ToString()
            );

            return ExecuteSql(sql, args.ToArray());
        }

        /// <summary>
        /// 执行插入，返回ID
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="htField">字段和值</param>
        /// <param name="id">ID</param>
        public void Insert(string table, Hashtable htField, out int id)
        {
            StringBuilder sbField = new StringBuilder();
            StringBuilder sbValue = new StringBuilder();
            ArrayList args = new ArrayList();

            foreach (DictionaryEntry de in htField)
            {
                // 参数集合
                args.Add(new SqlParameter(de.Key.ToString(), de.Value));
                // 字段和值
                sbField.Append("[" + de.Key.ToString() + "],");
                sbValue.Append("@" + de.Key.ToString() + ",");
            }

            // 去除最后的逗号
            sbField.Remove(sbField.Length - 1, 1);
            sbValue.Remove(sbValue.Length - 1, 1);

            string sql = string.Format
            (
                "INSERT INTO {0}({1})VALUES({2});SELECT @@IDENTITY",
                table,
                sbField.ToString(),
                sbValue.ToString()
            );

            id = Convert.ToInt32(ExecuteScalar(sql, args.ToArray()));
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="htFilds">字段和值</param>
        /// <param name="id">主键值</param>
        /// <param name="pk">主键字段名称</param>
        /// <returns>结果</returns>
        public int Update(string table, Hashtable htField, string id, string pk)
        {
            StringBuilder sbField = new StringBuilder();
            ArrayList args = new ArrayList();

            foreach (DictionaryEntry de in htField)
            {
                // 参数集合
                args.Add(new SqlParameter(de.Key.ToString(), de.Value));
                // 要更新的字段
                sbField.Append(de.Key.ToString() + "=@" + de.Key + ",");
            }

            // 去除最后的逗号
            sbField.Remove(sbField.Length - 1, 1);

            // 添加ID
            args.Add(new SqlParameter(pk, id));

            string sql = string.Format
            (
                "UPDATE {0} SET {1} WHERE " + pk + "=@" + pk,
                table,
                sbField.ToString()
            );

            return ExecuteSql(sql, args.ToArray());

        }

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="htField">字段和值</param>
        /// <param name="htCondition">查询条件</param>
        /// <returns></returns>
        public int Update(string table, Hashtable htField, Hashtable htCondition)
        {
            // 更新字段和值
            StringBuilder sbField = new StringBuilder();
            ArrayList array = new ArrayList();

            // 查询字段和值
            string condition = GetConditionString1(htCondition, out array);

            foreach (DictionaryEntry de in htField)
            {
                // 参数集合
                array.Add(new SqlParameter(de.Key.ToString(), de.Value));
                // 要更新的字段
                sbField.Append(de.Key.ToString() + "=@" + de.Key + ",");
            }

            // 去除最后的逗号
            sbField.Remove(sbField.Length - 1, 1);

            // 拼接查询结果
            string sql = string.Format
            (
                "UPDATE {0} SET {1} WHERE 1=1 {2}",
                table,
                sbField.ToString(),
                condition
            );

            return ExecuteSql(sql, array.ToArray());

        }

        /// <summary>
        /// 获取单个字段值
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="field">字段名</param>
        /// <param name="id">主键值</param>
        /// <param name="pk">主键字段名称</param>
        /// <returns></returns>
        public string GetFieldValue(string table, string field, string id, string pk)
        {
            // 执行查询
            var args = new ArrayList();
            args.Add(new SqlParameter(pk, id));
            string sql = string.Format("SELECT {0} FROM {1} WHERE {2}=@{2}", field, table, pk);

            var dtFieldValue = GetDataTable(sql, args.ToArray());

            // 没有查到任何结果
            if (dtFieldValue.Rows.Count == 0) { return null; }

            // 如果结果中包含多表连接查询，去除(.)，如(a.Name)去除(a.)
            if (field.IndexOf('.') != -1) field = field.Substring(field.IndexOf('.') + 1);
            return dtFieldValue.Rows[0][field].ToString();
        }
        public string GetFieldValue(string table, string field, Hashtable htCondition, string order)
        {
            // 查询条件和参数
            Array args;
            string sql = string.Format("SELECT {0} FROM {1} WHERE 1=1{2}", field, table, GetConditionString(htCondition, out args));

            // 查询条件
            if (!string.IsNullOrEmpty(order)) sql += " " + order;

            // 执行查询
            object result = ExecuteScalar(sql, args);

            if (result == null) return null;

            return result.ToString();
        }

        /// <summary>
        /// 获取记录数量
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="htCondtion">查询条件</param>
        /// <returns></returns>
        public int GetRecordCount(string table, Hashtable htCondition)
        {
            // 查询命令
            string sql = "SELECT COUNT(*) FROM " + table + " WHERE 1=1";

            // 查询条件和参数
            Array args;
            sql += GetConditionString(htCondition, out args);

            // 执行查询
            int count = Convert.ToInt32(ExecuteScalar(sql, args));
            return count;
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="htCondtion">查询条件</param>
        /// <returns></returns>
        public bool Exists(string table, Hashtable htCondition)
        {
            // 查询命令
            string sql = "SELECT COUNT(*) FROM " + table + " WHERE 1=1";

            // 查询条件和参数
            Array args;
            sql += GetConditionString(htCondition, out args);

            // 执行查询
            object result = ExecuteScalar(sql, args);

            // 返回结果
            if (result == null) { return false; }
            if (result.ToString() == "0") { return false; }
            return true;
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="id">主键值</param>
        /// <param name="pk">主键字段名称</param>
        /// <returns></returns>
        public bool Exists(string table, string id, string pk)
        {
            string sql = "SELECT COUNT(*) FROM " + table + " WHERE " + pk + "=" + id;

            object result = ExecuteScalar(sql);

            if (result == null) { return false; }
            if (result.ToString() == "0") { return false; }
            return true;
        }

        /// <summary>
        /// 获取查询条件字符串
        /// </summary>
        /// <param name="htCondition">查询条件</param>
        /// <param name="args">参数集合</param>
        /// <returns></returns>
        public string GetConditionString(Hashtable htCondition, out Array args)
        {
            StringBuilder sbCondition = new StringBuilder();
            ArrayList array = new ArrayList();

            // 拼接查询条件
            foreach (DictionaryEntry de in htCondition)
            {
                if (de.Key.ToString().IndexOf(' ') != -1)
                {
                    sbCondition.Append(" AND " + de.Key);
                    if (de.Value != null)
                    {
                        array.Add(de.Value as SqlParameter);
                    }
                    continue;
                }

                sbCondition.Append(" AND " + de.Key + "=@" + de.Key);
                array.Add(new SqlParameter(de.Key.ToString(), de.Value));
            }
            args = array.ToArray();

            // 返回查询条件字符串
            return sbCondition.ToString();
        }

        /// <summary>
        /// 获取查询条件字符串
        /// </summary>
        /// <param name="htCondition">查询条件</param>
        /// <param name="args">参数集合</param>
        /// <returns></returns>
        public string GetConditionString1(Hashtable htCondition, out ArrayList array)
        {
            ////属性////
            StringBuilder sbCondition = new StringBuilder();
            array = new ArrayList();

            ////拼接查询条件////
            foreach (DictionaryEntry de in htCondition)
            {
                if (de.Key.ToString().IndexOf(' ') != -1)
                {
                    sbCondition.Append(" AND " + de.Key);
                    array.Add(de.Value as SqlParameter);
                    continue;
                }

                sbCondition.Append(" AND " + de.Key + "=@" + de.Key);
                array.Add(new SqlParameter(de.Key.ToString(), de.Value.ToString()));
            }

            ////返回查询条件字符串////
            return sbCondition.ToString();
        }

        /// <summary>
        /// 多表查询 - 通过ID获取
        /// </summary>
        /// <param name="field">要查询的字段，如：* | Id,Field</param>
        /// <param name="table">表名</param>
        /// <param name="id">主键值</param>
        /// <param name="pk">主键字段名称</param>
        /// <returns></returns>
        public DataTable GetDataTable(string field, string table, string id, string pk)
        {
            ArrayList args = new ArrayList();
            args.Add(new SqlParameter("@Id", id));
            string sql = string.Format("SELECT {0} FROM {1} WHERE " + pk + "=@Id", field, table);
            return GetDataTable(sql, args.ToArray());
        }

        /// <summary>
        /// 多表查询 - 通过查询条件获取
        /// </summary>
        /// <param name="field">查询字段，如： A.Field, B.Field</param>
        /// <param name="table">表名，如： TableOne as A, TableTwo as B</param>
        /// <param name="htCondition">查询条件</param>
        ///  <param name="order">排序,包括字段及排序方式(如：ordernum desc),也可以用""不排序</param>
        /// <returns></returns>
        public DataTable GetDataTable(string field, string table, Hashtable htCondition, string order)
        {
            Array args;
            string condition = GetConditionString(htCondition, out args);

            string sql = string.Format("SELECT {0} FROM {1} WHERE 1=1 {2} ", field, table, condition);
            if (order != "")
                sql += " order by " + order;

            return GetDataTable(sql, args);
        }

        /// <summary>
        /// 多表查询 - 分页
        /// </summary>
        /// <param name="key">主键字段</param>
        /// <param name="field">要查询的字段，如：* | Id,Field</param>
        /// <param name="table">表名</param>
        /// <param name="htCondition">条件字符串</param>
        /// <param name="order">排序，包括排序字段和排序方式(如：id desc)</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页显示行数</param>
        /// <param name="recordCount">总行数</param>
        /// <param name="pageCount">总共页码数</param>
        /// <returns></returns>
        public DataTable GetDataTable(string key, string field, string table, Hashtable htCondition,
            string order, int pageIndex, int pageSize, out int recordCount, out int pageCount)
        {
            Array args;
            string condition = GetConditionString(htCondition, out args);

            return GetPageNavigateDataTable(key, field, table, condition, order, args, pageIndex, pageSize, out recordCount, out pageCount);

        }

        /// <summary>
        /// 多表查询 - 通过存储过程获取
        /// </summary>
        /// <param name="procName">存储过程</param>
        /// <param name="htCondition">查询条件</param>
        /// <returns></returns>
        public DataTable GetDataTableByProcName(string procName, Hashtable htCondition)
        {
            Array args;
            string condition = GetConditionString(htCondition, out args);

            return ExecuteProcedureToDt(procName, args);
        }

        //--------------------------------------
        /// <summary>
        /// 执行SQL语句,返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        return cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (cmd != null)
                        {
                            cmd.Dispose();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 执行SQL语句,返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public int ExecuteSql(string sql, Array args)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    try
                    {
                        int result = 0;
                        connection.Open();
                        cmd.Parameters.AddRange(args);
                        result = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return result;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (cmd != null)
                        {
                            cmd.Dispose();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句,返回单个对象
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if (Object.Equals(obj, null) || Object.Equals(obj, System.DBNull.Value)) { return null; } else { return obj; }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (cmd != null)
                        {
                            cmd.Dispose();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 执行查询语句,返回单个对象
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="arg">参数</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, Array args)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.Parameters.AddRange(args);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if (Object.Equals(obj, null) || Object.Equals(obj, System.DBNull.Value)) { return null; } else { return obj; }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (cmd != null)
                        {
                            cmd.Dispose();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 执行存储过程,返回单个对象
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="args">参数</param>
        /// <returns>单个结果</returns>
        public object ExecuteProcedure(string procName, Array args)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(procName, connection))
                {
                    try
                    {

                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(args);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if (Object.Equals(obj, null) || Object.Equals(obj, System.DBNull.Value)) { return null; }
                        return obj;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (cmd != null)
                        {
                            cmd.Dispose();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行存储过程,返回DataTable
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public DataTable ExecuteProcedureToDt(string procName, Array args)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand(procName, connection))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.SelectCommand.Parameters.AddRange(args);
                        adapter.Fill(dt);
                        adapter.SelectCommand.Parameters.Clear();
                        return dt;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (cmd != null)
                        {
                            cmd.Dispose();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行SQL语句 返回DataTable
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(string sql)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(sql, connection))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        adapter.Fill(dt);
                        return dt;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (adapter != null)
                        {
                            adapter.Dispose();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行SQL语句 返回DataTable
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="args">参数集合</param>
        /// <returns></returns>
        public System.Data.DataTable GetDataTable(string sql, Array args)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(sql, connection))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        adapter.SelectCommand.Parameters.AddRange(args);
                        adapter.Fill(dt);
                        adapter.SelectCommand.Parameters.Clear();
                        return dt;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        if (adapter != null)
                        {
                            adapter.Dispose();
                        }
                        if (connection != null)
                        {
                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 多表查询 - 分页
        /// </summary>
        /// <param name="key">主键字段</param>
        /// <param name="field">要查询的字段，如：* | Id,Field</param>
        /// <param name="table">表名</param>
        /// <param name="htCondition">条件字符串</param>
        /// <param name="order">排序，包括排序字段和排序方式(如：id desc)</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页显示行数</param>
        /// <param name="recordCount">总行数</param>
        /// <param name="pageCount">总共页码数</param>
        /// <returns></returns>
        public DataTable GetPageNavigateDataTable(string key, string field, string table, string condition,
            string order, Array args, int pageIndex, int pageSize, out int recordCount, out int pageCount)
        {
            ////获取记录数////
            string sql = "SELECT COUNT (*) FROM " + table + " WHERE 1=1" + condition;
            if (args.Length > 0)
                recordCount = (int)ExecuteScalar(sql, args);
            else
                recordCount = (int)ExecuteScalar(sql);

            ////获取页数////
            pageCount = recordCount % pageSize == 0 ? recordCount / pageSize : recordCount / pageSize + 1;

            ////显示当前页的范围，下面两行上下顺序不可颠倒，如果颠倒，记录为0时会出现 负值////
            if (pageIndex > pageCount) { pageIndex = pageCount; }
            if (pageIndex < 1) { pageIndex = 1; }

            //判断排序参数order是否为空
            string orderNew = "";
            if (!string.IsNullOrEmpty(order))
            {
                orderNew = " order by " + order;
            }

            ////执行查询////
            if (pageIndex == 1)
            {
                sql = "SELECT TOP " + pageSize + " " + field + " FROM " + table + " WHERE 1=1" + condition + orderNew;
            }
            else
            {
                sql = "SELECT TOP " + pageSize + " " + field + " FROM " + table + " WHERE " + key + " NOT IN";
                sql += "(SELECT TOP " + (pageIndex - 1) * pageSize + " " + key + " FROM " + table + " WHERE 1=1" + condition + orderNew + ")";
                sql += condition + orderNew;
            }

            ////返回查询结果////
            return GetDataTable(sql, args);
        }
    }
}
