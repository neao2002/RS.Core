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


        void Write(string content);
        void Write(byte[] bytes);
        void Output();




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
