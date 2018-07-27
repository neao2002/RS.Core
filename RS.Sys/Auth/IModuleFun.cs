using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Sys.App
{
    public interface IModuleFun
    {
        /// <summary>
        /// 模块功能ID
        /// </summary>
        string ModuleFunID
        {
            get;
            set;
        }

        /// <summary>
        /// 功能名称 
        /// </summary>
        string ModuleFunName
        {
            get;
            set;
        }
        
        /// <summary>
        /// 所属系统模块ID 
        /// </summary>
        string ModuleID
        {
            get;
            set;
        }
        /// <summary>
        /// 是否需要授权，如果为否的话，则本模块的基本功能点是不需要授权限，可对所有人公开，
        /// </summary>
        bool IsNeedPower
        {
            get;
            set;
        }

        /// <summary>
        /// 是否为依附模块
        /// 普通或多表模块则一定为独立权限模块。
        /// 如果不是独立模块，则不能设置权限功能点，其可见权限功能点为指定模块的指定功能
        /// </summary>
        bool IsAttach
        {
            get;set;
        }

        /// <summary>
        /// 所依附的功能模块
        /// 如果不是独立权限模块，则其一定是依附在指定模块权限中
        /// </summary>
        string AttachModuleFunID
        {
            get;
            set;
        }
        /// <summary>
        /// 依附指定模块功能的功能点
        /// </summary>
        string AttachFunID
        {
            get;
            set;
        }

        #region 功能菜单入口
        /// <summary>
        /// 是否需要菜单入口
        /// 部分功能是不需要菜单入口的，其入口处可能为其它模块功能的依附功能
        /// </summary>
        bool IsHasMenu
        {
            get;
            set;
        }

        /// <summary>
        ///  功能链接网址
        /// </summary>
        string WebUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 功能所属集成系统访问URL(针对属于集成模块的功能，该属性是必须的，一般等于所属集成模块的SysSiteUrl)
        /// </summary>
        string SysSiteUrl
        {
            get;
            set;
        }


        #endregion
        /// <summary>
        /// 当前模块功能的所有明细功能点
        /// </summary>
        List<IFun> FunList { get; set; }
    }
}
