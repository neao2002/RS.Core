using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;

namespace RS.Data
{
    /// <summary>
    /// 针对MS Sql server的数据驱动访问对象
    /// </summary>
    internal class SqlContext: IDbContext
    {
        #region 字段定义
        



        private SqlTransaction trans = null;
        protected SqlConnection m_conn;
        private string connString = "";
        #endregion

        #region 构造函数
        public SqlContext(string strConnect)
        {
            m_conn = new SqlConnection(strConnect);
            connString = strConnect;
        }
        public SqlContext(SqlConnection cnn)
        {
            m_conn = cnn;
            connString = cnn.ConnectionString;
        }
        public SqlContext(SqlConnection cnn, SqlTransaction mytran)
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
            if (trans == null)  m_conn.Open();
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
        /// 检测指定SQL语句是否为安全SQL语句
        /// </summary>
        /// <param name="strQuery"></param>
        private void CheckIsSafetySql(string strQuery)
        {
            if (!strQuery.IsSafety())
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Debug("SQL语句不安全语句");
                throw new Exception("因检索语句中存在不安全语句,拒绝执行！");
            }
        }

        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        public DataTable GetDataTable(string strQuery)
        {
            CheckIsSafetySql(strQuery);

            Open();
            Exception curError = null;
            DataTable ds = null;
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(strQuery, m_conn);
                // adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                if (this.trans != null)
                    adapter.SelectCommand.Transaction = this.trans;

                if (adapter.SelectCommand != null)
                    adapter.SelectCommand.CommandTimeout = 0;

                ds = new DataTable();
                ds.BeginLoadData();
                adapter.Fill(ds);
                ds.EndLoadData();
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Debug(e.Message);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }
            return ds;
        }

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        /// <param name="strQuery">查询条件</param>
        public void GetDataTable(string strQuery, DataTable dt)
        {
            CheckIsSafetySql(strQuery);

            Open();
            Exception curError = null;
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(strQuery, m_conn);
                //adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                if (this.trans != null)
                    adapter.SelectCommand.Transaction = this.trans;

                if (adapter.SelectCommand != null)
                    adapter.SelectCommand.CommandTimeout = 0;
                dt.BeginLoadData();
                adapter.Fill(dt);
                dt.EndLoadData();
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Debug(e.Message);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }
        }


        /// <summary>
        /// 按指定查询条件返回一个DataTable
        /// </summary>
        public DataTable GetDataTable(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            CheckIsSafetySql(cmdText);

            Open();
            Exception curError = null;
            DataTable ds = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                //adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                ds.BeginLoadData();
                adapter.Fill(ds);
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Debug(e.Message);
                curError = e;
            }
            finally
            {
                Close();
                ds.EndLoadData();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }

            return ds;
        }

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        public void GetDataTable(DataTable dt, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            CheckIsSafetySql(cmdText);

            Open();
            Exception curError = null;
            try
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
               // adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                dt.BeginLoadData();
                adapter.Fill(dt);
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Debug(e.Message);
                curError = e;
            }
            finally
            {
                Close();
                dt.EndLoadData();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
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
            CheckIsSafetySql(TableName);

            Open();
            Exception curError = null;
            DataTable ds = null;
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(string.Format("select * from {0} with(nolock) where 1=2", TableName), m_conn)
                {
                    MissingSchemaAction = MissingSchemaAction.AddWithKey
                };
                if (this.trans != null)
                    adapter.SelectCommand.Transaction = this.trans;

                if (adapter.SelectCommand != null)
                    adapter.SelectCommand.CommandTimeout = 0;

                ds = new DataTable(TableName);
                ds.BeginLoadData();
                adapter.Fill(ds);
                ds.EndLoadData();
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL取表结构】{0}",TableName));
                Loger.Debug(e.Message);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }
            return ds;
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
            CheckIsSafetySql(cmdText);

            Open();
            Exception curError = null;
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(ds);
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }

            return ds;
        }

        /// <summary>
        /// 按指定查询条件填充DataTable
        /// </summary>
        public void GetDataSet(DataSet ds, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            CheckIsSafetySql(cmdText);

            Open();
            Exception curError = null;
            try
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(ds);
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
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
        public TList GetEntityList<TList, TValue>(Func<DbDataReader, TValue> Method, string strQuery) where TList : List<TValue>, new()
        {
            CheckIsSafetySql(strQuery);


            if (Method == null) return null;
            TList list = new TList();
            Open();
            Exception curError = null;
            try
            {
                SqlCommand cmd = new SqlCommand(strQuery, m_conn)
                {
                    CommandTimeout = 0
                };

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                using (SqlDataReader reader = cmd.ExecuteReader())
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
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }
            return list;
        }

        public TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField,  Func<DbDataReader, TValue> Method, string strQuery) where TDictionary : Dictionary<TKey, TValue>, new()
        {
            return GetEntityDictionary<TDictionary, TKey, TValue>(KeyField, default(TKey), Method, strQuery);
        }
        /// <summary>
        /// 将数据表中记录转换为具有键值的实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="B">泛型集合名</typeparam>
        /// <typeparam name="T">泛型类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        public TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, Func<DbDataReader, TValue> Method, string strQuery) where TDictionary : Dictionary<TKey, TValue>, new()
        {
            CheckIsSafetySql(strQuery);

            if (Method == null) return null;
            if (KeyFieldDefaultValue.IsNull()) //当键值默认值为空值时，抛出异常
            {
                Exception ex=new Exception("KeyFieldDefaultValue is null");
                Loger.Debug(ex);
                throw ex;
            }

            TDictionary list = new TDictionary();
            Open();
            Exception curError = null;
            try
            {
                SqlCommand cmd = new SqlCommand(strQuery, m_conn)
                {
                    CommandTimeout = 0
                };

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TKey key =reader[KeyField].Evaluate<TKey>(KeyFieldDefaultValue);
                        list[key] = Method(reader);
                    }
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
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
        public TList GetEntityList<TList, TValue>(Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms) where TList : List<TValue>, new()
        {
            CheckIsSafetySql(cmdText);

            if (Method == null) return null;
            TList list = new TList();

            Open();
            Exception curError = null;
            try
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                using (SqlDataReader reader = cmd.ExecuteReader())
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
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }
            return list;
        }
        public TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms) where TDictionary : Dictionary<TKey, TValue>, new()
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
        public TDictionary GetEntityDictionary<TDictionary, TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms) where TDictionary : Dictionary<TKey, TValue>, new()
        {
            CheckIsSafetySql(cmdText);

            if (Method == null) return null;
            if (KeyFieldDefaultValue.IsNull()) //当键值默认值为空值时，抛出异常
            {
                Exception ex = new Exception("KeyFieldDefaultValue is null");
                Loger.Debug(ex);
                throw ex;
            }

            TDictionary list = new TDictionary();
            Open();
            Exception curError = null;
            try
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                using (SqlDataReader reader = cmd.ExecuteReader())
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
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
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
        public List<T> GetEntityList<T>(Func<DbDataReader, T> Method, string strQuery)
        {
            return GetEntityList<List<T>, T>(Method, strQuery);            
        }

        public Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, Func<DbDataReader, TValue> Method, string strQuery)
        {
            return GetEntityDictionary<Dictionary<TKey, TValue>, TKey, TValue>(KeyField, Method, strQuery);
        }
        
        /// <summary>
        /// 将数据表中记录转换实例对象集合（由委托定义实例）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="B">泛型键值类名</typeparam>
        /// <param name="Method">将DbDataReader记录转成实例对象的委托</param>
        /// <param name="strQuery">查询语句</param>        
        /// <returns>指定类型的集合</returns>
        public Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, Func<DbDataReader, TValue> Method, string strQuery)
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
        public List<T> GetEntityList<T>(Func<DbDataReader, T> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return GetEntityList<List<T>, T>(Method, cmdType, cmdText, cmdParms);
        }
        public Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
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
        public Dictionary<TKey, TValue> GetEntityDictionary<TKey, TValue>(string KeyField, TKey KeyFieldDefaultValue, Func<DbDataReader, TValue> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
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
        public T GetEntity<T>(Func<DbDataReader, T> Method, string strQuery)
        {
            CheckIsSafetySql(strQuery);


            if (Method == null) return default(T);

            Open();
            Exception curError = null;
            T item = default(T);
            try
            {
                SqlCommand cmd = new SqlCommand(strQuery, m_conn)
                {
                    CommandTimeout = 0
                };

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        item = Method(reader);
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", strQuery));
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
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
        public T GetEntity<T>(Func<DbDataReader, T> Method, CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            CheckIsSafetySql(cmdText);

            if (Method == null) return default(T);

            Open();
            Exception curError = null;
            T item = default(T);
            try
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        item = Method(reader);
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
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
            List<DynamicObj> rows= GetEntityListByPage<DynamicObj>(dr => DynamicObj.ToDynamic(dr), cmdText, page, orders);
            if (page.PageSize<=0) //不分页
            {
                page.Totals = rows.Count;
            }
            return rows;
        }
        #endregion

        public List<T> GetEntityListByPage<T>(Func<DbDataReader, T> method, string cmdText, PageInfo page, List<OrderItem> orders)
        {
            string sql = Function.GetPageSQL(cmdText, page,this, orders);
            List<T> rows= GetEntityList<T>(method, sql);

            if (page.PageSize <= 0) //不分页
            {
                page.Totals = rows.Count;
            }
            return rows;

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
            IFilter isfilter = LibHelper.CreateIFilter(this);
            IFilter isnofilter = LibHelper.CreateIFilter(this);
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

            string sql = string.Format(@"with locs as(
select * from {0} {3} UNION ALL  
        select l.* from {0} l  
            INNER JOIN locs p ON l.{1}=p.{2}
)
select * from locs {4}
OPTION (MAXRECURSION 2000);", fun.ConvertTable(TableName), fun.ConvertField(ParentField), fun.ConvertField(KeyField), isfilter.GetFilter("where"), IsIncludeSelf ? "" : isnofilter.GetFilter("where"));
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
            IFilter isfilter = LibHelper.CreateIFilter(this);
            IFilter isnofilter = LibHelper.CreateIFilter(this);
            if (KeyValue.IsNull())
            { 
                isfilter.AppendDefineItems(KeyField, FilterDataType.String, CompareType.DBNull, DBNull.Value);
                isnofilter.AppendDefineItems(KeyField, FilterDataType.String, CompareType.NotDBNull, DBNull.Value);
            }
            else
            {
                FilterDataType type = KeyValue.GetType().ToFilterDataType();
                isfilter.AppendDefineItems(KeyField,type, CompareType.Equal,KeyValue);
                isnofilter.AppendDefineItems(KeyField, type,CompareType.NoEqual, KeyValue);
            }

            string sql = string.Format(@"with locs as(
select * from {0} {3} UNION ALL  
        select l.* from {0} l  
            INNER JOIN locs p ON l.{2}=p.{1}
)
select * from locs {4}
OPTION (MAXRECURSION 2000);", fun.ConvertTable(TableName), fun.ConvertField(ParentField), fun.ConvertField(KeyField), isfilter.GetFilter("where"),IsIncludeSelf?"":isnofilter.GetFilter("where"));
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
            string sql = string.Format(@"with locs as(
select * from {0} {3} UNION ALL  
        select l.* from {0} l  
            INNER JOIN locs p ON l.{1}=p.{2}
)
select * from locs
OPTION (MAXRECURSION 2000);", fun.ConvertTable(TableName),fun.ConvertField(ParentField),fun.ConvertField(KeyField),filter.GetFilter("where"));
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
            string sql = string.Format(@"with locs as(
select * from {0} {3} UNION ALL  
        select l.* from {0} l  
            INNER JOIN locs p ON p.{1}=l.{2}
)
select * from locs
OPTION (MAXRECURSION 2000);", fun.ConvertTable(TableName), fun.ConvertField(ParentField), fun.ConvertField(KeyField), filter.GetFilter("where"));
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
            return GetAllChildRecsByRelation<T>(Method,null,new List<OrderItem>(), factSQL, MiddleRelationTable, MiddleRelaField, factField, FactRelationTable, FactRelaField, MiddleRelaFieldForFactRelaField, FactRelaParentIDField, FactRelaFieldValue);
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



            IFilter filter = LibHelper.CreateIFilter(this);
            filter.AppendDefineItems(FactRelaField, FactRelaFieldValue.GetType().ToFilterDataType(), CompareType.Equal, FactRelaFieldValue);

            string wSQL = string.Format(@"
with rec as (
    select {0} from {1} {3}
    UNION ALL  
    select l.{0} from {1} l  
                INNER JOIN rec p ON l.{2}=p.{0}
)",FactRelaField,FactRelationTable,FactRelaParentIDField,filter.GetFilter("where"));

            fromSQL = string.Format(@"
select distinct a.*
	from ({0}) a inner join
	{1} gl on a.{2}=gl.{3} inner join
	rec on rec.{4}=gl.{5}
OPTION (MAXRECURSION 2000);", fromSQL,
                   MiddleRelationTable,
                   factField,
                   MiddleRelaField,
                   FactRelaField,
                   MiddleRelaFieldForFactRelaField);

            DBHelper.AppentSort(Orders, order);

      


            string sql = string.Concat(wSQL,fromSQL);
            return GetEntityListByPage<T>(rec => Method(rec), sql,pageInfo,Orders);
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



            IFilter filter = LibHelper.CreateIFilter(this);
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
	rec on rec.{4}=gl.{5}
OPTION (MAXRECURSION 2000);", fromSQL,
                   MiddleRelationTable,
                   factField,
                   MiddleRelaField,
                   FactRelaField,
                   MiddleRelaFieldForFactRelaField);


            DBHelper.AppentSort(Orders, order);
            string sql = string.Concat(wSQL, fromSQL);
            return GetEntityListByPage<T>(rec => Method(rec), sql,pageInfo,Orders);
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
            CheckIsSafetySql(cmdText);

            Open();
            Exception curError = null;
            object val = null;
            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandTimeout = 0
                };
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);
                val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }
            return val;
        }

        public T GetField<T>(string strQuery)
        {
            return GetScalar(strQuery).GetValue<T>();
        }
        public T GetField<T>(CommandType cmdType, string cmdText, params DbParameter[] cmdParms)
        {
            return GetScalar(cmdType, cmdText, cmdParms).GetValue<T>();
        }


        public Dictionary<string, TValue> GetRowItemArray<TValue>(string strQuery)
        {
            CheckIsSafetySql(strQuery);


            Open();
            Exception curError = null;
            Dictionary<string, TValue> v = null;
            try
            {
                SqlCommand cmd = new SqlCommand(strQuery, m_conn)
                {
                    CommandTimeout = 0
                };

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                using (SqlDataReader reader = cmd.ExecuteReader())
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
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }
            return v;
        }
        public List<Dictionary<string, TValue>> GetRowItems<TValue>(string strQuery)
        {
            CheckIsSafetySql(strQuery);

            Open();
            Exception curError = null;
            List<Dictionary<string, TValue>> items = new List<Dictionary<string, TValue>>();
            try
            {
                SqlCommand cmd = new SqlCommand(strQuery, m_conn)
                {
                    CommandTimeout = 0
                };

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
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
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }
            return items;

        }
        /// <summary>
        /// 获取指定SQL返回的行值，取第一行
        /// </summary>
        /// <param name="strQuery"></param>
        /// <returns></returns>
        public object[] GetRowItemArray(string strQuery)
        {
            CheckIsSafetySql(strQuery);

            Open();
            Exception curError = null;
            object[] v = null;
            try
            {
                SqlCommand sqlCommand = new SqlCommand(strQuery, m_conn)
                {
                    CommandTimeout = 0
                };
                SqlCommand cmd = sqlCommand;

                if (this.trans != null)
                    cmd.Transaction = this.trans;

                using (SqlDataReader reader = cmd.ExecuteReader())
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
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
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
            CheckIsSafetySql(cmdText);

            Open();
            Exception curError = null;
            object[] v = null;
            try
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                using (SqlDataReader reader = cmd.ExecuteReader())
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
                Loger.Debug(e);

                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }
            return v;
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
        public int ExecuteCommand(SqlBuilder builder)
        {
            return ExecuteCommand(builder.ToSql());
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
            Exception curError = null;
            int val = 0;
            try
            {
                SqlCommand cmd = new SqlCommand()
                {
                    CommandTimeout = 0
                };
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);
                val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
            }
            return val;
        }
               
        /// <summary>
        ///增加参数
        /// </summary>
        private SqlCommand PrepareCommand(SqlCommand cmd, CommandType cmdType, string cmdText,params DbParameter[] cmdParms)
        {
            cmd.Connection = this.m_conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            cmd.CommandTimeout = 0;

            if (this.trans!=null)
                cmd.Transaction = this.trans;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
            return cmd;
        }
        #endregion

        #region 批量执行SQL指令
        protected List<SqlCommand> cmdList=null;

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
            if (cmdList == null) cmdList = new List<SqlCommand>();
            cmdList.Add(PrepareCommand(new SqlCommand(),cmdType,cmdText,cmdParms));
        }
        /// <summary>
        /// 批量执行指令
        /// </summary>
        public void Execute()
        {
            if (cmdList == null || cmdList.Count == 0)
            {
                Exception ex = new Exception("No Command To Execute");
                Loger.Debug(ex);
                throw ex;
            }
            string cmdText = "";
            Open();
            Exception curError = null;
            try
            {                
                foreach (SqlCommand cmd in cmdList)
                {
                    cmdText = cmd.CommandText;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Loger.Debug(string.Format("【SQL】{0}", cmdText));
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                cmdList.Clear();
                cmdList = null;
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
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
                Loger.Debug(e);
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
                Exception ex=new Exception("No Set TableName");
                Loger.Debug(ex);
                throw ex;
            }

            Open();
            Exception curError = null;
            try
            {
                SqlDataAdapter m_adapter = new SqlDataAdapter(string.Format("select * from {0} where 1<>1", Function.ConvertTable(TableName)), m_conn)
                {
                    MissingSchemaAction = MissingSchemaAction.AddWithKey
                };

                if (this.trans != null)
                    m_adapter.SelectCommand.Transaction = this.trans;

               // SqlCommandBuilder cb = new SqlCommandBuilder(m_adapter);

                m_adapter.RowUpdating += new SqlRowUpdatingEventHandler(M_adapter_RowUpdating);


                DataTable updt = dt.GetChanges();
                if (updt != null)
                {
                    m_adapter.Update(updt);
                }
            }
            catch (Exception e)
            {
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
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
        
        private void M_adapter_RowUpdating(object sender, SqlRowUpdatingEventArgs e)
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
            SqlDataAdapter m_adapter = new SqlDataAdapter(string.Format("select * from {0} where 1<>1", Function.ConvertTable(dt.TableName)), m_conn)
            {
                MissingSchemaAction = MissingSchemaAction.AddWithKey
            };

            if (this.trans != null)
                m_adapter.SelectCommand.Transaction = this.trans;

          //  SqlCommandBuilder cb = new SqlCommandBuilder(m_adapter);

            m_adapter.RowUpdating += new SqlRowUpdatingEventHandler(M_adapter_RowUpdating);


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
            Exception curError = null;
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
                Loger.Debug(e);
                curError = e;
            }
            finally
            {
                Close();
                if (curError != null) throw new Exception("SQL Failed" + curError.ToString());
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
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        hasRec = true;
                        Method?.Invoke(reader);
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
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, cmdType, cmdText, cmdParms);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        hasRec = true;
                        Method?.Invoke(reader);
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
            string SelectCommand = strQuery;
            if (SelectCommand.IsWhiteSpace())
                return false;
            else //移除 Order By
            {
                #region 增加对With的支持
                string withPerstr = "";
                if (SelectCommand.Trim().ToLower().StartsWith("with ")) //视其为with表达式
                {
                    string alias = "";
                    int b = SelectCommand.IndexOf("(");
                    int e = 0;
                    if (b > "with ".Length + " as ".Length) //为有效with表达式
                    {
                        withPerstr = SelectCommand.Substring(0, b).ToLower();
                        if (withPerstr.StartsWith("with ") && withPerstr.StartsWith(" as ") && withPerstr.Length > 9)
                            alias = withPerstr.Substring(5, withPerstr.Length - 9);

                        e = GetKHEndIndex(0, SelectCommand);
                        if (e == -1) //则视为无效，不理会
                            withPerstr = "";
                        else if (e >= SelectCommand.Length - 1) //为最后一个字符
                        {
                            SelectCommand = "select * from " + alias;
                        }
                        else
                        {
                            withPerstr = strQuery.Substring(0, e + 1);
                            SelectCommand = strQuery.Substring(e + 1);
                        }
                    }
                }                
                #endregion

                //移除sql中的order by
                string sql = "";
                string temp = SelectCommand.ToLower();
                int pos = temp.LastIndexOf(" order by ");
                int len = " order by ".Length;
                if (pos == -1)
                {
                    pos = temp.LastIndexOf("\r\norder by ");
                    len = "\r\norder by ".Length;
                    if (pos == -1)
                    {
                        pos = temp.LastIndexOf("\norder by ");
                        len = "\norder by ".Length;
                    }
                }

                if (pos > -1)
                {
                    sql = SelectCommand.Substring(0, pos);
                }
                else
                    sql = SelectCommand;

                sql = string.Format(@"{0} 
select count(*) from ({1}) t1",withPerstr,sql);


                return GetScalar(sql).ToLong() > 0;
            }
        }

        /// <summary>
        /// 获取指定字符串中双括号内容(最外层括号）
        /// </summary>
        /// <returns></returns>
        private int GetKHEndIndex(int startIndex, string context)
        {
            int bIndex = -1, eIndex = -1;
            bIndex = context.IndexOf("(", startIndex);
            if (bIndex == -1 || bIndex == context.Length - 1) //表示没有括号，中止
            {
                return -1;
            }
            int sI = bIndex + 1;
            int cbIndex = context.IndexOf("(", sI);
            int ceIndex = context.IndexOf(")", sI);

            while (cbIndex >= 0) //有子级,则进行递归
            {
                if (cbIndex < ceIndex) //成对
                {
                    ceIndex = GetKHEndIndex(sI, context);
                    if (ceIndex >= 0 && ceIndex < context.Length - 1)
                    {
                        sI = ceIndex + 1;
                        cbIndex = context.IndexOf("(", sI);
                        ceIndex = context.IndexOf(")", sI);
                    }
                    else
                        break;
                }
                else
                {
                    break;
                }
            }
            //无子级
            eIndex = ceIndex;
            return eIndex;
        }

        #region 事务执行
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
                    Method?.Invoke();
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
                    Method?.Invoke();
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
        /// 在事务内执行指定方法
        /// </summary>
        /// <param name="Method"></param>
        /// <returns></returns>
        public JsonReturn RunTransaction(Func<JsonReturn> Method)
        {
            JsonReturn jr = new JsonReturn();
            if (IsInTransaction) //如果当前已是在外层事务中进行处理，则不再进行事务控制
            {
                try
                {
                    if (Method != null)
                        jr = Method();
                    else //没有方法
                        jr = JsonReturn.RunSuccess();
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
                        jr = Method();
                    else //没有方法
                        jr = JsonReturn.RunSuccess();
                    Commit();
                }
                catch (Exception e)
                {
                    Rollback();
                    jr = JsonReturn.RunFail(e.Message);
                }
            }
            return jr;
        }
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
            if (entity.IsNull())//如果对像为null，则自动不保存
            {
                throw new Exception("不能保存null对象");
            }

            //自动值列
            List<string> autoValueFields = new List<string>();
            string autoValueKey = "";
            Action<object, string, object> SetEntityPropertyValue;

            //获取该表的结构
            DataTable dt = GetEmptyTable(TableName);
            List<string> fns = new List<string>();
            dt.Columns.ForEach<DataColumn>(dc => {
                fns.Add(dc.ColumnName.ToLower());
                if (dc.AutoIncrement == true) //自动标识值
                {
                    autoValueFields.Add(dc.ColumnName.ToLower());
                    autoValueKey = dc.ColumnName;
                }
            });

            Type type = entity.GetType();

            List<string> Lists1 = new List<string>();//临时列表,用来存储insert后面的字段名列表
            List<string> Lists2 = new List<string>();//临时列表,用来存储values后面的参数值列表

            CommandInfo cmd = new CommandInfo
            {
                CommandText = string.Empty,
                Parameters = new List<DbParameter>()
            };

            if (entity is IDictionary) //为键值对象
            {
                //定义赋值方法
                SetEntityPropertyValue = new Action<object, string, object>((rec, fn, v) =>
                {
                    IDictionary curObj = (IDictionary)rec;
                    if (curObj.Contains(fn))
                    {
                        try
                        {
                            curObj[fn] = v;
                        }
                        catch { }
                    }
                });


                IDictionary obj = (IDictionary)entity;
                foreach (object key in obj.Keys)
                {
                    string fn = key.ToString();
                    object v = DBHelper.ConvertValue(obj[key]);
                    //检测是否有映射
                    if (Prop2FieldMap.ContainsKey(fn))
                    {
                        fn = Prop2FieldMap[fn];
                    }
                    if (fns.Contains(fn.ToLower())) //该属性值可以保存
                    {
                        if (!autoValueFields.Contains(fn.ToLower()))//如果是自动增长字段，则不能进行保存或赋值
                        {
                            Lists1.Add(Function.ConvertField(fn));//要保存该字段
                            cmd.Parameters.Add(Function.CreateParameter(fn, v));
                            Lists2.Add(Function.ConvertParameter(fn));
                        }
                    }
                }
            }
            else
            {
                //用于保存的字段
                //获取该对象的所有属性
                PropertyInfo[] allprops = type.GetProperties();
                //定义赋值方法
                SetEntityPropertyValue = new Action<object, string, object>((rec, fn, v) =>
                {
                    PropertyInfo ps = type.GetProperty(fn);
                    if (ps != null)
                    {
                        try
                        {
                            ps.SetValue(rec, v.GetObjectValue(ps.PropertyType), null);
                        }
                        catch { }
                    }
                });


                foreach (PropertyInfo prop in allprops)
                {
                    if (!prop.CanRead) continue; //不可读，则不保存该属性值
                    string fn = prop.Name;
                    object v = DBHelper.ConvertValue(prop.GetValue(entity, null));
                    //检测是否有映射
                    if (Prop2FieldMap.ContainsKey(fn))
                    {
                        fn = Prop2FieldMap[fn];
                    }
                    if (fns.Contains(fn.ToLower())) //该属性值可以保存
                    {
                        if (!autoValueFields.Contains(fn.ToLower()))//如果是自动增长字段，则不能进行保存或赋值
                        {
                            Lists1.Add(Function.ConvertField(fn));
                            cmd.Parameters.Add(Function.CreateParameter(fn, v));
                            Lists2.Add(Function.ConvertParameter(fn));
                        }
                    }
                }
            }

            cmd.CommandText = string.Format("insert into {0} ({1}) values ({2})", TableName, string.Join(",", Lists1.ToArray()), string.Join(",", Lists2.ToArray()));

            if (Lists1.HasElement())
            {
                //先执行当前命令
                ExecuteCommand(CommandType.Text, cmd.CommandText, cmd.Parameters.ToArray());

                if (autoValueKey.IsNotWhiteSpace())
                {
                    object autoV = GetScalar("select @@identity");
                    if (autoV.IsNotNull())
                    {
                        //设置对象指定属性值
                        SetEntityPropertyValue(entity, autoValueKey, autoV);
                    }
                }
            }
            return true;
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
            return SaveNewObjects(entitys, typeof(T), TableName, new Dictionary<string, string>());
        }
        /// <summary>
        /// 批量新增保存对象集
        /// </summary>
        /// <param name="entitys">要保存的对象集</param>
        /// <param name="type">对象类型</param>
        /// <param name="TableName">保存记录表名</param>
        /// <param name="Prop2FieldMap">个性属性与字段映身关系</param>
        /// <returns></returns>
        private bool SaveNewObjects(IList entitys, Type type, string TableName, Dictionary<string, string> Prop2FieldMap,DataTable emptydt = null, PropertyInfo[] pinfos = null)
        {
            if (entitys == null || entitys.Count == 0) return true;//如果对像为null，则自动不保存


            //自动值列
            List<string> autoValueFields = new List<string>();
            string autoValueKey = "";
            Action<object, string, object> SetEntityPropertyValue;



            //获取该表的结构
            DataTable dt = emptydt ?? GetEmptyTable(TableName);
            List<string> fns = new List<string>();
            dt.Columns.ForEach<DataColumn>(dc =>
            {
                fns.Add(dc.ColumnName.ToLower());
                if (dc.AutoIncrement == true) //自动标识值
                {
                    autoValueFields.Add(dc.ColumnName.ToLower());
                    autoValueKey = dc.ColumnName;
                }

            });

            PropertyInfo[] allprops = pinfos ?? type.GetProperties();




            foreach (object entity in entitys)
            {
                List<string> Lists1 = new List<string>();//临时列表,用来存储insert后面的字段名列表
                List<string> Lists2 = new List<string>();//临时列表,用来存储values后面的参数值列表

                CommandInfo cmd = new CommandInfo
                {
                    CommandText = string.Empty,
                    Parameters = new List<DbParameter>()
                };

                //用于保存的字段
                //获取该对象的所有属性
                if (entity is IDictionary) //为键值对象
                {
                    SetEntityPropertyValue = new Action<object, string, object>((rec, fn, v) =>
                    {
                        IDictionary curObj = (IDictionary)rec;
                        if (curObj.Contains(fn))
                        {
                            try
                            {
                                curObj[fn] = v;
                            }
                            catch { }
                        }
                    });


                    IDictionary obj = (IDictionary)entity;
                    foreach (object key in obj.Keys)
                    {
                        string fn = key.ToString();
                        object v = DBHelper.ConvertValue(obj[key]);
                        //检测是否有映射
                        if (Prop2FieldMap.ContainsKey(fn))
                        {
                            fn = Prop2FieldMap[fn];
                        }
                        if (fns.Contains(fn.ToLower())) //该属性值可以保存
                        {
                            if (!autoValueFields.Contains(fn.ToLower()))//如果是自动增长字段，则不能进行保存或赋值
                            {
                                //为新增准备
                                Lists1.Add(Function.ConvertField(fn));
                                Lists2.Add(Function.ConvertParameter(fn));

                                cmd.Parameters.Add(Function.CreateParameter(fn, v));
                            }
                        }
                    }
                }
                else
                {
                    SetEntityPropertyValue = new Action<object, string, object>((rec, fn, v) =>
                    {
                        PropertyInfo ps = type.GetProperty(fn);
                        if (ps != null)
                        {
                            try
                            {
                                ps.SetValue(rec, v.GetObjectValue(ps.PropertyType), null);
                            }
                            catch { }
                        }
                    });


                    foreach (PropertyInfo prop in allprops)
                    {
                        if (!prop.CanRead) continue; //不可读，则不保存该属性值
                        string fn = prop.Name;
                        object v = DBHelper.ConvertValue(prop.GetValue(entity, null));
                        //检测是否有映射
                        if (Prop2FieldMap.ContainsKey(fn))
                        {
                            fn = Prop2FieldMap[fn];
                        }
                        if (fns.Contains(fn.ToLower())) //该属性值可以保存
                        {
                            //为新增准备
                            Lists1.Add(Function.ConvertField(fn));
                            Lists2.Add(Function.ConvertParameter(fn));

                            cmd.Parameters.Add(Function.CreateParameter(fn, v));
                        }
                    }
                }

                cmd.CommandText = string.Format("insert into {0} ({1}) values ({2})", TableName, string.Join(",", Lists1.ToArray()), string.Join(",", Lists2.ToArray()));
                if (Lists1.HasElement())
                {
                    //先执行当前命令
                    ExecuteCommand(CommandType.Text, cmd.CommandText, cmd.Parameters.ToArray());

                    if (autoValueKey.IsNotWhiteSpace())
                    {
                        object autoV = GetScalar("select @@identity");
                        if (autoV.IsNotNull())
                        {
                            //设置对象指定属性值
                            SetEntityPropertyValue(entity, autoValueKey, autoV);
                        }
                    }
                }
            }
            return true;
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
            return SaveObjectList<T>(entitys, TableName, new Dictionary<string, string>(),KeyPropertyNames);
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
            return SaveObjects(entitys, typeof(T), TableName, Prop2FieldMap, null, null, KeyPropertyNames);
        }

        private bool SaveObjects(IList entitys, Type type, string TableName, Dictionary<string, string> Prop2FieldMap, DataTable emptydt = null, PropertyInfo[] pinfos = null, params string[] KeyPropertyNames)
        {            
            if (entitys == null || entitys.Count == 0) return true;//如果对像为null，则自动不保存
            if (!KeyPropertyNames.HasElement())
            {
                throw new Exception("保存对象实例时未获取任何主键条件，为数据安全，这里不再进行任何保存。");
            }
            //自动值列
            List<string> autoValueFields = new List<string>();
            string autoValueKey = "";
            Action<object, string, object> SetEntityPropertyValue;

            List<string> keys = new List<string>(KeyPropertyNames);

            //获取该表的结构
            DataTable dt = emptydt ?? GetEmptyTable(TableName);
            List<string> fns = new List<string>();
            dt.Columns.ForEach<DataColumn>(dc => {
                fns.Add(dc.ColumnName.ToLower());
                if (dc.AutoIncrement == true) //自动标识值
                {
                    autoValueFields.Add(dc.ColumnName.ToLower());
                    autoValueKey = dc.ColumnName;
                }
            });

            PropertyInfo[] allprops = pinfos ?? type.GetProperties();
            foreach (object entity in entitys)
            {
                List<string> Lists1 = new List<string>();//临时列表,用来存储insert后面的字段名列表
                List<string> Lists2 = new List<string>();//临时列表,用来存储values后面的参数值列表

                List<string> Lists = new List<string>();//临时列表，用来存储update
                IFilter filter = LibHelper.CreateIFilter(this);

                CommandInfo cmd = new CommandInfo
                {
                    CommandText = string.Empty,
                    Parameters = new List<DbParameter>()
                };

                //用于保存的字段
                //获取该对象的所有属性
                if (entity is IDictionary) //为键值对象
                {
                    SetEntityPropertyValue = new Action<object, string, object>((rec, fn, v) =>
                    {
                        IDictionary curObj = (IDictionary)rec;
                        if (curObj.Contains(fn))
                        {
                            try
                            {
                                curObj[fn] = v;
                            }
                            catch { }
                        }
                    });

                    IDictionary obj = (IDictionary)entity;
                    foreach (object key in obj.Keys)
                    {
                        string fn = key.ToString();
                        object v = DBHelper.ConvertValue(obj[key]);
                        //检测是否有映射
                        if (Prop2FieldMap.ContainsKey(fn))
                        {
                            fn = Prop2FieldMap[fn];
                        }
                        if (fns.Contains(fn.ToLower())) //该属性值可以保存
                        {
                            if (!autoValueFields.Contains(fn.ToLower()))//如果是自动增长字段，则不能进行保存或赋值
                            {
                                //为新增准备
                                Lists1.Add(Function.ConvertField(fn));
                                Lists2.Add(Function.ConvertParameter(fn));

                                Lists.Add(string.Format("{0}={1}", Function.ConvertField(fn), Function.ConvertParameter(fn)));
                                cmd.Parameters.Add(Function.CreateParameter(fn, v));
                            }

                            if (keys.Contains(key.ToString())) //为主键
                            {
                                filter.AppendDefineItems(LibHelper.CreateSearchItem(Function.ConvertField(fn), v.GetType().ToFilterDataType(), v));
                            }
                        }
                    }
                }
                else
                {
                    SetEntityPropertyValue = new Action<object, string, object>((rec, fn, v) =>
                    {
                        PropertyInfo ps = type.GetProperty(fn);
                        if (ps != null)
                        {
                            try
                            {
                                ps.SetValue(rec, v.GetObjectValue(ps.PropertyType), null);
                            }
                            catch { }
                        }
                    });

                    foreach (PropertyInfo prop in allprops)
                    {
                        if (!prop.CanRead) continue; //不可读，则不保存该属性值
                        string fn = prop.Name;
                        object v = DBHelper.ConvertValue(prop.GetValue(entity, null));
                        //检测是否有映射
                        if (Prop2FieldMap.ContainsKey(fn))
                        {
                            fn = Prop2FieldMap[fn];
                        }
                        if (fns.Contains(fn.ToLower())) //该属性值可以保存
                        {
                            if (!autoValueFields.Contains(fn.ToLower()))//如果是自动增长字段，则不能进行保存或赋值
                            {
                                //为新增准备
                                Lists1.Add(Function.ConvertField(fn));
                                Lists2.Add(Function.ConvertParameter(fn));

                                Lists.Add(string.Format("{0}={1}", Function.ConvertField(fn), Function.ConvertParameter(fn)));
                                cmd.Parameters.Add(Function.CreateParameter(fn, v));
                            }

                            if (keys.Contains(prop.Name)) //为主键
                            {
                                filter.AppendDefineItems(LibHelper.CreateSearchItem(Function.ConvertField(fn), FilterHelper.ToFilterDataType(v.GetType()), v));
                            }
                        }
                    }
                }

                if (Exists(string.Format("select * from {0} {1}", TableName, filter.GetFilter("where")))) //已存在该键值记录，则为修改
                {
                    if (filter.ListFilter.FilterItems.Count == 0)
                    {
                        throw new Exception("保存对象实例时未获取任何主键条件，为数据安全，这里不再进行任何保存。");
                    }
                    cmd.CommandText = string.Format("update {0} set {1} {2}", TableName, string.Join(",", Lists.ToArray()), filter.GetFilter("where"));
                    if (Lists.HasElement())
                    {
                        ExecuteCommand(CommandType.Text, cmd.CommandText, cmd.Parameters.ToArray());
                    }
                }
                else
                {
                    cmd.CommandText = string.Format("insert into {0} ({1}) values ({2})", TableName, string.Join(",", Lists1.ToArray()), string.Join(",", Lists2.ToArray()));
                    if (Lists1.HasElement())
                    {
                        //先执行当前命令
                        ExecuteCommand(CommandType.Text, cmd.CommandText, cmd.Parameters.ToArray());
                        if (autoValueKey.IsNotWhiteSpace())
                        {
                            object autoV = GetScalar("select @@identity");
                            if (autoV.IsNotNull())
                            {
                                //设置对象指定属性值
                                SetEntityPropertyValue(entity, autoValueKey, autoV);
                            }
                        }
                    }
                }
            }
            return true;
        }

        private bool DeleteObjects<T>(List<T> entitys, Type type, string TableName, Dictionary<string, string> Prop2FieldMap, DataTable emptydt = null, PropertyInfo[] pinfos = null, params string[] KeyPropertyNames)
        {
            if (entitys == null || entitys.Count == 0) return true;//如果对像为null，则自动不保存
            if (!KeyPropertyNames.HasElement())
            {
                throw new Exception("保存对象实例时未获取任何主键条件，为数据安全，这里不再进行任何保存。");
            }
            List<string> keys = new List<string>(KeyPropertyNames);

            //获取该表的结构
            DataTable dt = emptydt ?? GetEmptyTable(TableName);
            List<string> fns = new List<string>();
            dt.Columns.ForEach<DataColumn>(dc => fns.Add(dc.ColumnName.ToLower()));

            PropertyInfo[] allprops = pinfos ?? type.GetProperties();

            foreach (object entity in entitys)
            {
                IFilter filter = LibHelper.CreateIFilter(this);
                CommandInfo cmd = new CommandInfo
                {
                    CommandText = string.Empty,
                    Parameters = new List<DbParameter>()
                };

                //用于保存的字段
                //获取该对象的所有属性
                if (entity is IDictionary obj) //为键值对象
                {
                    foreach (string key in obj.Keys)
                    {
                        string fn = key.ToString();
                        object v = DBHelper.ConvertValue(obj[key]);
                        //检测是否有映射
                        if (Prop2FieldMap.ContainsKey(fn))
                        {
                            fn = Prop2FieldMap[fn];
                        }
                        if (fns.Contains(fn.ToLower())) //该属性值可以保存
                        {
                            if (keys.Contains(key.ToString())) //为主键
                            {
                                filter.AppendDefineItems(LibHelper.CreateSearchItem(Function.ConvertField(fn), v.GetType().ToFilterDataType(), v));
                            }
                        }
                    }
                }
                else
                {
                    foreach (PropertyInfo prop in allprops)
                    {
                        if (!prop.CanRead) continue; //不可读，则不保存该属性值
                        string fn = prop.Name;
                        object v = DBHelper.ConvertValue(prop.GetValue(entity, null));
                        //检测是否有映射
                        if (Prop2FieldMap.ContainsKey(fn))
                        {
                            fn = Prop2FieldMap[fn];
                        }
                        if (fns.Contains(fn.ToLower())) //该属性值可以保存
                        {
                            if (keys.Contains(prop.Name)) //为主键
                            {
                                filter.AppendDefineItems(LibHelper.CreateSearchItem(Function.ConvertField(fn), v.GetType().ToFilterDataType(), v));
                            }
                        }
                    }
                }
                if (filter.ListFilter.FilterItems.Count == 0)
                {
                    throw new Exception("保存对象实例时未获取任何主键条件，为数据安全，这里不再进行任何保存。");
                }
                cmd.CommandText = string.Format("delete from {0} {1}", TableName, filter.GetFilter("where"));
                ExecuteCommand(CommandType.Text, cmd.CommandText, cmd.Parameters.ToArray());
            }
            return true;
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
            return SaveObject(entity, TableName, Prop2FieldMap, null, KeyPropertyNames);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="TableName"></param>
        /// <param name="Prop2FieldMap"></param>
        /// <param name="NoUpdateFields"></param>
        /// <param name="KeyPropertyNames"></param>
        /// <returns></returns>
        private bool SaveObject(object entity, string TableName, Dictionary<string, string> Prop2FieldMap, string[] NoUpdateFields,params string[] KeyPropertyNames)
        {
            if (entity.IsNull())//如果对像为null，则自动不保存
            {
                throw new Exception("不能保存null对象");
            }
            if (!KeyPropertyNames.HasElement())
            {
                throw new Exception("保存对象实例时未获取任何主键条件，为数据安全，这里不再进行任何保存。");
            }
            List<string> keys = new List<string>(KeyPropertyNames);

            //获取该表的结构
            DataTable dt = GetEmptyTable(TableName);
            //自动值列
            List<string> autoValueFields = new List<string>();



            string autoValueKey = "";


            List<string> fns = new List<string>();
            dt.Columns.ForEach<DataColumn>(dc => {
                fns.Add(dc.ColumnName.ToLower());
                if (dc.AutoIncrement == true) //自动标识值
                {
                    autoValueFields.Add(dc.ColumnName.ToLower());
                    if (keys.Exists(k=>k.IsEquals(dc.ColumnName)))
                        autoValueKey = dc.ColumnName;
                }
            });

            //在保存时要注意，对于自动增长类型字段，是不需要设置保存值的，在保存时会自动增加，这里就要移除



            Type type = entity.GetType();
            //用于保存主键值信息

            List<string> Lists = new List<string>(); //用于修改
            List<string> Lists1 = new List<string>();//用于新增
            List<string> Lists2 = new List<string>();//用于新增

            List<string> NoUpdates = NoUpdateFields.HasElement() ? new List<string>(NoUpdateFields) : new List<string>();

            CommandInfo cmd = new CommandInfo
            {
                CommandText = string.Empty,
                Parameters = new List<DbParameter>()
            };
            CommandInfo cmdNew = new CommandInfo
            {
                CommandText = string.Empty,
                Parameters = new List<DbParameter>()
            };

            //用于保存的字段
            //获取该对象的所有属性

            IFilter filter = LibHelper.CreateIFilter(this);

            Action<object, string, object> SetEntityPropertyValue;

            if (entity is IDictionary obj) //为键值对象
            {
                SetEntityPropertyValue = new Action<object, string, object>((rec, fn, v) =>
                {
                    IDictionary curObj = (IDictionary)rec;
                    if (curObj.Contains(fn))
                    {
                        try
                        {
                            curObj[fn] = v;
                        }
                        catch { }
                    }
                });

                foreach (string key in obj.Keys)
                {
                    string fn = key.ToString();
                    object v = DBHelper.ConvertValue(obj[key]);
                    //检测是否有映射
                    if (Prop2FieldMap.ContainsKey(fn))
                    {
                        fn = Prop2FieldMap[fn];
                    }
                    if (fns.Contains(fn.ToLower())) //该属性值可以保存
                    {
                        if (!autoValueFields.Exists(f => f.IsEquals(fn)))//如果是自动增长字段，则不能进行保存或赋值
                        {
                            Lists1.Add(Function.ConvertField(fn));
                            Lists2.Add(Function.ConvertParameter(fn));
                            cmdNew.Parameters.Add(Function.CreateParameter(fn, v));

                            if (NoUpdates.Count == 0 || !NoUpdates.Exists(f => f.IsEquals(fn)))
                            {
                                Lists.Add(string.Format("{0}={1}", Function.ConvertField(fn), Function.ConvertParameter(fn)));
                                cmd.Parameters.Add(Function.CreateParameter(fn, v));
                            }
                        }
                        if (keys.Contains(key.ToString())) //为主键
                        {
                            filter.AppendDefineItems(LibHelper.CreateSearchItem(Function.ConvertField(fn), v.GetType().ToFilterDataType(), v));
                        }
                    }
                }
            }
            else
            {
                PropertyInfo[] allprops = type.GetProperties();

                SetEntityPropertyValue = new Action<object, string, object>((rec, fn, v) =>
                {
                    PropertyInfo ps = type.GetProperty(fn);
                    if (ps != null)
                    {
                        try
                        {
                            ps.SetValue(rec, v.GetObjectValue(ps.PropertyType), null);
                        }
                        catch { }
                    }
                });


                foreach (PropertyInfo prop in allprops)
                {
                    if (!prop.CanRead) continue; //不可读，则不保存该属性值
                    string fn = prop.Name;
                    object v = DBHelper.ConvertValue(prop.GetValue(entity, null));
                    //检测是否有映射
                    if (Prop2FieldMap.ContainsKey(fn))
                    {
                        fn = Prop2FieldMap[fn];
                    }
                    if (fns.Contains(fn.ToLower())) //该属性值可以保存
                    {
                        if (!autoValueFields.Exists(f => f.IsEquals(fn)))//如果是自动增长字段，则不能进行保存或赋值
                        {
                            Lists1.Add(Function.ConvertField(fn));
                            Lists2.Add(Function.ConvertParameter(fn));
                            cmdNew.Parameters.Add(Function.CreateParameter(fn, v));

                            Lists.Add(string.Format("{0}={1}", Function.ConvertField(fn), Function.ConvertParameter(fn)));
                            cmd.Parameters.Add(Function.CreateParameter(fn, v));
                        }
                        if (keys.Contains(prop.Name)) //为主键
                        {
                            filter.AppendDefineItems(LibHelper.CreateSearchItem(Function.ConvertField(fn), v.GetType().ToFilterDataType(), v));
                        }
                    }
                }
            }
            if (filter.ListFilter.FilterItems.Count == 0)
            {
                throw new Exception("保存对象实例时未获取任何主键条件，为数据安全，这里不再进行任何保存。");
            }
            //如果是保存全部字段，且数据库中并不存在该主键值记录，则视为新增
            if (!NoUpdates.HasElement() && !Exists(string.Format("select * from {0}  {1}", TableName, filter.GetFilter("where"))))
            {
                cmdNew.CommandText = string.Format("insert into {0} ({1}) values ({2})", TableName, string.Join(",", Lists1.ToArray()), string.Join(",", Lists2.ToArray()));

                if (Lists1.HasElement())
                {
                    //先执行当前命令
                    ExecuteCommand(CommandType.Text, cmdNew.CommandText, cmdNew.Parameters.ToArray());
                    if (autoValueKey.IsNotWhiteSpace())
                    {
                        object autoV = GetScalar("select @@identity");
                        if (autoV.IsNotNull())
                        {
                            //设置对象指定属性值
                            SetEntityPropertyValue(entity, autoValueKey, autoV);
                        }
                    }
                }
            }
            else //否则视为修改
            {
                cmd.CommandText = string.Format("update {0} set {1} {2}", TableName, string.Join(",", Lists.ToArray()), filter.GetFilter("where"));
                if (Lists.HasElement())
                {
                    ExecuteCommand(CommandType.Text, cmd.CommandText, cmd.Parameters.ToArray());
                }
            }
            return true;
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
            return UpdateObject(entity, TableName,new string[] { KeyPropertyName }, UpdatePropertyNames);
        }

        public bool UpdateObject(object entity, string TableName, string[] KeyPropertyNames,params string[] UpdatePropertyNames)
        {
            return UpdateObject(entity, TableName, new Dictionary<string, string>(), KeyPropertyNames, UpdatePropertyNames);
        }
        public bool UpdateObject(object entity, string TableName, Dictionary<string, string> Prop2FieldMap, string[] KeyPropertyNames, string[] UpdatePropertyNames)
        {
            //未设置要更新的字段，则视为全部更新
            if (!UpdatePropertyNames.HasElement()) return SaveObject(entity, TableName, Prop2FieldMap, KeyPropertyNames);

            if (entity.IsNull())//如果对像为null，则自动不保存
            {
                throw new Exception("不能保存null对象");
            }
            if (!KeyPropertyNames.HasElement())
            {
                throw new Exception("保存对象实例时未获取任何主键条件，为数据安全，这里不再进行任何保存。");
            }
            List<string> keys = new List<string>(KeyPropertyNames);

            //获取该表的结构
            DataTable dt = GetEmptyTable(TableName);
            List<string> fns = new List<string>();
            dt.Columns.ForEach<DataColumn>(dc => fns.Add(dc.ColumnName.ToLower()));

            Type type = entity.GetType();
            //用于保存主键值信息

            List<string> Lists = new List<string>();

            CommandInfo cmd = new CommandInfo
            {
                CommandText = string.Empty,
                Parameters = new List<DbParameter>()
            };

            //用于保存的字段
            //获取该对象的所有属性

            IFilter filter = LibHelper.CreateIFilter(this);

            List<string> UpFields = new List<string>(UpdatePropertyNames);


            if (entity is IDictionary obj) //为键值对象
            {
                foreach (object key in obj.Keys)
                {
                    string fn = key.ToString();
                    object v = DBHelper.ConvertValue(obj[key]);
                    //检测是否有映射

                    if (Prop2FieldMap != null && Prop2FieldMap.Count > 0 && Prop2FieldMap.ContainsKey(fn))
                    {
                        fn = Prop2FieldMap[fn];
                    }
                    if (fns.Contains(fn.ToLower())) //该属性值可以保存
                    {
                        if (UpFields.Exists(f => f.IsEquals(fn))) //该字段需要更新
                        {
                            Lists.Add(string.Format("{0}={1}", Function.ConvertField(fn), Function.ConvertParameter(fn)));
                            cmd.Parameters.Add(Function.CreateParameter(fn, v));
                        }
                        if (keys.Contains(key.ToString())) //为主键
                        {
                            filter.AppendDefineItems(LibHelper.CreateSearchItem(Function.ConvertField(fn), v.GetType().ToFilterDataType(), v));
                        }
                    }
                }
            }
            else
            {
                PropertyInfo[] allprops = type.GetProperties();
                foreach (PropertyInfo prop in allprops)
                {
                    if (!prop.CanRead) continue; //不可读，则不保存该属性值
                    string fn = prop.Name;
                    object v = DBHelper.ConvertValue(prop.GetValue(entity, null));
                    //检测是否有映射
                    if (Prop2FieldMap != null && Prop2FieldMap.ContainsKey(fn))
                    {
                        fn = Prop2FieldMap[fn];
                    }
                    if (fns.Contains(fn.ToLower())) //该属性值可以保存
                    {
                        if (UpFields.Exists(f => f.IsEquals(fn))) //该字段需要更新
                        {
                            Lists.Add(string.Format("{0}={1}", Function.ConvertField(fn), Function.ConvertParameter(fn)));
                            cmd.Parameters.Add(Function.CreateParameter(fn, v));
                        }
                        if (keys.Contains(prop.Name)) //为主键
                        {
                            filter.AppendDefineItems(LibHelper.CreateSearchItem(Function.ConvertField(fn), v.GetType().ToFilterDataType(), v));
                        }
                    }
                }
            }
            if (filter.ListFilter.FilterItems.Count == 0)
            {
                throw new Exception("保存对象实例时未获取任何主键条件，为数据安全，这里不再进行任何保存。");
            }

            cmd.CommandText = string.Format("update {0} set {1} {2}", TableName, string.Join(",", Lists.ToArray()), filter.GetFilter("where"));
            if (Lists.HasElement())//有要保存的字段
            {
                ExecuteCommand(CommandType.Text, cmd.CommandText, cmd.Parameters.ToArray());
            }
            return true;
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
            //先保存主表记录
            object MasterObj = MDMaps.MasterEntity;
            SaveNewObject(MDMaps.MasterEntity, MDMaps.MasterTableName, MDMaps.Property2FieldMap);
            Type mtype = MDMaps.MasterEntity?.GetType();

            //再保存明细表记录
            foreach (DetailMapInfo dmap in MDMaps.DetailInfos)
            {
                if (dmap.Details != null && dmap.Details.Count > 0) //有明细记录要新增
                {
                    Type type = dmap.GetItemType();
                    if (mtype != null)
                    {
                        //先设置关联属性值                        
                        dmap.DetailRelations.ForEach(dr =>
                        {
                            PropertyInfo dpi = type.GetProperty(dr.DetailRelaPropertyName);
                            PropertyInfo mpi = mtype.GetProperty(dr.MasterKeyPropertyName);
                            if (dpi != null && dpi.CanWrite && mpi != null && mpi.CanRead)
                            {
                                foreach (object detail in dmap.Details)
                                {
                                    dpi.SetValue(detail, mpi.GetValue(MDMaps.MasterEntity, null), null);
                                }
                            }
                        });
                    }
                    SaveNewObjects(dmap.Details, type, dmap.TableName, dmap.Property2FieldMap);
                }
            }
            return true;
        }

        #endregion

        #region 修改保存父子表，即同时保存父子明细表
        public bool SaveMasterAndDetail(MasterDetailMapInfo MDMaps)
        {
            //先保存主表
            SaveObject(MDMaps.MasterEntity, MDMaps.MasterTableName, MDMaps.Property2FieldMap, MDMaps.KeyPropertyNames);
            Type mtype = MDMaps.MasterEntity?.GetType();
            //再保存明细表，注意，这里一定要区分哪些是新增、哪些是修改、哪些是册除
            foreach (DetailMapInfo dmap in MDMaps.DetailInfos)
            {
                if (dmap.Details != null && dmap.Details.Count > 0)
                {
                    Type type = dmap.GetItemType();
                    if (mtype != null)
                    {
                        //先设置关联属性值                        
                        dmap.DetailRelations.ForEach(dr =>
                        {
                            PropertyInfo dpi = type.GetProperty(dr.DetailRelaPropertyName);
                            PropertyInfo mpi = mtype.GetProperty(dr.MasterKeyPropertyName);
                            if (dpi != null && dpi.CanWrite && mpi != null && mpi.CanRead)
                            {
                                foreach (object detail in dmap.Details)
                                {
                                    dpi.SetValue(detail, mpi.GetValue(MDMaps.MasterEntity, null), null);
                                }
                            }
                        });
                    }
                }


                //获取该属性的初始值
                List<DynamicObj> oldv = GetDetailRecords(dmap, MDMaps);

                //取出哪些是要新增，哪些是要删除，哪些是要修改
                DynamicObj datas = dmap.CompareNoO(oldv);

                List<object> inserts = datas.Get("Insert") as List<object>;
                List<object> updates = datas.Get("Update") as List<object>;
                List<DynamicObj> deletes = datas.Get("Delete") as List<DynamicObj>;
                DataTable dt = GetEmptyTable(dmap.TableName);

                if (inserts.HasElement())
                {
                    SaveNewObjects(inserts, dmap.GetItemType(), dmap.TableName, dmap.Property2FieldMap, dt, dmap.GetPropertyInfos());
                }

                if (updates.HasElement())
                    SaveObjects(updates, dmap.GetItemType(), dmap.TableName, dmap.Property2FieldMap,  dt, dmap.GetPropertyInfos(), dmap.KeyPropertyNames);

                if (deletes.HasElement())
                    DeleteObjects(deletes, dmap.GetItemType(), dmap.TableName, dmap.Property2FieldMap, dt, dmap.GetPropertyInfos(), dmap.KeyPropertyNames);
            }
            return true;
        }

        #endregion
        #endregion

        #region 泛对象读取相关方法

        public List<T> GetDetailRecords<T>(DetailMapInfo child, MasterDetailMapInfo master) where T : new()
        {
            Type mtype = master.MasterEntity.GetType();
            IFilter filter = LibHelper.CreateIFilter(this);
            if (child.Filter.IsNotNull()) child.Filter.CopySearchItemsTo(filter);

            foreach (DetailRelation key in child.DetailRelations)
            {
                PropertyInfo pi = mtype.GetProperty(key.MasterKeyPropertyName);
                if (pi == null) return null; //不存在，则不进行填充

                object v = DBHelper.ConvertValue(pi.GetValue(master.MasterEntity, null));

                if (child.Property2FieldMap.ContainsKey(key.DetailRelaPropertyName))
                    filter.AppendDefineItems(Function.ConvertField(child.Property2FieldMap[key.DetailRelaPropertyName]), pi.PropertyType.ToFilterDataType(), CompareType.Equal, v);
                else
                    filter.AppendDefineItems(Function.ConvertField(key.DetailRelaPropertyName), pi.PropertyType.ToFilterDataType(), CompareType.Equal, v);
            }

            Type type = typeof(T);
            PropertyInfo[] ps = type.GetProperties();
            DynamicObj plist = new DynamicObj();
            ps.ForEach<PropertyInfo>(p =>
            {
                if (p.CanRead) plist.Set(p.Name, p);
            });

            return GetEntityList<T>(dr => DBHelper.CreateObject<T>(dr, plist), string.Format("select * from {0} {1}", child.TableName, filter.GetFilter("where")));
        }
        /// <summary>
        /// 根据主表对象填充子表对像集（从数据库中获取最新的）
        /// </summary>
        /// <param name="child"></param>
        /// <param name="master"></param>
        private List<DynamicObj> GetDetailRecords(DetailMapInfo child, MasterDetailMapInfo master)
        {
            Type mtype = master.MasterEntity.GetType();
            IFilter filter = LibHelper.CreateIFilter(this);
            if (child.Filter.IsNotNull()) child.Filter.CopySearchItemsTo(filter);

            foreach (DetailRelation key in child.DetailRelations)
            {
                PropertyInfo pi = mtype.GetProperty(key.MasterKeyPropertyName);
                if (pi == null) return null; //不存在，则不进行填充

                object v = DBHelper.ConvertValue(pi.GetValue(master.MasterEntity, null));

                if (child.Property2FieldMap.ContainsKey(key.DetailRelaPropertyName))
                    filter.AppendDefineItems(Function.ConvertField(child.Property2FieldMap[key.DetailRelaPropertyName]), pi.PropertyType.ToFilterDataType(), CompareType.Equal, v);
                else
                    filter.AppendDefineItems(Function.ConvertField(key.DetailRelaPropertyName), pi.PropertyType.ToFilterDataType(), CompareType.Equal, v);
            }
            return GetDynamicObjs(string.Format("select * from {0} {1}", child.TableName, filter.GetFilter("where")));
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
            IFilter filter = LibHelper.CreateIFilter(this);
            if (KeyFieldValue == null)
                filter.AppendDefineItems(Function.ConvertField(KeyField), FilterDataType.Guid, CompareType.DBNull, DBNull.Value);
            else
                filter.AppendDefineItems(Function.ConvertField(KeyField), KeyFieldValue.GetType().ToFilterDataType(), CompareType.Equal, KeyFieldValue);


            string sql = string.Format("select * from {0} {1}", TableName, filter.GetFilter("where"));

            return GetEntity<T>(dr => DBHelper.CreateObject<T>(dr), sql);
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
            IFilter filter = LibHelper.CreateIFilter(this);
            foreach (string KeyField in KeyFieldValues.Keys)
            {
                object KeyFieldValue = KeyFieldValues[KeyField];
                if (KeyFieldValue == null)
                    filter.AppendDefineItems(Function.ConvertField(KeyField), FilterDataType.Guid, CompareType.DBNull, DBNull.Value);
                else
                    filter.AppendDefineItems(Function.ConvertField(KeyField), KeyFieldValue.GetType().ToFilterDataType(), CompareType.Equal, KeyFieldValue);
            }

            string sql = string.Format("select * from {0} {1}", TableName, filter.GetFilter("where"));

            return GetEntity<T>(dr => DBHelper.CreateObject<T>(dr), sql);
        }


        public List<T> GetObjectList<T>(string sql, Action<DbDataReader> OtherSetMethod = null) where T : new()
        {
            DynamicObj ps = DBHelper.GetObjectPropertys(typeof(T));
            return GetEntityList<T>(dr => DBHelper.CreateObjectRec<T>(dr, ps, OtherSetMethod), sql);
        }
        public List<T> GetObjectList<T>(string sql, PageInfo page, List<OrderItem> orderbys, Action<DbDataReader> OtherSetMethod = null) where T : new()
        {
            DynamicObj ps = DBHelper.GetObjectPropertys(typeof(T));
            return GetEntityListByPage<T>(dr => DBHelper.CreateObjectRec<T>(dr, ps, OtherSetMethod), sql, page, orderbys);
        }

        #endregion
    }
}