using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Data;
using System.Net;
using RS.Core.Filter;
using RS.Core.Cache;
using RS.Core.Data;
using RS.Core.Net;
using RS.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace RS.Core
{
    /// <summary>
    /// 框架类库总线助手类，用于创建框架底层类库的各对象
    /// </summary>
    public static class LibHelper
    {
        #region 创建缓存对象
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName)
        {
            return CacheHelper.OpenCache<TParam, TKey, TValue>(CacheName);
        }
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, Func<TParam, TValue> method)
        {
            return CacheHelper.OpenCache<TParam, TKey, TValue>(CacheName, method);
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="eliminateMethod">缓存淘汰算法</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, IEliminateMethod eliminateMethod)
        {
            return CacheHelper.OpenCache<TParam, TKey, TValue>(CacheName, eliminateMethod);
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="eliminateMethod">缓存淘汰算法</param>
        /// <param name="method">获取单项值的委托</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, IEliminateMethod eliminateMethod, Func<TParam, TValue> method)
        {
            return CacheHelper.OpenCache<TParam, TKey, TValue>(CacheName, eliminateMethod, method);
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="GapSpan">缓存同步时间间隔</param>
        /// <param name="method">同步获取所有数据源的委托</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, TimeSpan GapSpan, Func<Dictionary<TKey, TValue>> method)
        {
            return CacheHelper.OpenCache<TParam, TKey, TValue>(CacheName, GapSpan, method);
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <typeparam name="TParam"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="GapSpanExp">缓存同步时间间隔，定时器设置格式如下：
        /// 1、定时触发：0|h|1,表示按每小时激活,0|m|1 表示按每分钟激活,0|s|1 表示按每秒钟激活,0|ms|1 表示按每毫秒激活
        /// 2、时点触发：1|17:30;17:12;02:36</param>
        /// <param name="method">同步获取所有数据源的委托</param>
        /// <param name="datas">初始值</param>
        /// <returns></returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, string GapSpanExp, Func<Dictionary<TKey, TValue>> method, Dictionary<TKey, TValue> datas)
        {
            return CacheHelper.OpenCache<TParam, TKey, TValue>(CacheName, GapSpanExp, method, datas);
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="GapSpan">单项缓存过期时间间隔</param>
        /// <param name="method">过期时获取单项数据的委托</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, TimeSpan GapSpan, Func<TParam, TValue> method)
        {
            return CacheHelper.OpenCache<TParam, TKey, TValue>(CacheName, GapSpan, method);
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="GapSpan">同步时间间隔</param>
        /// <param name="method">获取单项数据源的委托</param>
        /// <param name="CacheDatas">初始缓存数据</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, TimeSpan GapSpan, Func<TParam, TValue> method, Dictionary<TKey, TValue> CacheDatas)
        {
            return CacheHelper.OpenCache(CacheName, GapSpan, method, CacheDatas);
        }
        /// <summary>
        /// 打开指定缓存,如缓存项不存在，则自动创建该项缓存
        /// </summary>
        /// <param name="CacheName">缓存项键值</param>
        /// <param name="method">获取单项数据源的委托</param>
        /// <param name="CacheDatas">初始缓存数据</param>
        /// <returns>该项缓存</returns>
        public static CacheItem<TParam, TKey, TValue> OpenCache<TParam, TKey, TValue>(string CacheName, Func<TParam, TValue> method, Dictionary<TKey, TValue> CacheDatas)
        {
            return CacheHelper.OpenCache(CacheName, method, CacheDatas);
        }
        #endregion

        #region 创建线程服务容器
        /// <summary>
        /// 创建服务控制容器:根据指定定时配置及线程方法
        /// </summary>
        /// <param name="appconfigExp">定时配置键值</param>
        /// <param name="method">线程服务要执行的具体代码委托</param>
        public static ThreadSvrUtil CreateThreadSvrForExp(string appconfigExp, ThreadStart method)
        {
            return ThreadSvrUtil.CreateSvrUtilByExp(appconfigExp, method);
        }
        /// <summary>
        /// 创建服务控制容器:根据指定定时配置及线程方法
        /// </summary>
        /// <param name="ts">定时间隔</param>
        /// <param name="method">线程服务要执行的具体代码委托</param>
        /// <returns></returns>
        public static ThreadSvrUtil CreateThreadSvr(TimeSpan ts, ThreadStart method)
        {
            return ThreadSvrUtil.CreateSvrUtil(ts, method);
        }
        /// <summary>
        /// 创建服务控制容器:根据指定毫秒及线程方法
        /// </summary>
        /// <param name="Milliseconds"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static ThreadSvrUtil CreateThreadSvr(int Milliseconds, ThreadStart method)
        {
            return ThreadSvrUtil.CreateSvrUtil(Milliseconds, method);
        }
        #endregion

        #region 创建数据访问对象
        /// <summary>
        /// 创建数据访问对象实例
        /// </summary>
        public static DbUtils CreateDbUtil()
        {
            return DbUtils.NewDB();
        }
        /// <summary>
        /// 创建数据访问对象实例
        /// </summary>
        /// <returns></returns>
        public static DbUtils CreateDbUtil(string ConnectionString, DbDriverType type)
        {
            return DbUtils.NewDB(ConnectionString, type);
        }
        /// <summary>
        /// 创建数据访问对象实例
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbUtils CreateDbUtil(string ConnectionString, Type type)
        {
            return DbUtils.NewDB(ConnectionString, type);
        }
        /// <summary>
        /// 注册标准的数据访问对象信息(注册系统内置数据驱动)，一般在系统初始化时注册
        /// </summary>
        public static void InitDbUtils(string ConnectionString, DbDriverType type)
        {
            DbUtils.RegStandDbLinkinfo(ConnectionString, type);
        }
        /// <summary>
        /// 注册标准的数据访问对象信息(注册扩展的数据驱动)，一般在系统初始化时注册
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="DefineDriverType"></param>
        public static void InitDbUtils(string ConnectionString, Type DefineDriverType)
        {
            DbUtils.RegStandDbLinkinfo(ConnectionString, DefineDriverType);
        }
        /// <summary>
        /// 注册标准的数据访问对象信息(注册系统内置数据驱动)，一般在系统初始化时注册
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="AssemblyFile"></param>
        /// <param name="TypeName"></param>
        public static void InitDbUtils(string ConnectionString, string AssemblyFile, string TypeName)
        {
            DbUtils.RegStandDbLinkinfo(ConnectionString, AssemblyFile, TypeName);
        }
        /// <summary>
        /// 测试当前默认连接是否成功
        /// </summary>
        /// <returns></returns>
        public static bool IsDbConnected(this DbUtils db)
        {
            bool rtn = false;
            try
            {
                db.GetScalar("select 1");
                rtn = true;
            }
            catch { }
            return rtn;
        }
        #endregion

        #region 过滤条件类

        public static IFilter CreateIFilter(IDbDriver db = null)
        {
            return new EmptySqlFilter(db);
        }


        #region 创建检索控件所需的构造函数
        public static SearchItem CreateSearchItem(FilterDataType datatype, params SearchFieldInfo[] Fields)
        {
            return new SearchItem(datatype, Fields);
        }
        public static SearchItem CreateSearchItem(string title, FilterDataType datatype, params SearchFieldInfo[] Fields)
        {
            return new SearchItem(title, datatype, Fields);
        }
        /// <summary>
        /// 构造函数:主要用于定义检索控件条件项(同一条件多字段)。
        /// </summary>
        /// <param name="field">要检索的字段</param>
        /// <param name="title">该检索项标题</param>
        /// <param name="datatype">检索的数据类型</param>
        /// <param name="_getDataSource">获取检索项数据来源的委托</param>
        /// <param name="_setControl">进行控件定义的委托</param>
        public static SearchItem CreateSearchItem(string title, FilterDataType datatype, Func<List<DynamicObj>> _getDataSource, params SearchFieldInfo[] Fields)
        {
            return new SearchItem(title, datatype, _getDataSource, Fields);
        }

        public static SearchItem CreateSearchItem(string title, FilterDataType datatype, Func<List<DynamicObj>> _getDataSource, Guid QueryID, params SearchFieldInfo[] Fields)
        {
            return new SearchItem(title, datatype, _getDataSource, QueryID, Fields);
        }
        #endregion
        #region 为创建检索过滤条件而自定义创建的检索值的构造函数
        /// <summary>
        /// 创建指定字段的检索实例(单字段)
        /// </summary>
        /// <param name="field"></param>
        /// <param name="datatype"></param>
        /// <param name="value"></param>
        public static SearchItem CreateSearchItem(string field, FilterDataType datatype, CompareType ctype, DateCompareType dtype, object v1, object v2)
        {
            return new SearchItem(field, datatype, ctype, dtype, v1, v2);
        }
        /// <summary>
        /// 创建多字段的检索实例,这里是同时检索多字段，每个字段过滤条件是以 or 连接
        /// </summary>
        /// <param name="datatype"></param>
        /// <param name="value"></param>
        /// <param name="Fields"></param>
        public static SearchItem CreateSearchItem(FilterDataType datatype, object value, SearchFieldInfo[] Fields)
        {
            return new SearchItem(datatype, value, Fields);
        }
        /// <summary>
        /// 创建多字段的检索实例,这里是同时检索多字段，每个字段过滤条件是以 or 连接
        /// </summary>
        /// <param name="datatype"></param>
        /// <param name="ctype"></param>
        /// <param name="dtype"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="Fields"></param>
        public static SearchItem CreateSearchItem(FilterDataType datatype, CompareType ctype, DateCompareType dtype, object v1, object v2, SearchFieldInfo[] Fields)
        {
            return new SearchItem(datatype, ctype, dtype, v1, v2, Fields);
        }

        public static SearchItem CreateSearchItem(string field, FilterDataType datatype, object value)
        {
            return new SearchItem(field, datatype, value);
        }
        #endregion


        #endregion


        #region 文件操作

        #region 读取或写入文本文件
        /// <summary>
        /// 读取指定文件文本内容（默认编码格式),如文件不存在或文件已被占用，或没有权限，则会出现异常
        /// </summary>
        /// <param name="FilePath">要读取的文件路径</param>
        /// <returns></returns>
        public static string ReadTextFile(string FilePath)
        {
            return FileUtils.ReadTextFile(FilePath, true);
        }

        public static string ReadTextFile(string FilePath, Encoding encoding)
        {
            return FileUtils.ReadTextFile(FilePath, encoding, true);
        }

        /// <summary>
        /// 将指定字符以文本方式写入到文件中(覆盖写入)
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Value"></param>
        public static void WriteTextFile(string FilePath, string Value, bool HasTry = false)
        {
            FileUtils.WriteTextFile(FilePath, Value, HasTry);
        }

        public static void WriteTextFile(string FilePath, string Value, Encoding encoding, bool HasTry = false)
        {
            FileUtils.WriteTextFile(FilePath, Value, encoding, HasTry);
        }

        /// <summary>
        /// 向指定文件追加写入文本，如文件不存在，则新建文件
        /// </summary>
        /// <param name="FieldPath"></param>
        /// <param name="Value"></param>
        public static void AppendTextFile(string FilePath, string Value)
        {
            FileUtils.AppendTextFile(FilePath, Value, true);
        }
        /// <summary>
        /// 向指定文件追加写入文本，如文件不存在，则新建文件
        /// </summary>
        /// <param name="FieldPath"></param>
        /// <param name="Value"></param>
        public static void AppendTextFileLine(string FilePath, string Value, bool HasTry = false)
        {
            FileUtils.AppendTextFileLine(FilePath, Value, HasTry);
        }
        /// <summary>
        /// 向指定文件追加一行文本，如文件不存在，则新建文件
        /// </summary>
        /// <param name="FilePath">文件名</param>
        /// <param name="Value">要写入的内容</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="HasTry">是否自动带出异常(在有BUG时)</param>
        public static void AppendTextFile(string FilePath, string Value, Encoding encoding, bool HasTry = false)
        {
            FileUtils.AppendTextFile(FilePath, Value, encoding, HasTry);
        }

        public static void AppendTextFileLine(string FilePath, string Value, Encoding encoding, bool HasTry = false)
        {
            FileUtils.AppendTextFileLine(FilePath, Value, encoding, HasTry);
        }
        #endregion

        #region 二进制文件读取及写入相关操作
        public static byte[] ReadBinaryFile(string FilePath, bool HasTry = false)
        {
            return FileUtils.ReadBinaryFile(FilePath, HasTry);
        }

        /// <summary>
        /// 将指定字节以二进制方式写入文件中，文件不存在，则新建
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="bytes"></param>
        public static void WriteBinaryFile(string FilePath, byte[] bytes, bool HasTry = false)
        {
            FileUtils.WriteBinaryFile(FilePath, bytes, HasTry);
        }
        #endregion

        #region 以二进制方式保存文本内容
        /// <summary>
        /// 将指定字符以二进制方式写入文件中，默认保存为UTF8格式，文件不存在，则新建
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="bytes"></param>
        public static void WriteBinaryFile(string FilePath, string value, bool HasTry = false)
        {
            FileUtils.WriteBinaryFile(FilePath, value, HasTry);
        }
        #endregion

        #region 对象持久化二进制文件存取
        public static void WriteObjectToFile(string FilePath, object obj)
        {
            //先序列化为json对象，再将json对象转为字节保存
            byte[] bs = Serializer.BinarySerialize(obj);

            WriteBinaryFile(FilePath,bs, true);
        }
        public static T ReadObjectFromFile<T>(string File)
        {
            byte[] bs = ReadBinaryFile(File, true);
            return (T)Serializer.BinaryDeserialize(bs);
        }
        #endregion
        #endregion

        #region 序列化操作

        /// <summary>
        /// XML序列化成字节
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }
        /// <summary>
        /// XML反序列化为对象
        /// </summary>
        /// <param name="array"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialize(byte[] array, Type type)
        {
            return Serializer.Deserialize(array, type);
        }

        /// <summary>
        /// [加密压缩]将指定对象序列化成二进制数组，以便进行网络传输给客户端或服务端
        /// 如果处理失败或对象无效，则返回null
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static byte[] BinarySerialize(object o)
        {
            return Serializer.BinarySerialize(o);
        }

        /// <summary>
        /// [加密压缩]二进制反序化成具体实例对象
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static object BinaryDeserialize(byte[] array)
        {
            return Serializer.BinaryDeserialize(array);
        }

        #endregion

        #region 初始组件注册
        /// <summary>
        /// 注册加密的密约
        /// </summary>
        /// <param name="Key"></param>
        public static void RegisEncryptKey(string Key)
        {
            StringEncrypt.PassKey = Key;
        }


        /// <summary>
        /// 注册系统动行日志类型及日去存放路径
        /// </summary>
        /// <param name="type"></param>
        /// <param name="LogPath"></param>
        public static void RegisLogger(LogType type, string LogPath)
        {
            Loger.RegisLogger(type, LogPath);
        }
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="type"></param>
        public static void RegisLogger(LogType type)
        {
            Loger.RegisLogger(type);
        }
        public static void RegisLogger(string TypeExp)
        {
            Loger.RegisLogger(TypeExp);
        }
        public static void RegisLogger(string TypeExp, string LogPath)
        {
            Loger.RegisLogger(TypeExp, LogPath);
        }

        public static void RegisLogger(LogType type, string SqlConnectstring, string LogTableName,DbDriverType dbType = DbDriverType.SQLServer)
        {
            Loger.RegisLogger(type, SqlConnectstring, LogTableName, dbType);
        }
        public static void RegisLogger(string TypeExp, string SqlConnectstring, string LogTableName, DbDriverType dbType = DbDriverType.SQLServer)
        {
            Loger.RegisLogger(TypeExp, SqlConnectstring, LogTableName, dbType);
        }

        #endregion


        #region 转成标准树型集

        /// <summary>
        /// 根据指定的XPath对象集转成标准树型集
        /// </summary>
        /// <param name="items"></param>
        /// <param name="XPathProperty"></param>
        /// <param name="IsSameLenth"></param>
        /// <param name="XPathLength"></param>
        /// <returns></returns>
        public static TreeEntityList ToTreeEntity(this IEnumerable items, string XPathProperty, bool IsSameLenth, int XPathLength)
        {
            return ToTreeEntity(items, XPathProperty, IsSameLenth, XPathLength, 0, 0);
        }
        /// <summary>
        /// 根据指定的XPath对象集转成标准树型集
        /// </summary>
        /// <param name="items"></param>
        /// <param name="XPathProperty"></param>
        /// <param name="IsSameLenth"></param>
        /// <param name="XPathLength"></param>
        /// <returns></returns>
        public static TreeEntityList ToTreeEntity(this IEnumerable items, string XPathProperty, bool IsSameLenth, int XPathLength, int PrefixLength, int LstfixLength)
        {
            Dictionary<object, TreeContainer> tmpList = new Dictionary<object, TreeContainer>();
            TreeEntityList nodes = new TreeEntityList();
            Type type = null;
            int count = 0;
            if (XPathProperty.IsWhiteSpace())
            {
                foreach (object o in items)
                    nodes.Add(new TreeContainer(o));
            }
            else
            {
                foreach (object o in items)
                {
                    count++;
                    if (o == null) continue;
                    TreeContainer tc;
                    if (tmpList.ContainsKey(o))
                        tc = tmpList[o];
                    else
                    {
                        tc = new TreeContainer(o);
                        tmpList[o] = tc;
                    }

                    if (type == null) type = o.GetType();
                    string xpath = GetPropertyValue(o, XPathProperty, type).ToStringValue();//获取该层的XPath值
                    //为顶层
                    if (xpath.IsWhiteSpace() || xpath.Length == XPathLength + PrefixLength + LstfixLength) //顶层
                    {
                        nodes.Add(tc);
                    }
                    //查找其子集
                    var s = from item in items.Cast<object>()
                            where IsChild(o, item, XPathProperty, type, XPathLength, PrefixLength, LstfixLength, xpath, IsSameLenth)
                            select item;

                    foreach (object obj in s)
                    {
                        TreeContainer ctc;
                        if (tmpList.ContainsKey(obj))
                            ctc = tmpList[obj];
                        else
                        {
                            ctc = new TreeContainer(obj);
                            tmpList[obj] = ctc;
                        }
                        if (!tc.Childs.Contains(ctc))
                            tc.Childs.Add(ctc);
                    }
                }
            }
            return nodes;
        }

        private static bool IsChild(object o, object item, string XPathProperty, Type type, int xpathlength, int PrefixLength, int LstFixLength, string parentXPath, bool isSameXpathLen)
        {
            if (item == o) return false;
            string xpath = GetPropertyValue(item, XPathProperty, type).ToStringValue();
            if (isSameXpathLen) //相同长度
            {
                //'M0100','M0101' 有前缀
                //'M0100D','M0101D' 有后缀
                int pxlen = xpathlength + PrefixLength + LstFixLength;
                //父级XPath
                string px = PrefixLength <= 0 ? "" : parentXPath.Substring(0, PrefixLength); //取前缀
                for (int i = PrefixLength; i < parentXPath.Length - LstFixLength; i += xpathlength)
                {
                    string x;
                    if (i + xpathlength + LstFixLength >= parentXPath.Length)
                        x = parentXPath.Substring(i);
                    else
                        x = parentXPath.Substring(i, xpathlength);

                    if (x.Length == xpathlength && x != "0".PadLeft(xpathlength, '0'))
                    {
                        px += x;
                    }
                    else
                        break;
                }

                string cx = PrefixLength <= 0 ? "" : xpath.Substring(0, PrefixLength); //子级XPath
                for (int i = PrefixLength; i < xpath.Length - LstFixLength; i += xpathlength)
                {
                    string x;
                    if (i + xpathlength + LstFixLength >= xpath.Length)
                        x = xpath.Substring(i);
                    else
                        x = xpath.Substring(i, xpathlength);
                    if (x.Length == xpathlength && x != "0".PadLeft(xpathlength, '0'))
                    {
                        cx += x;
                    }
                    else
                        break;
                }

                string pfactxpath = px.Substring(PrefixLength, px.Length - PrefixLength - LstFixLength);
                string factxpath = cx.Substring(PrefixLength, cx.Length - PrefixLength - LstFixLength);

                return factxpath.StartsWith(pfactxpath) && factxpath.Length == pfactxpath.Length + xpathlength;
            }
            else
            {
                string pfactxpath = parentXPath.Substring(PrefixLength, parentXPath.Length - PrefixLength - LstFixLength);
                string factxpath = xpath.Substring(PrefixLength, xpath.Length - PrefixLength - LstFixLength);

                return factxpath.StartsWith(pfactxpath) && factxpath.Length == pfactxpath.Length + xpathlength;
            }
        }

        /// <summary>
        /// 根据属性的关联将指定对象集封装成标准树型结构
        /// </summary>
        /// <param name="items"></param>
        /// <param name="ParentIDProperty"></param>
        /// <param name="KeyProperty"></param>
        /// <returns></returns>
        public static TreeEntityList ToTreeEntity(this IEnumerable items, string ParentIDProperty, string KeyProperty)
        {
            Dictionary<object, TreeContainer> tmpList = new Dictionary<object, TreeContainer>();
            TreeEntityList nodes = new TreeEntityList();
            Type type = null;
            int count = 0;
            if (ParentIDProperty.IsWhiteSpace() || KeyProperty.IsWhiteSpace())
            {
                foreach (object o in items)
                    nodes.Add(new TreeContainer(o));
            }
            else
            {
                foreach (object o in items)
                {
                    count++;
                    if (o == null) continue;
                    TreeContainer tc;
                    if (tmpList.ContainsKey(o))
                        tc = tmpList[o];
                    else
                    {
                        tc = new TreeContainer(o);
                        tmpList[o] = tc;
                    }

                    if (type == null) type = o.GetType();
                    object pv = GetPropertyValue(o, ParentIDProperty, type);
                    //为顶层(没有上一级，则视为顶层)
                    if (pv.IsNullOrDBNull() || pv.ToStringValue().IsWhiteSpace())
                    {
                        nodes.Add(tc);
                    }

                    //查找其子集
                    object kv = GetPropertyValue(o, KeyProperty, type);
                    if (kv == null) //当前项无值,则作为顶层
                    {
                        if (!nodes.Contains(tc)) nodes.Add(tc);
                        continue;
                    }
                    var s = from item in items.Cast<object>()
                            where (item != o && kv.Equals(GetPropertyValue(item, ParentIDProperty, type)))
                            select item;

                    foreach (object obj in s)
                    {
                        TreeContainer ctc;
                        if (tmpList.ContainsKey(obj))
                            ctc = tmpList[obj];
                        else
                        {
                            ctc = new TreeContainer(obj);
                            tmpList[obj] = ctc;
                        }
                        if (!tc.Childs.Contains(ctc))
                        {
                            ctc.Parent = tc;
                            tc.Childs.Add(ctc);
                            if (nodes.Contains(ctc))
                                nodes.Remove(ctc);
                        }
                    }
                    if (tc.Parent == null && !nodes.Contains(tc))
                        nodes.Add(tc);
                }
            }
            return nodes;
        }

        /// <summary>
        /// 根据属性的关联将指定对象集封装成标准树型结构
        /// </summary>
        /// <param name="items"></param>
        /// <param name="ParentIDProperty"></param>
        /// <param name="KeyProperty"></param>
        /// <returns></returns>
        public static List<T> ToTreeList<T>(List<T> items, string ChildsProperty, string ParentIDProperty, string KeyProperty)
        {
            Dictionary<object, object[]> tmpList = new Dictionary<object, object[]>();
            List<T> nodes = new List<T>();
            Type type = null;
            int count = 0;
            if (ChildsProperty.IsWhiteSpace() || ParentIDProperty.IsWhiteSpace() || KeyProperty.IsWhiteSpace())
            {
                return items.ConvertAll<T>(s => s);
            }
            else
            {
                foreach (T o in items)
                {
                    object parent = null;
                    count++;
                    if (o == null) continue;
                    T tc;
                    if (tmpList.ContainsKey(o))
                    {
                        tc = (T)tmpList[o][0];
                        parent = tmpList[o][1];
                    }
                    else
                    {
                        tc = o;
                        parent = null;
                        tmpList[o] = new object[] { tc, parent };
                    }

                    if (type == null) type = typeof(T);

                    object pv = GetPropertyValue(o, ParentIDProperty, type);
                    //为顶层(没有上一级，则视为顶层)
                    if (pv.IsNullOrDBNull() || pv.ToStringValue().IsWhiteSpace() || pv.Equals(Guid.Empty) || pv.Equals(0))
                    {
                        nodes.Add(tc);
                    }

                    //查找其子集
                    object kv = GetPropertyValue(o, KeyProperty, type);
                    if (kv == null) //当前项无值,则作为顶层
                    {
                        if (!nodes.Contains(tc)) nodes.Add(tc);
                        continue;
                    }
                    var s = from item in items.Cast<object>()
                            where (item != (object)o && kv.Equals(GetPropertyValue(item, ParentIDProperty, type)))
                            select item;

                    foreach (T obj in s)
                    {
                        object cparent = null;
                        T ctc;
                        if (tmpList.ContainsKey(obj))
                        {
                            ctc = (T)tmpList[obj][0];
                            cparent = tmpList[obj][1];
                        }
                        else
                        {
                            ctc = obj;
                            cparent = tc;
                            tmpList[obj] = new object[] { ctc, cparent };
                        }
                        List<T> Childs = GetPropertyValue(tc, ChildsProperty, type) as List<T>;
                        if (!Childs.Contains(ctc))
                        {
                            Childs.Add(ctc);
                            if (nodes.Contains(ctc))
                                nodes.Remove(ctc);
                        }
                    }
                    if (parent == null && !nodes.Contains(tc))
                        nodes.Add(tc);
                }
            }
            return nodes;
        }



        private static object GetPropertyValue(object o, string propertyName, Type type)
        {
            object rtn = null;
            if (o is System.Data.DataRow)
            {
                try
                {
                    rtn = ((System.Data.DataRow)o)[propertyName];
                }
                catch { }
                return rtn;
            }
            else if (o is System.Data.DataRowView)
            {
                try
                {
                    rtn = ((System.Data.DataRowView)o)[propertyName];
                }
                catch { }
                return rtn;
            }
            else if (o is IDictionary)
            {
                try
                {
                    rtn = ((IDictionary)o)[propertyName];
                }
                catch { }
                return rtn;
            }
            else
            {
                PropertyInfo pi = type.GetProperty(propertyName);
                if (pi == null) return null;
                if (pi.CanRead)
                    return pi.GetValue(o, null);
                else
                    return null;
            }
        }
        /// <summary>
        /// 将指定树型对象集合转成标准树形集合
        /// </summary>
        /// <param name="items"></param>
        /// <param name="ChildPropertys"></param>
        /// <returns></returns>
        public static TreeEntityList ToTreeEntity(this IEnumerable items, string ChildPropertys)
        {
            TreeEntityList nodes = new TreeEntityList();
            foreach (object o in items)
            {
                TreeContainer node = new TreeContainer(o);
                IEnumerable cnodes = GetTreeEntityChilds(o, ChildPropertys);
                if (cnodes != null)
                {
                    foreach (TreeContainer tc in cnodes)
                        node.Childs.Add(tc);
                }
                nodes.Add(node);
            }
            return nodes;
        }

        private static IEnumerable GetTreeEntityChilds(object item, string ChildPropertys)
        {
            if (item == null) return null;
            Type t = item.GetType();
            if (t.IsEnum || t.IsValueType) return null;
            PropertyInfo pi = t.GetProperty(ChildPropertys);
            if (pi == null) return null;
            object o = pi.GetValue(item, null);
            if (o == null) return null;
            if (o is IEnumerable)
            {
                return ToTreeEntity(o as IEnumerable, ChildPropertys);
            }
            else
                return null;
        }

        #endregion

        #region Socket通讯相关
        /// <summary>
        /// 获取Socket通讯客户端，用于连接指定IP和端口，以便进行Socket通讯
        /// </summary>
        /// <returns></returns>
        public static ClientHandler CreateSocketClient(string IP, int Port)
        {
            return new ClientHandler(IP, Port);
        }
        /// <summary>
        /// 创建服务监听通道，用于监听指定端口，以便接收其它服务请求
        /// </summary>
        /// <returns></returns>
        public static ClientHandler CreateSocketListener(int Port, int maxClient = 0)
        {
            if (maxClient == 0)
                return new ClientHandler(Port);
            else
                return new ClientHandler(Port, maxClient);
        }
        #endregion

        #region 将指定对象集转成标准树型对象Json


        public static List<Dictionary<string, object>> ToTreeGridDatas(this IEnumerable items, string ChildrenName, Converter<object, object> ConvertMethod = null)
        {
            List<Dictionary<string, object>> nodes = new List<Dictionary<string, object>>();
            Type itemType = null;
            foreach (object o in items)
            {
                if (o == null) continue;
                var item = o;
                if (ConvertMethod != null)
                {
                    item = ConvertMethod(o);
                    if (item == null) continue;
                }

                if (itemType == null)
                {
                    itemType = item.GetType();
                }
                Dictionary<string, object> newitem = item.ToDictonaryObj(itemType, null);
                if (newitem.IsNotNull()) nodes.Add(newitem);
            }
            foreach (Dictionary<string, object> node in nodes)
            {
                if (node.ContainsKey(ChildrenName))
                {
                    object o = node[ChildrenName];
                    if (o is IEnumerable)
                        node[ChildrenName] = ToTreeGridDatas((IEnumerable)o, ChildrenName, ConvertMethod);
                }
            }
            return nodes;
        }

        public static List<Dictionary<string, object>> ToTreeGridDatas(this TreeEntityList items, string jsonChildrenName)
        {
            List<Dictionary<string, object>> nodes = new List<Dictionary<string, object>>();
            Type type = null;
            foreach (TreeContainer item in items)
            {
                var obj = item.Current;
                if (item.Current == null) continue;

                if (type == null) type = obj.GetType();
                Dictionary<string, object> node = ToDictonaryObj(obj, type);
                if (node.ContainsKey("Childs"))
                {
                    node.Remove("Childs");
                }
                node[jsonChildrenName] = ToTreeGridDatas(item.Childs, jsonChildrenName);
                nodes.Add(node);
            }
            return nodes;
        }
        public static List<Dictionary<string, object>> ToTreeGridDatas(this IEnumerable items, string ChildrenName, string jsonChildrenName)
        {
            List<Dictionary<string, object>> nodes = new List<Dictionary<string, object>>();
            Type type = null;
            foreach (object obj in items)
            {
                if (type == null) type = obj.GetType();
                Dictionary<string, object> node = ToDictonaryObj(obj, type);
                if (node.ContainsKey(ChildrenName))
                {
                    object o = node[ChildrenName];
                    node.Remove(ChildrenName);
                    if (o is IEnumerable)
                    {
                        node[jsonChildrenName] = ToTreeGridDatas((IEnumerable)o, ChildrenName, jsonChildrenName);
                    }
                    else
                        node[jsonChildrenName] = o;
                }
                nodes.Add(node);
            }
            return nodes;
        }


        public static Dictionary<string, object> ToDictonaryObj(this object item, Type itemType, List<string> PropertyNames = null)
        {
            if (item == null) return null;
            if (PropertyNames == null) PropertyNames = GetAllPropertyName(item, itemType);
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string name in PropertyNames)
            {
                object v = GetPropertyValue(item, name, itemType);
                data[name] = v.IsNull() ? DBNull.Value : v;
            }
            return data;
        }

        public static DynamicObj ToDynamicObj(this object item, Type itemType, List<string> PropertyNames = null)
        {
            if (item == null) return null;
            if (PropertyNames == null) PropertyNames = GetAllPropertyName(item, itemType);
            DynamicObj data = new DynamicObj();
            foreach (string name in PropertyNames)
            {
                object v = GetPropertyValue(item, name, itemType);
                data[name] = v.IsNull() ? DBNull.Value : v;
            }
            return data;
        }





        public static List<Dictionary<string, object>> ToTreeGridDatas(this IEnumerable items, string ParentIDProperty, string KeyProperty, string jsonChildrenName)
        {
            Dictionary<object, Dictionary<string, object>> tmpList = new Dictionary<object, Dictionary<string, object>>();
            List<Dictionary<string, object>> nodes = new List<Dictionary<string, object>>();
            Type type = null;
            int count = 0;
            if (ParentIDProperty.IsWhiteSpace() || KeyProperty.IsWhiteSpace() || jsonChildrenName.IsWhiteSpace())
            {
                foreach (object o in items)
                {
                    if (o == null) continue;
                    if (type == null) type = o.GetType();
                    Dictionary<string, object> obj = ToDictonaryObj(o, type);
                    if (obj != null) nodes.Add(obj);
                }
            }
            else
            {
                foreach (object o in items)
                {
                    count++;
                    if (o == null) continue;
                    if (type == null) type = o.GetType();
                    Dictionary<string, object> tc;
                    List<Dictionary<string, object>> children;
                    if (tmpList.ContainsKey(o)) //该对像之前已遍历过
                    {
                        tc = tmpList[o];
                        children = (List<Dictionary<string, object>>)tc[jsonChildrenName];
                    }
                    else
                    {
                        children = new List<Dictionary<string, object>>();
                        tc = ToDictonaryObj(o, type);
                        tc[jsonChildrenName] = children;
                        tmpList[o] = tc;
                    }

                    if (type == null) type = o.GetType();
                    object pv = GetPropertyValue(o, ParentIDProperty, type);
                    //为顶层
                    if (pv.IsNullOrDBNull() || pv.ToStringValue().IsWhiteSpace() || pv.ToLong() == 0)
                    {
                        nodes.Add(tc);
                    }

                    //查找其子集
                    object kv = GetPropertyValue(o, KeyProperty, type);
                    if (kv == null) //当前项无值,则作为厅层
                    {
                        if (!nodes.Contains(tc)) nodes.Add(tc);
                        continue;
                    }
                    var s = from item in items.Cast<object>()
                            where (item != o && kv.Equals(GetPropertyValue(item, ParentIDProperty, type)))
                            select item;

                    foreach (object obj in s)
                    {
                        Dictionary<string, object> ctc;
                        if (tmpList.ContainsKey(obj))
                            ctc = tmpList[obj];
                        else
                        {
                            ctc = ToDictonaryObj(obj, type);
                            ctc[jsonChildrenName] = new List<Dictionary<string, object>>();
                            tmpList[obj] = ctc;
                        }
                        if (!children.Contains(ctc))
                            children.Add(ctc);
                    }
                }
            }
            return nodes;
        }

        public static List<string> GetAllPropertyName(object dataItem, Type type)
        {
            List<string> items = new List<string>();
            if (dataItem is DataRowView)
            {
                DataRowView dv = dataItem as DataRowView;
                DataColumnCollection cs = dv.DataView.Table.Columns;
                foreach (DataColumn dc in cs)
                {
                    items.Add(dc.ColumnName);
                }
            }
            else if (dataItem is DataRow)
            {
                DataRow dv = dataItem as DataRow;
                DataColumnCollection cs = dv.Table.Columns;
                foreach (DataColumn dc in cs)
                {
                    items.Add(dc.ColumnName);
                }
            }
            else if (dataItem is IDictionary)
            {
                IDictionary lists = dataItem as IDictionary;
                foreach (string s in lists.Keys)
                {
                    if (lists[s] == null)
                        items.Add(s);
                    else
                        items.Add(s);
                }
            }
            else
            {
                PropertyInfo[] ps = type.GetProperties();
                foreach (PropertyInfo p in ps)
                {
                    items.Add(p.Name);
                }
            }
            return items;
        }

        #endregion


        #region 读取配置文件

        //public static IConfiguration Configuration { get; set; }
        //static AppConfigurtaionServices()
        //{
        //    //ReloadOnChange = true 当appsettings.json被修改时重新加载            
        //    Configuration = new ConfigurationBuilder()
        //    .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
        //    .Build();
        //}




        public static string GetAppSetting(string key)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
.Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
.Build();
            return Configuration[key];

           // return "";// System.Configuration.ConfigurationManager.AppSettings[key].ToStringValue();
        }

        public static string GetConnectionString(string key)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
.Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
.Build();
            return Configuration.GetConnectionString(key);


            //return "";// System.Configuration.ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
        #endregion

        #region MD5加密
        //// <summary>
        /// 取得MD5加密串
        /// </summary>
        /// <param name="input">源明文字符串</param>
        /// <returns>密文字符串</returns>
        public static string MD5Hash(this string input)
        {
            if (input.IsWhiteSpace()) return input;
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = md5.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            string password = s.ToString();
            return password;
        }
        /// <summary>
        /// 得到SHA1加密字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SHA1Hash(this string input)
        {
            if (input.IsWhiteSpace()) return input;
            System.Security.Cryptography.SHA1CryptoServiceProvider sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = sha1.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            string password = s.ToString();
            return password;
        }
        #endregion

        #region 上传目录创建相关

        public static UploadPathInfo CreateUploadPath(string MainDirName, UploadPathType PathType, string UploadRootPath, string WebVirthPath)
        {
            return PathUtils.CreateUploadPath(MainDirName, PathType, UploadRootPath, WebVirthPath);
        }
        #endregion


        //获取当前请求系统的IP 
        public static string GetClientIP()
        {
            return "";
        }
    }


}
