using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ArgHelper
{
    public class Arg
    {
        private static readonly string Prefix = "-";
        private IDictionary<string, string> dict;

        #region constructor
        /// <summary>
        /// Generate T object which inherits ArgAttribute.
        /// </summary>
        /// <typeparam name="T">class which inherits ArgAttribute</typeparam>
        /// <param name="args">arguments. e.g. -in aaa -out bbb</param>
        /// <returns>T object</returns>
        public static T Build<T>(string[] args) where T : new()
        {
            T obj = new T();
            IEnumerable<(PropertyInfo, ArgAttribute)> propertyInfos = GetPropertyInfos(obj.GetType());

            Arg arg = Arg.Build(args);
            foreach ((PropertyInfo, ArgAttribute) infos in propertyInfos)
            {
                PropertyInfo propertyInfo = infos.Item1;
                ArgAttribute argAttribute = infos.Item2;

                if (argAttribute.IsMandatory)
                    Log.Require(arg.Keys.Contains(argAttribute.Key), string.Format("key={0} is mandatory.", argAttribute.Key));

                if (arg.Keys.Contains(argAttribute.Key))
                    propertyInfo.SetValue(obj, StringParser.Parse(arg[argAttribute.Key], propertyInfo.PropertyType));
            }
            return obj;
        }
        /// <summary>
        /// Generate Arg class. 
        /// - Decompose args and contain as key and value.
        /// - Key is case sensitive.
        /// - If there is no corresponding value, the value in the dictionary is the key.
        /// </summary>
        /// <param name="args">arguments. e.g. -in aaa -out bbb -calc</param>
        /// <returns>Arg class object</returns>
        public static Arg Build(string[] args)
        {
            var ret = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];
                if (IsKey(arg, Prefix))
                {
                    string key = arg.Replace(Prefix, "");
                    string value = (i != args.Length - 1 && !IsKey(args[i + 1], Prefix))
                        ? args[i + 1]
                        : key;
                    ret.Add(key, value);
                }
            }
            return new Arg(ret);
        }
        private Arg(IDictionary<string, string> dict)
        {
            this.dict = dict;
        }
        #endregion

        /// <summary>
        /// Get value with the key.
        /// </summary>
        /// <param name="key">key, without prefix.</param>
        /// <returns>value. if there is no corresponding value,return the key.</returns>
        public string this[string key]
        {
            get
            {
                string ret;
                if (this.dict.TryGetValue(key, out ret))
                    return ret;
                else
                    throw new Exception(string.Format("{0} is not included. Available keys={{1}}", key, string.Join(",", this.dict.Keys)));
            }
        }
        /// <summary>
        /// Get keys.
        /// </summary>
        public IEnumerable<string> Keys { get { return dict.Keys; } }

        #region help
        /// <summary>
        /// Return true when args contain "-help" or "help" or "-h" or "h"
        /// </summary>
        /// <param name="args">args</param>
        /// <returns>bool</returns>
        public static bool IsHelp(string[] args)
        {
            var lowerArgs = args.Select(arg => arg.ToLower());
            return lowerArgs.Contains("-help")
                || lowerArgs.Contains("help")
                || lowerArgs.Contains("-h")
                || lowerArgs.Contains("h")
                || lowerArgs.Count() == 0;
        }
        /// <summary>
        /// Display help description of type which has ArgAttribute properties.
        /// </summary>
        /// <param name="type">type</param>
        public static void DisplayHelpDescription(Type type)
        {
            string description = GetHelpDescription(type);
            Console.WriteLine(description);
        }

        private static string GetHelpDescription(Type type)
        {
            IEnumerable<(PropertyInfo, ArgAttribute)> propertyInfos = GetPropertyInfos(type);
            var mandatory = new List<(string, Type, string)>();
            var optional = new List<(string, Type, string)>();
            foreach ((PropertyInfo, ArgAttribute) infos in propertyInfos)
            {
                PropertyInfo propertyInfo = infos.Item1;
                ArgAttribute argAttribute = infos.Item2;

                (string, Type, string) tuple = (argAttribute.Key, propertyInfo.PropertyType, argAttribute.Description);
                if (argAttribute.IsMandatory)
                    mandatory.Add(tuple);
                else
                    optional.Add(tuple);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("+++++ user guide start +++++");
            //arguments
            IEnumerable<string> mandatoryKeys = mandatory.Select(tuple => tuple.Item1);
            IEnumerable<string> optionalKeys = optional.Select(tuple => tuple.Item1);
            sb.AppendLine(string.Format("  args = {0}",
                string.Join(" ", GenerateArgSamples(mandatoryKeys, isMandatory: true)),
                string.Join(" ", GenerateArgSamples(optionalKeys, isMandatory: false))));
            //description
            int maxLength = mandatoryKeys.Concat(optionalKeys).Max(key => key.Length);
            sb.AppendLine("  - mandatory");
            foreach (string description in GenerateArgDescription(mandatory, maxLength))
                sb.AppendLine(description);
            sb.AppendLine("  - optional");
            foreach (string description in GenerateArgDescription(optional, maxLength))
                sb.AppendLine(description);

            sb.AppendLine("+++++ user guide end +++++++");
            return sb.ToString();
        }
        private static IEnumerable<string> GenerateArgSamples(IEnumerable<string> keys, bool isMandatory)
        {
            var ret = keys.Select(key => string.Format("-{0} {0}Value", key));
            return isMandatory
                ? ret
                : ret.Select(v => "[" + v + "]");
        }
        private static IEnumerable<string> GenerateArgDescription(IEnumerable<(string, Type, string)> tuples, int maxLength)
        {
            return tuples.Select(tuple =>
            {
                string key = tuple.Item1;
                Type type = tuple.Item2;
                string description = tuple.Item3;
                int nbSpace = maxLength - key.Length + 1;
                string spaces = string.Join("", Enumerable.Range(1, nbSpace).Select(i => " "));

                return string.Format("      -{0}{1}: {2}. {3}", key, spaces, type.ToString(), description);
            });
        }
        #endregion

        private static bool IsKey(string arg, string prefix)
        {
            return arg.StartsWith(prefix);
        }
        private static IEnumerable<(PropertyInfo, ArgAttribute)> GetPropertyInfos(Type type)
        {
            return type
                .GetProperties()
                .Select(propertyInfo =>
                {
                    ArgAttribute argAttribute = propertyInfo.GetCustomAttribute(typeof(ArgAttribute)) as ArgAttribute;
                    return (PropertyInfo: propertyInfo, Attribute: argAttribute);
                })
                .Where(info => info.Attribute != null);
        }
    }
}