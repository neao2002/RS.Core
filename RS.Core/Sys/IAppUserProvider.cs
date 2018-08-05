using System;
using System.Collections.Generic;
using System.Text;

namespace RS
{
    public interface IAppUserProvider {
        IAppOperator GetAppOperator();
        IAppOperator Login(IAppUser user);
        IAppUserProvider Logout();
        IAppOperator SaveAppOperator(IAppUser user);
    }
    public interface IAppUserProvider<T>:IAppUserProvider where T:IAppUser
    {
        /// <summary>
        /// 当前群体下用户类型标准值
        /// </summary>
        int UserType { get; }
        /// <summary>
        /// 获取用户对象ID，该ID必须存在且为唯一性（在同类用户中）如为空，则视为游客
        /// </summary>
        /// <returns></returns>
        string GetUserID(T user);
        /// <summary>
        /// 判断是否已登录的标准
        /// </summary>
        /// <returns></returns>
        bool IsLogined(T user);
        bool IsBidden(T user);
        bool IsGlobalUser(T user);
        /// <summary>
        /// 获取该类用户的编号（ID）用于呈现
        /// </summary>
        /// <returns></returns>
        string GetUserNo(T user);
        /// <summary>
        /// 获取该用户的名称，用于呈现
        /// </summary>
        /// <returns></returns>
        string GetUserName(T user);

    }
}
