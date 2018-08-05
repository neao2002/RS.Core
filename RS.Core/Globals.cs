using RS.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RS
{
    /// <summary>
    /// 应用组件全局访问对象
    /// 用于全局配置处理
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// 当前加载所有应用的方法
        /// </summary>
        private static Func<List<Assembly>> funLoadAssemblies;
        /// <summary>
        /// 程序全局配置对象，用于解析文件信息
        /// </summary>
        public static IRSConfiguration Configuration { get; internal set; }

        /// <summary>
        /// 数据库上下文构建工厂，用于根据需求实时创建数据访问对象，以便业务对象调用
        /// </summary>
        public static DbContextFactory DbContextFactory { get; internal set; }

        /// <summary>
        /// 全局应用服务提供对象（用于获取单例注册或实时获取注册对象）
        /// </summary>
        public static IServiceProvider Services { get; internal set; }

        /// <summary>
        /// 系统默认用户供应者
        /// </summary>
        public static IAppUserProvider DefaultUserProvider { get; internal set; }
        public static Type DefaultUserProviderType { get; internal set; }
        public static Type DefaultUserType { get; internal set; }
        /// <summary>
        /// 注意全局服务提供对象
        /// </summary>
        /// <param name="provider">全局服务供应者</param>
        public static void RegServiceProvider(IServiceProvider provider)
        {
            Services = provider;
        }
        public static void RegDefaultUserProvider<T,U>(U provider) where T: IAppUserProvider where U:T
        {
            DefaultUserProviderType = typeof(T);
            DefaultUserType = typeof(IAppOperatorUser<T>);
            DefaultUserProvider = provider;
        }

        /// <summary>
        /// 调用数据工厂创建一个新的数据访问对象
        /// 注意：每个新对象都将开启新的数据访问会话，在会话业务中，请尽量使用同一数据对象
        /// </summary>
        /// <returns></returns>
        public static IDbContext CreateDbContext()
        {            
            return DbContextFactory.CreateContext();
        }
        /// <summary>
        /// 全局初始配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="_LoadCompileAssemblies"></param>
        /// <returns></returns>
        public static IAppRegister GlobalRegistrar(IRSConfiguration configuration,Func<List<Assembly>> _LoadCompileAssemblies)
        {
            Configuration = configuration;
            funLoadAssemblies = _LoadCompileAssemblies;
            
            return new AppRegistrar(configuration);
        }

        /// <summary>
        /// 获取指定健值的配置值
        /// </summary>
        /// <param name="Key">健值</param>
        /// <returns>配置值</returns>
        public static string GetAppSetting(string Key)
        {
            if (Configuration == null)
                return "";
            else
                return Configuration.GetAppSetting(Key);
        }
        /// <summary>
        /// 获取指定名称的数据连接配置值
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static string GetConnectionString(string Name)
        {
            if (Configuration == null)
                return "";
            else
                return Configuration.GetConnectionString(Name);

        }
        /// <summary>
        /// 加载当前应用所有基于IAppService的应用服务，以便为依赖注入进行注册
        /// </summary>
        /// <returns></returns>
        public static List<Assembly> LoadCompileAssemblies()
        {
            if (funLoadAssemblies == null)
                return new List<Assembly>();
            else
                return funLoadAssemblies.Invoke();
        }

    }
    /// <summary>
    /// 应用组件注册类接口
    /// </summary>
    public interface IAppRegister
    {
        #region 注册全局数据库工厂对象
        /// <summary>
        /// 注册根据配置连接及指定驱动类型注册全局数据区厂对象，以便工厂直接创建数据库上下文
        /// 连接字符串未加密
        /// </summary>
        /// <param name="ConnectionStringKey">配置数据链接字符串健值，如果有多屋，则应如下：db:ConnStr</param>
        /// <param name="dbDriverType">指定驱动类型</param>
        /// <returns>返回应用注册对象，以便继续进行注册</returns>
        IAppRegister RegGlobalDbContext(string ConnectionStringKey, DbDriverType dbDriverType);
        /// <summary>
        /// 注册根据配置连接及指定驱动类型注册全局数据区厂对象，以便工厂直接创建数据库上下文
        /// 连接字符串未加密
        /// </summary>
        /// <param name="ConnectionStringKey">配置数据链接字符串健值，如果有多屋，则应如下：db:ConnStr</param>
        /// <param name="DbDriverTypeKey">指定驱动类型的配置键值</param>
        /// <returns>返回应用注册对象，以便继续进行注册</returns>
        IAppRegister RegGlobalDbContext(string ConnectionStringKey, string DbDriverTypeKey);
        /// <summary>
        /// 注册根据配置连接及指定驱动类型注册全局数据区厂对象，以便工厂直接创建数据库上下文
        /// 连接字符串已加密
        /// </summary>
        /// <param name="ConnectionStringKey">配置数据链接字符串健值，如果有多屋，则应如下：db:ConnStr</param>
        /// <param name="dbDriverType">指定驱动类型</param>
        /// <param name="Key">加密的密钥</param>
        /// <returns>返回应用注册对象，以便继续进行注册</returns>
        IAppRegister RegGlobalDbContextForDecrypt(string ConnectionStringKey, DbDriverType dbDriverType, string Key = "");
        /// <summary>
        /// 注册根据配置连接及指定驱动类型注册全局数据区厂对象，以便工厂直接创建数据库上下文
        /// 连接字符串已加密
        /// </summary>
        /// <param name="ConnectionStringKey">配置数据链接字符串健值，如果有多屋，则应如下：db:ConnStr</param>
        /// <param name="DbDriverTypeKey">指定驱动类型的配置键值</param>
        /// <param name="Key">加密的密钥</param>
        /// <returns>返回应用注册对象，以便继续进行注册</returns>
        IAppRegister RegGlobalDbContextForDecrypt(string ConnectionStringKey, string DbDriverTypeKey, string Key = "");
        #endregion

        /// <summary>
        /// 注册加密的密约
        /// </summary>
        /// <param name="Key"></param>
        IAppRegister RegEncryptKey(string Key);


        /// <summary>
        /// 注册系统动行日志类型及日去存放路径
        /// </summary>
        /// <param name="type"></param>
        /// <param name="LogPath"></param>
        IAppRegister RegLogger(LogType type, string LogPath);
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="type"></param>
        IAppRegister RegLogger(LogType type);
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="TypeConfigKey"></param>
        /// <returns></returns>
        IAppRegister RegLogger(string TypeConfigKey);
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="TypeConfigKey"></param>
        /// <param name="LogPath"></param>
        /// <returns></returns>
        IAppRegister RegLogger(string TypeConfigKey, string LogPath);
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="SqlConnectstring"></param>
        /// <param name="LogTableName"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        IAppRegister RegDbLogger(LogType type, string SqlConnectstring, string LogTableName, DbDriverType dbType = DbDriverType.SQLServer);
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="TypeConfigKey"></param>
        /// <param name="SqlConnectstring"></param>
        /// <param name="LogTableName"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        IAppRegister RegDbLogger(string TypeConfigKey, string SqlConnectstring, string LogTableName, DbDriverType dbType = DbDriverType.SQLServer);
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="LogTableName"></param>
        /// <returns></returns>
        IAppRegister RegDbLogger(LogType Type, string LogTableName);
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="TypeConfigKey"></param>
        /// <param name="LogTableName"></param>
        /// <returns></returns>
        IAppRegister RegDbLogger(string TypeConfigKey, string LogTableName);


        /// <summary>
        /// 注册请求服务方法
        /// </summary>
        /// <param name="register"></param>
        /// <param name="getRequestService"></param>
        /// <returns></returns>
        IAppRegister RegRequestService(Func<IServiceProvider> getRequestService);
    }

    class AppRegistrar : IAppRegister
    {
        private IRSConfiguration config;
        internal AppRegistrar(IRSConfiguration _config)
        {
            config = _config;
        }

        #region 注册全局数据库工厂对象
        /// <summary>
        /// 注册根据配置连接及指定驱动类型注册全局数据区厂对象，以便工厂直接创建数据库上下文
        /// 连接字符串未加密
        /// </summary>
        /// <param name="ConnectionStringKey">配置数据链接字符串健值，如果有多屋，则应如下：db:ConnStr</param>
        /// <param name="dbDriverType">指定驱动类型</param>
        /// <returns>返回应用注册对象，以便继续进行注册</returns>
        public IAppRegister RegGlobalDbContext(string ConnectionStringKey, DbDriverType dbDriverType)
        {
            string connstr = config.GetAppSetting(ConnectionStringKey).ToStringValue();
            Global.DbContextFactory = DbContextFactory.CreateFactory(connstr, dbDriverType);
            return this;
        }
        /// <summary>
        /// 注册根据配置连接及指定驱动类型注册全局数据区厂对象，以便工厂直接创建数据库上下文
        /// 连接字符串未加密
        /// </summary>
        /// <param name="ConnectionStringKey">配置数据链接字符串健值，如果有多屋，则应如下：db:ConnStr</param>
        /// <param name="DbDriverTypeKey">指定驱动类型的配置键值</param>
        /// <returns>返回应用注册对象，以便继续进行注册</returns>
        public IAppRegister RegGlobalDbContext(string ConnectionStringKey, string DbDriverTypeKey)
        {
            string connstr = config.GetAppSetting(ConnectionStringKey).ToStringValue();
            DbDriverType dbDriverType = (DbDriverType)config.GetAppSetting(DbDriverTypeKey).ToInt();
            Global.DbContextFactory = DbContextFactory.CreateFactory(connstr, dbDriverType);
            return this;
        }
        /// <summary>
        /// 注册根据配置连接及指定驱动类型注册全局数据区厂对象，以便工厂直接创建数据库上下文
        /// 连接字符串已加密
        /// </summary>
        /// <param name="ConnectionStringKey">配置数据链接字符串健值，如果有多屋，则应如下：db:ConnStr</param>
        /// <param name="dbDriverType">指定驱动类型</param>
        /// <param name="Key">加密的密钥</param>
        /// <returns>返回应用注册对象，以便继续进行注册</returns>
        public IAppRegister RegGlobalDbContextForDecrypt(string ConnectionStringKey, DbDriverType dbDriverType, string Key = "")
        {
            string connstr = config.GetAppSetting(ConnectionStringKey).ToStringValue();
            Global.DbContextFactory = DbContextFactory.CreateFactory(connstr, dbDriverType);
            return this;
        }
        /// <summary>
        /// 注册根据配置连接及指定驱动类型注册全局数据区厂对象，以便工厂直接创建数据库上下文
        /// 连接字符串已加密
        /// </summary>
        /// <param name="ConnectionStringKey">配置数据链接字符串健值，如果有多屋，则应如下：db:ConnStr</param>
        /// <param name="DbDriverTypeKey">指定驱动类型的配置键值</param>
        /// <param name="Key">加密的密钥</param>
        /// <returns>返回应用注册对象，以便继续进行注册</returns>
        public IAppRegister RegGlobalDbContextForDecrypt(string ConnectionStringKey, string DbDriverTypeKey, string Key = "")
        {
            string connstr = config.GetAppSetting(ConnectionStringKey).ToStringValue().Decrypt(Key);

            DbDriverType dbDriverType = (DbDriverType)config.GetAppSetting(DbDriverTypeKey).ToInt();
            Global.DbContextFactory = DbContextFactory.CreateFactory(connstr, dbDriverType);
            return this;
        }
        #endregion

        /// <summary>
        /// 注册加密的密约
        /// </summary>
        /// <param name="Key"></param>
        public IAppRegister RegEncryptKey(string Key)
        {
            StringEncrypt.PassKey = Key;
            return this;
        }


        /// <summary>
        /// 注册系统动行日志类型及日去存放路径
        /// </summary>
        /// <param name="type"></param>
        /// <param name="LogPath"></param>
        public IAppRegister RegLogger(LogType type, string LogPath)
        {
            Loger.RegisLogger(type, LogPath);
            return this;
        }
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="type"></param>
        public IAppRegister RegLogger(LogType type)
        {
            Loger.RegisLogger(type);
            return this;
        }
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="TypeConfigKey"></param>
        /// <returns></returns>
        public IAppRegister RegLogger(string TypeConfigKey)
        {
            Loger.RegisLogger(LibHelper.GetAppSetting(TypeConfigKey));
            return this;
        }
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="TypeConfigKey"></param>
        /// <param name="LogPath"></param>
        /// <returns></returns>
        public IAppRegister RegLogger(string TypeConfigKey, string LogPath)
        {
            Loger.RegisLogger(LibHelper.GetAppSetting(TypeConfigKey), LogPath);
            return this;
        }
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="SqlConnectstring"></param>
        /// <param name="LogTableName"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public IAppRegister RegDbLogger(LogType type, string SqlConnectstring, string LogTableName, DbDriverType dbType = DbDriverType.SQLServer)
        {
            Loger.RegisLogger(type, SqlConnectstring, LogTableName, dbType);
            return this;
        }
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="TypeConfigKey"></param>
        /// <param name="SqlConnectstring"></param>
        /// <param name="LogTableName"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public IAppRegister RegDbLogger(string TypeConfigKey, string SqlConnectstring, string LogTableName, DbDriverType dbType = DbDriverType.SQLServer)
        {
            Loger.RegisLogger(LibHelper.GetAppSetting(TypeConfigKey), SqlConnectstring, LogTableName, dbType);
            return this;
        }
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="LogTableName"></param>
        /// <returns></returns>
        public IAppRegister RegDbLogger(LogType Type, string LogTableName)
        {
            return RegDbLogger(Type, "", LogTableName);
        }
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="TypeConfigKey"></param>
        /// <param name="LogTableName"></param>
        /// <returns></returns>
        public IAppRegister RegDbLogger(string TypeConfigKey, string LogTableName)
        {
            return RegDbLogger(TypeConfigKey, "", LogTableName);
        }


        /// <summary>
        /// 注册请求服务方法
        /// </summary>
        /// <param name="register"></param>
        /// <param name="getRequestService"></param>
        /// <returns></returns>
        public IAppRegister RegRequestService(Func<IServiceProvider> getRequestService)
        {
            App.GetServiceProvider = getRequestService;
            return this;
        }
    }


}
