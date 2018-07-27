using System;
using RS.Core;


namespace RS.Sys
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
        Release=0,
        /// <summary>
        /// 测试环境
        /// </summary>
        TestEnv=1
    }
    public class AppUser:IUser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AppUser()
        {   
            UserID = "";
            UserNo = "";
            UserName = "";
            UserType = 0;
            IsGoladUser = false;
            LoginTime = DateTime.MinValue;
            ExtendInfo = "";
            Tag = null;        
        }

        EnvInfo Environment { get; set; } = new EnvInfo();


        /// <summary>
        /// 检测输入的ID是否合法
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static bool IsValid(string id)
        {
            //^[0-9a-zA-Z|_|-]{1,}$  数字+字母
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[0-9a-zA-Z|_|-]{1,}$");
            if (reg.IsMatch(id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; } = "";
        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserNo { get; set; } = "";
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; } = "";
        /// <summary>
        /// 用户类型
        /// </summary>
        public int UserType { get; set; } = 0;
        /// <summary>
        /// 当前用户登录到系统的时间，注：由于BS是采用缓存，这里特指进入主界面的时间
        /// </summary>
        public DateTime LoginTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 是否为超级管理员
        /// </summary>
        public bool IsGoladUser { get; set; } = false;

        public bool IsBidden { get; set; } = false;
        /// <summary>
        /// 自定义对象
        /// </summary>
        public object Tag { get; set; }
        /// <summary>
        /// 扩展属性信息（字数不能过长）
        /// </summary>
        public string ExtendInfo { get; set; } = "";

        /// <summary>
        /// 当前用户是否已登录（即是个合法的用户)
        /// </summary>
        /// <returns></returns>
        
        public bool IsLogined()
        {
            return UserID.IsNotWhiteSpace();
        }
        public void Login(IUser u)
        {
            UserID = u.UserID;
            UserNo = u.UserNo;
            UserName = u.UserName;
            IsGoladUser = u.IsGoladUser;
            UserType = u.UserType;
            IsBidden = u.IsBidden;
            Tag = u.Tag;
            ExtendInfo = u.ExtendInfo;
        }
        public void Logout()
        {
            UserID = "";
            UserNo = "";
            UserName = "";
            IsGoladUser =false;
            UserType = 0;
            IsBidden = false;
            Tag = null;
            ExtendInfo = "";
        }
        

        /// <summary>
        /// 创建当前实例的副本
        /// </summary>
        /// <returns></returns>
        public AppUser Clone()
        {
            AppUser app = (AppUser)MemberwiseClone();
            app.Tag = null;
            return app;
        }

        #region 静态方法，用于标识当前应用系统唯一标识ID
        internal static void InitAppID(string id)
        {
            if (IsValid(id))
                AppSystemID = id;
            else
                throw new Exception("No Valid AppID");
        }
        /// <summary>
        /// 系统标识ID，用于标明当前系统与其它系统的区别，ID号必须为字母、数字、或下划线或横线
        /// </summary>
        public static string AppSystemID { get; private set; } = "RSCore";
        #endregion

    }
}
