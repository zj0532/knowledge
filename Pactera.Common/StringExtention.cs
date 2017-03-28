using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Pactera.Common
{
    /// <summary>
    /// 字符串对象扩展方法
    /// </summary>
    public static class StringExtention
    {
        /// <summary>
        /// 返回字符串的真实长度, UniCode字符包含2位.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetRealLength(this string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }

        /// <summary>
        /// 从左边剪切字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Left(this string str, int length)
        {
            length = Math.Max(length, 0);
            return (str.Length > length) ? str.Substring(0, length) : str;
        }

        /// <summary>
        /// 从右边剪切字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(this string str, int length)
        {
            length = Math.Max(length, 0);
            return (str.Length > length) ? str.Substring(str.Length - length, length) : str;
        }

		/// <summary>
		/// 检测字符串是否是数字格式
		/// </summary>
		/// <param name="strNumber">源字符串</param>
		/// <returns>如果是一个数字，则返回true，否则返回false</returns>
		public static bool IsNumber(this string strNumber)
		{
			double n = 0;
			return double.TryParse(strNumber, out n);
		}

		/// <summary>
		/// 转换字符串为一个数字变量。如果转换失败，则返回缺省值。
		/// </summary>
		/// <param name="str">源字符串</param>
		/// <param name="defValue">缺省值</param>
		/// <returns>转换结果</returns>
		public static int ToInt(this string str, int defValue)
		{
			int result = 0;

			if (!int.TryParse(str, out result))
			{
				result = defValue;
			}

			return result;
		}

		/// <summary>
		/// 转换字符串为一个数字变量。如果转换失败，则返回缺省值。
		/// </summary>
		/// <param name="str">源字符串</param>
		/// <param name="defValue">缺省值</param>
		/// <returns>转换结果</returns>
		public static double ToDouble(this string str, double defValue)
		{
			double result = 0;

			if (double.TryParse(str, out result) == false)
			{
				result = defValue;
			}

			return result;
		}

		/// <summary>
		/// 转换字符串为一个数字变量，取值范围在最小与最大之间。如果转换失败，则取最小值
		/// </summary>
		/// <param name="str"></param>
		/// <param name="minValue"></param>
		/// <param name="maxValue"></param>
		/// <returns></returns>
		public static int ToInt(this string str, int minValue, int maxValue)
		{
			int n = str.ToInt(minValue);
			if (n < minValue) n = minValue;
			else if (n > maxValue) n = maxValue;
			return n;
		}

		/// <summary>
		/// 转换字符串为一个整数变量。如果转换失败，则返回缺省值。
		/// </summary>
		/// <param name="str">源字符串</param>
		/// <param name="defValue">缺省值</param>
		/// <returns>转换结果</returns>
		public static long ToLong(this string str, long defValue)
		{
			long result = 0;

			if (long.TryParse(str, out result) == false)
			{
			}

			return result;
		}

        /// <summary>
        /// 转换字符串为一个整数变量。如果转换失败，则返回缺省值。
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换结果</returns>
        public static short ToShort(this string str, short defValue)
        {
            short result = 0;

            if (short.TryParse(str, out result) == false)
            {
                result = defValue;
            }

            return result;
        }

		/// <summary>
		/// 转换字符串为布尔变量。如果转换失败，则返回缺省值。
		/// </summary>
		/// <param name="str">源字符串</param>
		/// <param name="defValue">缺省值</param>
		/// <returns>转换结果</returns>
		public static bool ToBool(this string str, bool defValue)
		{
			bool boolValue = defValue;

			str = str.Trim().ToLower();
			if (str == "true" || str == "t" || str == "yes" || str == "y" || str == "1") boolValue = true;
			else if (str == "false" || str == "f" || str == "no" || str == "n" || str == "0") boolValue = false;

			return boolValue;
		}

        /// <summary>
        /// 检查字符串是否是邮件地址的格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsValidEmailAddress(this string s)
        {
            Regex regex = new Regex(@"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
            return regex.IsMatch(s);
        }

        /// <summary>
        /// 验证是否是日期
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDate(string str)
        {
            bool Result = true;
            if (str.Length < 10)
            {
                Result = false;
            }
            else
            {
                try
                {
                    System.Convert.ToDateTime(str);
                }
                catch
                {
                    Result = false;
                }
            }
            return Result;
        }

        /// <summary>
        /// 验证是否符合用户名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckUser(string str, int minLengh, int maxLength)
        {
            bool flag = false;
            if (str.Count() >= minLengh && str.Count() <= maxLength)
            {
                Regex regex = new Regex(@"^[A-Za-z0-9-_][A-Za-z0-9-_]{"+minLengh+","+maxLength+"}");

                flag = regex.IsMatch(str);
            }
            return flag;           
        }


        /// <summary>
        /// 验证是否是手机号码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsPhone(string str)
        {
            Regex regex = new Regex(@"\d{11}");
            return regex.IsMatch(str);
        }

        /// <summary>
        /// 验证是否是电话号码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsCall(string str)
        {
            Regex regex = new Regex(@"(\(\d{3,4}\)|\d{3,4}-)?\d{7,8}");
            return regex.IsMatch(str);
        }
    }
}
