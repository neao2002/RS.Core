using System;
using System.Collections.Generic;
using System.Text;

namespace RS
{
    /// <summary>
    /// 系统环境信息表
    /// 系统环境信息可以系统初始时初始化
    /// </summary>
    public class EnvInfo
    {
        /// <summary>
        /// 物理路径
        /// </summary>
        public string ApplicationPath { get; set; }
        /// <summary>
        /// 虚拟路径（专指WEB)
        /// </summary>
        public string VirtualPath { get; set; }
        /// <summary>
        /// 获取或设置当前应用程序站点完整访问路径，如：
        /// 是站点，则为:http://localhost:8001/
        /// 是虚拟目录RS,则为：http://localhost:8001/RS/
        /// </summary>
        public string SiteUrl { get; set; }
        /// <summary>
        /// 当前操作用户访问IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 系统环境类型:0-生产环境，1-测试环境
        /// 默认是生产环境，由外部应用自动控制，对框架本身无实际意义
        /// </summary>
        public EnvType EnvType { get; set; }
    }
    public enum EnvType
    {
        /// <summary>
        /// 发行版，即正式生产环境
        /// </summary>
        Release = 0,
        /// <summary>
        /// 测试环境
        /// </summary>
        TestEnv = 1
    }
}
