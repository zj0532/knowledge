using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core
{
    class Guard
    {
        /// <summary>
        /// 指示指定的字符串是 null 还是 System.String.Empty 字符串。
        /// 如果是，则抛出异常
        /// </summary>
        /// <param name="value">指定的字符串</param>
        /// <param name="message">异常信息</param>
        public static void IsNullOrEmpty(string value, string message)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(value, message);
        }

        /// <summary>
        /// 指示指定的对象是 null
        /// 如果是，则抛出异常
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        public static void IsNull(object value, string message)
        {
            if (value == null)
                throw new ArgumentNullException(message);
        }
    }
}
