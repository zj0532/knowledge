using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;

namespace Pactera.Data
{
    public interface IDataBaseMgr
    {
        #region ..执行删除..
        /// <summary>
        /// 执行删除
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="ID">主键值</param>
        /// <param name="pk">主键字段名称</param>
        /// <returns>删除结果</returns>
        int Delete(string table, string id, string pk = "Id");
        /// <summary>
        /// 根据查询条件执行删除
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="htCondition">查询条件</param>
        /// <returns></returns>
        int Delete(string table, Hashtable htCondition);
        #endregion

        #region ..执行插入..
        /// <summary>
        /// 执行插入
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="htFilds">字段和值</param>
        /// <returns>结果</returns>
        int Insert(string table, Hashtable htField);
        #endregion

        #region ..执行插入，返回ID..
        /// <summary>
        /// 执行插入，返回ID
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="htField">字段和值</param>
        /// <param name="id">ID</param>
        void Insert(string table, Hashtable htField, out int id);
        #endregion

        #region ..执行更新..
        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="htFilds">字段和值</param>
        /// <param name="id">主键值</param>
        /// <param name="pk">主键字段名称</param>
        /// <returns>结果</returns>
        int Update(string table, Hashtable htField, string id, string pk = "Id");

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="htField">字段和值</param>
        /// <param name="htCondition">查询条件</param>
        /// <returns></returns>
        int Update(string table, Hashtable htField, Hashtable htCondition);
        #endregion

        #region ..获取单个字段值..
        /// <summary>
        /// 获取单个字段值
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="field">字段名</param>
        /// <param name="id">主键值</param>
        /// <param name="pk">主键字段名称</param>
        /// <returns></returns>
        string GetFieldValue(string table, string field, string id, string pk = "Id");
        /// <summary>
        /// 获取单个字段的值（默认取第一行）
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="field">字段名</param>
        /// <param name="htCondition">查询条件</param>
        /// <param name="order">排序</param>
        /// <returns>单个字段的值，如果不存在，返回 null</returns>
        string GetFieldValue(string table, string field, Hashtable htCondition, string order);
        #endregion

        #region ..获取记录数量..
        /// <summary>
        /// 获取记录数量
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="htCondtion">查询条件</param>
        /// <returns></returns>
        int GetRecordCount(string table, Hashtable htCondition);
        #endregion

        #region ..判断是否存在..
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="htCondtion">查询条件</param>
        /// <returns></returns>
        bool Exists(string table, Hashtable htCondition);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="id">主键值</param>
        /// <param name="pk">主键字段名称</param>
        /// <returns></returns>
        bool Exists(string table, string id, string pk = "Id");
        #endregion

        #region ..获取查询条件字符串..
        /// <summary>
        /// 获取查询条件字符串
        /// </summary>
        /// <param name="htCondition">条件字符串</param>
        /// <param name="args">参数集合</param>
        /// <returns></returns>
        string GetConditionString(Hashtable htCondition, out Array args);
        #endregion

        #region ..多表查询..
        /// <summary>
        /// 多表查询 - 通过ID获取
        /// </summary>
        /// <param name="field">要查询的字段，如：* | Id,Field</param>
        /// <param name="table">表名</param>
        /// <param name="id">主键值</param>
        /// <param name="pk">主键字段名称</param>
        /// <returns></returns>
        DataTable GetDataTable(string field, string table, string id, string pk = "Id");

        /// <summary>
        /// 多表查询 - 通过查询条件获取
        /// </summary>
        /// <param name="field">查询字段，如： A.Field, B.Field</param>
        /// <param name="table">表名，如： TableOne as A, TableTwo as B</param>
        /// <param name="htCondition">查询条件</param>
        ///  <param name="order">排序,包括字段及排序方式(如：ordernum desc),也可以用""不排序</param>
        /// <returns></returns>
        DataTable GetDataTable(string field, string table, Hashtable htCondition, string order);

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
        DataTable GetDataTable(string key, string field, string table, Hashtable htCondition, string order, int pageIndex, int pageSize, out int recordCount, out int pageCount);

        /// <summary>
        /// 多表查询 - 通过存储过程获取
        /// </summary>
        /// <param name="procName">存储过程</param>
        /// <param name="htCondition">查询条件</param>
        /// <returns></returns>
        DataTable GetDataTableByProcName(string procName, Hashtable htCondition);

        #endregion

        #region ..执行SQL语句,返回受影响的行数..
        /// <summary>
        /// 执行SQL语句,返回受影响的行数
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <returns></returns>
        int ExecuteSql(string sql);

        /// <summary>
        /// 执行SQL语句,返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        int ExecuteSql(string sql, Array args);
        #endregion

        #region ..执行查询语句,返回单个对象..
        /// <summary>
        /// 执行查询语句,返回单个对象
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>单个查询结果(object)</returns>
        object ExecuteScalar(string sql);

        /// <summary>
        /// 执行查询语句,返回单个对象
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="arg">参数</param>
        /// <returns></returns>
        object ExecuteScalar(string sql, Array args);
        #endregion

        #region ..执行存储过程..
        /// <summary>
        /// 执行存储过程,返回单个对象
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="args">参数</param>
        /// <returns>单个结果</returns>
        object ExecuteProcedure(string procName, Array args);

        /// <summary>
        /// 执行存储过程,返回DataTable
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        DataTable ExecuteProcedureToDt(string procName, Array args);
        #endregion

        #region ..执行SQL语句 返回DataTable..
        /// <summary>
        /// 执行SQL语句 返回DataTable
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>DataTable</returns>
        DataTable GetDataTable(string sql);

        /// <summary>
        /// 执行SQL语句 返回DataTable
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="args">参数集合</param>
        /// <returns></returns>
        DataTable GetDataTable(string sql, Array args);
        #endregion
    }
}
