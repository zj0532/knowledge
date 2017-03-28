using System;
using System.Linq;
using System.Xml.Linq;

namespace Pactera.Core
{
	static class XmlExtensions
	{
		/// <summary>
		/// 通过属性名获取XElement的属性值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="e">元素对象</param>
		/// <param name="name">属性名，属性名不区分大小写</param>
		/// <returns></returns>
		/// <remarks>属性名不区分大小写</remarks>
		public static T GetAttributeValue<T>(this XElement e, string name)
		{
			if (e.Attribute(name) == null) return default(T);

			string value = e.Attribute(name).Value;
			if (typeof(T) == typeof(string)) return (T)Convert.ChangeType(value, typeof(T));
			if (typeof(T) == typeof(int)) return (T)Convert.ChangeType(int.Parse(value), typeof(T));
			if (typeof(T) == typeof(bool)) return (T)Convert.ChangeType(bool.Parse(value), typeof(T));
			if (typeof(T).IsEnum) return (T)Convert.ChangeType(Enum.Parse(typeof(T), value), typeof(T));

			return default(T);
		}

		/// <summary>
		/// 查找某个元素下的子元素
		/// </summary>
		/// <param name="e"></param>
		/// <param name="name">子元素节点名称，不区分大小写</param>
		/// <returns></returns>
		public static XElement GetElement(this XElement e, string name)
		{
			var element = e.Element(name);
			if (element == null)
				element = e.Elements().FirstOrDefault(p => string.Equals(p.Name.LocalName, name, StringComparison.InvariantCultureIgnoreCase));
			return element;
		}

		/// <summary>
		/// 查找某个元素下的所有子元素
		/// </summary>
		/// <param name="e"></param>
		/// <param name="name">子元素节点名称，不区分大小写</param>
		/// <returns></returns>
		public static XElement[] GetElements(this XElement e, string name)
		{
			var element = e.Elements(name).ToArray();
			if (element.Length == 0)
				element = e.Elements()
					.Where(p => string.Equals(p.Name.LocalName, name, StringComparison.InvariantCultureIgnoreCase))
					.ToArray();
			return element;
		}
	}
}
