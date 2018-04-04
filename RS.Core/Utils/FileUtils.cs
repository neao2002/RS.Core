using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RS.Core
{
    /// <summary>
    /// 文件操作助手类：主要用于取指定文件内容或将指定内容写入文件
    /// </summary>
    public static class FileUtils
    {
        #region 读取或写入文本文件
        /// <summary>
        /// 读取指定文件文本内容（默认编码格式),如文件不存在或文件已被占用，或没有权限，则会出现异常
        /// </summary>
        /// <param name="FilePath">要读取的文件路径</param>
        /// <returns></returns>
        public static string ReadTextFile(string FilePath, bool HasTry = false)
        {
            string rtn = string.Empty;
            try
            {
                rtn = File.ReadAllText(FilePath);
            }
            catch (Exception e)
            {
                if (!HasTry) throw e; //如果需要出现异常                    
            }
            return rtn;
        }

        public static string ReadTextFile(string FilePath, Encoding encoding, bool HasTry = false)
        {
            string rtn = string.Empty;
            try{
                rtn = File.ReadAllText(FilePath, encoding);
            }
            catch (Exception e)
            {
                if (!HasTry) throw e; //如果需要出现异常                    
            }
            return rtn;
        }

        /// <summary>
        /// 将指定字符以文本方式写入到文件中(覆盖写入)
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Value"></param>
        public static void WriteTextFile(string FilePath, string Value, bool HasTry = false)
        {
            try
            {
                File.WriteAllText(FilePath,Value==null?"": Value);
            }
            catch (Exception e)
            {
                if (!HasTry) throw e;
            }
        }

        public static void WriteTextFile(string FilePath, string Value, Encoding encoding, bool HasTry = false)
        {
            try
            {
                File.WriteAllText(FilePath, Value == null ? "" : Value,encoding);
            }
            catch (Exception e)
            {
                if (!HasTry) throw e;
            }
        }

        /// <summary>
        /// 向指定文件追加写入文本，如文件不存在，则新建文件
        /// </summary>
        /// <param name="FieldPath"></param>
        /// <param name="Value"></param>
        public static void AppendTextFile(string FilePath,string Value, bool HasTry = false)
        {
            try
            {
                File.AppendAllText(FilePath, Value);
            }
            catch (Exception e)
            {
                if (!HasTry) throw e;
            }
        }
        /// <summary>
        /// 向指定文件追加写入文本，如文件不存在，则新建文件
        /// </summary>
        /// <param name="FieldPath"></param>
        /// <param name="Value"></param>
        public static void AppendTextFileLine(string FilePath, string Value, bool HasTry = false)
        {
            try
            {
                File.AppendAllText(FilePath, System.Environment.NewLine);
                File.AppendAllText(FilePath, Value);
            }
            catch (Exception e)
            {
                if (!HasTry) throw e;
            }
        }
        /// <summary>
        /// 向指定文件追加一行文本，如文件不存在，则新建文件
        /// </summary>
        /// <param name="FilePath">文件名</param>
        /// <param name="Value">要写入的内容</param>
        /// <param name="encoding">编码方式</param>
        /// <param name="HasTry">是否自动带出异常(在有BUG时)</param>
        public static void AppendTextFile(string FilePath, string Value, Encoding encoding, bool HasTry = false)
        {
            try
            {
                File.AppendAllText(FilePath, Value, encoding);
            }
            catch (Exception e)
            {
                if (!HasTry) throw e;
            }
        }

        public static void AppendTextFileLine(string FilePath, string Value, Encoding encoding, bool HasTry = false)
        {
            try
            {
                File.AppendAllText(FilePath, System.Environment.NewLine, encoding);
                File.AppendAllText(FilePath, Value, encoding);
            }
            catch (Exception e)
            {
                if (!HasTry) throw e;
            }
        }
        #endregion

        #region 二进制文件读取及写入相关操作
        public static byte[] ReadBinaryFile(string FilePath,bool HasTry=false)
        {
            List<byte> bytes = new List<byte>();
            try
            {
                bytes.AddRange(File.ReadAllBytes(FilePath));
            }
            catch(Exception e)
            {
                if (!HasTry) throw e;
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// 将指定字节以二进制方式写入文件中，文件不存在，则新建
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="bytes"></param>
        public static void WriteBinaryFile(string FilePath, byte[] bytes, bool HasTry = false)
        {
            try
            {
                File.WriteAllBytes(FilePath, bytes);
            }
            catch (Exception e)
            {
                if (!HasTry) throw e;
            }
        }
        #endregion

        #region 以二进制方式保存文本内容
        /// <summary>
        /// 将指定字符以二进制方式写入文件中，默认保存为UTF8格式，文件不存在，则新建
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="bytes"></param>
        public static void WriteBinaryFile(string FilePath, string value, bool HasTry = false)
        {
            try
            {
                byte[] bytes= Encoding.UTF8.GetBytes(value);
                File.WriteAllBytes(FilePath, bytes);
            }
            catch (Exception e)
            {
                if (!HasTry) throw e;
            }
        }
        #endregion
    }
}
