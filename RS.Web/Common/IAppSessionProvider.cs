using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Web
{
    public interface IAppSessionProvider
    {
        string UserCookieNameKey { get; }
        /// <summary>
        /// 提供必须的属性选项，用于构建当前群体用户信息
        /// </summary>
        IAppSessionOptions GetOptions();
        /// <summary>
        /// 当前角色群体操作用户控制工厂
        /// 获取与当前供应者相配套的会话工厂对象
        /// </summary>
        IAppSessionFactory GetFactory();
    }
    /// <summary>
    /// 角色用户供应者标准接口
    /// 通常一个系统只有一个角色用户群体，即操作员，但仍有一些系统有不同的角色用户群体，如教育管理系统，有老师用户界面，学生用户界面，还有后台工作人员界面，有三个用户群体，
    /// 每个群体的认证方式可能各不相同，所以这里对不同的角色采用不同的供应，以便结合业务构建相应的用户会话控制
    /// 
    /// </summary>
    public interface IAppSessionProvider<T> : IAppSessionProvider, IAppUserProvider<T> where T : IAppUser
    {
        T ParseUser(string json);
    }

}
