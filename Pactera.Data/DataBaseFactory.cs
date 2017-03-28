using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Data
{
    public class DataBaseFactory
    {
        private static DataBaseFactory _dataBaseFactory;

        public DataBaseFactory()
        {

        }

        #region >>获取单例<<
        /// <summary>
        /// DataBaseFactory 的单例
        /// </summary>
        public static DataBaseFactory Instance
        {
            get
            {
                if (_dataBaseFactory == null) _dataBaseFactory = new DataBaseFactory();
                return _dataBaseFactory;
            }
        }

        #endregion

        #region >>创建数据库管理器<<
        /// <summary>
        /// 创建默认SQL数据库管理者
        /// </summary>
        /// <returns>Microsoft SQL Server数据库管理者</returns>
        public IDataBaseMgr Create()
        {
            MsSQLDataBaseMgr mgr = new MsSQLDataBaseMgr();
            return mgr;
        }

        /// <summary>
        /// 创建指定类型数据库管理者
        /// </summary>
        /// <param name="choice"></param>
        /// <returns>支持的数据库管理者 或 空</returns>
        public IDataBaseMgr Create(DataBaseChoice choice)
        {
			switch (choice)
			{
				case DataBaseChoice.MSSQLSERVER:
					return new MsSQLDataBaseMgr();
				case DataBaseChoice.Oracle:
					return new OracleDataBaseMgr(choice);
				case DataBaseChoice.OaUserOracle:
					return new OracleDataBaseMgr(choice);
			}

            return null;
        }
        #endregion
    }
}
