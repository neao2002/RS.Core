using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RS.Data
{
    /// <summary>
    /// 父子明细表映射对象类，主要用于存储父子明细表信息
    /// 主要针对非ORM对象来获取
    /// </summary>
    [Serializable]
    public sealed class MasterDetailMapInfo
    {
        public MasterDetailMapInfo(string TableName, object MasterObject, params string[] keyPropertyNames)
        {
            MasterTableName = TableName;
            MasterEntity = MasterObject;
            DetailInfos = new List<DetailMapInfo>();
            Property2FieldMap = new Dictionary<string, string>();
            KeyPropertyNames = keyPropertyNames;
        }
        /// <summary>
        /// 子表表明
        /// </summary>
        public string MasterTableName { get; set; }

        /// <summary>
        /// 主表对象实例
        /// </summary>
        public object MasterEntity { get; set; }

        /// <summary>
        /// 个性属性与对象名映射
        /// </summary>
        public Dictionary<string, string> Property2FieldMap { get; set; }

        public string[] KeyPropertyNames { get; set; }

        /// <summary>
        /// 明细表信息
        /// </summary>
        public List<DetailMapInfo> DetailInfos { get; set; }

        public DetailMapInfo AppendDetailMap(string ChildTableName, IList ChildObjs, List<DetailRelation> relations,IFilter filter, params string[] KeyPropertys)
        {
            DetailMapInfo dmi= new DetailMapInfo()
            {
                TableName = ChildTableName,
                DetailRelations = relations,
                Details = ChildObjs,
                Property2FieldMap = new Dictionary<string, string>(),
                KeyPropertyNames=KeyPropertys,
                Filter=filter
            };
            DetailInfos.Add(dmi);
            return dmi;
        }
        public DetailMapInfo AppendDetailMap(string ChildTableName, IList ChildObjs, DetailRelation relation,IFilter filter, params string[] KeyPropertys)
        {
            return AppendDetailMap(ChildTableName, ChildObjs, new List<DetailRelation>() { relation },filter,KeyPropertys);
        }
        public DetailMapInfo AppendDetailMap(string ChildTableName, IList ChildObjs, string MasterPropertyKey, string ChildRelaProperty,IFilter filter, params string[] KeyPropertys)
        {
            return AppendDetailMap(ChildTableName, ChildObjs, new DetailRelation(MasterPropertyKey, ChildRelaProperty),filter,KeyPropertys);
        }

        public DetailMapInfo AppendDetailMap(string ChildTableName, IList ChildObjs, List<DetailRelation> relations, params string[] KeyPropertys)
        {
            return AppendDetailMap(ChildTableName, ChildObjs, relations,LibHelper.CreateIFilter(), KeyPropertys);
        }
        public DetailMapInfo AppendDetailMap(string ChildTableName, IList ChildObjs, DetailRelation relation,  params string[] KeyPropertys)
        {
            return AppendDetailMap(ChildTableName, ChildObjs, new List<DetailRelation>() { relation },LibHelper.CreateIFilter() , KeyPropertys);
        }
        public DetailMapInfo AppendDetailMap(string ChildTableName, IList ChildObjs, string MasterPropertyKey, string ChildRelaProperty, params string[] KeyPropertys)
        {
            return AppendDetailMap(ChildTableName, ChildObjs, new DetailRelation(MasterPropertyKey, ChildRelaProperty), LibHelper.CreateIFilter(), KeyPropertys);
        }
        public void AppendP2F(string PropertyName, string FieldName)
        {
            Property2FieldMap[PropertyName] = FieldName;
        }
    }
    [Serializable]
    public sealed class DetailMapInfo
    {
        public string TableName { get; set; }
        public IList Details { get; set; }
        /// <summary>
        /// 当是取数据时的检索过虑条件
        /// </summary>
        public IFilter Filter { get; set; }
        /// <summary>
        /// 父表与子表关联字段映射信息
        /// </summary>
        public List<DetailRelation> DetailRelations { get; set; }

        public Dictionary<string, string> Property2FieldMap { get; set; }

        public string[] KeyPropertyNames { get; set; }
        public void AppendP2F(string PropertyName, string FieldName)
        {
            Property2FieldMap[PropertyName] = FieldName;
        }
        Type type = null;
        public Type GetItemType()
        {
            if (type == null&& Details!=null && Details.Count>0)
            {
                type = Details[0].GetType();
            }
            return type;
        }
        PropertyInfo[] ps = null;
        public PropertyInfo[] GetPropertyInfos()
        {
            if (type!=null)
            {
                ps = type.GetProperties();
            }
            return ps;
        }
        
        /// <summary>
        /// 比较新旧明细记录，以便确认哪些是新增，哪些是修改，哪是删除，并分别存入到三个键值对象中
        /// Insert:新增
        /// Update:修改
        /// Delete:删除
        /// </summary>
        /// <param name="olds"></param>
        /// <returns></returns>
        public DynamicObj CompareNoO(List<DynamicObj> olds)
        {
            List<object> Insert = new List<object>();
            List<object> Update = new List<object>();
            Type type = null;
            foreach (object n in Details)
            {
                if (n == null) continue;
                if (n is IDictionary)
                {
                    DynamicObj obj = olds.Find(o => IsEqual((IDictionary)n, o));
                    if (obj != null) //修改
                    {
                        Update.Add(n);
                        olds.Remove(obj);
                    }
                    else //新增
                        Insert.Add(n);
                }
                else
                {
                    if (type == null) type = n.GetType();
                    DynamicObj obj = olds.Find(o => IsEqual(n, o, type));
                    if (obj != null) //修改
                    {
                        Update.Add(n);
                        olds.Remove(obj);
                    }
                    else //新增
                        Insert.Add(n);
                }
            }

            DynamicObj data=new DynamicObj();
            if (Insert.HasElement())
                data.Set("Insert", Insert);
            if (Update.HasElement())
                data.Set("Update", Update);
            if (olds.HasElement())
                data.Set("Delete", olds);
            return data;
        }
        private bool IsEqual(IDictionary o1, DynamicObj o2)
        {
            bool isEq = KeyPropertyNames.HasElement();
            foreach (string pn in KeyPropertyNames)
            {
                string kv1 = o1.Contains(pn)?o1[pn].ToStringValue():"";
                string kv2 = "";
                if (Property2FieldMap.ContainsKey(pn))
                    kv2 = o2.Get(Property2FieldMap[pn]).ToStringValue();
                else
                    kv2 = o2.Get(pn).ToStringValue();

                if (!kv1.IsEquals(kv2))
                {
                    isEq = false;
                    break;
                }
            }
            return isEq;
        }
        private bool IsEqual(object o1, DynamicObj o2, Type type)
        {
            bool isEq = KeyPropertyNames.HasElement();
            foreach (string pn in KeyPropertyNames)
            {
                PropertyInfo pi = type.GetProperty(pn);
                if (pi == null)
                {
                    isEq = false;
                    break;
                }

                string kv1 = pi.GetValue(o1, null).ToStringValue();
                string kv2 = "";
                if (Property2FieldMap.ContainsKey(pn))
                    kv2 = o2.Get(Property2FieldMap[pn]).ToStringValue();
                else
                    kv2 = o2.Get(pn).ToStringValue();

                if (!kv1.IsEquals(kv2))
                {
                    isEq = false;
                    break;
                }
            }
            return isEq;
        }
    }
    [Serializable]
    public class DetailRelation
    {
        public DetailRelation(string MasterKey, string DetailRela)
        {
            MasterKeyPropertyName = MasterKey;
            DetailRelaPropertyName = DetailRela;
        }
        public string MasterKeyPropertyName { get; set; }
        public string DetailRelaPropertyName { get; set; }
    }
}
