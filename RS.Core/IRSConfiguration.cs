using System;
using System.Collections.Generic;
using System.Text;

namespace RS
{
    /// <summary>
    /// 框架配置组件标准接口
    /// 以便前端应用可用.Net 或.Net Core
    /// </summary>
    public interface IRSConfiguration
    {
        string GetConnectionString(string Name);
        string GetAppSetting(string Key);
    }
}
