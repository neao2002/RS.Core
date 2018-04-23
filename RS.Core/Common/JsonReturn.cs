using System;
using System.Runtime.Serialization;

namespace RS.Core
{
    /// <summary>
    /// 执行结果返回对象，用于表述方法执行结果
    /// </summary>
    [DataContract]
    [Serializable]
    public class JsonReturn
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        [DataMember]
        public virtual bool IsSuccess { get; set; } = false;

        /// <summary>
        /// 执行完后返回的提示信息
        /// </summary>
        [DataMember]
        public virtual string Message { get; set; } = "";
        

        /// <summary>
        /// 执行的方法实际返回值
        /// </summary>
        [DataMember]
        public virtual object ReturnValue
        { get; set; }

        /// <summary>
        /// 执行成功
        /// </summary>
        /// <param name="returnValue">执行返回值，如果不需要值，则直接传null</param>
        /// <param name="msg">提示信息，默认为空</param>
        /// <returns></returns>
        public static JsonReturn RunSuccess(object returnValue=null,string msg="")
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
        /// <param name="msg">执行失败时的提示信息(必填)</param>
        /// <param name="rtnV">执行失败时的返回对象</param>
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