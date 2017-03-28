using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace Pactera.Data
{
    //public class OracleDataBaseMgr : BaseDataBaseMgr, IDataBaseMgr
	public class OracleDataBaseMgr : IDataBaseMgr
    {
        //private readonly string connString = ConfigurationManager.ConnectionStrings["Oracle"].ConnectionString;
		private string _connString = string.Empty;
		public OracleDataBaseMgr(DataBaseChoice choice)
		{
			if (choice == DataBaseChoice.OaUserOracle) _connString = ConfigurationManager.ConnectionStrings["OaUserOracle"].ConnectionString;
		}

		#region >>执行删除<<
		/// <summary>
        /// 执行删除
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="ID">主键值</param>
        /// <param name="pk">主键字段名称</param>
        /// <returns>删除结果</returns>
        public int Delete(string table, string id, string pk)
        {
            string sql = "DELETE FROM " + table + " WHERE " + pk + "=:Id";
            ArrayList args = new ArrayList();
            args.Add(new OracleParameter(pk, id));
            //注意参数定义：IDataParameter[] params={new OracleParameter("Id", id)}
            return ExecuteSql(sql, args.ToArray());
        }

        public int Delete(string table, Hashtable htCondition)
        {
			Array args;
			string condition = GetConditionString(htCondition, out args);

			string sql = string.Format("DELETE FROM " + table + " WHERE 1=1 {1}", condition);
			return ExecuteSql(sql, args);
        }
		#endregion

		#region >>执行插入<<
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
                args.Add(new OracleParameter(de.Key.ToString(), de.Value));
                // 字段和值
                sbField.Append("[" + de.Key.ToString() + "],");
                sbValue.Append(":" + de.Key.ToString() + ",");
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
                args.Add(new OracleParameter(de.Key.ToString(), de.Value));
                // 字段和值
                sbField.Append("[" + de.Key.ToString() + "],");
                sbValue.Append(":" + de.Key.ToString() + ",");
            }

            // 去除最后的逗号
            sbField.Remove(sbField.Length - 1, 1);
            sbValue.Remove(sbValue.Length - 1, 1);

            string sql = string.Format
            (
                "INSERT INTO {0}({1})VALUES({2});return[PrimaryKey INTO]",
                table,
                sbField.ToString(),
                sbValue.ToString()
            );
            
            id = Convert.ToInt32(ExecuteScalar(sql, args.ToArray()));
        }
		#endregion

		#region >>执行更新<<
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
                args.Add(new OracleParameter(de.Key.ToString(), de.Value));
                // 要更新的字段
                sbField.Append(de.Key.ToString() + "=:" + de.Key + ",");
            }

            // 去除最后的逗号
            sbField.Remove(sbField.Length - 1, 1);

            // 添加ID
            args.Add(new OracleParameter(pk, id));

            string sql = string.Format
            (
                "UPDATE {0} SET {1} WHERE {2}=:Id",
                table,
                sbField.ToString(),
				pk
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
			Array args;
			ArrayList argList = new ArrayList();

            // 查询字段和值
            string condition = GetConditionString(htCondition, out args);

            foreach (DictionaryEntry de in htField)
            {
                // 参数集合
				argList.Add(new OracleParameter(de.Key.ToString(), de.Value));

                // 要更新的字段
                sbField.Append(de.Key.ToString() + "=:" + de.Key + ",");
            }

			argList.AddRange(args);

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

			return ExecuteSql(sql, argList.ToArray());
        }
		#endregion

		#region >>获取单个字段值<<
		/// <summary>
        /// 获取单个字段值
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="field">字段名</param>
        /// <param name="id">主键值</param>
        /// <param name="pk">主键字段名称</param>
        /// <returns></returns>
        public string GetFieldValue(string table, string field, string id, string pk)
        {
            // 执行查询
            string sql = "SELECT " + field + " FROM " + table + " WHERE " + pk + "=" + id;
            DataTable dtFieldValue = GetDataTable(sql);

            // 没有查到任何结果
            if (dtFieldValue.Rows.Count == 0) { return null; }

            // 返回结果
            return dtFieldValue.Rows[0][field].ToString();
        }

		/// <summary>
		/// 获取单个字段值
		/// </summary>
		/// <param name="table"></param>
		/// <param name="field"></param>
		/// <param name="htCondition"></param>
		/// <param name="order">排序字段 | ORDER BY Field DESC</param>
		/// <returns></returns>
		public string GetFieldValue(string table, string field, Hashtable htCondition, string order)
		{
			Array args;
			string condition = GetConditionString(htCondition, out args);

			string sql = string.Format("SELECT " + field + " FROM " + table + " WHERE 1=1 {1} {2}", condition, order);
			DataTable dtFieldValue = GetDataTable(sql);

			// 没有查到任何结果
			if (dtFieldValue.Rows.Count == 0) return null;

			// 返回结果
			return dtFieldValue.Rows[0][field].ToString();
		}
		#endregion

		#region >>获取记录数量<<
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
		#endregion

		#region >>判断是否存在<<
		/// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="table">表名</param>
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
		#endregion

		#region >>获取查询条件字符串<<
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
						array.Add(de.Value as OracleParameter);
					}
					continue;
				}

				sbCondition.Append(" AND " + de.Key + "=:" + de.Key);
				array.Add(new OracleParameter(de.Key.ToString(), de.Value));
			}
			args = array.ToArray();

			// 返回查询条件字符串
			return sbCondition.ToString();
        }
		#endregion

		#region >>多表查询<<
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
            args.Add(new OracleParameter(pk, id));
            string sql = string.Format("SELECT {0} FROM {1} WHERE " + pk + "=:Id", field, table);
            return GetDataTable(sql, args.ToArray());
        }

        /// <summary>
        /// 多表查询 - 通过查询条件获取
        /// </summary>
        /// <param name="field">查询字段，如： A.Field, B.Field</param>
        /// <param name="table">表名，如： TableOne as A, TableTwo as B</param>
        /// <param name="htCondition">查询条件</param>
        /// <param name="order">排序,包括字段及排序方式(如：ordernum desc),也可以用""不排序</param>
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
        public System.Data.DataTable GetDataTable(string key, string field, string table, System.Collections.Hashtable htCondition,
            string order, int pageIndex, int pageSize, out int recordCount, out int pageCount)
        {
            Array args;
            string condition = GetConditionString(htCondition, out args);

            return GetDataTable(key, field, table, condition, order, args, pageIndex, pageSize, out recordCount, out pageCount);
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
		public DataTable GetDataTable(string key, string field, string table, string condition,
			string order, Array args, int pageIndex, int pageSize, out int recordCount, out int pageCount)
		{
			// 1 获取记录数
			string sql = "SELECT COUNT (*) FROM " + table + " WHERE 1=1" + condition;
			object obj = ExecuteScalar(sql, args);
			recordCount = Convert.ToInt32(obj);

			// 2 获取页数
			pageCount = recordCount % pageSize == 0 ? recordCount / pageSize : recordCount / pageSize + 1;

			// 3 显示当前页的范围，下面两行上下顺序不可颠倒，如果颠倒，记录为0时会出现 负值
			if (pageIndex > pageCount) { pageIndex = pageCount; }
			if (pageIndex < 1) { pageIndex = 1; }

			// 4 执行查询
			if (pageIndex == 1)
			{
				sql = string.Format("SELECT ROWNUM,{0} FROM {1} WHERE ROWNUM <= {2}{3} {4}", field, table, pageSize, condition, order);
			}
			else
			{
				int startNum = (pageIndex - 1) * pageSize;
				sql = string.Format("SELECT {0} FROM (SELECT {0},ROWNUM RN FROM {1} WHERE ROWNUM <= {2}) WHERE RN > {3}", field, table, startNum + pageSize, startNum);
			}

			// 5 返回查询结果
			return GetDataTable(sql, args);
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
		#endregion

		#region >>执行SQL语句，返回受影响的行数<<
		/// <summary>
        /// 执行SQL语句,返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteSql(string sql)
        {
			using (OracleConnection connection = new OracleConnection(_connString))
            {
                using (OracleCommand cmd = new OracleCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
						int result = cmd.ExecuteNonQuery();
						connection.Close();

						return result;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
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
			using (OracleConnection connection = new OracleConnection(_connString))
            {
                using (OracleCommand cmd = new OracleCommand(sql, connection))
                {
                    try
                    {
                        int result = 0;
                        connection.Open();
                        cmd.Parameters.AddRange(args);
                        result = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
						connection.Close();

                        return result;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
		#endregion

		#region >>执行查询语句，返回单个对象<<
		/// <summary>
        /// 执行查询语句,返回单个对象
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql)
        {
			using (OracleConnection connection = new OracleConnection(_connString))
            {
                using (OracleCommand cmd = new OracleCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
						object obj = cmd.ExecuteScalar();
						connection.Close();

						if (Object.Equals(obj, null) || Object.Equals(obj, System.DBNull.Value)) return null;
						return obj;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
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
			using (OracleConnection connection = new OracleConnection(_connString))
            {
                using (OracleCommand cmd = new OracleCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.Parameters.AddRange(args);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
						connection.Close();

                        if (Object.Equals(obj, null) || Object.Equals(obj, System.DBNull.Value)) return null;
						return obj;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }

        }
		#endregion

		#region >>执行存储过程<<
		/// <summary>
        /// 执行存储过程，返回单个对象
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="args">参数</param>
        /// <returns>单个结果</returns>
        public object ExecuteProcedure(string procName, Array args)
        {
			using (OracleConnection connection = new OracleConnection(_connString))
            {
                using (OracleCommand cmd = new OracleCommand(procName, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(args);
                        object obj = cmd.ExecuteScalar();
						cmd.Parameters.Clear();
						connection.Close();

						if (Object.Equals(obj, null) || Object.Equals(obj, System.DBNull.Value)) return null;
						return obj;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 执行存储过程，返回DataTable
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public DataTable ExecuteProcedureToDt(string procName, Array args)
        {
			using (OracleConnection connection = new OracleConnection(_connString))
            {
                using (OracleCommand cmd = new OracleCommand(procName, connection))
                {
                    DataTable dt = new DataTable();
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                        adapter.SelectCommand.Parameters.AddRange(args);
                        adapter.Fill(dt);
                        adapter.SelectCommand.Parameters.Clear();
                        return dt;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
		#endregion

		#region >>执行SQL语句 返回DataTable<<
		/// <summary>
        /// 执行SQL语句 返回DataTable
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>DataTable</returns>
        public DataTable GetDataTable(string sql)
        {
			using (OracleConnection connection = new OracleConnection(_connString))
            {
                using (OracleDataAdapter adapter = new OracleDataAdapter(sql, connection))
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
                }
            }
        }

        /// <summary>
        /// 执行SQL语句 返回DataTable
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="args">参数集合</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, Array args)
        {
			using (OracleConnection connection = new OracleConnection(_connString))
            {
                using (OracleDataAdapter adapter = new OracleDataAdapter(sql, connection))
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
                }
            }
        }
		#endregion

	}
}

