using System;

namespace TackleBox.Extensions.Localization.Json
{
    /// <summary>
    /// Specifies translation resource file name for the class or entire assembly
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class)]
    public class LocalizedFromAttribute : Attribute
    {
        /// <inheritdoc />
        public LocalizedFromAttribute(string resourceFileName)
        {
            if (string.IsNullOrEmpty(resourceFileName))
            {
                throw new ArgumentNullException(nameof(resourceFileName));
            }

            ResourceFileName = resourceFileName;
        }

        /// <summary>
        /// </summary>
        public string ResourceFileName { get; }
    }
}
