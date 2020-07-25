using System;
using System.Text.RegularExpressions;

namespace ArgHelper
{
    public static class StringParser
    {
        private static Regex RgxYYYYMMDDWithSlash = new Regex(@"^\d{4}\/\d{2}\/\d{2}$");
        private static Regex RgxYYYYMMDDWithHyphen = new Regex(@"^\d{4}-\d{2}-\d{2}$");
        private static Regex RgxYYYYMMDD = new Regex(@"^\d{4}\d{2}\d{2}$");
        private static Regex RgxYYMMDD = new Regex(@"^\d{2}\d{2}\d{2}$");
        public static object Parse(string str, Type type)
        {
            if (typeof(string) == type)
                return (object)str;
            else if (typeof(bool) == type)
            {
                bool ret;
                if (bool.TryParse(str, out ret))
                    return (object)ret;
                else
                    throw new Exception(string.Format("{0} can't be parsed into bool.", str));
            }
            else if (typeof(double) == type)
            {
                double ret;
                if (double.TryParse(str, out ret))
                    return (object)ret;
                else
                    throw new Exception(string.Format("{0} can't be parsed into double.", str));
            }
            else if (typeof(float) == type)
            {
                float ret;
                if (float.TryParse(str, out ret))
                    return (object)ret;
                else
                    throw new Exception(string.Format("{0} can't be parsed into float.", str));
            }
            else if (typeof(int) == type)
            {
                int ret;
                if (int.TryParse(str, out ret))
                    return (object)ret;
                else
                    throw new Exception(string.Format("{0} can't be parsed into int.", str));
            }
            else if (typeof(DateTime) == type)
            {
                if (RgxYYYYMMDDWithSlash.IsMatch(str))
                {
                    string[] args = str.Split('/');
                    return (object)new DateTime(
                        int.Parse(args[0]),
                        int.Parse(args[1]),
                        int.Parse(args[2]));
                }
                if (RgxYYYYMMDDWithHyphen.IsMatch(str))
                {
                    string[] args = str.Split('-');
                    return (object)new DateTime(
                        int.Parse(args[0]),
                        int.Parse(args[1]),
                        int.Parse(args[2]));
                }
                else if (RgxYYYYMMDD.IsMatch(str))
                {
                    return (object)new DateTime(
                        int.Parse(str.Substring(0, 4)),
                        int.Parse(str.Substring(4, 2)),
                        int.Parse(str.Substring(6, 2)));
                }
                else if (RgxYYMMDD.IsMatch(str))
                {
                    return (object)new DateTime(
                        int.Parse("20" + str.Substring(0, 2)),
                        int.Parse(str.Substring(2, 2)),
                        int.Parse(str.Substring(4, 2)));
                }
                else
                    throw new Exception(string.Format("{0} is not supported DateTime format", str));
            }
            else if (type.IsEnum)
                return Enum.Parse(type, str, true);
            else
                throw new NotImplementedException(string.Format("not supported type."));
        }
        public static T Parse<T>(string str)
        {
            Type type = typeof(T);
            return (T)Parse(str, type);
        }
    }
}