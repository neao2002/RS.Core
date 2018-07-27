using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Sys.Auth
{
    /// <summary>
    /// 标准系统模块（子系统或模块系统，本身不代表具体模块功能，类似于功能目录）
    /// </summary>
    interface IModule
    {
        /// <summary>
        /// 模块编号（必须具体唯一性）
        /// </summary>
        string ModuleID
        {
            get;
            set;
        }

        /// <summary>
        /// 模块名称 
        /// </summary>
        string ModuleName
        {
            get;
            set;
        }

        /// <summary>
        /// 上级模块(这样模块与模块之前形成树形层次关系)
        /// </summary>
        string ParentID
        {
            get;
            set;
        }
        /// <summary>
        /// 集成系统网址(对于是集成模块，则要设置集成体系统访问URL)
        /// </summary>
        string SysSiteUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 是否为集成系统模块
        /// （对于集成在本系统下的功能，则直接在主页中呈现,未集成的，则采用外部链接进入外部系统，对于未集成功能的集成系统，不能包含子级模块功能）
        /// </summary>
        bool IsIntegrate
        {
            get;
            set;
        }
        /// <summary>
        /// 获取该模块下所有模块信息列表,这里应该直接从缓存中取
        /// 这里是以树型方式呈现
        /// </summary>
        /// <returns></returns>
        List<T> GetModules<T>() where T : IModuleFun;
    }
}