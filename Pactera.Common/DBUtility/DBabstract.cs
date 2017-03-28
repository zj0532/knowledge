/************************************************************************************				                                            	
 *      描述:数据库连接抽象类  																											
 ***********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Data;
namespace Pactera.Common.DBUtility
{
    internal abstract class DBabstract
    {
        public DBabstract() { }

        public abstract IDbConnection getConn(string constr);

        public abstract IDbCommand getCmd();

        public abstract IDbDataAdapter getDataAdapte(IDbCommand icmd);

        public abstract IDbDataParameter getDBParameter<T>(string paraName, T paraValue);

        public abstract IDbDataParameter getDBParameter<T>(string paraName, T paraValue, myDbType dbtp);

        //public abstract IDbDataParameter getDBParameter(string paraName, string paraValue, myDbType dbtp, ParameterDirection pd);

        public abstract IDbDataParameter getDBParameter<T>(string paraName, T paraValue, myDbType dbtp, ParameterDirection pd);

        public abstract IDbDataParameter getDBParameter<T>(string paraName, T paraValue, myDbType dbtp, int intLen, ParameterDirection pd);
    }

    public enum myDbType
    {
        Int,
        Varchar,
        NVarChar,
        DateTime,
        Numeric,
        Float
    }
}
