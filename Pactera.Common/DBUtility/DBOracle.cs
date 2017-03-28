using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Data.OracleClient;
using System.Data;

namespace Pactera.Common.DBUtility
{
    internal class DBOracle : DBabstract
    {
        public override IDbConnection getConn(string constr)
        {
            //return new OracleConnection(constr);
            return null;
        }

        public override IDbCommand getCmd()
        {
            //return new OracleCommand();
            return null;
        }

        public override IDbDataAdapter getDataAdapte(IDbCommand icmd)
        {
            //return new OracleDataAdapter((OracleCommand)icmd);
            return null;
        }

        #region Parameter创建
        public override IDbDataParameter getDBParameter<T>(string paraName, T paraValue)
        {
            IDbDataParameter para = new OracleParameter("@" + paraName, paraValue);
            return para;
        }

        public override IDbDataParameter getDBParameter<T>(string paraName, T paraValue, myDbType dbtp)
        {
            return getDBParameter(paraName, paraValue, dbtp, ParameterDirection.Input);
        }

        public override IDbDataParameter getDBParameter<T>(string paraName, T paraValue, myDbType dbtp, ParameterDirection pd)
        {
            OracleParameter para = new OracleParameter(paraName, getOraType(dbtp));
            para.Value = paraValue;
            para.Direction = pd;
            return para;
        }

        public override IDbDataParameter getDBParameter<T>(string paraName, T paraValue, myDbType dbtp, int intLen, ParameterDirection pd)
        {
            OracleParameter para = new OracleParameter(paraName, getOraType(dbtp), intLen);
            para.Value = paraValue;
            para.Direction = pd;
            return para;
        }
        #endregion

        #region 私有方法
        private OracleType getOraType(myDbType dbtp)
        {
            OracleType ot = OracleType.VarChar;
            switch (dbtp)
            {
                case myDbType.Int:
                    ot = OracleType.Int16;
                    break;
                case myDbType.Varchar:
                    ot = OracleType.VarChar;
                    break;
                case myDbType.NVarChar:
                    ot = OracleType.NVarChar;
                    break;
                case myDbType.DateTime:
                    ot = OracleType.DateTime;
                    break;
                case myDbType.Float:
                    ot = OracleType.Float;
                    break;
                case myDbType.Numeric:
                    ot = OracleType.Number;
                    break;
            }

            return ot;
        }
        #endregion
    }
}
