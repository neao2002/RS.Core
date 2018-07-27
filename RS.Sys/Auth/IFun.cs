using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Sys.App
{
    public interface IFun
    {
        /// <summary>
        ///  主键ID
        /// </summary>
        string FunID
        {
            get;
            set;
        }

        /// <summary>
        /// 功能点名称 
        /// </summary>
        string FunName
        {
            get;
            set;
        }

        /// <summary>
        /// 所属模块功能 
        /// </summary>
        string ModuleFunID
        {
            get;
            set;
        }
    }
}
