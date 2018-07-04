using RS.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RS.Core
{
    public static class StringUtils
    {
        #region 是否为空及空字符串相关方法
        /// <summary>
        /// 检测两个字符串是否相等，不区分大小写
        /// </summary>
        /// <param name="v"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsEquals(this string v, object o)
        {
            if (v == null && o == null)
                return true;
            else if ((v == null && o != null) || (v != null && o == null))
                return false;
            else
            {
                return v.ToStringValue().ToLower() == o.ToStringValue().ToLower();
            }
        }

        /// <summary>
        /// 是否为空值或空字符串
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string v)
        {
            return string.IsNullOrEmpty(v);
        }
        /// <summary>
        /// 是否为非空字符串
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string v)
        {
            return !IsEmpty(v);
        }

        /// <summary>
        /// 指示指定的字符串是 null、空还是仅由空白字符组成。
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool IsWhiteSpace(this string v)
        {
            return string.IsNullOrWhiteSpace(v);
        }

        public static bool IsNotWhiteSpace(this string v)
        {
            return !IsWhiteSpace(v);
        }
        #endregion

        /// <summary>
        /// 对字符进行HTML编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            str = str.Replace("&", "&amp;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("<", "&lt;");
            str = str.Replace("\"", "&quot;");
            str = str.Replace(" ", "&nbsp;");
            str = str.Replace("\n", "<br>");
            return str;
        }
        /// <summary>
        /// 对字符进行HTML解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            str = str.Replace("&gt;", ">");
            str = str.Replace("&lt;", "<");
            str = str.Replace("&quot;", "\"");
            str = str.Replace("&amp;", "&");
            return str;
        }

        /// <summary>
        /// 由普通文本转为HTML
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToHtml(this string str)
        {
            if (str.IsEmpty()) return "";
            str = str.Replace(" ", "&nbsp;");
            str = str.Replace(System.Environment.NewLine, "<br>");
            str = str.Replace("\n", "<br>");
            return str;
        }

        /// <summary>
        /// 获取指定字符串的字节数,以UTF8为标准
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static int BLength(this string str)
        {
            Encoding encoding1;
            int num1;
            encoding1 = Encoding.UTF8;
            num1 = encoding1.GetByteCount(str);
            return num1;
        }

        public static string TrimALL(this string str)
        {
            if (str.IsWhiteSpace())
                return "";
            else
                return str.Replace(" ", "");
        }


        #region 加解密
        /// <summary>
        /// 对指定字符串进行加密
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string Encrypt(this string Value)
        {
            if (Value.IsWhiteSpace()) return Value.ToStringValue();
            return StringEncrypt.Encrypt(Value);
        }

        /// <summary> 
        /// 使用给定密钥加密 
        /// </summary> 
        /// <param name="original">原始文字</param> 
        /// <param name="key">密钥</param> 
        /// <param name="encoding">字符编码方案</param> 
        /// <returns>密文</returns> 
        public static string Encrypt(this string Value, string key)
        {
            if (Value.IsWhiteSpace()) return Value.ToStringValue();
            return StringEncrypt.Encrypt(Value, key);
        }


        /// <summary>
        /// 对指定字符串进行解密
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static string Decrypt(this string Value)
        {
            if (Value.IsWhiteSpace()) return Value.ToStringValue();
            return StringEncrypt.Decrypt(Value);
        }


        /// <summary> 
        /// 使用给定密钥解密 
        /// </summary> 
        /// <param name="original">密文</param> 
        /// <param name="key">密钥</param> 
        /// <returns>明文</returns> 
        public static string Decrypt(this string original, string key)
        {
            if (original.IsWhiteSpace()) return original.ToStringValue(); 
            return StringEncrypt.Decrypt(original, key);
        }
        /// <summary> 
        /// 使用缺省密钥解密,返回指定编码方式明文 
        /// </summary> 
        /// <param name="original">密文</param> 
        /// <param name="encoding">编码方式</param> 
        /// <returns>明文</returns> 
        public static string Decrypt(this string Value, Encoding encoding)
        {
            if (Value.IsWhiteSpace()) return Value.ToStringValue();
            return StringEncrypt.Decrypt(Value, encoding);
        }
        /// <summary> 
        /// 使用给定密钥解密 
        /// </summary> 
        /// <param name="encrypted">密文</param> 
        /// <param name="key">密钥</param> 
        /// <param name="encoding">字符编码方案</param> 
        /// <returns>明文</returns> 
        public static string Decrypt(this string Value, string key, Encoding encoding)
        {
            if (Value.IsWhiteSpace()) return Value.ToStringValue();
            return StringEncrypt.Decrypt(Value, key, encoding);
        }
        public static string Encrypt(this string original, string key,Encoding encoding)
        {
            if (original.IsWhiteSpace()) return original.ToStringValue();
            return StringEncrypt.Encrypt(original, key,encoding);
        }



        /// <summary> 
        /// 使用给定密钥加密 
        /// </summary> 
        /// <param name="original">明文</param> 
        /// <param name="key">密钥</param> 
        /// <returns>密文</returns> 
        public static string Encrypt(this byte[] original, string key,Encoding encoding)
        {
            byte[] keydata = encoding.GetBytes(key);
            return StringEncrypt.GetEncryptString(StringEncrypt.Encrypt(original, keydata));
        }
        /// <summary> 
        /// 使用给定密钥加密 
        /// </summary> 
        /// <param name="original">原始数据</param> 
        /// <param name="key">密钥</param> 
        /// <returns>密文</returns> 
        public static string Encrypt(this byte[] original)
        {
            return StringEncrypt.GetEncryptString(StringEncrypt.Encrypt(original));
        }

        /// <summary> 
        /// 使用给定密钥加密 
        /// </summary> 
        /// <param name="original">原始数据</param> 
        /// <param name="key">密钥</param> 
        /// <returns>密文</returns> 
        public static string Encrypt(this byte[] original,Encoding encoding)
        {
            return StringEncrypt.GetEncryptString(StringEncrypt.Encrypt(original, encoding));
        }

        
        /// <summary> 
        /// 使用给定密钥解密 
        /// </summary> 
        /// <param name="original">明文</param> 
        /// <param name="key">密钥</param> 
        /// <returns>密文</returns> 
        public static byte[] DecryptData(this string estring)
        {
            return StringEncrypt.GetDecryptData(estring);
        }
        #endregion
    }
}
