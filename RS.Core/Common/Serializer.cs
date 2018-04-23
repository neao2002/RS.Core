using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Threading;

namespace RS.Core
{
    /// <summary>
    /// 序列化处理类，主要提示对象二进制序列化及反序列化
    /// </summary>
    [Serializable]
    public static class Serializer
    {
        /// <summary>
        /// XML序列化成字节
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(object obj)
        {
            return XMLSerializer.CompressEncryptToBytes(obj);
        }
        /// <summary>
        /// XML反序列化为对象
        /// </summary>
        /// <param name="array"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialize(byte[] array,Type type)
        {
            return XMLSerializer.DecompressDecryptToObject(array,type);
        }

        /// <summary>
        /// [加密压缩]将指定对象序列化成二进制数组，以便进行网络传输给客户端或服务端
        /// 如果处理失败或对象无效，则返回null
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static byte[] BinarySerialize(object o)
        {
            return  BinarySerializer.CompressEncryptToBytes(o);
        }

        /// <summary>
        /// [加密压缩]二进制反序化成具体实例对象
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static object BinaryDeserialize(byte[] array)
        {
            return BinarySerializer.DecompressDecryptToObject(array);
        }
    }
    #region 二进制序列化
    internal static class BinarySerializer
    {
        /// <summary>
        /// 将对象压缩加密到字节数据
        /// </summary>
        /// <param name="obj">要压缩加密的对象</param>
        /// <returns>处理后生成的字节数组</returns>
        public static byte[] CompressEncryptToBytes(object obj)
        {
            if (obj == null) return new byte[0];

            byte[] array = null;
            //第一步,将对象序列化为字节流            
            MemoryStream ms = new MemoryStream();
            try
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(ms, obj);
                array=ms.ToArray();
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
            finally {
                ms.Close();
            }
            return array.HasElement()?array:new byte[0];
        }

        /// <summary>
        /// 将字节数组进行解密解压还原成对象
        /// </summary>
        /// <param name="ary">要处理的字节数组</param>
        /// <returns>被还原的对象</returns>
        public static object DecompressDecryptToObject(byte[] ary)
        {
            if (!ary.HasElement()) return null;
            object o = null;
            //解压字节流
            try
            {
                MemoryStream ms = new MemoryStream(ary);
                //进行反序列化
            
                BinaryFormatter serializer = new BinaryFormatter();
                o = serializer.Deserialize(ms);
                ms.Close();
            }
            catch (Exception e)
            {
                Loger.Error(e);
            }
            return o;
        }
    }
    #endregion

    #region XML序列化

    internal static class XMLSerializer
    {
        /// <summary>
        /// 将对象压缩加密到字节数据
        /// </summary>
        /// <param name="obj">要压缩加密的对象</param>
        /// <returns>处理后生成的字节数组</returns>
        public static byte[] CompressEncryptToBytes(object obj)
        {
            if (obj == null) return new byte[0];
            // 建立对称密码信息
            byte[] array = null;
            MemoryStream ms = new MemoryStream();
            try
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(ms, obj);
                array = ms.ToArray();
            }
            catch (Exception ex)
            {
                Loger.Error(ex);
            }
            finally
            {
                ms.Close();
            }
            return array.HasElement() ? array : new byte[0];
        }

        /// <summary>
        /// 将字节数组进行解密解压还原成对象
        /// </summary>
        /// <param name="ary">要处理的字节数组</param>
        /// <returns>被还原的对象</returns>
        [System.Xml.Serialization.XmlInclude(typeof(DBNull))]
        public static object DecompressDecryptToObject(byte[] ary, Type type)
        {
            if (!ary.HasElement()) return null;
            object o = null;
            //解压字节流
            MemoryStream ms = new MemoryStream(ary);
            //进行反序列化
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                o = serializer.Deserialize(ms);
                ms.Close();
            }
            catch (Exception e)
            {
                Loger.Error(e);
            }
            return o;
        }
    }
    #endregion
}
