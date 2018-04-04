using System;
using System.Runtime.Serialization;

namespace RS.Core
{
    [DataContract]
    [Serializable]
    public class JsonReturn
    {
        public JsonReturn()
        {
            IsSuccess = false;
            Message = "";
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        [DataMember]
        public virtual bool IsSuccess
        {
            get;
            set;
        }

        /// <summary>
        /// 执行完后返回的提示信息
        /// </summary>
        [DataMember]
        public virtual string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 执行的方法实际返回值
        /// </summary>
        [DataMember]
        public virtual object ReturnValue
        { get; set; }

        /// <summary>
        /// 执行成功
        /// </summary>
        /// <param name="returnValue"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static JsonReturn RunSuccess(object returnValue,string msg="")
        {
            return new JsonReturn()
            {
                IsSuccess = true,
                Message = msg,
                ReturnValue = returnValue
            };
        }
        /// <summary>
        /// 执行失败
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="rtnV"></param>
        /// <returns></returns>
        public static JsonReturn RunFail(string msg,object rtnV=null)
        {
            return new JsonReturn()
            {
                IsSuccess = false,
                Message = msg,
                ReturnValue = rtnV
            };
        }
    }
}