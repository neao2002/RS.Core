using System;
using System.Text;
using System.Globalization;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace RS
{
    /// <summary>
    /// 字符串加解密类
    /// </summary>
    [Serializable]
    internal class StringEncrypt
    {
        private static string passKey="";
        internal static string PassKey
        {
            get {
                if (passKey.IsWhiteSpace())
                    return "RSSOFT";
                else
                    return passKey;
            }
            set { passKey = value; }
        }
        /// <summary> 
        /// 构造方法 
        /// </summary>
        public StringEncrypt()
        {
        }
        /// <summary> 
        /// 使用缺省密钥字符串加密 
        /// </summary> 
        /// <param name="original">明文</param> 
        /// <returns>密文</returns> 
        public static string Encrypt(string original)
        {
            return Encrypt(original,PassKey);
        }
        /// <summary> 
        /// 使用缺省密钥解密 
        /// </summary> 
        /// <param name="original">密文</param> 
        /// <returns>明文</returns> 
        public static string Decrypt(string original)
        {
            return Decrypt(original, PassKey, System.Text.Encoding.Default);
        }
        /// <summary> 
        /// 使用给定密钥解密 
        /// </summary> 
        /// <param name="original">密文</param> 
        /// <param name="key">密钥</param> 
        /// <returns>明文</returns> 
        public static string Decrypt(string original, string key)
        {
            return Decrypt(original, key, System.Text.Encoding.Default);
        }
        /// <summary> 
        /// 使用缺省密钥解密,返回指定编码方式明文 
        /// </summary> 
        /// <param name="original">密文</param> 
        /// <param name="encoding">编码方式</param> 
        /// <returns>明文</returns> 
        public static string Decrypt(string original, Encoding encoding)
        {
            return Decrypt(original, PassKey, encoding);
        }
        public static string Encrypt(string original, string key)
        {
            return Encrypt(original, key, Encoding.Default);
        }
        /// <summary> 
        /// 使用给定密钥加密 
        /// </summary> 
        /// <param name="original">原始文字</param> 
        /// <param name="key">密钥</param> 
        /// <param name="encoding">字符编码方案</param> 
        /// <returns>密文</returns> 
        public static string Encrypt(string original, string key,Encoding encoding)
        {
            byte[] buff = encoding.GetBytes(original);
            byte[] kb = encoding.GetBytes(key);
            //弃用ToBase64String;
            //return Convert.ToBase64String(Encrypt(buff, kb));
            return GetEncryptString(Encrypt(buff, kb));
        }
        /// <summary> 
        /// 使用给定密钥解密 
        /// </summary> 
        /// <param name="encrypted">密文</param> 
        /// <param name="key">密钥</param> 
        /// <param name="encoding">字符编码方案</param> 
        /// <returns>明文</returns> 
        public static string Decrypt(string encrypted, string key, Encoding encoding)
        {
            //弃用FromBase64String
           // byte[] buff = Convert.FromBase64String(encrypted);
            byte[] buff = GetDecryptData(encrypted);
            byte[] kb = encoding.GetBytes(key);
            return encoding.GetString(Decrypt(buff, kb));
        }
        /// <summary> 
        /// 生成MD5摘要 
        /// </summary> 
        /// <param name="original">数据源</param> 
        /// <returns>摘要</returns> 
        public static byte[] MakeMD5(byte[] original)
        {
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyhash = hashmd5.ComputeHash(original);
            hashmd5 = null;
            return keyhash;
        }
        /// <summary> 
        /// 使用给定密钥加密 
        /// </summary> 
        /// <param name="original">明文</param> 
        /// <param name="key">密钥</param> 
        /// <returns>密文</returns> 
        public static byte[] Encrypt(byte[] original, byte[] key)
        {
            if (!original.HasElement()) return new byte[0];

            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.Key = MakeMD5(key);
                des.Mode = CipherMode.ECB;

                return des.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
            }
            catch {
                return new byte[0];
            }
        }
        /// <summary> 
        /// 使用给定密钥解密数据 
        /// </summary> 
        /// <param name="encrypted">密文</param> 
        /// <param name="key">密钥</param> 
        /// <returns>明文</returns> 
        public static byte[] Decrypt(byte[] encrypted, byte[] key)
        {
            if (!encrypted.HasElement()) return new byte[0];
            try
            {
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                des.Key = MakeMD5(key);
                des.Mode = CipherMode.ECB;
                return des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
            }
            catch
            {
                return new byte[0];
            }
        }

        /// <summary> 
        /// 使用给定密钥加密 
        /// </summary> 
        /// <param name="original">原始数据</param> 
        /// <param name="key">密钥</param> 
        /// <returns>密文</returns> 
        public static byte[] Encrypt(byte[] original, Encoding encoding)
        {
            byte[] key = encoding.GetBytes(PassKey);
            return Encrypt(original, key);
        }
        /// <summary> 
        /// 使用缺省密钥解密数据 
        /// </summary> 
        /// <param name="encrypted">密文</param> 
        /// <param name="key">密钥</param> 
        /// <returns>明文</returns> 
        public static byte[] Decrypt(byte[] encrypted, Encoding encoding)
        {
            byte[] key = encoding.GetBytes(PassKey);
            return Decrypt(encrypted, key);
        }

        /// <summary> 
        /// 使用给定密钥加密 
        /// </summary> 
        /// <param name="original">原始数据</param> 
        /// <param name="key">密钥</param> 
        /// <returns>密文</returns> 
        public static byte[] Encrypt(byte[] original)
        {
            byte[] key = System.Text.Encoding.Default.GetBytes(PassKey);
            return Encrypt(original, key);
        }
        /// <summary> 
        /// 使用缺省密钥解密数据 
        /// </summary> 
        /// <param name="encrypted">密文</param> 
        /// <param name="key">密钥</param> 
        /// <returns>明文</returns> 
        public static byte[] Decrypt(byte[] encrypted)
        {
            byte[] key = System.Text.Encoding.Default.GetBytes(PassKey);
            return Decrypt(encrypted, key);
        }



        /// <summary>
        /// 将指定字节数组以字符方式呈现,为迷惑用户，这里对字符进行位移，最多可位移16，如F，位移后，就变成
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static string GetEncryptString(byte[] bytes)
        {
            try
            {
                string zf = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                //采用动态方式，即同一个明文，加密后呈现的密文可能不同
                Random rd = new Random(DateTime.Now.Millisecond);
                int random=rd.Next(1, 20);

                StringBuilder Hash = new StringBuilder();
                string sz = "";
                foreach (byte b in bytes)
                {
                    sz = string.Format("{0:X2}", b);
                    for (int i = 0; i < sz.Length; i++)
                    {
                        int v = getV(sz.Substring(i, 1));                        
                        Hash.Append(zf.Substring(v + random , 1));
                    }
                }

                //对随机种子进行加密处理
                sz = string.Format("{0:X2}", (byte)random);
                for (int i = 0; i < sz.Length; i++)
                {
                    int v = getV(sz.Substring(i, 1));

                    if (Math.IEEERemainder(v, 2) == 0)
                        Hash.Append(zf.Substring(2*v + 1, 1));
                    else
                        Hash.Append(zf.Substring(2*v + 2, 1));
                }

                return Hash.ToString();
            }
            catch {
                return "";
            }
        }
        internal static byte[] GetDecryptData(string estring)
        {
            List<byte> arr = new List<byte>();
            try
            {
                string zf = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

                //第一步，先解密随机种子，后面两个字符
                if (estring.Length <= 2) return arr.ToArray();
                string randomStr = "";
                for (int i = estring.Length-2; i < estring.Length; i++)
                {
                    int z = zf.IndexOf(estring.Substring(i, 1));

                    if (Math.IEEERemainder(z - 1, 2) == 0)
                        randomStr+=zf.Substring((z - 1) / 2, 1);
                    else
                        randomStr+=zf.Substring((z - 2) / 2, 1);
                }

                //取到加密种子
                int random =Convert.ToInt32(randomStr, 16);

                //if (Math.IEEERemainder(v, 2) == 0)
                //    Hash.Append(zf.Substring(v + random - 1, 1));
                //else
                //    Hash.Append(zf.Substring(v + random, 1));

                //v+random-1=z;v=z-random+1; //v为偶数
                //v+random=z;v=z-random  //v为奇数



                StringBuilder Hash = new StringBuilder();
                for (int i = 0; i < estring.Length-2; i++)
                {
                    int z = zf.IndexOf(estring.Substring(i, 1));

                    Hash.Append(zf.Substring(z-random, 1));
                }

                string Token = Hash.ToString();
                List<byte> bytes = new List<byte>();

                for (int i = 0; i < Token.Length; i += 2)
                {
                    bytes.Add(Convert.ToByte(Convert.ToInt32(Token.Substring(i, 2), 16)));
                }
                arr.AddRange(bytes);
            }
            catch { } //由于有可能是非常数据，
            return arr.ToArray();
        }
        /// <summary>
        /// 获取位移数
        /// </summary>
        /// <param name="zf"></param>
        /// <returns></returns>
        private static int getV(string z)
        {
            if (z.IsIn("0", "1", "2", "3", "4", "5", "6", "7", "8", "9"))
                return z.ToInt();
            else if (z == "A")
                return 10;
            else if (z == "B")
                return 11;
            else if (z == "C")
                return 12;
            else if (z == "D")
                return 13;
            else if (z == "E")
                return 14;
            else if (z == "F")
                return 15;
            else
                return 0;
        }
    }
}