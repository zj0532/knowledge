using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Data;
using Pactera.Common;
using System.Data.OracleClient;
using System.Reflection;
namespace Pactera.Common.DBUtility
{
    public class DBHelper
    {
        private DBabstract myDB;
        private IDbConnection conn;
        private string strConnStr;
        private string strProviderName;
        private string strCharacter;  //获取使用存储过程的定义
        private int intDatabaseType = 0; //数据库类型 0 mssql，1 oracle

        string exErr = null;  //异常信息

        #region 构造函数--获取数据库信息，开关数据库
        /// <summary>
        /// 获取数据库信息，连接数据库
        /// </summary>
        /// <param name="key"></param>
        public DBHelper(string key)
        {
            strProviderName = AppConfig.getConnProviderName(key);
            strConnStr = AppConfig.getConnStr(key);
            getConn(strProviderName, strConnStr);
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <param name="ProviderName"></param>
        /// <param name="ConnStr"></param>
        public DBHelper(string ProviderName, string ConnStr)
        {
            getConn(ProviderName, strConnStr);
        }

        /// <summary>
        /// 析构函数,关闭数据库连接
        /// </summary>
        ~DBHelper()
        {
            this.close();
        }
        #endregion

        /// <summary>
        /// 使用存储过程的定义
        /// </summary>
        public string Character
        {
            get { return this.strCharacter; }
        }

        /// <summary>
        /// 工厂模式返回数据库连接类
        /// </summary>
        /// <param name="ProviderName">连接类</param>
        /// <param name="ConnStr">连接字符串</param>
        /// <returns></returns>
        public void getConn(string ProviderName, string ConnStr)
        {
            this.strProviderName = ProviderName.ToLower();
            switch (ProviderName)
            {
                case "system.data.oracleclient":  //oracle数据库
                    myDB = new DBOracle();
                    strCharacter = ":";
                    intDatabaseType = 1;
                    break;
                case "system.data.sqlclient":    //Ms-sql数据库
                default:
                    myDB = new DBMssql();
                    strCharacter = "@";
                    intDatabaseType = 0;
                    break;
            }
            strConnStr = ConnStr;
        }

        #region 数据库开关
        /// <summary>
        /// 开
        /// </summary>
        /// <returns></returns>
        public bool open()
        {
            bool bl = false;
            conn = myDB.getConn(strConnStr);
            try
            {
                conn.Open();
                bl = (conn.State == ConnectionState.Open) ? true : false;
            }
            catch { }
            return bl;
        }

        /// <summary>
        /// 关
        /// </summary>
        public void close()
        {
            try
            {
                if (conn.State == ConnectionState.Open) conn.Close();
                conn.Dispose();
            }
            catch { }
        }

        /// <summary>
        /// 判断是否是连接着
        /// </summary>
        /// <returns></returns>
        public bool isConnect()
        {
            if (conn == null) return false;
            return (conn.State == ConnectionState.Open) ? true : false;
        }
        #endregion

        #region 执行简单SQL语句
        /// <summary>
        /// 执行sql，返回影响行数()
        /// </summary>
        /// <param name="strSql">sql</param>
        /// <param name="paramenters">参数集</param>
        /// <returns></returns>
        public int execNonQuery(string strSql, params IDataParameter[] paramenters)
        {
            return execNonQuery(strSql, false, paramenters);
        }

        /// <summary>
        /// 执行存储过程，返回影响行数
        /// </summary>
        /// <param name="strSql">sql</param>
        /// <param name="isPro">是否是存储过程--true：是，false：否</param>
        /// <param name="paramenters">参数集</param>
        /// <returns></returns>
        public int execNonQuery(string strSql, bool isPro, params IDataParameter[] paramenters)
        {
            int rows = 0;
            //替换数据库连接字符串
            if (intDatabaseType == 1) strSql = strSql.Replace("@", this.strCharacter);
            if (isConnect())
            {
                IDbCommand cmd = null;
                try
                {
                    cmd = myDB.getCmd();
                    cmd.Connection = conn;
                    cmd.CommandText = strSql;
                    cmd.CommandType = CommandType.Text;
                    if (paramenters != null)
                    {
                        foreach (IDbDataParameter parm in paramenters)
                        {
                            cmd.Parameters.Add(parm);
                        }
                    }
                    if (isPro)
                        cmd.CommandType = CommandType.StoredProcedure;
                    else
                        cmd.CommandType = CommandType.Text;

                    rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                catch (Exception exp)
                {
                    exErr = exp.ToString();
                }
                finally
                {
                    cmd.Dispose();
                    cmd = null;
                }


            }

            return rows;
        }
        #endregion

        #region 执行返回DataSet结果集的SQL语句
        /// <summary>
        /// 执行sql，返回DataSet
        /// </summary>
        /// <param name="strSql">sql</param>
        /// <param name="paramenters">参数集</param>
        /// <returns></returns>
        public DataSet getDataSet(string strSql, params IDataParameter[] paramenters)
        {
            return getDataSet(strSql, false, paramenters);
        }

        /// <summary>
        /// 执行存储过程，返回DataSet
        /// </summary>
        /// <param name="strSql">sql</param>
        /// <param name="isPro">是否是存储过程--true：是，false：否</param>
        /// <param name="paramenters">参数集</param>
        /// <returns></returns>
        public DataSet getDataSet(string strSql, bool isPro, params IDataParameter[] paramenters)
        {
            //替换数据库连接字符串
            if (intDatabaseType == 1) strSql = strSql.Replace("@", this.strCharacter);

            DataSet ds = new DataSet();
            if (isConnect())
            {
                IDbCommand cmd = null;
                try
                {
                    cmd = myDB.getCmd();
                    cmd.Connection = conn;
                    cmd.CommandText = strSql;
                    cmd.CommandType = CommandType.Text;
                    if (paramenters != null)
                    {
                        foreach (IDbDataParameter parm in paramenters)
                        {
                            cmd.Parameters.Add(parm);
                        }
                    }
                    if (isPro)
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        //针对Oracle特殊处理下
                        if (intDatabaseType == 1)
                        {
                            OracleParameter op = new OracleParameter("my_cursor", OracleType.Cursor);
                            op.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(op);
                        }

                    }
                    else
                        cmd.CommandType = CommandType.Text;

                    IDataAdapter iada = myDB.getDataAdapte(cmd);

                    iada.Fill(ds);

                    cmd.Parameters.Clear();

                }
                catch (Exception exp)
                {
                    exErr = exp.ToString();
                }
                finally
                {
                    cmd.Dispose();
                    cmd = null;
                }

            }
            return ds;
        }
        #endregion

        #region 执行返回dataTable结果集的SQL语句
        /// <summary>
        /// 执行sql，返回DataTable
        /// </summary>
        /// <param name="strSql">sql</param>
        /// <param name="paramenters">参数集</param>
        /// <returns></returns>
        public DataTable getDataTable(string strSql, params IDataParameter[] paramenters)
        {
            return getDataTable(strSql, false, paramenters);
        }

        /// <summary>
        /// 执行存储过程，返回DataTable
        /// </summary>
        /// <param name="strSql">sql</param>
        /// <param name="isPro">是否是存储过程--true：是，false：否</param>
        /// <param name="paramenters">参数集</param>
        /// <returns></returns>
        public DataTable getDataTable(string strSql, bool isPro, params IDataParameter[] paramenters)
        {
            DataSet ds;
            DataTable dt = new DataTable();
            ds = getDataSet(strSql, isPro, paramenters);
            if (ds.Tables.Count > 0)
                dt = ds.Tables[0].Copy();
            return dt;
        }
        #endregion

        #region Parameter创建
        public IDbDataParameter getDBParameter<T>(string paraName, T paraValue)
        {
            return myDB.getDBParameter(paraName, paraValue);
        }

        public IDbDataParameter getDBParameter<T>(string paraName, T paraValue, myDbType dbtp)
        {
            return myDB.getDBParameter(paraName, paraValue, dbtp);
        }
        public IDbDataParameter getDBParameter<T>(string paraName, T paraValue, myDbType dbtp, ParameterDirection pd)
        {
            return myDB.getDBParameter(paraName, paraValue, dbtp, pd);
        }

        public IDbDataParameter getDBParameter<T>(string paraName, T paraValue, myDbType dbtp, int intLen, ParameterDirection pd)
        {
            return myDB.getDBParameter(paraName, paraValue, dbtp, intLen, pd);
        }

        #endregion

        /// <summary>
        /// 执行SQL返回总行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int getTotal(string sql)
        {
            DataTable dt = getDataTable(sql);
            if (dt.Rows.Count == 0) return 0;
            return int.Parse(dt.Rows[0][0].ToString());
        }

        /// <summary>
        /// 简单增加
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tabName"></param>
        /// <returns></returns>
        public bool Insert(object obj, string tabName)
        {
            string[] ModelInfo = Common<object>.GetModelInfo(obj);
            StringBuilder cols = new StringBuilder("");
            StringBuilder vals = new StringBuilder("");
            Type t = obj.GetType();
            PropertyInfo[] pis = t.GetProperties();

            string strSql = "insert into " + tabName + "({0}) values({1})";

            for (int i = 0; i < pis.Length; i++)
            {
                if (pis[i].Name.ToString().ToLower() != ModelInfo[1].ToLower())
                {
                    if (pis[i].GetValue(obj, null) == null) continue;
                    if (string.IsNullOrEmpty(pis[i].GetValue(obj, null).ToString()) || pis[i].GetValue(obj, null).ToString() == "0001/1/1 0:00:00") continue;
                    cols.Append(pis[i].Name.ToString());
                    if (pis[i].PropertyType.Name.ToString().ToLower() == "datetime")
                        if (Character == ":")
                            vals.Append("to_date('" + pis[i].GetValue(obj, null).ToString().Replace("'", "’") + "','yyyy-mm-dd hh24:mi:ss')");
                        else
                            vals.Append("'" + pis[i].GetValue(obj, null).ToString().Replace("'", "’") + "'");
                    else if (pis[i].PropertyType.Name.ToString().ToLower() == "string")
                        vals.Append("'" + pis[i].GetValue(obj, null).ToString().Replace("'", "’") + "'");
                    else
                        vals.Append(pis[i].GetValue(obj, null).ToString().Replace("'", "’"));
                    cols.Append(",");
                    vals.Append(",");
                }
                else
                {
                    if (Character == ":")
                    {
                        cols.Append(pis[i].Name.ToString() + ",");
                        vals.Append("SEQ_" + ModelInfo[0] + ".nextval,");
                    }
                }
            }
            strSql = string.Format(strSql, cols.ToString().TrimEnd(','), vals.ToString().TrimEnd(','));

            return (execNonQuery(strSql) > 0);
        }
        
        /// <summary>
        /// 简单修改
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tabName"></param>
        /// <param name="pKey"></param>
        /// <returns></returns>
        public bool Update(object obj, string tabName, string pKey)
        {
            StringBuilder Values = new StringBuilder("");
            string conditions = pKey + "=";
            Type t = obj.GetType();
            string sql = "update " + tabName + " set ";
            PropertyInfo[] pis = t.GetProperties();
            for (int i = 0; i < pis.Length; i++)
            {
                if (pis[i].Name.ToString().ToLower() != pKey.ToLower())
                {
                    if (pis[i].GetValue(obj, null) == null) continue;
                    //if (string.IsNullOrEmpty(pis[i].GetValue(obj, null).ToString()) || pis[i].GetValue(obj, null).ToString() == "0001/1/1 0:00:00") continue;
                    Values.Append(pis[i].Name.ToString());
                    Values.Append("=");

                    if (pis[i].PropertyType.Name.ToString().ToLower() == "datetime")
                    {
                        if (Character == ":")
                            Values.Append("to_date('" + pis[i].GetValue(obj, null).ToString().Replace("'", "’") + "','yyyy-mm-dd hh24:mi:ss')");
                        else
                            Values.Append("'" + pis[i].GetValue(obj, null).ToString().Replace("'", "’") + "'");
                    }
                    else if (pis[i].PropertyType.Name.ToString().ToLower() == "string")
                        Values.Append("'" + pis[i].GetValue(obj, null).ToString().Replace("'", "’") + "'");
                    else
                        Values.Append(pis[i].GetValue(obj, null).ToString().Replace("'", "’"));

                    Values.Append(",");
                }
                else
                    if (pis[i].PropertyType.Name.ToString().ToLower() == "string")
                        conditions += "'" + pis[i].GetValue(obj, null).ToString() + "'";
                    else
                        conditions += pis[i].GetValue(obj, null).ToString();
            }
            sql = sql + Values.ToString().TrimEnd(',') + " where " + conditions;

            return (execNonQuery(sql) > 0);
        }
        
        /// <summary>
        /// 简单删除
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public bool Delete(string tabName, string conditions)
        {
            string strSql = "delete from " + tabName + (conditions.Trim().Length > 0 ? " where " + conditions : "");
            return (execNonQuery(strSql) > 0);
        }


        /// <summary>
        /// 返回不带参数的结果集的第一行第一列的值
        /// </summary>
        /// <param name="strconn"></param>
        /// <param name="strcomm"></param>
        /// <returns></returns>
        public object ExecuteScalar(string strcomm)
        {
            object str = null;
            if (isConnect())
            {
                IDbCommand cmd = null;

                cmd = myDB.getCmd();
                cmd.Connection = conn;
                cmd.CommandText = strcomm;
                cmd.CommandType = CommandType.Text;
                str = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                cmd.Dispose();
                cmd = null;
            }
            return str;
        }

        /// <summary>
        /// 返回带参数的结果集的第一行第一列的值
        /// </summary>
        /// <param name="strconn"></param>
        /// <param name="strcomm"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public object ExecuteScalar(string strcomm, params IDataParameter[] paramenters)
        {
            //替换数据库连接字符串
            if (intDatabaseType == 1) strcomm = strcomm.Replace("@", this.strCharacter);

            object str = null;
            if (isConnect())
            {
                IDbCommand cmd = null;

                cmd = myDB.getCmd();
                cmd.Connection = conn;
                cmd.CommandText = strcomm;
                cmd.CommandType = CommandType.Text;
                if (paramenters != null)
                {
                    foreach (IDbDataParameter parm in paramenters)
                    {
                        cmd.Parameters.Add(parm);
                    }
                }
                str = cmd.ExecuteScalar();
                cmd.Parameters.Clear();

                cmd.Dispose();
                cmd = null;
            }
            return str;
        }

        /// <summary>
        /// 获取分页数据，并返回总行数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="selfieldNames">查询的字段（多个字段用逗号隔开）</param>
        /// <param name="strWhere">条件(注意：不带where )</param>
        /// <param name="strOrder">排序自段+排序</param>
        /// <param name="pagesize">每页显示多少数据</param>
        /// <param name="curpage">当前页码</param>
        /// <param name="flag">是否重新查询页数--0：否，1：是</param>
        /// <param name="pageCount">数据总行数</param>
        /// <returns></returns>
        public DataTable getPageDataTable(string tableName, string selfieldNames, string strWhere, string strOrder, int pagesize, int curpage, int flag, out int pageCount)
        {
            string strSql = "sis_sp_Pagination";
            IDbDataParameter[] paras = new IDbDataParameter[8];
            paras[0] = getDBParameter("p_tablename", tableName, myDbType.Varchar);
            paras[1] = getDBParameter("p_tablecolumn", selfieldNames, myDbType.Varchar);
            paras[2] = getDBParameter("p_conditions", strWhere, myDbType.Varchar);
            paras[3] = getDBParameter("p_order", strOrder, myDbType.Varchar);
            paras[4] = getDBParameter("p_curpage", curpage, myDbType.Int);
            paras[5] = getDBParameter("p_pagesize", pagesize, myDbType.Int);
            paras[6] = getDBParameter("p_flag", flag, myDbType.Int);
            paras[7] = getDBParameter("p_total", 0, myDbType.Int, ParameterDirection.Output);

            DataTable dt = getDataTable(strSql, true, paras);
            pageCount = int.Parse(paras[7].Value.ToString());
            return dt;
        }
    }
}
