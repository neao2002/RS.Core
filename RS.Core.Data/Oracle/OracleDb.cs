using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OracleClient;
using System.Data.Common;
using System.Data;
using RS.Lib.Filter;

namespace RS.Lib.Data
{
    /// <summary>
    /// 针对Oracle的数据驱动访问对象
    /// </summary>
    internal class OracleDb : IDbDriver
    {
        #region 字段定义
        private OracleTransaction trans = null;
        protected OracleConnection m_conn;
        public static string ConnectString;
        private string connString = "";
        #endregion

        #region 构造函数
        public OracleDb(string strConnect)
        {
            m_conn = new OracleConnection(strConnect);
            connString = strConnect;
        }
        public OracleDb(OracleConnection cnn)
        {
            m_conn = cnn;
            connString = cnn.ConnectionString;
        }
        public OracleDb(OracleConnection cnn,OracleTransaction mytran)
        {
            m_conn = cnn;
            connString = cnn.ConnectionString;
            trans = mytran;
        }
        #endregion

        #region 连接及事务
        /// <summary>
        /// 获取当前数据链接
        /// </summary>
        public DbConnection Connection
        {
            get { return m_conn; }
        }

        /// <summary>
        /// 打开数据库连接对象
        /// </summary>
        protected void Open()
        {
            if (trans == null) m_conn.Open();
        }

        protected void Close()
        {
            if (trans == null) m_conn.Close();
        }


        /// <summary>
        /// 获取当前链接字符串
        /// </summary>
        /// <returns></returns>
        public string GetDbConnectionString()
        {
            return connString;
        }

        /// <summary>
        /// 是否已在事务中
        /// </summary>
        public bool IsInTransaction
        {
            get { return trans != null; }
        }
        /// <summary>
        /// 获取当前事务处理对象
        /// </summary>
        public DbTransaction Trans
        {
            get { return trans; }
        }
        /// <summary>
        /// 开始事务处理
        /// </summary>
        public void BeginTransaction()
        {
            //如果已经开始了事务，就不再开始。
            if (this.trans == null)
            {
                bool isOpen = false;
                try
                {
                    switch (m_conn.State)
                    {
                        case ConnectionState.Broken:
                            m_conn.Close();
                            break;
                        case ConnectionState.Connecting:
                            isOpen = true;
                            break;
                        case ConnectionState.Executing:
                            isOpen = true;
                            break;
                        case ConnectionState.Fetching:
                            isOpen = true;
                            break;
                        case ConnectionState.Open:
                            isOpen = true;
                            break;
                    }
                }
                catch { }
                if (!isOpen) m_conn.Open();
                trans = m_conn.BeginTransaction();
            }
        }
        /// <summary>
        /// 开始事务处理
        /// </summary>
        /// <param name="level">
        /// 指定连接的事务锁定行为
        /// </param>
        public void BeginTransaction(IsolationLevel level)
        {
            //如果已经开始了事务，就不再开始。
            if (this.trans == null)
            {
                bool isOpen = false;
                try
                {
                    switch (m_conn.State)
                    {
                        case ConnectionState.Broken:
                            m_conn.Close();
                            break;
                        case ConnectionState.Connecting:
                            isOpen = true;
                            break;
                        case ConnectionState.Executing:
                            isOpen = true;
                            break;
                        case ConnectionState.Fetching:
                            isOpen = true;
                            break;
                        case ConnectionState.Open:
                            isOpen = true;
                            break;
                    }
                }
                catch { }
                if (!isOpen) m_conn.Open();
                trans = m_conn.BeginTransaction(level);
            }
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            if (cmdList.HasElement())
            {
                Exception ex = new Exception("Command not Execute");
                Loger.Debug(ex);
                throw ex;
            }


            if (trans != null) trans.Commit();
            if (trans != null) m_conn.Close();
            trans = null;
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void Rollback()
        {
            if (trans != null) trans.Rollback();
            if (trans != null) m_conn.Close();
            trans = null;
        }

        #endregion

        #region 获取DataTable相关方法
        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        public DataTable GetDataTable(string strQuery)
        {
            Open();
            DataTable ds = null;
            try
            {
                OracleDataAdapter adapter = new OracleDataAdapter(strQuery, m_conn);
                //adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                if (this.trans != null)
                    adapter.SelectCommand.Transaction = this.trans;

                if (adapter.SelectCommand != null)
                    adapter.SelectCommand.CommandTimeout = 0;

                ds = new DataTable();
                adapter.Fill(ds);
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return ds;
        }

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        public void GetDataTable(string strQuery, DataTable dt)
        {
            Open();
            try
            {
                OracleDataAdapter adapter = new OracleDataAdapter(strQuery, m_conn);
                //adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                if (this.trans != null)
                    adapter.SelectCommand.Transaction = this.trans;

                if (adapter.SelectCommand != null)
                    adapter.SelectCommand.CommandTimeout = 0;

                adapter.Fill(dt);
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
        }


        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        public DataTable GetDataTable(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            Open();

            DataTable ds = new DataTable();
            try
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                //adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                adapter.Fill(ds);
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }

            return ds;
        }

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        public void GetDataTable(DataTable dt, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            Open();

            try
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                //adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                adapter.Fill(dt);
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 获取指定SQL产生的数据结构,无记录
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        public DataTable GetSchemaTableForSql(string Sql)
        {
            return GetDataTable("SET FMTONLY ON;" + Sql + ";SET FMTONLY OFF;");
        }
        /// <summary>
        /// 获取指定表名的空表结构
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public DataTable GetEmptyTable(string TableName)
        {
            string sql = string.Format("select * from {0} where 1=2", Function.ConvertTable(TableName));
            return GetDataTable(sql);
        }

        #endregion

        #region 获取DataSet的相关方法
        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        public DataSet GetDataSet(string strQuery)
        {
            return GetDataSet(CommandType.Text, strQuery);
        }

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        public void GetDataSet(string strQuery, DataSet ds)
        {
            GetDataSet(ds, CommandType.Text, strQuery);
        }

        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        public DataSet GetDataSet(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            Open();

            DataSet ds = new DataSet();
            try
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                OracleDataAdapter adapter = new OracleDataAdapter(cmd);

                adapter.Fill(ds);
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }

            return ds;
        }

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        public void GetDataSet(DataSet ds, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            Open();

            try
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                OracleDataAdapter adapter = new OracleDataAdapter(cmd);

                adapter.Fill(ds);
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Error(e);
                throw new Exception("SQL Update Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
        }
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
        public TList GetEntityList<TList, TValue>(DelegateOfDbDataReadAssign<TValue> Method, string strQuery) where TList : List<TValue>, new()
        {
            if (Method == null) return null;
            TList list = new TList();
            Open();
            try
            {
                OracleCommand cmd = new OracleCommand(strQuery, m_conn);
                cmd.CommandTimeout = 0;

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(Method(reader));
                    }
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return list;
        }

        public TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, DelegateOfDbDataReadAssign<TValue> Method, string strQuery) where TDictionary : Dictionary<TKey, TValue>, new()
        { 
            return GetEntityDictionary<TDictionary, TKey, TValue>(KeyField,default(TKey),Method, strQuery);
        }

        /// <summary>
        /// 将数据表中记录转换为具有键值的实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="B">泛型集合名</typeparam>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        public TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, DelegateOfDbDataReadAssign<TValue> Method, string strQuery) where TDictionary : Dictionary<TKey, TValue>, new()
        {
            if (Method == null) return null;
            if (KeyFieldDefaultValue.IsNull()) //当键值默认值为空值时，抛出异常
            {
                Exception ex = new Exception("KeyFieldDefaultValue is null");
                Loger.Error(ex);
                throw ex;
            }

            TDictionary list = new TDictionary();
            Open();
            try
            {
                OracleCommand cmd = new OracleCommand(strQuery, m_conn);
                cmd.CommandTimeout = 0;

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TKey key = reader[KeyField].Evaluate<TKey>(KeyFieldDefaultValue);
                        list[key] = Method(reader);
                    }
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return list;
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
        public TList GetEntityList<TList, TValue>(DelegateOfDbDataReadAssign<TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms) where TList : List<TValue>, new()
        {
            if (Method == null) return null;
            TList list = new TList();

            Open();
            try
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(Method(reader));
                    }
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return list;
        }

        public TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, DelegateOfDbDataReadAssign<TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms) where TDictionary : Dictionary<TKey, TValue>, new()
        { 
            return GetEntityDictionary<TDictionary, TKey, TValue>(KeyField, default(TKey), Method, cmdType, cmdText, cmdParms);
        }
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
        public TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, DelegateOfDbDataReadAssign<TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms) where TDictionary : Dictionary<TKey, TValue>, new()
        {
            if (Method == null) return null;
            if (KeyFieldDefaultValue.IsNull()) //当键值默认值为空值时，抛出异常
            {
                Exception ex = new Exception("KeyFieldDefaultValue is null");
                Loger.Error(ex);
                throw ex;
            }

            TDictionary list = new TDictionary();
            Open();
            try
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TKey key = reader[KeyField].Evaluate<TKey>(KeyFieldDefaultValue);
                        list[key] = Method(reader);
                    }
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return list;
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
        public List<T> GetEntityList<T>(DelegateOfDbDataReadAssign<T> Method, string strQuery)
        {
            return GetEntityList<List<T>, T>(Method, strQuery);
        }
        public Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField,  DelegateOfDbDataReadAssign<TValue> Method, string strQuery)
        {
            return GetEntityDictionary<Dictionary<TKey, TValue>, TKey, TValue>(KeyField,  Method, strQuery);
        }
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="B">泛型键值类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        public Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, DelegateOfDbDataReadAssign<TValue> Method, string strQuery)
        {
            return GetEntityDictionary<Dictionary<TKey, TValue>, TKey, TValue>(KeyField, KeyFieldDefaultValue, Method, strQuery);
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
        public List<T> GetEntityList<T>(DelegateOfDbDataReadAssign<T> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return GetEntityList<List<T>, T>(Method, cmdType, cmdText, cmdParms);
        }
        public Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField,  DelegateOfDbDataReadAssign<TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return GetEntityDictionary<Dictionary<TKey, TValue>, TKey, TValue>(KeyField,  Method, cmdType, cmdText, cmdParms);
        }

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
        public Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, DelegateOfDbDataReadAssign<TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return GetEntityDictionary<Dictionary<TKey, TValue>, TKey, TValue>(KeyField, KeyFieldDefaultValue, Method, cmdType, cmdText, cmdParms);
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
        public T GetEntity<T>(DelegateOfDbDataReadAssign<T> Method, string strQuery)
        {
            if (Method == null) return default(T);

            Open();
            T item = default(T);
            try
            {
                OracleCommand cmd = new OracleCommand(strQuery, m_conn);
                cmd.CommandTimeout = 0;

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        item = Method(reader);
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return item;
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
        public T GetEntity<T>(DelegateOfDbDataReadAssign<T> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            if (Method == null) return default(T);

            Open();
            T item = default(T);
            try
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        item = Method(reader);
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return item;
        }
        #endregion

        #region 获取DynamicObj动态对象集合
        public DynamicObj GetDynamicObj(string strQuery)
        {
            return GetEntity<DynamicObj>(dr => DynamicObj.ToDynamic(dr), strQuery);
        }
        public DynamicObj GetDynamicObj(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return GetEntity<DynamicObj>(dr => DynamicObj.ToDynamic(dr), cmdType, cmdText, cmdParms);
        }

        public List<DynamicObj> GetDynamicObjs(string strQuery)
        {
            return GetEntityList<DynamicObj>(dr => DynamicObj.ToDynamic(dr), strQuery);
        }
        public List<DynamicObj> GetDynamicObjs(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return GetEntityList<DynamicObj>(dr => DynamicObj.ToDynamic(dr), cmdType, cmdText, cmdParms);
        }

        public List<DynamicObj> GetDynamicObjsByPage(string cmdText, PageInfo page, List<OrderItem> orders)
        {
            return GetEntityListByPage<DynamicObj>(dr => DynamicObj.ToDynamic(dr), cmdText, page, orders);
        }
        #endregion

        public List<T> GetEntityListByPage<T>(DelegateOfDbDataReadAssign<T> method, string cmdText, PageInfo page, List<OrderItem> orders)
        {
            string sql = Function.GetPageSQL(cmdText, page, this, orders);
            return GetEntityList<T>(method, sql);
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
            IFilter isfilter = LibHelper.CreateIFilter();
            IFilter isnofilter = LibHelper.CreateIFilter();
            if (KeyValue.IsNull())
            {
                isfilter.AppendDefineItems(KeyField, FilterDataType.String, CompareType.DBNull, DBNull.Value);
                isnofilter.AppendDefineItems(KeyField, FilterDataType.String, CompareType.NotDBNull, DBNull.Value);
            }
            else
            {
                FilterDataType type = KeyValue.GetType().ToFilterDataType();
                isfilter.AppendDefineItems(KeyField, type, CompareType.Equal, KeyValue);
                isnofilter.AppendDefineItems(KeyField, type, CompareType.NoEqual, KeyValue);
            }

            string sql = string.Format(@"with locs{{
select * from {0} {3} UNION ALL  
        select l.* from {0} l  
            INNER JOIN locs p ON l.{1}=p.{2}
}}
select * from locs {4}", fun.ConvertTable(TableName), fun.ConvertField(ParentField), fun.ConvertField(KeyField), isfilter.GetFilter("where"), IsIncludeSelf ? "" : isnofilter.GetFilter("where"));
            return GetEntityList<T>(rec => Method(rec), sql);
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
            IFilter isfilter = LibHelper.CreateIFilter();
            IFilter isnofilter = LibHelper.CreateIFilter();
            if (KeyValue.IsNull())
            {
                isfilter.AppendDefineItems(KeyField, FilterDataType.String, CompareType.DBNull, DBNull.Value);
                isnofilter.AppendDefineItems(KeyField, FilterDataType.String, CompareType.NotDBNull, DBNull.Value);
            }
            else
            {
                FilterDataType type = KeyValue.GetType().ToFilterDataType();
                isfilter.AppendDefineItems(KeyField, type, CompareType.Equal, KeyValue);
                isnofilter.AppendDefineItems(KeyField, type, CompareType.NoEqual, KeyValue);
            }

            string sql = string.Format(@"with locs{{
select * from {0} {3} UNION ALL  
        select l.* from {0} l  
            INNER JOIN locs p ON l.{2}=p.{1}
}}
select * from locs {4}", fun.ConvertTable(TableName), fun.ConvertField(ParentField), fun.ConvertField(KeyField), isfilter.GetFilter("where"), IsIncludeSelf ? "" : isnofilter.GetFilter("where"));
            return GetEntityList<T>(rec => Method(rec), sql);
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
            string sql = string.Format(@"with locs{{
select * from {0} {3} UNION ALL  
        select l.* from {0} l  
            INNER JOIN locs p ON l.{1}=p.{2}
}}
select * from locs", fun.ConvertTable(TableName), fun.ConvertField(ParentField), fun.ConvertField(KeyField), filter.GetFilter("where"));
            return GetEntityList<T>(rec => Method(rec), sql);
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
            string sql = string.Format(@"with locs{{
select * from {0} {3} UNION ALL  
        select l.* from {0} l  
            INNER JOIN locs p ON l.{2}=p.{1}
}}
select * from locs", fun.ConvertTable(TableName), fun.ConvertField(ParentField), fun.ConvertField(KeyField), filter.GetFilter("where"));
            return GetEntityList<T>(rec => Method(rec), sql);
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
            return GetAllChildRecsByRelation<T>(Method, null, new List<OrderItem>(), factSQL, MiddleRelationTable, MiddleRelaField, factField, FactRelationTable, FactRelaField, MiddleRelaFieldForFactRelaField, FactRelaParentIDField, FactRelaFieldValue);
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
            return GetAllParentRecsByRelation<T>(Method, null, new List<OrderItem>(), factSQL, MiddleRelationTable, MiddleRelaField, factField, FactRelationTable, FactRelaField, MiddleRelaFieldForFactRelaField, FactRelaParentIDField, FactRelaFieldValue);
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
        public List<T> GetAllChildRecsByRelation<T>(Func<DbDataReader, T> Method, PageInfo pageInfo, List<OrderItem> Orders, string factSQL, string MiddleRelationTable, string MiddleRelaField, string factField, string FactRelationTable, string FactRelaField, string MiddleRelaFieldForFactRelaField, string FactRelaParentIDField, object FactRelaFieldValue)
        {
            //移除factSQL中的order by语句
            int orderbyIndex = factSQL.ToLower().LastIndexOf(" order by ");
            string order = "";
            if (orderbyIndex >= 0 && orderbyIndex + 10 < factSQL.Length - 1)
            {
                order = factSQL.Substring(orderbyIndex + 10);
            }

            string fromSQL = "";
            if (orderbyIndex > -1)
                fromSQL = factSQL.Substring(0, orderbyIndex);
            else
                fromSQL = factSQL;



            IFilter filter = LibHelper.CreateIFilter();
            filter.AppendDefineItems(FactRelaField, FactRelaFieldValue.GetType().ToFilterDataType(), CompareType.Equal, FactRelaFieldValue);

            string wSQL = string.Format(@"
with rec as (
    select {0} from {1} {3}
    UNION ALL  
    select l.{0} from {1} l  
                INNER JOIN rec p ON l.{2}=p.{0}
)", FactRelaField, FactRelationTable, FactRelaParentIDField, filter.GetFilter("where"));

            fromSQL = string.Format(@"
select distinct a.*
	from ({0}) a inner join
	{1} gl on a.{2}=gl.{3} inner join
	rec on rec.{4}=gl.{5}", fromSQL,
                   MiddleRelationTable,
                   factField,
                   MiddleRelaField,
                   FactRelaField,
                   MiddleRelaFieldForFactRelaField);


            DbHelper.AppentSort(Orders, order);

            string totalSQL = string.Format("select count(1) from ({0}) t1", fromSQL);

            //进行分页处理
            fromSQL = getPageSql(string.Concat(wSQL, totalSQL), fromSQL, pageInfo, Orders);

            string sql = string.Concat(wSQL, fromSQL);
            return GetEntityList<T>(rec => Method(rec), sql);
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
            //移除factSQL中的order by语句
            int orderbyIndex = factSQL.ToLower().LastIndexOf(" order by ");
            string order = "";
            if (orderbyIndex >= 0 && orderbyIndex + 10 < factSQL.Length - 1)
            {
                order = factSQL.Substring(orderbyIndex + 10);
            }

            string fromSQL = "";
            if (orderbyIndex > -1)
                fromSQL = factSQL.Substring(0, orderbyIndex);
            else
                fromSQL = factSQL;



            IFilter filter = LibHelper.CreateIFilter();
            filter.AppendDefineItems(FactRelaField, FactRelaFieldValue.GetType().ToFilterDataType(), CompareType.Equal, FactRelaFieldValue);

            string wSQL = string.Format(@"
with rec as (
    select {0} from {1} {3}
    UNION ALL  
    select l.{0} from {1} l  
                INNER JOIN rec p ON p.{2}=l.{0}
)", FactRelaField, FactRelationTable, FactRelaParentIDField, filter.GetFilter("where"));

            fromSQL = string.Format(@"
select distinct a.*
	from ({0}) a inner join
	{1} gl on a.{2}=gl.{3} inner join
	rec on rec.{4}=gl.{5}", fromSQL,
                   MiddleRelationTable,
                   factField,
                   MiddleRelaField,
                   FactRelaField,
                   MiddleRelaFieldForFactRelaField);


            DbHelper.AppentSort(Orders, order);

            string totalSQL = string.Format("select count(1) from ({0}) t1", fromSQL);

            //进行分页处理
            fromSQL = getPageSql(string.Concat(wSQL, totalSQL), fromSQL, pageInfo, Orders);


            string sql = string.Concat(wSQL, fromSQL);
            return GetEntityList<T>(rec => Method(rec), sql);
        }


        private string getPageSql(string totalSQL, string SelectCommand, PageInfo page, List<OrderItem> Orders)
        {
            if (Orders == null) Orders = new List<OrderItem>();
            if (page != null && page.PageSize > 0)
            {
                //移除sql中的order by
                string sql = "";
                string temp = SelectCommand.ToLower();
                int pos = temp.LastIndexOf(" order by ");
                if (pos > -1)
                    sql = SelectCommand.Substring(0, pos);
                else
                    sql = SelectCommand;

                page.Totals = GetScalar(totalSQL).ToLong();

                int pageIndex = page.PageIndex < 0 ? 0 : page.PageIndex;

                long bIndex = page.PageSize * pageIndex;
                long eIndex = page.PageSize * (pageIndex + 1);

                sql = string.Format(@"
select * from (
    select row_number() over({0}) as RecordNumberNo,totalAbyPage.* from ({1}) totalAbyPage
) totalBbyPage where RecordNumberNo>{2} and RecordNumberNo<={3} order by RecordNumberNo ", DbHelper.GetOrderASCInfo(Orders), sql, bIndex, eIndex);
                return sql;
            }
            else
            {
                page.Totals = GetScalar(string.Format("SELECT COUNT(1) FROM ({0}) t1", SelectCommand)).ToLong();
                return SelectCommand;
            }
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
            return GetScalar(CommandType.Text, strQuery);
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
            Open();

            object val = null;
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandTimeout = 0;
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);
                val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Error(e);
                throw new Exception("SQL Update Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return val;
        }

        /// <summary>
        /// 获取指定SQL返回的行值，取第一行
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public object[] GetRowItemArray(string strQuery)
        {
            Open();
            object[] v = null;
            try
            {
                OracleCommand cmd = new OracleCommand(strQuery, m_conn);
                cmd.CommandTimeout = 0;

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        v = new object[reader.FieldCount];
                        for (int i = 0; i < v.Length; i++)
                            v[i] = reader[i];
                    }
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return v;
        }
        public Dictionary<string, TValue> GetRowItemArray<TValue>(string strQuery)
        {
            Open();
            Dictionary<string, TValue> v = null;
            try
            {
                OracleCommand cmd = new OracleCommand(strQuery, m_conn);
                cmd.CommandTimeout = 0;

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        v = new Dictionary<string, TValue>();
                        for (int i = 0; i < reader.FieldCount; i++)
                            v[reader.GetName(i)] = reader[i].Evaluate<TValue>();
                    }
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return v;
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
            Open();
            object[] v = null;
            try
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        v = new object[reader.FieldCount];
                        for (int i = 0; i < v.Length; i++)
                            v[i] = reader[i];
                    }
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return v;
        }
        public List<Dictionary<string, TValue>> GetRowItems<TValue>(string strQuery)
        {
            Open();
            List<Dictionary<string, TValue>> items = new List<Dictionary<string, TValue>>();
            try
            {
                OracleCommand cmd = new OracleCommand(strQuery, m_conn);
                cmd.CommandTimeout = 0;

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                using (OracleDataReader reader = cmd.ExecuteReader())
                {

                    if (reader.Read())
                    {
                        Dictionary<string, TValue> v = new Dictionary<string, TValue>();
                        for (int i = 0; i < reader.FieldCount; i++)
                            v[reader.GetName(i)] = reader[i].Evaluate<TValue>();
                        items.Add(v);
                    }
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Error(e);
                throw new Exception("SQL Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return items;

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
            return ExecuteCommand(CommandType.Text, cmd);
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
            Open();

            int val = 0;
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandTimeout = 0;
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);
                val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Error(e);
                throw new Exception("SQL Update Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
            return val;
        }

        /// <summary>
        ///增加参数
        /// </summary>
        private OracleCommand PrepareCommand(OracleCommand cmd, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            cmd.Connection = this.m_conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            cmd.CommandTimeout = 0;

            if (this.trans != null)
                cmd.Transaction = this.trans;

            if (cmdParms != null)
            {
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
            return cmd;
        }
        #endregion

        #region 批量执行SQL指令
        protected List<OracleCommand> cmdList = null;

        /// <summary>
        /// 追加一条SQL执行指令
        /// </summary>
        /// <param name="cmd"></param>
        public void AppendCommand(string cmd)
        {
            AppendCommand(CommandType.Text, cmd);
        }
        /// <summary>
        /// 追加一条SQL执行指令
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        public void AppendCommand(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            if (cmdList == null) cmdList = new List<OracleCommand>();
            cmdList.Add(PrepareCommand(new OracleCommand(), cmdType, cmdText, cmdParms));
        }
        /// <summary>
        /// 批量执行指令
        /// </summary>
        public void Execute()
        {
            if (cmdList == null || cmdList.Count == 0)
            {
                Exception ex = new Exception("No Command To Execute");
                Loger.Error(ex);
                throw ex;
            }
            string cmdText = "";
            Open();
            try
            {
                foreach (OracleCommand cmd in cmdList)
                {
                    cmdText = cmd.CommandText;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Error(e);
                throw new Exception("SQL Update Failed" + e.ToString());
            }
            finally
            {
                Close();
                cmdList.Clear();
                cmdList = null;
            }
        }

        /// <summary>
        /// 清空要执行的批量命令
        /// </summary>
        public void ClearCommand()
        {
            if (cmdList.HasElement()) cmdList.Clear();
            cmdList = null;
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
            List<string> list = new List<string>();
            Open();
            try
            {
                DataTable dt = m_conn.GetSchema("Tables");
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(dr["TABLE_NAME"].ToString());
                }
            }
            catch (Exception e)
            {
                Loger.Error(e);
                throw new Exception("GetSchema Failed:" + e.ToString());
            }
            finally
            {
                Close();
            }
            return list;
        }
        /// <summary>
        /// 获取当前操作数据库所有用户视图名集
        /// </summary>
        /// <returns></returns>
        public List<string> GetViewNames()
        {
            List<string> list = new List<string>();
            return list;
        }
        /// <summary>
        /// 获取当前操作数据库所有存储过程名
        /// </summary>
        /// <returns></returns>
        public List<string> GetProcedureNames()
        {
            List<string> list = new List<string>();
            return list;
        }
        /// <summary>
        /// 获取当前操作数据库所有用户自定义函数名
        /// </summary>
        /// <returns></returns>
        public List<string> GetFunctionNames()
        {
            List<string> list = new List<string>();
            return list;
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
            return GetEmptyTable(strTableName).PrimaryKey;
        }
        /// <summary>
        /// 获取指定表的父关系集合
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public DataRelationCollection GetParentForeignKeys(string strTableName)
        {
            return GetEmptyTable(strTableName).ParentRelations;
        }
        /// <summary>
        /// 获取指定表的子关系集合
        /// </summary>
        /// <param name="strTableName"></param>
        /// <returns></returns>
        public DataRelationCollection GetChildForeignKeys(string strTableName)
        {
            return GetEmptyTable(strTableName).ChildRelations;
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
            return GetEmptyTable(strTableName);
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
            if (TableName.IsWhiteSpace())
            {
                Exception ex = new Exception("No Set TableName");
                Loger.Error(ex);
                throw ex;
            }

            Open();
            try
            {
                OracleDataAdapter m_adapter = new OracleDataAdapter(string.Format("select * from {0} where 1<>1", Function.ConvertTable(TableName)), m_conn);
                m_adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                if (this.trans != null)
                    m_adapter.SelectCommand.Transaction = this.trans;

                OracleCommandBuilder cb = new OracleCommandBuilder(m_adapter);

                m_adapter.RowUpdating += new OracleRowUpdatingEventHandler(m_adapter_RowUpdating);


                DataTable updt = dt.GetChanges();
                if (updt != null)
                {
                    m_adapter.Update(updt);
                }
            }
            catch (Exception e)
            {
                Loger.Error(e);
                throw new Exception("SQL Update Failed" + e.ToString());
            }
            finally
            {
                Close();
            }

        }

        /// <summary>
        /// 提交数据表,表必须指定相关的表名,并且数据结构与系统数据完全相同
        /// </summary>
        /// <param name="dt"></param>
        public void UpdateDataTable(DataTable dt)
        {
            UpdateDataTable(dt, dt.TableName);
        }

        private void m_adapter_RowUpdating(object sender, OracleRowUpdatingEventArgs e)
        {
            if (e.Status == UpdateStatus.ErrorsOccurred)
            {
                throw e.Errors;
            }
            if (this.trans != null)//有事务时
            {
                if (e.Command == null)
                {
                    e.Status = UpdateStatus.SkipCurrentRow;
                }
                else
                {
                    if (string.IsNullOrEmpty(e.Command.CommandText))
                        e.Status = UpdateStatus.SkipCurrentRow;
                    else
                        e.Command.Transaction = this.trans;
                }
            }
        }
        #endregion

        #region 提交DataSet至数据库

        private void UpdateDataSetTable(DataTable dt)
        {
            OracleDataAdapter m_adapter = new OracleDataAdapter(string.Format("select * from {0} where 1<>1", Function.ConvertTable(dt.TableName)), m_conn);
            m_adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            if (this.trans != null)
                m_adapter.SelectCommand.Transaction = this.trans;

            OracleCommandBuilder cb = new OracleCommandBuilder(m_adapter);

            m_adapter.RowUpdating += new OracleRowUpdatingEventHandler(m_adapter_RowUpdating);


            DataTable updt = dt.GetChanges();
            if (updt != null)
            {
                m_adapter.Update(updt);
            }
        }



        /// <summary>
        /// 提交数据集到数据库中
        /// </summary>
        /// <param name="dt"></param>
        public void UpdateDataSet(DataSet ds)
        {
            if (ds.Tables.Count == 0) return;
            Open();
            try
            {
                foreach (DataTable dt in ds.Tables)
                {
                    if (string.IsNullOrEmpty(dt.TableName)) continue;
                    UpdateDataSetTable(dt);
                }
            }
            catch (Exception e)
            {
                Loger.Error(e);
                throw new Exception("SQL Update Failed" + e.ToString());
            }
            finally
            {
                Close();
            }
        }
        #endregion

        #region 当前数据库驱动语法函数库
        private IFun fun = null;
        /// <summary>
        /// SQL Server数据驱动SQL函数处理实例
        /// </summary>
        public IFun Function
        {
            get
            {
                if (fun == null) fun = new SqlFun();
                return fun;
            }
        }
        #endregion

        #region 读取一条或多条记录相关方法

        /// <summary>
        /// 在事务内执行指定方法
        /// </summary>
        /// <param name="Method"></param>
        /// <returns></returns>
        public JsonReturn RunTransaction(Action Method)
        {
            JsonReturn jr = new JsonReturn();
            if (IsInTransaction) //如果当前已是在外层事务中进行处理，则不再进行事务控制
            {
                try
                {
                    if (Method != null)
                        Method();
                    jr = JsonReturn.RunSuccess(true);
                }
                catch (Exception e)
                {
                    jr = JsonReturn.RunFail(e.Message);
                }
            }
            else //否则则按本级事务进行处理
            {
                BeginTransaction();
                try
                {
                    if (Method != null)
                        Method();
                    Commit();
                    jr = JsonReturn.RunSuccess(true);
                }
                catch (Exception e)
                {
                    Rollback();
                    jr = JsonReturn.RunFail(e.Message);
                }
            }
            return jr;
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
            bool hasRec = false;
            Open();
            Exception curError = null;
            try
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hasRec = true;
                        if (Method != null) Method(reader);
                    }
                }
            }
            catch (Exception e)
            {
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }
            return hasRec;
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
            bool hasRec = false;
            Open();
            Exception curError = null;
            try
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        hasRec = true;
                        if (Method != null) Method(reader);
                    }
                }
            }
            catch (Exception e)
            {
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }
            return hasRec;
        }
        #endregion


        /// <summary>
        /// 确定指定的SQL语句是否有记录，注意，这个SQL语句必须有标准的检索数据语句
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public bool Exists(string strQuery)
        {
            if (strQuery.IsWhiteSpace())
                return false;
            else //移除 Order By
            {
                int index = strQuery.ToLower().IndexOf("from");
                if (index > 0)
                    return GetScalar(string.Format("select count(*) {0}", strQuery.Substring(index))).ToLong() > 0;
                else
                    return false;
            }
        }
    }
}
