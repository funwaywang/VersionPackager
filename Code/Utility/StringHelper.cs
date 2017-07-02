using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VPackager
{
    public static class StringHelper
    {
        public static bool TryGetInt(object value, out int intValue)
        {
            intValue = 0;
            if (value == null || value == DBNull.Value)
                return false;

            if (value is int)
                intValue = (int)value;
            else if (value is string)
                return int.TryParse((string)value, out intValue);
            else if (value is decimal)
                intValue = (int)(decimal)value;
            else if (value is double)
                intValue = (int)(double)value;
            else
                return false;

            return true;
        }

        public static bool IsAlphabet(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        public static string AddAccelerator(string str, char accelerator)
        {
            if (accelerator > 0)
            {
                int index = str.IndexOf(new string(accelerator, 1), StringComparison.OrdinalIgnoreCase);
                if (index > -1)
                    str.Insert(index, "&");
                else
                    str += string.Format("(&{0})", accelerator);
            }

            return str;
        }

        public static int? GetInt(object obj)
        {
            if (obj is int)
                return (int)obj;
            else if (obj is decimal)
                return (int)(decimal)obj;
            else if (obj is float)
                return (int)(float)obj;
            else if (obj is double)
                return (int)(double)obj;
            else if (obj is byte)
                return (int)(byte)obj;
            else if (obj is Single)
                return (int)(Single)obj;
            else if (obj is short)
                return (int)(short)obj;
            else if (obj is long)
                return (int)(long)obj;
            else if (obj is string)
            {
                int ri;
                if (int.TryParse((string)obj, out ri))
                {
                    return ri;
                }
            }

            return null;
        }

        public static int GetInt(object obj, int defaultValue)
        {
            var v = GetInt(obj);
            if (v.HasValue)
                return v.Value;
            else
                return defaultValue;
        }

        public static int GetIntDefault(object obj)
        {
            return GetInt(obj, 0);
        }

        public static bool GetBoolDefault(object value)
        {
            return GetBool(value, false);
        }

        public static bool? GetBool(object value)
        {
            if (value == null || value is DBNull)
                return null;
            else if (value is bool)
                return (bool)value;
            else if (value is int)
                return (int)value > 0;
            else if (value is decimal)
                return (decimal)value > 0;
            else if (value is long)
                return (long)value > 0;
            else if (value is short)
                return (short)value > 0;
            else if (value is byte)
                return (byte)value > 0;
            else
            {
                string v = value.ToString().ToLower();
                switch (v)
                {
                    case "1":
                    case "y":
                    case "yes":
                    case "true":
                        return true;
                    case "0":
                    case "n":
                    case "no":
                    case "false":
                        return false;
                    default:
                        return null;
                }
            }
        }

        public static bool GetBool(object value, bool defaultValue)
        {
            bool? b = GetBool(value);
            if (b.HasValue)
                return b.Value;
            else
                return defaultValue;
        }

        public static string JoinArray(IEnumerable array)
        {
            return JoinArray(array, ",");
        }

        public static string JoinArray(IEnumerable array, string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object item in array)
            {
                if (item == null)
                    continue;

                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }

                sb.Append(item.ToString());
            }

            return sb.ToString();
        }

        public static bool IsRightFileName(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return false;

            var illegal = Path.GetInvalidFileNameChars();
            return filename.All(c => !illegal.Contains(c));
        }
    }
}
