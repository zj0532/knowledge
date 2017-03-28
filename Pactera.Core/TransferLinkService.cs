using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pactera.Data;
using Pactera.Common.Serialization;
using System.Data;

namespace Pactera.Core
{
	public class TransferLinkService
    {
		public static string TableName = "BPM_Sys_TransferLink";

		#region >>新增回调跳转连接<<
		/// <summary>
		/// 新增回调跳转连接
		/// </summary>
		/// <param name="userId">要使用哪个用户登录</param>
		/// <param name="targetUrl">跳转的Url</param>
		/// <returns></returns>
		public static string Insert(int userId,string targetUrl)
        {
			string guid = System.Guid.NewGuid().ToString();

            Hashtable htField = new Hashtable();
			htField.Add("Token", guid);
            htField.Add("UserId", userId);
            htField.Add("TargetUrl", targetUrl);
			htField.Add("CreateDate", DateTime.Now);

            var dbm = DataBaseFactory.Instance.Create();
            dbm.Insert(TableName, htField);

			return guid;
        }
		#endregion

		#region >>根据Token获取跳转连接<<
		/// <summary>
		/// 根据Token获取跳转连接
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public static DataTable GetTransferByToken(string token)
		{
			var dbm = DataBaseFactory.Instance.Create();

			Hashtable htCondition = new Hashtable();
			htCondition.Add("Token", token);

			return dbm.GetDataTable("*", TableName, htCondition, "Id");
		}
		#endregion

		#region >>根据Token删除跳转入口<<
		/// <summary>
		/// 根据Token删除跳转入口
		/// </summary>
		/// <param name="token"></param>
		public static void DeleteTransferByToken(string token)
		{
			var dbm = DataBaseFactory.Instance.Create();

			Hashtable htCondition = new Hashtable();
			htCondition.Add("Token", token);

			dbm.Delete(TableName, htCondition);
		}
		#endregion
	}
}
