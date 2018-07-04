using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RS.Core
{
    public class DList<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public DList() : base()
        {
        }
        public DList(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {

        }
        public DList(IEqualityComparer<TKey> comparer) : base(comparer)
        {
        }
        public DList(int capacity) : base(capacity)
        {

        }
        public DList(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
        {
        }
        public DList(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }

        public new TValue this[TKey Key]
        {
            get
            {
                if (base.ContainsKey(Key))
                    return base[Key];
                else
                    return default(TValue);
            }
            set
            {

                base[Key] = value;
            }
        }

        #region 以下代码是为了实现序列化而设计
        public void WriteXml(XmlWriter write)       // Serializer
        {
            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            foreach (KeyValuePair<TKey, TValue> kv in this)
            {
                write.WriteStartElement("SerializableDictionary");
                write.WriteStartElement("key");
                KeySerializer.Serialize(write, kv.Key);
                write.WriteEndElement();
                write.WriteStartElement("value");
                ValueSerializer.Serialize(write, kv.Value);
                write.WriteEndElement();
                write.WriteEndElement();
            }
        }
        public void ReadXml(XmlReader reader)       // Deserializer
        {
            reader.Read();

            XmlSerializer KeySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer ValueSerializer = new XmlSerializer(typeof(TValue));

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("SerializableDictionary");
                reader.ReadStartElement("key");
                TKey tk = (TKey)KeySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue vl = (TValue)ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadEndElement();
                this.Add(tk, vl);
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }
        public XmlSchema GetSchema()
        {
            return null;
        }

        #endregion
    }
    /// <summary>
    /// 动态Object对象
    /// </summary>
    public class DynamicObj : DList<string, object>
    {
        public DynamicObj() : base()
        { }
       
        public DynamicObj(IDictionary<string, object> dictionary)
            : base(dictionary)
        {

        }
        public DynamicObj(IEqualityComparer<string> comparer) : base(comparer)
        { }
        public DynamicObj(int capacity) : base(capacity)
        {

        }
        public DynamicObj(IDictionary<string, object> dictionary, IEqualityComparer<string> comparer)
            : base(dictionary, comparer)
        { }
        public DynamicObj(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer)
        { }


        public object Get(string key)
        {
            return this[key];
        }

        public DynamicObj Set(string key, object value)
        {
            this[key] = value;
            return this;
        }

        public T Get<T>(string key)
        {
            return this[key].Evaluate<T>();
        }


        public static DynamicObj ToDynamic(IDataReader dr)
        {
            DynamicObj obj = new DynamicObj(StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < dr.FieldCount; index++)
            {
                object v = dr.GetValue(index);
                if (v == DBNull.Value)
                {
                    v = v.GetObjectValue(dr.GetFieldType(index));
                }
                obj.Set(dr.GetName(index), v);
            }
            return obj;
        }

        /// <summary>
        /// 将普通键值列表对象转为动态集合对象
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DynamicObj ToDynamic(Dictionary<string,object> items)
        {
            return new DynamicObj(items);
        }

        /// <summary>
        /// 创建一个空
        /// </summary>
        /// <returns></returns>
        public static DynamicObj NewObj(string Key,object Value)
        {
            DynamicObj obj = new DynamicObj();
            obj.Set(Key, Value);
            return obj;
        }
    }
}
