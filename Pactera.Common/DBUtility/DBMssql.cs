using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Data;
using System.Data.SqlClient;
namespace Pactera.Common.DBUtility
{
    internal class DBMssql : DBabstract
    {
        public override IDbConnection getConn(string constr)
        {
            return new SqlConnection(constr);
        }

        public override IDbCommand getCmd()
        {
            return new SqlCommand();
        }

        public override IDbDataAdapter getDataAdapte(IDbCommand icmd)
        {
            return new SqlDataAdapter((SqlCommand)icmd);
        }

        #region Parameter创建
        public override IDbDataParameter getDBParameter<T>(string paraName, T paraValue)
        {
            if (paraName.IndexOf('@') != 0) paraName = "@" + paraName;
            IDbDataParameter para = new SqlParameter(paraName, paraValue);
            return para;
        }

        public override IDbDataParameter getDBParameter<T>(string paraName, T paraValue, myDbType dbtp)
        {
            return getDBParameter(paraName, paraValue, dbtp, ParameterDirection.Input);
        }

        public override IDbDataParameter getDBParameter<T>(string paraName, T paraValue, myDbType dbtp, ParameterDirection pd)
        {
            if (paraName.IndexOf('@') != 0) paraName = "@" + paraName;
            IDbDataParameter para = new SqlParameter(paraName, getMsSqlType(dbtp));
            para.Value = paraValue;
            para.Direction = pd;
            return para;
        }

        public override IDbDataParameter getDBParameter<T>(string paraName, T paraValue, myDbType dbtp, int intLen, ParameterDirection pd)
        {
            if (paraName.IndexOf('@') != 0) paraName = "@" + paraName;
            IDbDataParameter para = new SqlParameter(paraName, getMsSqlType(dbtp), intLen);
            para.Value = paraValue;
            para.Direction = pd;
            return para;
        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 数据参数类型
        /// </summary>
        /// <param name="dbtp"></param>
        /// <returns></returns>
        private SqlDbType getMsSqlType(myDbType dbtp)
        {
            SqlDbType ot = SqlDbType.VarChar;
            switch (dbtp)
            {
                case myDbType.Int:
                    ot = SqlDbType.Int;
                    break;
                case myDbType.Varchar:
                    ot = SqlDbType.VarChar;
                    break;
                case myDbType.NVarChar:
                    ot = SqlDbType.NVarChar;
                    break;
                case myDbType.DateTime:
                    ot = SqlDbType.DateTime;
                    break;
                case myDbType.Float:
                    ot = SqlDbType.Float;
                    break;
                case myDbType.Numeric:
                    ot = SqlDbType.Decimal;
                    break;
            }

            return ot;
        }
        #endregion
    }
}
