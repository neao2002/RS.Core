using RS.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RS.Data
{
    /// <summary>
    /// 数据库会话上下文对象工厂
    /// </summary>
    public interface IDbContextFactory
    {
        IDbContext CreateContext();
    }
    /// <summary>
    /// 数据连接工厂
    /// </summary>
    public class DbContextFactory:IDbContextFactory
    {
        /// <summary>
        /// 构造函数:创建系统指定驱动的数据访问对象信息
        /// </summary>
        /// <param name="connStr">数据库联接字符</param>
        /// <param name="type">数据库类型</param>
        private DbContextFactory(string connStr, DbDriverType type)
        {
            ConnectionString = connStr;
            DbDriverType = type;
        }
       
        private DbContextFactory(DbConnection cnn)
        {
            if (cnn is SqlConnection)
            {
                DbConnection = cnn;
                DbDriverType = DbDriverType.SQLServer;
            }
            else if (cnn is OracleConnection)
            {
                DbConnection = cnn;
                DbDriverType = DbDriverType.Oracle;
            }
            else
            {
                throw new Exception("未知数据库驱动类型");
            }
            ConnectionString = cnn.ConnectionString;
        }
        private DbContextFactory(DbConnection cnn, DbTransaction tran)
        {
            if (cnn is SqlConnection)
            {
                DbConnection = cnn;
                DbTransaction = tran;
                DbDriverType = DbDriverType.SQLServer;
            }
            else if (cnn is OracleConnection)
            {
                DbConnection = cnn;
                DbTransaction = tran;
                DbDriverType = DbDriverType.Oracle;
            }
            else
            {
                throw new Exception("未知数据库驱动类型");
            }
            ConnectionString = cnn.ConnectionString;
        }
        private DbContextFactory(IDbContext _db)
        {
            dbContext = _db;
            DbDriverType = DbDriverType.Define;
        }

        /// <summary>
        /// 数据库类型：Access数据库，SQL Server数据库,Oracle数据库,自定义扩展数据驱动
        /// </summary>
        private DbDriverType DbDriverType;
        /// <summary>
        /// 数据库连接字符
        /// </summary>
        private string ConnectionString;

        private DbConnection DbConnection;

        private DbTransaction DbTransaction;
        private IDbContext dbContext;
        
        /// <summary>
        /// 创建数据库访问上下文对象
        /// </summary>
        /// <returns>生成数据库上下文对象</returns>
        public IDbContext CreateContext() 
        {
            if (dbContext != null) return dbContext;

            IDbContext db;
            if (DbConnection == null)
            {
                switch (DbDriverType)
                {
                    case DbDriverType.SQLServer:
                        db = new SqlContext(ConnectionString);
                        break;
                    case DbDriverType.Oracle:
                        db = new OracleContext(ConnectionString);
                        break;
                    default:
                        db = new SqlContext(ConnectionString);
                        break;
                }
            }
            else
            {
                if (DbConnection is SqlConnection)
                {
                    if (DbTransaction != null)
                        db = new SqlContext((SqlConnection)DbConnection, (SqlTransaction)DbTransaction);
                    else
                        db = new SqlContext((SqlConnection)DbConnection);
                }
                else if (DbConnection is OracleConnection)
                {
                    if (DbTransaction != null)
                        db = new SqlContext((SqlConnection)DbConnection, (SqlTransaction)DbTransaction);
                    else
                        db = new SqlContext((SqlConnection)DbConnection);
                }
                else
                    throw new Exception("未知数据库类型");
            }
            return db;
        }

        /// <summary>
        /// 根据链接字符串和数据驱动类型创建数据工厂
        /// </summary>
        /// <param name="connStr">链接字符串</param>
        /// <param name="type">驱动类型</param>
        /// <returns>返回新建的工厂</returns>
        public static DbContextFactory CreateFactory(string connStr, DbDriverType type)
        {
            return new DbContextFactory(connStr, type);
        }
        /// <summary>
        /// 根据指定链接创建数据工厂
        /// </summary>
        /// <param name="cnn">数据连接</param>
        /// <returns>返回新建的工厂</returns>
        public static DbContextFactory CreateFactory(DbConnection cnn)
        {
            return new DbContextFactory(cnn);
        }
        /// <summary>
        /// 根据指定链接和事务创建数据工厂
        /// </summary>
        /// <param name="cnn">数据连接</param>
        /// <param name="tran">事务对象</param>
        /// <returns>返回新建的工厂</returns>
        public static DbContextFactory CreateFactory(DbConnection cnn, DbTransaction tran)
        {
            return new DbContextFactory(cnn, tran);
        }
        /// <summary>
        /// 根据指定外部上下文创建数据工厂
        /// </summary>
        /// <param name="db">数据访问上下文对象</param>
        /// <returns>返回新建的工厂</returns>
        public static DbContextFactory CreateFactory(IDbContext db)
        {
            return new DbContextFactory(db);
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
        SQLServer = 0,
        /// <summary>
        /// Oracle数据库
        /// </summary>
        Oracle = 1,
        /// <summary>
        /// 用户自动定，对于自定义的驱动，必须要用户自已定义已实现接口IDbContext的类
        /// </summary>
        Define = 2
    }
}
