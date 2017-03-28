using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Data
{
    public enum DataBaseChoice
    {
        /// <summary>
        /// 选择使用 Microsoft SQL Server 数据库
        /// </summary>
        MSSQLSERVER,

        /// <summary>
        /// 选择使用 MySQL 数据库
        /// </summary>
        MYSQLSERVER,

        /// <summary>
        /// 选择使用 Oracle 数据库
        /// </summary>
        Oracle,

		/// <summary>
		/// 用于访问OA用户数据库
		/// </summary>
		OaUserOracle
    }
}
