using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS
{
    public class JsonUtils
    {
        /// <summary>
        ///  将 JSON 格式字符串转换为指定类型的对象。
        /// </summary>
        /// <typeparam name="T">所生成对象的类型。</typeparam>
        /// <param name="input">要反序列化的 JSON 字符串。</param>
        /// <returns>反序列化的对象。</returns>
        public T Deserialize<T>(string input)
        {
            T v=JsonConvert.DeserializeObject<T>(input);
            if (v is DynamicObj)
            {
                DynamicObj obj = v as DynamicObj;
                string[] keys = new string[obj.Count];
                obj.Keys.CopyTo(keys, 0);  // .ToList<string>();
                foreach (string key in keys)
                {
                    object co = obj[key];
                    if (co != null)
                    {
                        if (co is JObject)
                            obj[key] = ToDynamic((JObject)co);
                        else if (co is JArray)
                            obj[key] = ToDynamics((JArray)co);
                    }
                }
            }
            return v;
        }

        /// <summary>
        /// 将 JSON 格式字符串转换为指定类型的对象。
        /// </summary>
        /// <param name="input">要反序列化的 JSON 字符串。</param>
        /// <param name="targetType"> 所生成对象的类型。</param>
        /// <returns> 反序列化的对象。</returns>
        public object Deserialize(string input, Type targetType)
        {
            return JsonConvert.DeserializeObject(input, targetType);
        }
        
        /// <summary>
        /// 将指定的 JSON 字符串转换为对象图。
        /// </summary>
        /// <param name="input">要进行反序列化的 JSON 字符串。</param>
        /// <returns>反序列化的对象。</returns>
        public object DeserializeObject(string input)
        {   
            return JsonConvert.DeserializeObject(input);
        }


        public DynamicObj DeserializeDynamicObj(string input)
        {
            
               DynamicObj obj = JsonConvert.DeserializeObject<DynamicObj>(input);

            string[] keys =new string[obj.Count];
            obj.Keys.CopyTo(keys,0);  // .ToList<string>();
            foreach (string key in keys)
            {
                object co = obj[key];
                if (co != null)
                {
                    if (co is JObject)
                        obj[key] = ToDynamic((JObject)co);
                    else if (co is JArray)
                        obj[key] = ToDynamics((JArray)co);
                }
            }
            return obj;
        }


        public List<object> DeserializeDynamicObjItems(string itemarr)
        {
            object obj = JsonConvert.DeserializeObject(itemarr);
            if (obj is JArray)
                return ToDynamics((JArray)obj);
            else if (obj is JObject)
                return new List<object>() { ToDynamic((JObject)obj) };
            else
                return new List<object>() { obj };
        }

        #region 内部转换


        private DynamicObj ToDynamic(JObject jo)
        {
            if (jo == null) return null;

            DynamicObj obj = new DynamicObj();
            IEnumerable<JProperty> properties = jo.Properties();
            foreach (JProperty item in properties)
            {
                if (item.Value == null)
                    obj.Set(item.Name, null);
                else if (item.Value is JObject)
                {
                    obj.Set(item.Name, ToDynamic((JObject)item.Value));
                }
                else if (item.Value is JArray)
                {
                    obj.Set(item.Name, ToDynamics((JArray)item.Value));
                }
                else
                {
                    object ov = ((JValue)item.Value).Value;
                    if (ov is JObject)
                        obj.Set(item.Name, ToDynamic((JObject)ov));
                    else
                        obj.Set(item.Name, ov);
                }
            }
            return obj;
        }

        private List<object> ToDynamics(JArray arr)
        {
            List<object> objs = new List<object>();
            foreach (object v in arr)
            {
                if (v is JObject)
                    objs.Add(ToDynamic((JObject)v));
                else
                    objs.Add(v);
            }
            return objs;
        }
        #endregion








        /// <summary>
        /// 将对象转换为 JSON 字符串。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <returns>序列化的 JSON 字符串。</returns>
        public string Serialize(object obj)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            //使用默认方式，不更改元数据的key的大小写
            settings.ContractResolver = new DefaultContractResolver();
           
            if (obj == null)
                return JsonConvert.SerializeObject(obj,settings);
            else
            {
                var timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };//这里使用自定义日期格式，默认是ISO8601格式
                settings.Converters.Add(timeConverter);
                return JsonConvert.SerializeObject(obj, Formatting.Indented, timeConverter);
            }
        }
        
        /// <summary>
        /// 序列化对象并将生成的 JSON 字符串写入指定的 System.Text.StringBuilder 对象。
        /// </summary>
        /// <param name="obj">要序列化的对象。</param>
        /// <param name="output">用于写 JSON 字符串的 System.Text.StringBuilder 对象。</param>
        public void Serialize(object obj, StringBuilder output)
        {
            output.Append(Serialize(obj));
        }
    }
}
