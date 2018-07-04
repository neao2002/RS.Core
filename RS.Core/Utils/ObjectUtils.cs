using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using RS.Core.Common;

namespace RS.Core
{
    /// <summary>
    /// 对象方法集
    /// </summary>
    public static class ObjectUtils
    {
        #region 值对象转换相关函数
        /// <summary>
        /// 转成int类型,如为空值，则转为0;
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this object value)
        {
            if (value.IsNullOrDBNull()) return 0;
            string v = value.ToString();

            int index=v.IndexOf(".");
            if (index > 0) //则可能是个小数
            {
                v = v.Substring(0, index);
            }
            else if (index == 0)
                v = "";

            
            if (value is bool) return (bool)value ? 1 : 0;

            Int32.TryParse(v, out int iz);
            if (value.ToString() != "0" && iz == 0 && value.GetType().GetTypeInfo().IsEnum)
            {
                try
                {
                    iz = (int)value;
                }
                catch
                {
                    iz = 0;
                }
            }
            return iz;
        }
        /// <summary>
        /// 转成double,如为空值，则转为0;
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double ToDouble(this object z)
        {
            string vstr = z.ToStringValue();
            if (vstr.IsWhiteSpace()) return 0;
            double.TryParse(vstr, out double fz);
            return fz;
        }
        /// <summary>
        /// 转成double,如为空值，则转为0;
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public static long ToLong(this object z)
        {
            string vstr = z.ToStringValue();

            int index = vstr.IndexOf(".");
            if (index > 0) //则可能是个小数
            {
                vstr = vstr.Substring(0, index);
            }
            else if (index == 0)
                vstr = "";


            if (vstr.IsWhiteSpace()) return 0;

            long.TryParse(vstr, out long fz);
            return fz;
        }

        /// <summary>
        /// 将指定值转为字符，如果值为空，则自动转为空字符串
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public static string ToStringValue(this object z)
        {
            if (z.IsNotNull())
                return z.ToString();
            else
                return string.Empty;
        }
        /// <summary>
        /// 是否为Null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNull(this object value)
        {
            return value == null;
        }
        /// <summary>
        /// 不为null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotNull(this object value)
        {
            return value != null;
        }
        /// <summary>
        /// 转为GUID
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Guid ToGuid(this object z)
        {
            if (z.IsNullOrDBNull()) return Guid.Empty;
            if (z.ToString() == "") return Guid.Empty;
            Guid.TryParse(z.ToString(), out Guid id);
            return id;
        }
        /// <summary>
        /// 转为decimal
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object z)
        {
            string vstr = z.ToStringValue();
            if (vstr.IsWhiteSpace()) return 0;
            Decimal.TryParse(vstr, out decimal dz);
            return dz;
        }
        /// <summary>
        /// 转为float
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public static float ToFloat(this object z)
        {
            string vstr = z.ToStringValue();
            if (vstr.IsWhiteSpace()) return 0;
            Single.TryParse(vstr.ToString(), out float fz);
            return fz;
        }
        /// <summary>
        /// 转为DateTime
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object z)
        {
            if (z.IsNullOrDBNull()) return DateTime.MinValue;
            DateTime.TryParse(z.ToString(), out DateTime dz);
            return dz;
        }
        /// <summary>
        /// 转为bool
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public static bool ToBoolean(this object z)
        {
            if (z.IsNullOrDBNull()) return false;
            bool b = false;
            if (z.Equals(1))
                return true;
            else if (z.Equals(true))
                return true;
            else if (z.ToString().ToLower().Equals("true"))
                return true;
            else if (z.Equals('1'))
                return true;
            else if (z.Equals("1"))
                return true;
            else if (z.Equals(0))
                return false;
            else if (z.Equals(false))
                return false;
            else if (z.ToString().ToLower().Equals("false"))
                return false;
            else if (z.Equals('0'))
                return false;
            else if (z.Equals("0"))
                return false;
            else if (z.ToString().Equals("是") || z.ToString().Equals("真"))
                return true;
            else if (z.ToString().Equals("否") || z.ToString().Equals("假"))
                return false;
            else
                Boolean.TryParse(z.ToString(), out b);
            return b;
        }
        #endregion

        #region JSON对象序列化
        public static JsonUtil JSON { get; } = new JsonUtil();
        #endregion


        /// <summary>
        /// 将object对象转为json对象定义字符串(只对公共属性、字段有效)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj, Dictionary<string, string> FormatOption = null)
        {
            if (obj == null) return "null";
            return ToJson(obj, 0);
        }
        private static string ToJson(object obj, int level)
        {
            if (level > 2) return "null";
            StringBuilder sb = new StringBuilder();
            if (obj != null)
            {
                if (obj is Delegate)
                    return "null";
                else if (obj is Type)
                    return "null";
                else if (obj is IEnumerable)
                {
                    List<string> plist = new List<string>();
                    foreach (object o in (IEnumerable)obj)
                    {
                        plist.Add(ToJson(o, level));
                    }
                    sb.AppendFormat("[{0}]", string.Join(",", plist));
                }
                else
                {
                    Type type = obj.GetType();
                    if (type == typeof(string))
                        return string.Format("\"{0}\"", obj.ToStringValue().Replace("\"", "\\\"").Replace("</", "<\\/"));
                    else if (type.GetTypeInfo().IsEnum)
                        return ((int)obj).ToStringValue();
                    else if (type.GetTypeInfo().IsValueType)
                        return obj.ToStringValue();
                    else
                    { 
                        PropertyInfo[] ps = type.GetTypeInfo().GetProperties();
                        List<string> plist = new List<string>();
                        foreach (PropertyInfo p in ps)
                        {
                            if (p.CanRead)
                            {
                                if (p.PropertyType == typeof(string))
                                    plist.Add(string.Format("{0}:\"{1}\"", p.Name, p.GetValue(obj, null).ToStringValue().Replace("\"", "\\\"")));
                                else if (p.PropertyType.GetTypeInfo().IsEnum)
                                    plist.Add(string.Format("{0}:\"{1}\"", p.Name, (p.GetValue(obj, null).ToInt()).ToStringValue()));
                                else if (p.PropertyType.GetTypeInfo().IsValueType)
                                    plist.Add(string.Format("{0}:\"{1}\"", p.Name, p.GetValue(obj, null).ToStringValue()));
                                else if (p.PropertyType.GetTypeInfo().IsClass)
                                {

                                    object pv = null;
                                    try
                                    {
                                        pv = p.GetValue(obj, null);
                                    }
                                    finally
                                    {
                                        plist.Add(string.Format("{0}:{1}", p.Name, ToJson(pv, level + 1)));
                                    }
                                }
                            }
                        }
                        sb.AppendFormat("{{{0}}}", string.Join(",", plist));
                    }
                }
            }
            else
                sb.Append("null");

            return sb.ToString();
        }

        public static string SqlEncode(this object str)
        {
            if (str == null || string.IsNullOrEmpty(str.ToString())) return string.Empty;
            return str.ToString().Replace("\'", "\'\'");
        }


        public static bool IsSafety(this string s)
        {
            string str = s.Replace("%20", " ");
            str = Regex.Replace(str, @"\s", " ");
            string pattern = @"waitfor delay|insert |delete from |drop table|update |truncate |xp_cmdshell|exec master|net localgroup administrators|net user ";
            return !Regex.IsMatch(str, pattern, RegexOptions.IgnoreCase);
        }

        public static bool IsNullOrDBNull(this object o)
        {
            return o.IsNull() || o==DBNull.Value;
        }

        /// <summary>
        /// 返回具有指定T类型而且其值等效于指定对象的System.Object
        /// </summary>
        /// <param name="RecordFieldValue"></param>
        /// <param name="Variable"></param>
        public static T Evaluate<T>(this object RecordFieldValue, T Variable)
        {
            if (!IsNullOrDBNull(RecordFieldValue))
            {
                if (RecordFieldValue is T)
                    return (T)RecordFieldValue;
                else
                {
                    T v = Variable;
                    Type type = typeof(T);
                    if (RecordFieldValue is IConvertible)
                    {
                        try
                        {
                            v = (T)Convert.ChangeType(RecordFieldValue, type);
                        }
                        catch
                        {
                            v = (T)GetValue<T>(RecordFieldValue);
                        }
                    }
                    else
                    {
                        v = (T)GetValue<T>(RecordFieldValue);
                    }
                    return v;
                }
            }
            else
                return Variable;
        }
        /// <summary>
        /// 将指定值转换为指定类型的值，相当于Evaluate方法
        /// </summary>
        /// <param name="v">object值</param>
        /// <param name="t">要转换的类型</param>
        /// <returns>返回转换后的值，转换失败时，如是常规值类型，则返回该类型默认值，否则，则返回null</returns>
        public static object GetObjectValue(this object v, Type t)
        {
            object rtn = null;

            if (!v.IsNullOrDBNull())
            {
                try
                {
                    if (v != null && v.GetType() == t)
                    {
                        rtn = v;
                    }
                    else if (t == typeof(bool))
                        rtn = (bool)v.ToBoolean();
                    else if (t == typeof(float))
                        rtn = v.ToFloat();
                    else if (t == typeof(Int16) || t == typeof(Int32))
                        rtn = v.ToInt();
                    else if (t == typeof(Int64))
                        rtn = v.ToLong();
                    else if (t == typeof(UInt16) || t == typeof(UInt32))
                        rtn = v.ToInt();
                    else if (t == typeof(UInt64))
                        rtn = v.ToLong();
                    else if (t == typeof(decimal))
                        rtn = v.ToDecimal();
                    else if (t == typeof(double))
                        rtn = v.ToDouble();
                    else if (t == typeof(int))
                        rtn = v.ToInt();
                    else if (t == typeof(DateTime))
                        rtn = v.ToDateTime();
                    else if (t == typeof(Guid))
                        rtn = v.ToGuid();
                    else if (t.GetTypeInfo().IsEnum)
                    {
                        int z = v.ToInt();
                        if (Enum.IsDefined(t, z))
                            rtn = Enum.Parse(t, z.ToString());
                        else
                        {
                            rtn = Enum.Parse(t, Enum.GetNames(t)[0]);
                        }
                    }
                    else
                        rtn = Convert.ChangeType(v, t);
                }
                catch
                {
                    if (t == typeof(double))
                        rtn = default(double);
                    else if ( t == typeof(float))
                        rtn=default(float);
                    else if (t == typeof(Int16))
                        rtn = default(Int16);
                    else if (t == typeof(Int32))
                        rtn = default(Int32);
                    else if (t == typeof(Int64))
                        rtn = default(Int64);
                    else if (t == typeof(decimal))
                        rtn = default(decimal);
                    else if (t == typeof(UInt16))
                        rtn = default(UInt16);
                    else if (t == typeof(UInt32))
                        rtn = default(UInt32);
                    else if (t == typeof(UInt64))
                        rtn = default(UInt64);
                    else if (t == typeof(byte))
                        rtn = default(byte);
                    else if (t == typeof(DateTime))
                        rtn = DateTime.MinValue;
                    else if (t == typeof(bool))
                        rtn = false;
                    else if (t == typeof(Guid))
                        rtn = Guid.Empty;
                    else if (t == typeof(byte[]))
                        rtn = new byte[0];
                    else if (t == typeof(byte))
                        rtn = 0;
                    else if (t == typeof(char))
                        rtn = new char();
                    else if (t == typeof(string))
                        rtn = string.Empty;
                }
            }
            else
            {
                if (t == typeof(double))
                    rtn = default(double);
                else if (t == typeof(float))
                    rtn = default(float);
                else if (t == typeof(Int16))
                    rtn = default(Int16);
                else if (t == typeof(Int32))
                    rtn = default(Int32);
                else if (t == typeof(Int64))
                    rtn = default(Int64);
                else if (t == typeof(decimal))
                    rtn = default(decimal);
                else if (t == typeof(UInt16))
                    rtn = default(UInt16);
                else if (t == typeof(UInt32))
                    rtn = default(UInt32);
                else if (t == typeof(UInt64))
                    rtn = default(UInt64);
                else if (t == typeof(byte))
                    rtn = default(byte);
                else if (t == typeof(DateTime))
                    rtn = DateTime.MinValue;
                else if (t == typeof(bool))
                    rtn = false;
                else if (t == typeof(Guid))
                    rtn = Guid.Empty;
                else if (t == typeof(byte[]))
                    rtn = new byte[0];
                else if (t == typeof(byte))
                    rtn = 0;
                else if (t == typeof(char))
                    rtn = new char();
                else if (t == typeof(string))
                    rtn = string.Empty;
            }
            return rtn;
        }
        public static T GetValue<T>(this object v)
        {
            object rtn = null;
            Type t = typeof(T);
            if (!v.IsNullOrDBNull())
            {
                try
                {
                    if (v != null && v is T)
                    {
                        rtn = v;
                    }
                    else if (t == typeof(bool))
                        rtn = (bool)v.ToBoolean();
                    else if (t == typeof(float))
                        rtn = v.ToFloat();
                    else if (t == typeof(decimal))
                        rtn = v.ToDecimal();
                    else if (t == typeof(double))
                        rtn = v.ToDouble();
                    else if (t == typeof(int))
                        rtn = v.ToInt();
                    else if (t == typeof(long))
                        rtn = v.ToLong();
                    else if (t == typeof(DateTime))
                        rtn = v.ToDateTime();
                    else if (t == typeof(Guid))
                        rtn = v.ToGuid();
                    else if (t == typeof(string))
                        rtn = v.ToStringValue();
                    else if (t.GetTypeInfo().IsEnum)
                    {
                        int z = v.ToInt();
                        if (Enum.IsDefined(t, z))
                            rtn = Enum.Parse(t, z.ToString());
                        else
                        {
                            rtn = Enum.Parse(t, Enum.GetNames(t)[0]);
                        }
                    }
                    else
                        rtn = Convert.ChangeType(v, t);
                }
                catch
                {
                    rtn = default(T);
                }
            }
            else
                rtn = default(T);

            return (T)rtn;
        }
        /// <summary>
        /// 返回具有指定T类型而且其值等效于指定对象的System.Object
        /// </summary>
        /// <param name="RecordFieldValue"></param>
        /// <param name="Variable"></param>
        public static T Evaluate<T>(this object RecordFieldValue)
        {
            if (!IsNullOrDBNull(RecordFieldValue))
            {
                if (RecordFieldValue is T)
                    return (T)RecordFieldValue;
                else if (RecordFieldValue is JToken)
                {
                    T v;
                    JToken jo = (JToken)RecordFieldValue;
                    try
                    {
                        v = (T)jo.ToObject<T>();
                    }
                    catch
                    {
                        v = (T)GetValue<T>(jo.ToObject(typeof(object)));
                    }
                    return v;
                }
                else {
                    T v;
                    try
                    {
                        v = (T)Convert.ChangeType(RecordFieldValue, typeof(T));
                    }
                    catch
                    {
                        v = (T)GetValue<T>(RecordFieldValue);
                    }
                    return v;
                }
            }
            else if (typeof(T) == typeof(string))
                return (T)(object)string.Empty;
            else
                return default(T);
        }
        #region 特定属性相关扩展方法

        /// <summary>
        /// 获取指定值的四舍五入，如该值本身为整数，则不显示小数位
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static double FormatRound(this double v, int length)
        {
            double d = Math.Round(v, length);
            double fd = Math.Floor(d);
            if (fd == d)
                return fd;
            else
                return d;
        }
        public static decimal FormatRound(this decimal v, int length)
        {
            decimal d = Math.Round(v, length);
            decimal fd = Math.Floor(d);
            if (fd == d)
                return fd;
            else
                return d;
        }

        #endregion

        #region 值类型的判断
        /// <summary>
        /// 是否为int值,有小数位的则为否
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool IsInt(this object v)
        {
            if (v.IsNullOrDBNull()) return false;
            string z = v.ToString();
            double d = v.ToDouble();
            return (d.ToString() == z && d >= int.MinValue && d <= (double)int.MaxValue);
        }
        public static bool IsLong(this object v)
        {
            if (v.IsNullOrDBNull()) return false;
            string z = v.ToString();
            double d = v.ToDouble();
            return (d.ToString() == z && d >= long.MinValue && d <= (double)long.MaxValue);
        }
        public static bool IsNumeric(this object v)
        {
            if (v.IsNullOrDBNull()) return false;
            string z = v.ToString();
            double d = v.ToDouble();
            return (d.ToString() == z);
        }
        /// <summary>
        /// 是否为全数字
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool IsNumber(this object v)
        {
            if (v.IsNullOrDBNull()) return false;
            string z = v.ToString();
            if (z.IsWhiteSpace()) return false;
            for (int i = 0; i < z.Length; i++)
            {
                if (!Char.IsNumber(z, i))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsIn<T>(this T v, params T[] vlist)
        {
            return vlist.Contains<T>(v);
        }
        #endregion
    }
}
