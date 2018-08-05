using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;

namespace RS.Data
{
    /// <summary>
    /// 数据库驱动适配器接口
    /// 注：对于自定义的外部数据访问对象，构造函数必须要有一个参数：ConnectionString
    /// </summary>
    public interface IDbContext
    {
        
        #region 连接及事务
        /// <summary>
        /// 获取当前数据链接
        /// </summary>
        DbConnection Connection
        {
            get;
        }
        /// <summary>
        /// 获取当前链接字符串
        /// </summary>
        /// <returns></returns>
        string GetDbConnectionString();

        /// <summary>
        /// 是否已在事务中
        /// </summary>
        bool IsInTransaction
        {
            get;
        }

        DbTransaction Trans
        {
            get;
        }
        void BeginTransaction();
        void BeginTransaction(System.Data.IsolationLevel level);
        void Commit();
        void Rollback();

        #endregion

        #region 获取DataTable相关方法
        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        DataTable GetDataTable(string strQuery);

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        void GetDataTable(string strQuery, DataTable dt);

        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        DataTable GetDataTable(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        void GetDataTable(DataTable dt, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);
        
        /// <summary>
        /// 获取指定SQL产生的数据结构,无记录
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        DataTable GetSchemaTableForSql(string Sql);
        /// <summary>
        /// 获取指定表名的空表结构
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        DataTable GetEmptyTable(string TableName);
        
        #endregion

        #region 获取DataSet的相关方法
        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        DataSet GetDataSet(string strQuery);

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        void GetDataSet(string strQuery, DataSet ds);

        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        DataSet GetDataSet(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        void GetDataSet(DataSet ds, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);        
        #endregion

        #region 获取DataReader的相关方法,这里仅提供一个委托
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="B">泛型集合名</typeparam>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">赋值的方法</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        TList GetEntityList<TList, TValue>(Func<DbDataReader,TValue> Method, string strQuery) where TList : List<TValue>, new();
        /// <summary>
        /// 将数据表中记录转换为具有键值的实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="B">泛型集合名</typeparam>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, Func<DbDataReader, TValue> Method, string strQuery) where TDictionary : Dictionary<TKey, TValue>, new();
        /// <summary>
        /// 将数据表中记录转换为具有键值的实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="B">泛型集合名</typeparam>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, Func<DbDataReader, TValue> Method, string strQuery) where TDictionary : Dictionary<TKey, TValue>, new();
        /// <summary>
        ///  将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的集合</returns>
        TList GetEntityList<TList, TValue>(Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms) where TList : List<TValue>, new();
        /// <summary>
        /// 将数据表中记录转换为具有键值的实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <typeparam name="B">泛型集合名</typeparam>
        /// <param name="KeyField"></param>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的集合</returns>
        TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms) where TDictionary : Dictionary<TKey, TValue>, new();
        /// <summary>
        /// 将数据表中记录转换为具有键值的实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <typeparam name="B">泛型集合名</typeparam>
        /// <param name="KeyField"></param>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的集合</returns>
        TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms) where TDictionary : Dictionary<TKey, TValue>, new();

        #endregion

        #region 分页呈现列表
        List<T> GetEntityListByPage<T>(Func<DbDataReader, T> Method, string cmdText, PageInfo page, List<OrderItem> orders);
        #endregion




        #region 获取DataReader的相关方法,这里仅提供一个委托
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        List<T> GetEntityList<T>(Func<DbDataReader, T> Method, string strQuery);
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="B">泛型键值类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, Func<DbDataReader, TValue> Method, string strQuery);        
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="B">泛型键值类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, Func<DbDataReader, TValue> Method, string strQuery);
        /// <summary>
        ///  将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的集合</returns>
        List<T> GetEntityList<T>(Func<DbDataReader, T> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <typeparam name="B">泛型键值类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的集合</returns>
        Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);        
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <typeparam name="B">泛型键值类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的集合</returns>
        Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        #endregion

        #region 获取DataReader读取第一条数据集的相关方法,这里仅提供一个委托
        /// <summary>
        /// 获取DataReader的相关方法,获取第一行数据,这里仅提供一个委托
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的实例</returns>
        T GetEntity<T>(Func<DbDataReader, T> Method, string strQuery);

        /// <summary>
        ///  获取DataReader的相关方法,获取第一行数据,这里仅提供一个委托
        /// </summary>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">命令脚本</param>
        /// <param name="cmdParms">命令参数</param>
        /// <returns>指定类型的实例</returns>
        T GetEntity<T>(Func<DbDataReader, T> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);
        #endregion

        #region 获取DynamicObj动态对象集合
        DynamicObj GetDynamicObj(string strQuery);
        DynamicObj GetDynamicObj(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        List<DynamicObj> GetDynamicObjs(string strQuery);
        List<DynamicObj> GetDynamicObjs(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        List<DynamicObj> GetDynamicObjsByPage(string cmdText, PageInfo page, List<OrderItem> orders);
        #endregion




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
        List<T> GetAllChilds<T>(Func<DbDataReader, T> Method, string ParentField, string KeyField, object KeyValue, string TableName, bool IsIncludeSelf);
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
        List<T> GetAllParents<T>(Func<DbDataReader, T> Method, string ParentField, string KeyField, object KeyValue, string TableName, bool IsIncludeSelf);
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
        List<T> GetAllChilds<T>(Func<DbDataReader, T> Method, string ParentField, string KeyField, IFilter filter, string TableName);
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
        List<T> GetAllParents<T>(Func<DbDataReader, T> Method, string ParentField, string KeyField, IFilter filter, string TableName, bool IsIncludeSelf);


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
        List<T> GetAllChildRecsByRelation<T>(Func<DbDataReader, T> Method, string factSQL, string MiddleRelationTable, string MiddleRelaField, string factField, string FactRelationTable, string FactRelaField, string MiddleRelaFieldForFactRelaField, string FactRelaParentIDField, object FactRelaFieldValue);

        /// <summary>
        /// 获取通过中间关联表关联取值且关联对象为树型的取本级及所有上级相关联记录集的SQL语句
        /// 如用户信息表 UserInfo  主键字段UserID
        /// 与部门角色关联表 RoleUser 字段：ID,RoleID,UserID;
        /// 部门角色表 主键字段RoleID,父子关联字段为ParentID
        /// 通过这三个表就成了多层树型关联，如果要获取某个角色所有上级所属用户，则可通过这个方式构建获取数据的SQL语句
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
        List<T> GetAllParentRecsByRelation<T>(Func<DbDataReader, T> Method, string factSQL, string MiddleRelationTable, string MiddleRelaField, string factField, string FactRelationTable, string FactRelaField, string MiddleRelaFieldForFactRelaField, string FactRelaParentIDField, object FactRelaFieldValue);
     
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
        List<T> GetAllChildRecsByRelation<T>(Func<DbDataReader, T> Method, PageInfo pageInfo, List<OrderItem> Sorts, string factSQL, string MiddleRelationTable, string MiddleRelaField, string factField, string FactRelationTable, string FactRelaField, string MiddleRelaFieldForFactRelaField, string FactRelaParentIDField, object FactRelaFieldValue);
        
        /// <summary>
        /// 获取通过中间关联表关联取值且关联对象为树型的取本级及所有上级相关联记录集的SQL语句
        /// 如用户信息表 UserInfo  主键字段UserID
        /// 与部门角色关联表 RoleUser 字段：ID,RoleID,UserID;
        /// 部门角色表 主键字段RoleID,父子关联字段为ParentID
        /// 通过这三个表就成了多层树型关联，如果要获取某个角色所有上级所属用户，则可通过这个方式构建获取数据的SQL语句
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
        List<T> GetAllParentRecsByRelation<T>(Func<DbDataReader, T> Method, PageInfo pageInfo, List<OrderItem> Sorts, string factSQL, string MiddleRelationTable, string MiddleRelaField, string factField, string FactRelationTable, string FactRelaField, string MiddleRelaFieldForFactRelaField, string FactRelaParentIDField, object FactRelaFieldValue);
        
        #endregion

        #region 获取指定字段值
        /// <summary>
        /// 是否存在记录
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        bool Exists(string strQuery);

        /// <summary>
        /// 按指定条件得到一个值
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        object GetScalar(string strQuery);
        /// <summary>
        /// 使用已有连接串执行一个返回值的sql语句   有参SQL
        /// </summary>
        /// <param name="cmdType">命令类型</param>
        /// <param name="cmdText">语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>返回值</returns>
        object GetScalar(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        T GetField<T>(string strQuery);
        T GetField<T>(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        /// <summary>
        /// 获取指定SQL返回的行值，取第一行
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        object[] GetRowItemArray(string strQuery);
        Dictionary<string,TValue> GetRowItemArray<TValue>(string strQuery);
        List<Dictionary<string, TValue>> GetRowItems<TValue>(string strQuery);
        /// <summary>
        /// 获取指定SQL返回的行值，取第一行
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        object[] GetRowItemArray(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        #endregion

        #region 执行单SQL指令
        /// <summary>
        /// 执行一个sql指令
        /// </summary>
        /// <param name="cmd">SQL执行语句</param>
        /// <returns>返回语句执行受影响的记录数</returns>
        int ExecuteCommand(string cmd);
        /// <summary>
        /// 执行一个sql指令
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        int ExecuteCommand(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);

        int ExecuteCommand(SqlBuilder builder);

        #endregion

        #region 批量执行SQL指令
        /// <summary>
        /// 追加一条SQL执行指令
        /// </summary>
        /// <param name="cmd"></param>
        void AppendCommand(string cmd);
        /// <summary>
        /// 追加一条SQL执行指令
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        void AppendCommand(CommandType cmdType, string cmdText, params DbParameter[] cmdParms);
        /// <summary>
        /// 批量执行指令
        /// </summary>
        void Execute();
        /// <summary>
        /// 清空要执行的批量命令
        /// </summary>
        void ClearCommand();

        #endregion

        #region 数据库表结构
        /// <summary>
        /// 获取当前数据库所有用户表名集
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        List<string> GetTableNames();
        /// <summary>
        /// 获取当前操作数据库所有用户视图名集
        /// </summary>
        /// <returns></returns>
        List<string> GetViewNames();
        /// <summary>
        /// 获取当前操作数据库所有存储过程名
        /// </summary>
        /// <returns></returns>
        List<string> GetProcedureNames();
        /// <summary>
        /// 获取当前操作数据库所有用户自定义函数名
        /// </summary>
        /// <returns></returns>
        List<string> GetFunctionNames();
        #endregion

        #region 表结构操作相关函数
        /// <summary>
        /// 返回指定表名的表主键列信息
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        DataColumn[] GetPrimaryKeys(string strTableName);
        /// <summary>
        /// 获取指定表的父关系集合
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        DataRelationCollection GetParentForeignKeys(string strTableName);
        /// <summary>
        /// 获取指定表的子关系集合
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        DataRelationCollection GetChildForeignKeys(string strTableName);
        #endregion

        #region 以下为表结构相关
        /// <summary>
        /// 得到库中某表的列结构信息,与GetEmptyTable功能相同
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <returns>表结构信息表(无记录)</returns>
        DataTable GetSchemaTable(string strTableName);
        #endregion

        #region 以下是以DataTable或DataSet进行数据更新保存
        /// <summary>
        /// 更新数据表,通常用于修改和删除表记录
        /// </summary>
        /// <param name="dt">要提交的DataTable</param>
        /// <param name="TableName">更新的数据表名</param>
        void UpdateDataTable(DataTable dt, string TableName);

        /// <summary>
        /// 提交数据表,表必须指定相关的表名,并且数据结构与系统数据完全相同
        /// </summary>
        /// <param name="dt"></param>
        void UpdateDataTable(DataTable dt);
        #endregion

        #region 提交DataSet至数据库
        /// <summary>
        /// 提交数据集到数据库中
        /// </summary>
        /// <param name="dt"></param>
        void UpdateDataSet(DataSet ds);
        #endregion

        #region 当前数据库驱动语法函数库
        /// <summary>
        /// 当前数据库驱动下的函数库
        /// </summary>
        IFun Function { get; }
        #endregion

        #region 读取一条或多条记录相关方法



        /// <summary>
        /// 读取一条记录,读取成功，返回true,否则为false
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        bool Read(Action<DbDataReader> Method, string strQuery);
        /// <summary>
        /// 读取一条记录,读取成功，返回true,否则为false
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        bool Read(Action<DbDataReader> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);


        /// <summary>
        /// 读取一条记录,读取成功，返回true,否则为false
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        bool ReadMutil(Action<DbDataReader> Method, string strQuery);
        /// <summary>
        /// 读取一条记录,读取成功，返回true,否则为false
        /// </summary>
        /// <param name="Method"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        bool ReadMutil(Action<DbDataReader> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms);
        #endregion


        #region 事务相关方法
        /// <summary>
        /// 在事务内执行指定方法
        /// </summary>
        /// <param name="Method"></param>
        /// <returns></returns>
        JsonReturn RunTransaction(Action Method);
        JsonReturn RunTransaction(Func<JsonReturn> Method);

        #endregion


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
        bool SaveNewObject(object entity, string TableName);
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
        bool SaveNewObject(object entity, string TableName, Dictionary<string, string> Prop2FieldMap);
        #endregion

        #region 批量保存多个实例对象(可以是新增或修改，系统根据键值来原确认是新增还是修改)
        /// <summary>
        /// 批量新增保存对象集
        /// </summary>
        /// <param name="entitys">要保存的对象集</param>
        /// <param name="type">对象类型</param>
        /// <param name="TableName">保存记录表名</param>
        /// <returns></returns>
        bool SaveNewObjectList<T>(List<T> entitys, string TableName);
        /// <summary>
        /// 批量新增保存对象集
        /// </summary>
        /// <param name="entitys">要保存的对象集</param>
        /// <param name="type">对象类型</param>
        /// <param name="TableName">保存记录表名</param>
        /// <param name="Prop2FieldMap">个性属性与字段映身关系</param>
        /// <returns></returns>
        bool SaveNewObjectList<T>(List<T> entitys, string TableName, Dictionary<string, string> Prop2FieldMap);


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
        bool SaveObjectList<T>(List<T> entitys, string TableName, params string[] KeyPropertyNames);
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
        bool SaveObjectList<T>(List<T> entitys, string TableName, Dictionary<string, string> Prop2FieldMap, params string[] KeyPropertyNames);
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
        bool SaveObject(object entity, string TableName, params string[] KeyPropertyNames);
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
        bool SaveObject(object entity, string TableName, Dictionary<string, string> Prop2FieldMap, params string[] KeyPropertyNames);

        bool UpdateObject(object entity, string TableName, string KeyPropertyName, params string[] UpdatePropertyNames);
        bool UpdateObject(object entity, string TableName,string[] KeyPropertyNames,params string[] UpdatePropertyNames);
        bool UpdateObject(object entity, string TableName, Dictionary<string, string> Prop2FieldMap, string[] KeyPropertyNames, string[] UpdatePropertyNames);


        #endregion

        #region 新增保存父子表，即主表和明细表
        /// <summary>
        /// 新增保存父子表，即主表和明细表
        /// </summary>
        /// <param name="MDMaps"></param>
        /// <returns></returns>
        bool SaveNewMasterAndDetail(MasterDetailMapInfo MDMaps);


        #endregion

        #region 修改保存父子表，即同时保存父子明细表
        bool SaveMasterAndDetail(MasterDetailMapInfo MDMaps);


        #endregion
        #endregion

        #region 泛对象读取相关方法

        List<T> GetDetailRecords<T>(DetailMapInfo child, MasterDetailMapInfo master) where T : new();

        /// <summary>
        /// 根据指定主键字段值获对记录对象实例
        /// </summary>
        /// <typeparam name="T">返回的对象类型</typeparam>
        /// <param name="TableName">数据表名</param>
        /// <param name="KeyField">主键字段</param>
        /// <param name="KeyFieldValue">主键字段值</param>
        /// <returns></returns>
        T GetObject<T>(string TableName, string KeyField, object KeyFieldValue) where T : new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">返回的对象类型</typeparam>
        /// <param name="TableName">数据表名</param>
        /// <param name="KeyFieldValues">主键字段信息</param>
        /// <returns></returns>
        T GetObject<T>(string TableName, Dictionary<string, object> KeyFieldValues) where T : new();

        List<T> GetObjectList<T>(string sql, Action<DbDataReader> OtherSetMethod = null) where T : new();
        List<T> GetObjectList<T>(string sql, PageInfo page, List<OrderItem> orderbys, Action<DbDataReader> OtherSetMethod = null) where T : new();

        #endregion
    }
}
