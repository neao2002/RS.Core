using RS.Core.Data;
using RS.Core.Filter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace RS.Core
{
    /// <summary>
    /// 标准数据访问对象(对外公开)
    /// </summary>
    [Serializable]
    public sealed class DbUtils:IDbDriver
    {        
        #region 静态初始化控制

        private static DataConnInfo DbLinkinfo=null;
        /// <summary>
        /// 注册标准的数据访问对象信息(注册系统内置数据驱动)，一般在系统初始化时注册
        /// </summary>
        public static void RegStandDbLinkinfo(string ConnectionString,DbDriverType type)
        {
            DbLinkinfo = new DataConnInfo(ConnectionString, type);
        }
        /// <summary>
        /// 注册标准的数据访问对象信息(注册扩展的数据驱动)，一般在系统初始化时注册
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="DefineDriverType"></param>
        public static void RegStandDbLinkinfo(string ConnectionString,Type DefineDriverType)
        {
            DbLinkinfo = new DataConnInfo(ConnectionString,DefineDriverType);
        }
        /// <summary>
        /// 注册标准的数据访问对象信息(注册系统内置数据驱动)，一般在系统初始化时注册
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="AssemblyFile"></param>
        /// <param name="TypeName"></param>
        public static void RegStandDbLinkinfo(string ConnectionString, string AssemblyFile, string TypeName)
        { 
            System.Reflection.Assembly asm=Assembly.LoadFile(AssemblyFile);
            Type type=asm.GetType(TypeName);

            DbLinkinfo = new DataConnInfo(ConnectionString, type);
        }

        /// <summary>
        /// 创建数据访问对象实例
        /// </summary>
        /// <returns></returns>
        public static DbUtils NewDB()
        {
            return new DbUtils();
        }
        /// <summary>
        /// 是否已初始化
        /// </summary>
        /// <returns></returns>
        public static bool IsInited()
        {
            return DbLinkinfo != null;
        }
        public static DbDriverType GetDefaultDBType()
        {
            if (DbLinkinfo == null)
                return DbDriverType.SQLServer;
            else
                return DbLinkinfo.DbDriverType;
        }

        /// <summary>
        /// 创建数据访问对象实例
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbUtils NewDB(string ConnectionString, DbDriverType type)
        {
            return new DbUtils(ConnectionString, type);
        }
        public static DbUtils NewDB(string ConnectionString, Type type)
        {
            return new DbUtils(ConnectionString, type);
        }
        public static DbUtils NewDB(DbConnection cnn)
        {
            return new DbUtils(cnn);
        }
        public static DbUtils NewDB(DbConnection cnn, DbDriverType type)
        {
            return new DbUtils(cnn,type);
        }

        public static DbUtils NewDB(DbConnection cnn,DbTransaction mytran)
        {
            return new DbUtils(cnn,mytran);
        }



        #endregion
        
        
        private DataConnInfo LinkInfo;

        private DbUtils(string connStr, Type DbType)
        {
            LinkInfo = new DataConnInfo(connStr, DbType);
        }
        /// <summary>
        /// 构造函数:创建指定连接字符串及数据驱动类型的数据访问对象 
        /// </summary>
        /// <param name="ConnectionString">链接字符串</param>
        /// <param name="type">驱动类型</param>
        private DbUtils(string ConnectionString, DbDriverType type)
        {
            LinkInfo = new DataConnInfo(ConnectionString, type);
        }

        private DbUtils(DbConnection cnn)
        {
            LinkInfo = new DataConnInfo(cnn);
        }
        private DbUtils(DbConnection cnn,DbTransaction mytran)
        {
            LinkInfo = new DataConnInfo(cnn,mytran);
        }
        private DbUtils(DbConnection cnn, DbDriverType type)
        {
            LinkInfo = new DataConnInfo(cnn,type);
        }
        /// <summary>
        /// 以初始注册的标准数据驱动构造数据访问对象，如果未进行初始化，则默认是MS Sql server为数据驱动，字符串为空。
        /// </summary>
        private DbUtils()
        {
            if (DbLinkinfo == null)
                LinkInfo = new DataConnInfo("", DbDriverType.SQLServer);
            else
                LinkInfo = DbLinkinfo.Clone();
        }

        #region 连接及事务
        /// <summary>
        /// 获取当前数据链接
        /// </summary>
        public DbConnection Connection
        {
            get { return LinkInfo.DB.Connection; }
        }
        /// <summary>
        /// 获取当前链接字符串
        /// </summary>
        /// <returns></returns>
        public string GetDbConnectionString()
        {
            return LinkInfo.DB.GetDbConnectionString();
        }

        /// <summary>
        /// 是否已在事务中
        /// </summary>
        public bool IsInTransaction
        {
            get { return LinkInfo.DB.IsInTransaction; }
        }

        public DbTransaction Trans
        {
            get { return LinkInfo.DB.Trans; }
        }
        public void BeginTransaction()
        {
            LinkInfo.DB.BeginTransaction();
        }
        public void BeginTransaction(System.Data.IsolationLevel level)
        {
            LinkInfo.DB.BeginTransaction(level);
        }
        public void Commit()
        {
            LinkInfo.DB.Commit();
        }
        public void Rollback()
        {
            LinkInfo.DB.Rollback();
        }

        #endregion

        #region 获取DataTable相关方法
        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        public DataTable GetDataTable(string strQuery)
        {
            return LinkInfo.DB.GetDataTable(strQuery);
        }

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        public void GetDataTable(string strQuery, DataTable dt)
        {
            LinkInfo.DB.GetDataTable(strQuery, dt);
        }

        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        public DataTable GetDataTable(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.GetDataTable(cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        public void GetDataTable(DataTable dt, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            LinkInfo.DB.GetDataTable(dt, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 获取指定SQL产生的数据结构,无记录
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        public DataTable GetSchemaTableForSql(string Sql)
        {
            return LinkInfo.DB.GetSchemaTableForSql(Sql);
        }
        /// <summary>
        /// 获取指定表名的空表结构
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public DataTable GetEmptyTable(string TableName)
        {
            return LinkInfo.DB.GetEmptyTable(TableName);
        }

        #endregion

        #region 获取DataSet的相关方法
        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        public DataSet GetDataSet(string strQuery)
        {
            return LinkInfo.DB.GetDataSet(strQuery);
        }

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        public void GetDataSet(string strQuery, DataSet ds)
        {
            LinkInfo.DB.GetDataSet(strQuery, ds);
        }

        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        public DataSet GetDataSet(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.GetDataSet(cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        public void GetDataSet(DataSet ds, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            LinkInfo.DB.GetDataSet(ds, cmdType, cmdText, cmdParms);
        }
        #endregion

        #region 获取DataReader的相关方法,这里仅提供一个委托
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="TList">泛型集合名</typeparam>
        /// <typeparam name="TValue">泛型类名</typeparam>
        /// <param name="Method">赋值的方法</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        public TList GetEntityList<TList, TValue>(Func<DbDataReader, TValue> Method, string strQuery) where TList : List<TValue>, new()
        {
            return LinkInfo.DB.GetEntityList<TList, TValue>(Method, strQuery);
        }
        /// <summary>
        /// 将数据表中记录转换为具有键值的实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="TDictionary">泛型集合名</typeparam>
        /// <typeparam name="TKey">泛型键值名</typeparam>
        /// <typeparam name="TValue">泛型类名</typeparam>
        /// <param name="KeyField">作为键值的字段名</param>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        public TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, Func<DbDataReader, TValue> Method, string strQuery) where TDictionary : Dictionary<TKey, TValue>, new()
        {
            return LinkInfo.DB.GetEntityDictionary<TDictionary, TKey, TValue>(KeyField,Method, strQuery);
        }
        /// <summary>
        ///  将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="TList">泛型类名</typeparam>
        /// <typeparam name="TValue">泛型值名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的集合</returns>
        public TList GetEntityList<TList, TValue>(Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms) where TList : List<TValue>, new()
        {
            return LinkInfo.DB.GetEntityList<TList, TValue>(Method, cmdType, cmdText, cmdParms);
        }
        /// <summary>
        /// 将数据表中记录转换为具有键值的实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="TDictionary">泛型集合名</typeparam>
        /// <typeparam name="TKey">泛型键值名</typeparam>
        /// <typeparam name="TValue">泛型类名</typeparam>
        /// <param name="KeyField">作为键值的字段名</param>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的集合</returns>
        public TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms) where TDictionary : Dictionary<TKey, TValue>, new()
        {
            return LinkInfo.DB.GetEntityDictionary<TDictionary, TKey, TValue>(KeyField,  Method, cmdType, cmdText, cmdParms);
        }
        /// <summary>
        /// 将数据表中记录转换为具有键值的实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="TDictionary">泛型集合名</typeparam>
        /// <typeparam name="TKey">泛型键值名</typeparam>
        /// <typeparam name="TValue">泛型类名</typeparam>
        /// <param name="KeyField">作为键值的字段名</param>
        /// <param name="KeyFieldDefaultValue">键值类型默认值</param>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        public TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, Func<DbDataReader, TValue> Method, string strQuery) where TDictionary : Dictionary<TKey, TValue>, new()
        {
            return LinkInfo.DB.GetEntityDictionary<TDictionary, TKey, TValue>(KeyField, KeyFieldDefaultValue, Method, strQuery);
        }
        /// <summary>
        /// 将数据表中记录转换为具有键值的实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="TDictionary">泛型集合名</typeparam>
        /// <typeparam name="TKey">泛型键值名</typeparam>
        /// <typeparam name="TValue">泛型类名</typeparam>
        /// <param name="KeyField"></param>
        /// <param name="KeyField">作为键值的字段名</param>
        /// <param name="KeyFieldDefaultValue">键值类型默认值</param>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的集合</returns>
        public TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms) where TDictionary : Dictionary<TKey, TValue>, new()
        {
            return LinkInfo.DB.GetEntityDictionary<TDictionary, TKey, TValue>(KeyField, KeyFieldDefaultValue, Method, cmdType, cmdText, cmdParms);
        }

        #endregion

        #region 获取DataReader的相关方法,这里仅提供一个委托
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        public List<T> GetEntityList<T>(Func<DbDataReader, T> Method, string strQuery)
        {
            return LinkInfo.DB.GetEntityList<T>(Method, strQuery);
        }
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="TKey">泛型键值名</typeparam>
        /// <typeparam name="TValue">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        public Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, Func<DbDataReader, TValue> Method, string strQuery)
        {
            return LinkInfo.DB.GetEntityDictionary<TKey, TValue>(KeyField, Method,strQuery);
        }
        /// <summary>
        ///  将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的集合</returns>
        public List<T> GetEntityList<T>(Func<DbDataReader, T> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.GetEntityList<T>(Method, cmdType, cmdText, cmdParms);
        }
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="TKey">泛型键值名</typeparam>
        /// <typeparam name="TValue">泛型类名</typeparam>
        /// <param name="KeyField">作为键值的字段名</param>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的集合</returns>
        public Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.GetEntityDictionary<TKey, TValue>(KeyField, Method, cmdType, cmdText, cmdParms);
        }
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="TValue">泛型类名</typeparam>
        /// <typeparam name="TKey">泛型键值类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        public Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, Func<DbDataReader, TValue> Method, string strQuery)
        {
            return LinkInfo.DB.GetEntityDictionary<TKey, TValue>(KeyField, KeyFieldDefaultValue, Method, strQuery);
        }
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="TValue">泛型类名</typeparam>
        /// <typeparam name="TKey">泛型键值类名</typeparam>
        /// <param name="KeyField">作为键值的字段名</param>
        /// <param name="KeyFieldDefaultValue">键值类型默认值</param>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的集合</returns>
        public Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.GetEntityDictionary<TKey, TValue>(KeyField, KeyFieldDefaultValue, Method, cmdType, cmdText, cmdParms);
        }

        #endregion

        #region 获取DataReader读取第一条数据集的相关方法,这里仅提供一个委托
        /// <summary>
        /// 获取DataReader的相关方法,获取第一行数据,这里仅提供一个委托
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的实例</returns>
        public T GetEntity<T>(Func<DbDataReader,T> Method, string strQuery)
        {
            return LinkInfo.DB.GetEntity<T>(Method, strQuery);
        }

        /// <summary>
        ///  获取DataReader的相关方法,获取第一行数据,这里仅提供一个委托
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的实例</returns>
        public T GetEntity<T>(Func<DbDataReader, T> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.GetEntity<T>(Method, cmdType, cmdText, cmdParms);
        }
        #endregion

        #region 获取DynamicObj动态对象集合
        /// <summary>
        /// 获取指定SQL语句得到的记录对象
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public DynamicObj GetDynamicObj(string strQuery)
        {
            return LinkInfo.DB.GetDynamicObj(strQuery);
        }
        /// <summary>
        /// 获取指定SQL语句得到的记录对象
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public DynamicObj GetDynamicObj(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.GetDynamicObj(cmdType, cmdText, cmdParms);
        }
        /// <summary>
        /// 获取指定SQL语句得到的记录对象集
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public List<DynamicObj> GetDynamicObjs(string strQuery)
        {
            return LinkInfo.DB.GetDynamicObjs(strQuery);
        }
        /// <summary>
        /// 获取指定SQL语句得到的记录对象集
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public List<DynamicObj> GetDynamicObjs(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.GetDynamicObjs(cmdType, cmdText, cmdParms);
        }
        /// <summary>
        /// 获取指定SQL语句得到的记录对象集
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="page"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public List<DynamicObj> GetDynamicObjsByPage(string cmdText, PageInfo page, List<OrderItem> orders)
        {
            return LinkInfo.DB.GetDynamicObjsByPage(cmdText, page, orders);
        }
        #endregion

        public List<T> GetEntityListByPage<T>(Func<DbDataReader, T> method, string cmdText, PageInfo page, List<OrderItem> orders)
        {
            return LinkInfo.DB.GetEntityListByPage(method, cmdText, page, orders);
        }


        #region 获取树型对象集
        /// <summary>
        /// 获取指定值下的所对子级对象集（仅针对互子关系表）
        /// </summary>
        /// <typeparam name="T">返回的对象元素类型定义</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="ParentField">表示父级字段名</param>
        /// <param name="KeyField">表示主键字段名</param>
        /// <param name="KeyValue">主键字段值</param>
        /// <param name="SelectCommand">获取该表数据的SQL语句，必须包含parent和id字段</param>
        /// <param name="IsIncludeSelf">是否包含本身，即不单只获取下属记录，还包含本身记录</param>
        /// <returns>返回指定所有符合条件的记录对象集，不会层次</returns>
        public List<T> GetAllChilds<T>(Func<DbDataReader, T> Method, string ParentField, string KeyField, object KeyValue, string TableName, bool IsIncludeSelf)
        {
            return LinkInfo.DB.GetAllChilds<T>(Method, ParentField, KeyField, KeyValue, TableName, IsIncludeSelf);
        }
        /// <summary>
        /// 获取指定值下的所对父级对象集（仅针对互子关系表）
        /// </summary>
        /// <typeparam name="T">返回的对象元素类型定义</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="ParentField">表示父级字段名</param>
        /// <param name="KeyField">表示主键字段名</param>
        /// <param name="KeyValue">主键字段值</param>
        /// <param name="SelectCommand">获取该表数据的SQL语句，必须包含parent和id字段</param>
        /// <param name="IsIncludeSelf">是否包含本身，即不单只获取下属记录，还包含本身记录</param>
        /// <returns>返回指定所有符合条件的记录对象集，不会层次</returns>
        public List<T> GetAllParents<T>(Func<DbDataReader, T> Method, string ParentField, string KeyField, object KeyValue, string TableName, bool IsIncludeSelf)
        { 
            return LinkInfo.DB.GetAllParents<T>(Method, ParentField, KeyField, KeyValue, TableName, IsIncludeSelf);
        }
        /// <summary>
        /// 获取指定值下的所对子级对象集（仅针对互子关系表）
        /// </summary>
        /// <typeparam name="T">返回的对象元素类型定义</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="ParentField">表示父级字段名</param>
        /// <param name="KeyField">表示主键字段名</param>
        /// <param name="KeyValue">主键字段值</param>
        /// <param name="SelectCommand">获取该表数据的SQL语句，必须包含parent和id字段</param>
        /// <param name="IsIncludeSelf">是否包含本身，即不单只获取下属记录，还包含本身记录</param>
        /// <returns>返回指定所有符合条件的记录对象集，不会层次</returns>
        public List<T> GetAllChilds<T>(Func<DbDataReader, T> Method, string ParentField, string KeyField, IFilter filter, string TableName)
        {
            return LinkInfo.DB.GetAllChilds<T>(Method, ParentField, KeyField, filter, TableName);
        }
        /// <summary>
        /// 获取指定值下的所对父级对象集（仅针对互子关系表）
        /// </summary>
        /// <typeparam name="T">返回的对象元素类型定义</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="ParentField">表示父级字段名</param>
        /// <param name="KeyField">表示主键字段名</param>
        /// <param name="KeyValue">主键字段值</param>
        /// <param name="SelectCommand">获取该表数据的SQL语句，必须包含parent和id字段</param>
        /// <param name="IsIncludeSelf">是否包含本身，即不单只获取下属记录，还包含本身记录</param>
        /// <returns>返回指定所有符合条件的记录对象集，不会层次</returns>
        public List<T> GetAllParents<T>(Func<DbDataReader, T> Method, string ParentField, string KeyField, IFilter filter, string TableName, bool IsIncludeSelf)
        { 
            return LinkInfo.DB.GetAllParents<T>(Method,ParentField,KeyField,filter,TableName,IsIncludeSelf);
        }

        /// <summary>
        /// 获取通过中间关联表关联取值且关联对象为树型的取本级及所有下级相关联记录集的SQL语句
        /// 如用户信息表 UserInfo  主键字段UserID
        /// 与部门角色关联表 RoleUser 字段：ID,RoleID,UserID;
        /// 部门角色表 主键字段RoleID,父子关联字段为ParentID
        /// 通过这三个表就成了多层树型关联，如果要获取某个角色下所属用户，则可通过这个方式构建获取数据的SQL语句
        /// </summary>
        /// <param name="factSQL">实际取值SQL</param>
        /// <param name="MiddleRelationTable">中间关联表名，如RoleUser</param>
        /// <param name="MiddleRelaField">实际表与关联表关联字段，如表RoleUser中UserID字段</param>
        /// <param name="factField">实际取值表字段，如UserInfo中的UserID</param>
        /// <param name="FactRelationTable">实际要关联的表，如RoleInfo</param>
        /// <param name="FactRelaField">实际要关联的表字段，如表RoleInfo中RoleID</param>
        /// <param name="MiddleRelaFieldForFactRelaField">实际关联表和中间关联表关联的字段，即RoleUser中的RoleID</param>
        /// <param name="FactRelaParentIDField">实际关联表中表示树形的字段，如RoleInfo中的ParentID</param>
        /// <param name="FactRelaFieldValue">实际关联字段的值，可以为一个或多个</param>
        /// <returns>返回取所有条件下的SQL</returns>
        public List<T> GetAllChildRecsByRelation<T>(Func<DbDataReader, T> Method, string factSQL, string MiddleRelationTable, string MiddleRelaField, string factField, string FactRelationTable, string FactRelaField, string MiddleRelaFieldForFactRelaField, string FactRelaParentIDField, object FactRelaFieldValue)
        {
            return LinkInfo.DB.GetAllChildRecsByRelation<T>(Method, factSQL, MiddleRelationTable, MiddleRelaField, factField, FactRelationTable, FactRelaField, MiddleRelaFieldForFactRelaField, FactRelaParentIDField, FactRelaFieldValue);
        }

        /// <summary>
        /// 获取通过中间关联表关联取值且关联对象为树型的取本级及所有下级相关联记录集的SQL语句
        /// 如用户信息表 UserInfo  主键字段UserID
        /// 与部门角色关联表 RoleUser 字段：ID,RoleID,UserID;
        /// 部门角色表 主键字段RoleID,父子关联字段为ParentID
        /// 通过这三个表就成了多层树型关联，如果要获取某个角色下所属用户，则可通过这个方式构建获取数据的SQL语句
        /// </summary>
        /// <param name="factSQL">实际取值SQL</param>
        /// <param name="MiddleRelationTable">中间关联表名，如RoleUser</param>
        /// <param name="MiddleRelaField">实际表与关联表关联字段，如表RoleUser中UserID字段</param>
        /// <param name="factField">实际取值表字段，如UserInfo中的UserID</param>
        /// <param name="FactRelationTable">实际要关联的表，如RoleInfo</param>
        /// <param name="FactRelaField">实际要关联的表字段，如表RoleInfo中RoleID</param>
        /// <param name="MiddleRelaFieldForFactRelaField">实际关联表和中间关联表关联的字段，即RoleUser中的RoleID</param>
        /// <param name="FactRelaParentIDField">实际关联表中表示树形的字段，如RoleInfo中的ParentID</param>
        /// <param name="FactRelaFieldValue">实际关联字段的值，可以为一个或多个</param>
        /// <returns>返回取所有条件下的SQL</returns>
        public List<T> GetAllParentRecsByRelation<T>(Func<DbDataReader, T> Method, string factSQL, string MiddleRelationTable, string MiddleRelaField, string factField, string FactRelationTable, string FactRelaField, string MiddleRelaFieldForFactRelaField, string FactRelaParentIDField, object FactRelaFieldValue)
        {
            return LinkInfo.DB.GetAllParentRecsByRelation<T>(Method, factSQL, MiddleRelationTable, MiddleRelaField, factField, FactRelationTable, FactRelaField, MiddleRelaFieldForFactRelaField, FactRelaParentIDField, FactRelaFieldValue);
        }

        /// <summary>
        /// 获取通过中间关联表关联取值且关联对象为树型的取本级及所有下级相关联记录集的SQL语句
        /// 如用户信息表 UserInfo  主键字段UserID
        /// 与部门角色关联表 RoleUser 字段：ID,RoleID,UserID;
        /// 部门角色表 主键字段RoleID,父子关联字段为ParentID
        /// 通过这三个表就成了多层树型关联，如果要获取某个角色下所属用户，则可通过这个方式构建获取数据的SQL语句
        /// </summary>
        /// <param name="factSQL">实际取值SQL</param>
        /// <param name="MiddleRelationTable">中间关联表名，如RoleUser</param>
        /// <param name="MiddleRelaField">实际表与关联表关联字段，如表RoleUser中UserID字段</param>
        /// <param name="factField">实际取值表字段，如UserInfo中的UserID</param>
        /// <param name="FactRelationTable">实际要关联的表，如RoleInfo</param>
        /// <param name="FactRelaField">实际要关联的表字段，如表RoleInfo中RoleID</param>
        /// <param name="MiddleRelaFieldForFactRelaField">实际关联表和中间关联表关联的字段，即RoleUser中的RoleID</param>
        /// <param name="FactRelaParentIDField">实际关联表中表示树形的字段，如RoleInfo中的ParentID</param>
        /// <param name="FactRelaFieldValue">实际关联字段的值，可以为一个或多个</param>
        /// <returns>返回取所有条件下的SQL</returns>
        public List<T> GetAllChildRecsByRelation<T>(Func<DbDataReader, T> Method,PageInfo pageInfo,List<OrderItem> Orders, string factSQL, string MiddleRelationTable, string MiddleRelaField, string factField, string FactRelationTable, string FactRelaField, string MiddleRelaFieldForFactRelaField, string FactRelaParentIDField, object FactRelaFieldValue)
        {
            return LinkInfo.DB.GetAllChildRecsByRelation<T>(Method, pageInfo, Orders, factSQL, MiddleRelationTable, MiddleRelaField, factField, FactRelationTable, FactRelaField, MiddleRelaFieldForFactRelaField, FactRelaParentIDField, FactRelaFieldValue);
        }

        /// <summary>
        /// 获取通过中间关联表关联取值且关联对象为树型的取本级及所有下级相关联记录集的SQL语句
        /// 如用户信息表 UserInfo  主键字段UserID
        /// 与部门角色关联表 RoleUser 字段：ID,RoleID,UserID;
        /// 部门角色表 主键字段RoleID,父子关联字段为ParentID
        /// 通过这三个表就成了多层树型关联，如果要获取某个角色下所属用户，则可通过这个方式构建获取数据的SQL语句
        /// </summary>
        /// <param name="factSQL">实际取值SQL</param>
        /// <param name="MiddleRelationTable">中间关联表名，如RoleUser</param>
        /// <param name="MiddleRelaField">实际表与关联表关联字段，如表RoleUser中UserID字段</param>
        /// <param name="factField">实际取值表字段，如UserInfo中的UserID</param>
        /// <param name="FactRelationTable">实际要关联的表，如RoleInfo</param>
        /// <param name="FactRelaField">实际要关联的表字段，如表RoleInfo中RoleID</param>
        /// <param name="MiddleRelaFieldForFactRelaField">实际关联表和中间关联表关联的字段，即RoleUser中的RoleID</param>
        /// <param name="FactRelaParentIDField">实际关联表中表示树形的字段，如RoleInfo中的ParentID</param>
        /// <param name="FactRelaFieldValue">实际关联字段的值，可以为一个或多个</param>
        /// <returns>返回取所有条件下的SQL</returns>
        public List<T> GetAllParentRecsByRelation<T>(Func<DbDataReader, T> Method, PageInfo pageInfo, List<OrderItem> Orders, string factSQL, string MiddleRelationTable, string MiddleRelaField, string factField, string FactRelationTable, string FactRelaField, string MiddleRelaFieldForFactRelaField, string FactRelaParentIDField, object FactRelaFieldValue)
        {
            return LinkInfo.DB.GetAllParentRecsByRelation<T>(Method,pageInfo,Orders, factSQL, MiddleRelationTable, MiddleRelaField, factField, FactRelationTable, FactRelaField, MiddleRelaFieldForFactRelaField, FactRelaParentIDField, FactRelaFieldValue);
        }
        #endregion

        #region 获取指定字段值

        /// <summary>
        /// 按指定条件得到一个值
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public object GetScalar(string strQuery)
        {
            return LinkInfo.DB.GetScalar(strQuery);
        }

        /// <summary>
        /// 使用已有连接串执行一个返回值的sql语句   有参SQL
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>返回值</returns>
        public object GetScalar(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.GetScalar(cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// 获取指定SQL返回的行值，取第一行
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public object[] GetRowItemArray(string strQuery)
        {
            return LinkInfo.DB.GetRowItemArray(strQuery);
        }
        /// <summary>
        /// 获取指定SQL返回的行值，取第一行
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public object[] GetRowItemArray(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.GetRowItemArray(cmdType, cmdText, cmdParms);
        }
        public Dictionary<string, TValue> GetRowItemArray<TValue>(string strQuery)
        {
            return LinkInfo.DB.GetRowItemArray<TValue>(strQuery);
        }

        public List<Dictionary<string, TValue>> GetRowItems<TValue>(string strQuery)
        {
            return LinkInfo.DB.GetRowItems<TValue>(strQuery);
        }
        #endregion

        #region 执行单SQL指令
        /// <summary>
        /// 执行一个sql指令
        /// </summary>
        /// <param name="cmd">SQL执行语句</param>
        /// <returns>返回语句执行受影响的记录数</returns>
        public int ExecuteCommand(string cmd)
        {
            return LinkInfo.DB.ExecuteCommand(cmd);
        }
        /// <summary>
        /// 执行一个sql指令
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public int ExecuteCommand(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.ExecuteCommand(cmdType, cmdText, cmdParms);
        }

        #endregion

        #region 批量执行SQL指令
        /// <summary>
        /// 追加一条SQL执行指令
        /// </summary>
        /// <param name="cmd"></param>
        public void AppendCommand(string cmd)
        {
            LinkInfo.DB.AppendCommand(cmd);
        }
        /// <summary>
        /// 追加一条SQL执行指令
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        public void AppendCommand(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            LinkInfo.DB.AppendCommand(cmdType, cmdText, cmdParms);
        }
        /// <summary>
        /// 批量执行指令
        /// </summary>
        public void Execute()
        {
            LinkInfo.DB.Execute();
        }
        /// <summary>
        /// 清空要执行的批量命令
        /// </summary>
        public void ClearCommand()
        {
            LinkInfo.DB.ClearCommand();
        }

        #endregion

        #region 数据库表结构
        /// <summary>
        /// 获取当前数据库所有用户表名集
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public List<string> GetTableNames()
        {
            return LinkInfo.DB.GetTableNames();
        }
        /// <summary>
        /// 获取当前操作数据库所有用户视图名集
        /// </summary>
        /// <returns></returns>
        public List<string> GetViewNames()
        {
            return LinkInfo.DB.GetViewNames();
        }
        /// <summary>
        /// 获取当前操作数据库所有存储过程名
        /// </summary>
        /// <returns></returns>
        public List<string> GetProcedureNames()
        {
            return LinkInfo.DB.GetProcedureNames();
        }
        /// <summary>
        /// 获取当前操作数据库所有用户自定义函数名
        /// </summary>
        /// <returns></returns>
        public List<string> GetFunctionNames()
        {
            return LinkInfo.DB.GetFunctionNames();
        }
        #endregion

        #region 表结构操作相关函数
        /// <summary>
        /// 返回指定表名的表主键列信息
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public DataColumn[] GetPrimaryKeys(string strTableName)
        {
            return LinkInfo.DB.GetPrimaryKeys(strTableName);
        }
        /// <summary>
        /// 获取指定表的父关系集合
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public DataRelationCollection GetParentForeignKeys(string strTableName)
        {
            return LinkInfo.DB.GetParentForeignKeys(strTableName);
        }
        /// <summary>
        /// 获取指定表的子关系集合
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public DataRelationCollection GetChildForeignKeys(string strTableName)
        {
            return LinkInfo.DB.GetChildForeignKeys(strTableName);
        }
        #endregion

        #region 以下为表结构相关
        /// <summary>
        /// 得到库中某表的列结构信息,与GetEmptyTable功能相同
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <returns>表结构信息表(无记录)</returns>
        public DataTable GetSchemaTable(string strTableName)
        {
            return LinkInfo.DB.GetSchemaTable(strTableName);
        }
        #endregion

        #region 以下是以DataTable或DataSet进行数据更新保存
        /// <summary>
        /// 更新数据表,通常用于修改和删除表记录
        /// </summary>
        /// <param name="dt">要提交的DataTable</param>
        /// <param name="TableName">更新的数据表名</param>
        public void UpdateDataTable(DataTable dt, string TableName)
        {
            LinkInfo.DB.UpdateDataTable(dt, TableName);
        }

        /// <summary>
        /// 提交数据表,表必须指定相关的表名,并且数据结构与系统数据完全相同
        /// </summary>
        /// <param name="dt"></param>
        public void UpdateDataTable(DataTable dt)
        {
            LinkInfo.DB.UpdateDataTable(dt);
        }
        #endregion

        #region 提交DataSet至数据库
        /// <summary>
        /// 提交数据集到数据库中
        /// </summary>
        /// <param name="dt"></param>
        public void UpdateDataSet(DataSet ds)
        {
            LinkInfo.DB.UpdateDataSet(ds);
        }
        #endregion

        #region 当前数据库驱动语法函数库
        public IFun Function { get { return LinkInfo.DB.Function; } }
        #endregion  
        
        #region 读取一条或多条记录相关方法

        /// <summary>
        /// 在事务内执行指定方法
        /// </summary>
        /// <param name="Method"></param>
        /// <returns></returns>
        public JsonReturn RunTransaction(Action Method)
        {
            return LinkInfo.DB.RunTransaction(Method);
        }

        /// <summary>
        /// 在事务内执行指定方法
        /// </summary>
        /// <param name="Method"></param>
        /// <returns></returns>
        public JsonReturn RunTransaction(Func<JsonReturn> Method)
        {
            return LinkInfo.DB.RunTransaction(Method);
        }

        /// <summary>
        /// 读取一条记录,读取成功，返回true,否则为false
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public bool Read(Action<DbDataReader> Method, string strQuery)
        {
            return Read(Method, CommandType.Text, strQuery);
        }
        /// <summary>
        /// 读取一条记录,读取成功，返回true,否则为false
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public bool Read(Action<DbDataReader> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.Read(Method, cmdType, cmdText, cmdParms);
        }


        /// <summary>
        /// 读取一条记录,读取成功，返回true,否则为false
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public bool ReadMutil(Action<DbDataReader> Method, string strQuery)
        {
            return ReadMutil(Method, CommandType.Text, strQuery);
        }
        /// <summary>
        /// 读取一条记录,读取成功，返回true,否则为false
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public bool ReadMutil(Action<DbDataReader> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return LinkInfo.DB.ReadMutil(Method, cmdType, cmdText, cmdParms);
        }
        #endregion

        #region 保存指定对象

        #endregion









        /// <summary>
        /// 确定指定的SQL语句是否有记录，注意，这个SQL语句必须有标准的检索数据语句
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public bool Exists(string strQuery)
        {
            //先移除order by
            string strSQL = strQuery;
            strSQL = System.Text.RegularExpressions.Regex.Replace(strSQL, "(.*)?order\\s*by.* ", "$1 ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return LinkInfo.DB.Exists(strSQL);
        }

        #region 泛对象编辑相关方法，即对于相关object对象

        #region 新增单个实例对象
        /// <summary>
        /// 将指定的对象作为一条记录保存到指定数据表中:新增
        /// 注：
        /// 1、对象至少要有一个可写属性
        /// 2、属性的名称默认为数据表对应字段名
        /// 3、如果保存的为对象集，则请使用SaveObjectList方法
        /// 4、对象各属性值必须事先要设置好
        /// </summary>
        /// <param name="entity">要保存的对象实例</param>
        /// <param name="TableName">对应要保存的数据表</param>
        /// <param name="KeyPropertyNames">该对像主键属性，至少要设置一个字键属性，且该属性为对象属性</param>
        /// <returns></returns>
        public bool SaveNewObject(object entity, string TableName)
        {
            return SaveNewObject(entity, TableName, new Dictionary<string, string>());
        }
        /// <summary>
        /// 将指定的对象作为一条记录保存到指定数据表中:新增
        /// 注：
        /// 1、对象至少要有一个可写属性
        /// 2、属性的名称默认为数据表对应字段名
        /// 3、如果保存的为对象集，则请使用SaveObjectList方法
        /// 4、对象各属性值必须事先要设置好
        /// </summary>
        /// <param name="entity">要保存的对象实例</param>
        /// <param name="TableName">对应要保存的数据表</param>
        /// <param name="Prop2FieldMap">属性名与字段名映射关联，主要是针对个性的属性</param>
        /// <param name="KeyPropertyNames">该对像主键属性，至少要设置一个字键属性，且该属性为对象属性</param>
        /// <returns></returns>
        public bool SaveNewObject(object entity, string TableName, Dictionary<string, string> Prop2FieldMap)
        {
            return LinkInfo.DB.SaveNewObject(entity, TableName, Prop2FieldMap);
        }
        #endregion

        #region 批量保存多个实例对象(可以是新增或修改，系统根据键值来原确认是新增还是修改)
        /// <summary>
        /// 批量新增保存对象集
        /// </summary>
        /// <param name="entitys">要保存的对象集</param>
        /// <param name="type">对象类型</param>
        /// <param name="TableName">保存记录表名</param>
        /// <returns></returns>
        public bool SaveNewObjectList<T>(List<T> entitys, string TableName)
        {
            return SaveNewObjectList<T>(entitys, TableName, new Dictionary<string, string>());
        }
        /// <summary>
        /// 批量新增保存对象集
        /// </summary>
        /// <param name="entitys">要保存的对象集</param>
        /// <param name="type">对象类型</param>
        /// <param name="TableName">保存记录表名</param>
        /// <param name="Prop2FieldMap">个性属性与字段映身关系</param>
        /// <returns></returns>
        public bool SaveNewObjectList<T>(List<T> entitys, string TableName, Dictionary<string, string> Prop2FieldMap)
        {
            return LinkInfo.DB.SaveNewObjectList<T>(entitys, TableName, Prop2FieldMap);
        }


        /// <summary>
        /// 将指定的对象作为一条记录保存到指定数据表中:新增
        /// 注：
        /// 1、对象至少要有一个可写属性
        /// 2、属性的名称默认为数据表对应字段名
        /// 3、如果保存的为对象集，则请使用SaveObjectList方法
        /// 4、对象各属性值必须事先要设置好
        /// </summary>
        /// <param name="entity">要保存的对象实例</param>
        /// <param name="TableName">对应要保存的数据表</param>
        /// <param name="KeyPropertyNames">该对像主键属性，至少要设置一个字键属性，且该属性为对象属性</param>
        /// <returns></returns>
        public bool SaveObjectList<T>(List<T> entitys, string TableName, params string[] KeyPropertyNames)
        {
            return SaveObjectList<T>(entitys, TableName, new Dictionary<string, string>(), KeyPropertyNames);
        }
        /// <summary>
        /// 将指定的对象作为一条记录保存到指定数据表中:新增
        /// 注：
        /// 1、对象至少要有一个可写属性
        /// 2、属性的名称默认为数据表对应字段名
        /// 3、如果保存的为对象集，则请使用SaveObjectList方法
        /// 4、对象各属性值必须事先要设置好
        /// </summary>
        /// <param name="entity">要保存的对象实例</param>
        /// <param name="TableName">对应要保存的数据表</param>
        /// <param name="Prop2FieldMap">属性名与字段名映射关联，主要是针对个性的属性</param>
        /// <param name="KeyPropertyNames">该对像主键属性，至少要设置一个字键属性，且该属性为对象属性</param>
        /// <returns></returns>
        public bool SaveObjectList<T>(List<T> entitys, string TableName, Dictionary<string, string> Prop2FieldMap, params string[] KeyPropertyNames)
        {
            return LinkInfo.DB.SaveObjectList<T>(entitys, TableName, Prop2FieldMap, KeyPropertyNames);
        }
        #endregion

        #region 更新修改单个实例对象
        /// <summary>
        /// 将指定的对象作为一条记录保存到指定数据表中:新增
        /// 注：
        /// 1、对象至少要有一个可写属性
        /// 2、属性的名称默认为数据表对应字段名
        /// 3、如果保存的为对象集，则请使用SaveObjectList方法
        /// 4、对象各属性值必须事先要设置好
        /// </summary>
        /// <param name="entity">要保存的对象实例</param>
        /// <param name="TableName">对应要保存的数据表</param>
        /// <param name="KeyPropertyNames">该对像主键属性，至少要设置一个字键属性，且该属性为对象属性</param>
        /// <returns></returns>
        public bool SaveObject(object entity, string TableName, params string[] KeyPropertyNames)
        {
            return SaveObject(entity, TableName, new Dictionary<string, string>(), KeyPropertyNames);
        }
        /// <summary>
        /// 将指定的对象作为一条记录保存到指定数据表中:新增
        /// 注：
        /// 1、对象至少要有一个可写属性
        /// 2、属性的名称默认为数据表对应字段名
        /// 3、如果保存的为对象集，则请使用SaveObjectList方法
        /// 4、对象各属性值必须事先要设置好
        /// </summary>
        /// <param name="entity">要保存的对象实例</param>
        /// <param name="TableName">对应要保存的数据表</param>
        /// <param name="Prop2FieldMap">属性名与字段名映射关联，主要是针对个性的属性</param>
        /// <param name="KeyPropertyNames">该对像主键属性，至少要设置一个字键属性，且该属性为对象属性</param>
        /// <returns></returns>
        public bool SaveObject(object entity, string TableName, Dictionary<string, string> Prop2FieldMap, params string[] KeyPropertyNames)
        {
            return LinkInfo.DB.SaveObject(entity, TableName, Prop2FieldMap, KeyPropertyNames);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="TableName"></param>
        /// <param name="KeyPropertyName"></param>
        /// <param name="UpdatePropertyNames"></param>
        /// <returns></returns>
        public bool UpdateObject(object entity, string TableName, string KeyPropertyName, params string[] UpdatePropertyNames)
        {
            return UpdateObject(entity, TableName, KeyPropertyName, UpdatePropertyNames);
        }

        public bool UpdateObject(object entity, string TableName, string[] KeyPropertyNames, params string[] UpdatePropertyNames)
        {
            return UpdateObject(entity, TableName, new Dictionary<string, string>(), KeyPropertyNames, UpdatePropertyNames);
        }
        public bool UpdateObject(object entity, string TableName, Dictionary<string, string> Prop2FieldMap, string[] KeyPropertyNames, string[] UpdatePropertyNames)
        {
            return LinkInfo.DB.UpdateObject(entity, TableName, Prop2FieldMap, KeyPropertyNames, UpdatePropertyNames);
        }



        #endregion

        #region 新增保存父子表，即主表和明细表
        /// <summary>
        /// 新增保存父子表，即主表和明细表
        /// </summary>
        /// <param name="MDMaps"></param>
        /// <returns></returns>
        public bool SaveNewMasterAndDetail(MasterDetailMapInfo MDMaps)
        {
            return LinkInfo.DB.SaveNewMasterAndDetail(MDMaps);
        }

        #endregion

        #region 修改保存父子表，即同时保存父子明细表
        public bool SaveMasterAndDetail(MasterDetailMapInfo MDMaps)
        {
            return LinkInfo.DB.SaveMasterAndDetail(MDMaps);
        }

        #endregion
        #endregion

        #region 泛对象读取相关方法

        public List<T> GetDetailRecords<T>(DetailMapInfo child, MasterDetailMapInfo master) where T : new()
        {
            return LinkInfo.DB.GetDetailRecords<T>(child, master);
        }
        /// <summary>
        /// 根据主表对象填充子表对像集（从数据库中获取最新的）
        /// </summary>
        /// <param name="child"></param>
        /// <param name="master"></param>
        private List<DynamicObj> GetDetailRecords(DetailMapInfo child, MasterDetailMapInfo master)
        {
            return GetDetailRecords(child, master);
        }
        /// <summary>
        /// 根据指定主键字段值获对记录对象实例
        /// </summary>
        /// <typeparam name="T">返回的对象类型</typeparam>
        /// <param name="TableName">数据表名</param>
        /// <param name="KeyField">主键字段</param>
        /// <param name="KeyFieldValue">主键字段值</param>
        /// <returns></returns>
        public T GetObject<T>(string TableName, string KeyField, object KeyFieldValue) where T : new()
        {
            return LinkInfo.DB.GetObject<T>(TableName, KeyField, KeyFieldValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">返回的对象类型</typeparam>
        /// <param name="TableName">数据表名</param>
        /// <param name="KeyFieldValues">主键字段信息</param>
        /// <returns></returns>
        public T GetObject<T>(string TableName, Dictionary<string, object> KeyFieldValues) where T : new()
        {
            return LinkInfo.DB.GetObject<T>(TableName, KeyFieldValues);
        }


        public List<T> GetObjectList<T>(string sql, Action<DbDataReader> OtherSetMethod = null) where T : new()
        {
            return LinkInfo.DB.GetObjectList<T>(sql, OtherSetMethod);
        }
        public List<T> GetObjectList<T>(string sql, PageInfo page, List<OrderItem> orderbys, Action<DbDataReader> OtherSetMethod = null) where T : new()
        {
            return LinkInfo.DB.GetObjectList<T>(sql,page,orderbys,OtherSetMethod);
        }

        #endregion
    }
}
