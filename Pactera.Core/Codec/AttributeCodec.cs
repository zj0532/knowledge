using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core.Codec
{
    public class AttributeCodec
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="source">要编码的字符串</param>
        /// <returns></returns>
        public static string Encode(string source)
        {
            string temp = source.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;");
            temp = temp.Replace("'", "&apos;").Replace("\"", "&quot;");
            return temp;
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="source">要解码的字符串</param>
        /// <returns></returns>
        public static string Decode(string source)
        {
            string temp = source.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&");
            temp = temp.Replace("&apos;", "'").Replace("&quot;", "\"");
            return temp;
        }
    }
}
