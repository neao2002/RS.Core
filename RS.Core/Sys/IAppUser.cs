using System;

namespace RS
{
    /// <summary>
    /// 标准用户对象接口
    /// </summary>
    public interface IAppUser
    {
        bool IsLogined();
    }

    public abstract class AppUserBase<T> where T:IAppUserProvider
    {
        public static IAppOperator GetOperator()
        {
            return App.GetService<IAppOperatorUser<T>>();
        }
        public static IAppOperator Login(IAppUser user)
        {
            return App.GetService<T>().Login(user);
        }
        public static IAppUserProvider Logout()
        {
            return App.GetService<T>().Logout();
        }

        public static IAppOperator SaveAppOperator(IAppUser user)
        {
            return App.GetService<T>().SaveAppOperator(user);
        }
    }

}
