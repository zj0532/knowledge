using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core
{
	/// <summary>
	/// 请求扩展类
	/// </summary>
	public static class HttpRequestExtensions
	{
		/// <summary>
		/// 获取当前请求中指定的 JsonData 对象
		/// </summary>
		/// <param name="request"></param>
		/// <param name="key">JsonData 对象所在的集合的名称</param>
		/// <returns></returns>
		public static LitJson.JsonData GetJsonData(this System.Web.HttpRequest request, string key)
		{
			string json = request[key];
			LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(json);
			return jsonData;
		}

	}
}
