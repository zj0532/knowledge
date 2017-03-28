using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections;

namespace Pactera.Common.Serialization
{
    public class JsonSerializer
    {
        /// <summary>
        /// json转换为object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJsonString<T>(string json)
        {
            T obj;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                obj = (T)serializer.ReadObject(ms);
                ms.Close();
            }
            return obj;
        }

        /// <summary>
        /// object转化为json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonString<T>(T obj)
        {
            string jsonString;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                jsonString = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
            }
            return jsonString;
        }

        public static string HashtableToJson(Hashtable htJson)
        {
            if (htJson.Count < 1) return "";

            StringBuilder sbJson = new System.Text.StringBuilder();
            foreach (DictionaryEntry de in htJson)
            {
                string key = de.Key.ToString();
                string value = de.Value.ToString();

                sbJson.AppendFormat("\"{0}\":\"{1}\",", key, value);
            }
            string json = sbJson.ToString().TrimEnd(',');

            return "{" + json + "}";
        }
    }
}
