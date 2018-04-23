using RS.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RS.Core.Data
{
    /// <summary>
    /// 数据连接信息
    /// </summary>
    [Serializable]
    internal class DataConnInfo
    {
        private IDbDriver db;
        private string connectionString;
        private DbDriverType dbDriverType;

        /// <summary>
        /// 构造函数:创建系统指定驱动的数据访问对象信息
        /// </summary>
        /// <param name="connStr">数据库联接字符</param>
        /// <param name="type">数据库类型</param>
        public DataConnInfo(string connStr,DbDriverType type)
        {
            connectionString = connStr;
            dbDriverType = type;
        }
        public DataConnInfo(string connStr, Type _DbType)
        {
            DbDriverType = DbDriverType.Define;
            DbType = _DbType;
            connectionString = connStr;
        }

        public DataConnInfo(DbConnection cnn)
        {
            if (cnn is SqlConnection)
            {
                db = new SqlDb((SqlConnection)cnn);
                DbDriverType = DbDriverType.SQLServer;
            }
            else if (cnn is OracleConnection)
            {
                db = new OracleDb((OracleConnection)cnn);
                DbDriverType = DbDriverType.Oracle;
            }
            else
            {
                DbDriverType = DbDriverType.Define;
            }
            connectionString = cnn.ConnectionString;
        }
        public DataConnInfo(DbConnection cnn,DbTransaction tran)
        {
            if (cnn is SqlConnection)
            {
                db = new SqlDb((SqlConnection)cnn,(SqlTransaction)tran);
                DbDriverType = DbDriverType.SQLServer;
            }
            else if (cnn is OracleConnection)
            {
                db = new OracleDb((OracleConnection)cnn,(OracleTransaction)tran);
                DbDriverType = DbDriverType.Oracle;
            }
            else
            {
                DbDriverType = DbDriverType.Define;
            }
            connectionString = cnn.ConnectionString;
        }
        public DataConnInfo(DbConnection cnn, DbDriverType type):this(cnn)
        {
            DbDriverType = type;
        }

        
        /// <summary>
        /// 数据库类型：Access数据库，SQL Server数据库,Oracle数据库,自定义扩展数据驱动
        /// </summary>
        public DbDriverType DbDriverType
        {
            get { return dbDriverType; }
            set { dbDriverType = value; }
        }
        /// <summary>
        /// 数据库连接字符
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        /// <summary>
        /// 自定义扩展的数据驱动类型:只针对第三方数据驱动适配，对内置的无效
        /// </summary>
        public Type DbType
        {
            get;
            set;
        }
        
        public IDbDriver DB 
        {
            get {
                if (db == null)
                {
                    switch (dbDriverType)
                    { 
                        case DbDriverType.SQLServer:
                            db = new SqlDb(connectionString);
                            break;
                        case DbDriverType.Oracle:
                            db = new OracleDb(connectionString);
                            break;
                        case DbDriverType.Define:
                            db=System.Activator.CreateInstance(DbType, connectionString) as IDbDriver;
                            break;
                        default:
                            db = new SqlDb(connectionString);
                            break;
                    }
                }
                return db;
            }
        }

        /// <summary>
        /// 复制当前实例对象为新对象
        /// </summary>
        /// <returns></returns>
        public DataConnInfo Clone()
        {
            DataConnInfo newdb = new DataConnInfo(connectionString, dbDriverType);
            return newdb;
        }

    }
    /// <summary>
    /// 数据驱动类型:框架库内置：SQLServer,Oracle,Access三种数据库的驱动，默认为SQL
    /// </summary>
    public enum DbDriverType
    { 
        /// <summary>
        ///  SQL Server 数据库
        /// </summary>
        SQLServer=0,
        /// <summary>
        /// Oracle数据库
        /// </summary>
        Oracle=1,
        /// <summary>
        /// Assess数据库
        /// </summary>
        Access=2,
        /// <summary>
        /// 用户自动定，对于自定义的驱动，必须要用户自已定义已实现接口IDbDriver的类
        /// </summary>
        Define=3
    }
}
