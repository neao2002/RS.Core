using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Core.Common
{
    /// <summary>
    /// 标准环境对象信息接口
    /// </summary>
    public interface IEnvInfo
    {
        /// <summary>
        /// 应用系统类型（PC端系统，APP应用，微信应用，微信小程序)
        /// </summary>
        AppType AppType { get; }
        /// <summary>
        /// 当前应用系统默认数据库链接字符串
        /// </summary>
        string DbConnectstring { get;  }
        /// <summary>
        /// 获取当前操作终端IP地址信息
        /// </summary>
        /// <returns></returns>
        string GetClientIP();
        /// <summary>
        /// 应用系统ID表示该系统的唯一标识
        /// </summary>
        string SystemID { get; }
    }

    /// <summary>
    /// 所属系统类型,主要是区分PC系统，APP应用，微信应用，微信小程序等
    /// </summary>
    public enum AppType
    {
        /// <summary>
        /// PC系统
        /// </summary>
        PC,
        /// <summary>
        /// App应用
        /// </summary>
        App,
        /// <summary>
        /// 微信
        /// </summary>
        WX,
        /// <summary>
        /// 微信小程序
        /// </summary>
        WXsp
    }
}
