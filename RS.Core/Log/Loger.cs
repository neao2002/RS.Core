using RS.Core.Data;
using System;
using System.IO;

namespace RS.Core
{
    /// <summary>
    /// 日志服务
    /// </summary>
    public static class Loger
    {
        #region 属性区
        /// <summary>
        /// 获取是否允许显示调试级日志的标记
        /// 支持配置：DEBUG
        /// </summary>
        private static bool IsDebugEnabled
        {
            get
            {
                return (logType == LogType.Debug);
            }
        }

        /// <summary>
        /// 获取是否允许显示信息级日志的标记
        /// 支持配置：INFO,TRACE,DEBUG
        /// </summary>
        private static bool IsInfoEnabled
        {
            get
            {
                return logType == LogType.Info || logType == LogType.Trace || logType == LogType.Debug;
            }
        }

        /// <summary>
        /// 获取是否允许显示警告级日志的标记
        /// Loger.Warn适用于Debug,Warn,Error,Trace
        /// </summary>
        private static bool IsWarnEnabled
        {
            get
            {
                return logType == LogType.Debug || logType == LogType.Warn || logType == LogType.Error || logType == LogType.Trace;
            }
        }

        /// <summary>
        /// 获取是否允许显示错误级日志的标记
        /// Loger.Error适用于所有配置
        /// </summary>
        private static bool IsErrorEnabled
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 获取是否允许显示致命级日志的标记
        /// </summary>
        private static bool IsFatalEnabled
        {
            get
            {
                return IsErrorEnabled;
            }
        }

        #endregion
        #region 静态构造函数
        private static LogType logType;
        private static string logPath;//日志文件存放目录
        private static int LogSaveType = 0;//日志保存类型：0-文件保存，1-数据库保存
        private static string SqlConnectStr;//日志保存数据库名
        private static string logTable;//日志表名
        private static DbDriverType DBType;

        /// <summary>
        /// 注册系统动行日志类型及日去存放路径
        /// </summary>
        /// <param name="type"></param>
        /// <param name="LogPath"></param>
        public static void RegisLogger(LogType type, string LogPath)
        {
            logType = type;
            logPath = LogPath;
            LogSaveType = 0;
        }
        public static void RegisLogger(string TypeExp)
        {
            RegisLogger(TypeExp, "log");
        }
        public static void RegisLogger(string TypeExp, string LogPath)
        {
            if (TypeExp.IsWhiteSpace())
                logType = LogType.Error;
            else if (TypeExp.ToUpper().Equals("ERROR"))
                logType = LogType.Error;
            else if (TypeExp.ToUpper().Equals("INFO"))
                logType = LogType.Info;
            else if (TypeExp.ToUpper().Equals("DEBUG"))
                logType = LogType.Debug;
            else if (TypeExp.ToUpper().Equals("WARN"))
                logType = LogType.Warn;
            else if (TypeExp.ToUpper().Equals("TRACE"))
                logType = LogType.Trace;
            else
                logType = LogType.Error;
            logPath = LogPath;
            LogSaveType = 0;
        }
        /// <summary>
        /// 注册系统日志的类型
        /// </summary>
        /// <param name="type"></param>
        public static void RegisLogger(LogType type)
        {
            logType = type;
            logPath = "log";
            LogSaveType = 0;
        }

        /// <summary>
        /// 注册日志组件，采用数据库保存方式
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="SqlConnectstring">数据库链接</param>
        /// <param name="LogTableName">日志保存表名</param>
        public static void RegisLogger(LogType type, string SqlConnectstring, string LogTableName, DbDriverType dbType = DbDriverType.SQLServer)
        {
            logType = type;
            LogSaveType = 1;
            SqlConnectStr = SqlConnectstring;
            logTable = LogTableName;
            DBType = dbType;
        }
        public static void RegisLogger(string TypeExp, string SqlConnectstring, string LogTableName, DbDriverType dbType = DbDriverType.SQLServer)
        {
            if (TypeExp.IsWhiteSpace())
                logType = LogType.Error;
            else if (TypeExp.ToUpper().Equals("ERROR"))
                logType = LogType.Error;
            else if (TypeExp.ToUpper().Equals("INFO"))
                logType = LogType.Info;
            else if (TypeExp.ToUpper().Equals("DEBUG"))
                logType = LogType.Debug;
            else if (TypeExp.ToUpper().Equals("WARN"))
                logType = LogType.Warn;
            else if (TypeExp.ToUpper().Equals("TRACE"))
                logType = LogType.Trace;
            else
                logType = LogType.Error;

            RegisLogger(logType, SqlConnectstring, LogTableName, dbType);
        }


        public static LogType GetLogType()
        {
            return logType;
        }




        /// <summary>
        /// 静态构造函数,读取日志写入模式
        /// </summary>
        static Loger()
        {
            logType = LogType.Error;
            logPath = "log";
        }

        #endregion
        #region 内部静态方法区
        /// <summary>
        /// 获取当前可用的日志文件名,日志文本默认是保存在应用程序所在目录的LOG目录下，对于Web程序，则是存放在bin目录下的LOG目录下。
        /// </summary>
        /// <returns></returns>
        private static string GetLogFileName()
        {
            if (logPath.IsWhiteSpace())
                logPath = "log";
            try
            {
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
            }
            catch { }
            return Path.Combine(logPath, string.Format("Log{0:yyyyMMdd}.txt", DateTime.Now));
        }
        #endregion
        #region 公共静态方法区

        #region 数据库方式写日志
        //日志表必须包含以下几个字段:logid:主键ID(int 自动赠长），sysdomain:系统域名，ip:IP，logTime:日志时间，logcontent:日志详细内容
        private static void logToTable(string ip, string domain, string logtype, DateTime logtime, string logcontent)
        {
            try
            {
                DbUtils db = DbUtils.NewDB(SqlConnectStr, DBType);

                //自动清空2年前日志
                db.ExecuteCommand(System.Data.CommandType.Text, string.Format("delete from {0} where logtime<@logtime", logTable), db.Function.CreateParameter("@logtime", logtime.AddYears(-1)));

                //追加日志
                db.ExecuteCommand(System.Data.CommandType.Text, string.Format("insert into {0} (sysdomain,ip,logtime,logtype,logcontent) values (@sysdomain,@ip,@logtime,@logtype,@logcontent)", logTable),
                    db.Function.CreateParameter("@sysdomain", domain),
                    db.Function.CreateParameter("@ip", ip),
                    db.Function.CreateParameter("@logtime", logtime),
                    db.Function.CreateParameter("@logtype", logtype),
                    db.Function.CreateParameter("@logcontent", logcontent));
            }
            catch { }
        }
        #endregion

        private static void writeLog(string fitex, object message)
        {
            string ip = LibHelper.GetClientIP();
            DateTime time = DateTime.Now;

            if (LogSaveType == 1)
            {
                logToTable(ip, GetDomain(), fitex, time, string.Concat(message));
            }
            else
            {
                string logFilex = string.Format("【{0} - {1:yyyy-MM-dd HH:mm:ss,ms} {2}】", fitex, time, ip);
                FileUtils.AppendTextFileLine(GetLogFileName(), string.Concat(logFilex, message), true);
            }

        }
        private static void Formatted(string fitex, string format, params object[] args)
        {
            string ip = LibHelper.GetClientIP();
            DateTime time = DateTime.Now;
            string logFilex = string.Format("【{0} - {1:yyyy-MM-dd HH:mm:ss,ms} {2}】", fitex, time, ip);
            if (LogSaveType == 1)
            {
                logToTable(ip, GetDomain(), fitex, time, string.Concat(logFilex, string.Format(format, args)));
            }
            else
            {
                FileUtils.AppendTextFileLine(GetLogFileName(), string.Concat(logFilex, string.Format(format, args)), true);
            }
        }

        /// <summary>
        /// 获取当前系统的域名
        /// </summary>
        /// <returns></returns>
        private static string GetDomain()
        {
            string domain = "";
            //try
            //{
            //    HttpContext page = System.Web.HttpContext.Current;
            //    if (page == null) //则取本地
            //    {
            //        domain = "本地系统";
            //    }
            //    else if (page.Request != null && page.Request.Url != null)
            //    {

            //        domain = page.Request.Url.Authority;
            //    }
            //}
            //catch { }
            return domain;
        }
        /// <summary>
        /// 调试日志记录
        /// </summary>
        /// <param name="message">需要记录的消息</param>
        public static void Debug(object message)
        {
            if (!IsDebugEnabled) return;

            writeLog("DEBUG", message);
        }

        /// <summary>
        /// 调试日志记录(自定义格式化)
        /// </summary>
        /// <param name="format">包含零个或多个格式项</param>
        /// <param name="args">包含零个或多个要格式化的对象的 Object 数组。</param>
        public static void DebugFormatted(string format, params object[] args)
        {
            if (!IsDebugEnabled) return;

            Formatted("DEBUG", format, args);
        }

        /// <summary>
        /// 信息级日志记录
        /// </summary>
        /// <param name="message">需要记录的消息</param>
        public static void Info(object message)
        {
            if (!IsInfoEnabled) return;
            writeLog("INFO", message);
        }

        /// <summary>
        /// 信息级日志记录(自定义格式化)
        /// </summary>
        /// <param name="format">包含零个或多个格式项</param>
        /// <param name="args">包含零个或多个要格式化的对象的 Object 数组。</param>
        public static void InfoFormatted(string format, params object[] args)
        {
            if (!IsInfoEnabled) return;
            Formatted("INFO", format, args);
        }

        /// <summary>
        /// 警告级日志记录
        /// </summary>
        /// <param name="message">需要记录的消息</param>
        public static void Warn(object message)
        {
            if (!IsWarnEnabled) return;
            writeLog("WARN", message);
        }

        /// <summary>
        /// 警告级日志记录
        /// </summary>
        /// <param name="message">需要记录的消息</param>
        /// <param name="exception">异常</param>
        public static void Warn(object message, Exception exception)
        {
            if (!IsWarnEnabled) return;
            writeLog("WARN", string.Format("{0}{1}【ERROR】{2}", message, System.Environment.NewLine, exception));
        }

        /// <summary>
        /// 警告级日志记录(自定义格式化)
        /// </summary>
        /// <param name="format">包含零个或多个格式项</param>
        /// <param name="args">包含零个或多个要格式化的对象的 Object 数组。</param>
        public static void WarnFormatted(string format, params object[] args)
        {
            if (!IsWarnEnabled) return;
            Formatted("WARN", format, args);
        }

        /// <summary>
        /// 错误级日志记录
        /// </summary>
        /// <param name="message">需要记录的消息</param>
        public static void Error(object message)
        {
            if (!IsErrorEnabled) return;
            writeLog("ERROR", message);
        }

        /// <summary>
        /// 错误级日志记录
        /// </summary>
        /// <param name="message">需要记录的消息</param>
        /// <param name="exception">异常</param>
        public static void Error(object message, Exception exception)
        {
            if (!IsErrorEnabled) return;
            writeLog("INFO", string.Format("{0}{1}【ERROR】{2}", message, System.Environment.NewLine, exception));
        }

        /// <summary>
        /// 错误级日志记录(自定义格式化)
        /// </summary>
        /// <param name="format">包含零个或多个格式项</param>
        /// <param name="args">包含零个或多个要格式化的对象的 Object 数组。</param>
        public static void ErrorFormatted(string format, params object[] args)
        {
            if (!IsErrorEnabled) return;
            Formatted("ERROR", format, args);
        }

        /// <summary>
        /// 致命级日志记录
        /// </summary>
        /// <param name="message">需要记录的消息</param>
        public static void Fatal(object message)
        {
            if (!IsFatalEnabled) return;
            writeLog("FATAL", message);
        }

        /// <summary>
        /// 致命级日志记录
        /// </summary>
        /// <param name="message">需要记录的消息</param>
        /// <param name="exception">异常</param>
        public static void Fatal(object message, Exception exception)
        {
            if (!IsErrorEnabled) return;
            writeLog("FATAL", string.Format("{0}{1}【ERROR】{2}", message, System.Environment.NewLine, exception));
        }

        /// <summary>
        /// 致命级日志记录(自定义格式化)
        /// </summary>
        /// <param name="format">包含零个或多个格式项</param>
        /// <param name="args">包含零个或多个要格式化的对象的 Object 数组。</param>
        public static void FatalFormatted(string format, params object[] args)
        {
            if (!IsFatalEnabled) return;
            Formatted("FATAL", format, args);
        }
        #endregion
    }

    public enum LogType
    {
        /// <summary>
        /// 信息模式，该模式下，可显示Info,Warn,Error内容
        /// </summary>
        Info = 0,
        /// <summary>
        /// 错误模式,该模式下，仅显示Error内容
        /// </summary>
        Error = 1,
        /// <summary>
        /// 警示模式，该模式下，仅显示Warn,Error内容
        /// </summary>
        Warn = 2,
        /// <summary>
        /// 调试模式，该模式下可显示所有方法日志
        /// </summary>
        Debug = 3,
        /// <summary>
        /// 跟踪模式，该模式下会显示除Debug外，其它各方法写的日志
        /// </summary>
        Trace = 4
    }


}
