using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Pactera.Core
{
	/// <summary>
	/// LitJson - JsonData 对象扩展类
	/// </summary>
	public static class JsonDataExtension
	{
		#region >>是否包含指定的Key<<
		/// <summary>
		/// 是否包含指定的Key
		/// </summary>
		/// <param name="data">JsonData</param>
		/// <param name="key">要查找的Key</param>
		/// <returns></returns>
		public static bool HasKey(this LitJson.JsonData data, string key)
		{
			var jsonData = (IDictionary)data;
			return jsonData.Contains(key);
		}
		#endregion

		#region >>是否包含指定的Key和Value<<
		/// <summary>
		/// 是否包含指定的Key和Value
		/// </summary>
		/// <param name="data"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool HasKeyAndValue(this LitJson.JsonData data, string key)
		{
			bool hasKey = HasKey(data, key);

			LitJson.JsonData jsonData = data[key];
			return hasKey && jsonData != null;
		}
		#endregion

	}
}
