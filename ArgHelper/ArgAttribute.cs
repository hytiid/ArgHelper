using System;

namespace ArgHelper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ArgAttribute : Attribute
    {
        /// <summary>
        /// Key of the argument
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// true = is mandatory, false = optional
        /// </summary>
        public bool IsMandatory { get; private set; }
        /// <summary>
        /// Description of the argument
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="key">Key of the argument</param>
        /// <param name="isMandatory">true = is mandatory, false = optional</param>
        /// <param name="description">Description of the argument</param>
        public ArgAttribute(string key, bool isMandatory, string description)
        {
            this.Key = key;
            this.IsMandatory = isMandatory;
            this.Description = description;
        }
    }
}
