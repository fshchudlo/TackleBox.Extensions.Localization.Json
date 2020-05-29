using System;

namespace TackleBox.Extensions.Localization.Json
{
    /// <summary>
    /// </summary>
    public class ResourceIntegrityException : Exception
    {
        /// <summary>
        /// </summary>
        public Type CheckedType { get; }
        
        /// <summary>
        /// </summary>
        public string[] MissedStrings { get; }

        /// <summary>
        /// </summary>
        public ResourceIntegrityException(Type checkedType, string[] missedStrings):base($"Translations for some properties of {checkedType.Name} are missed.")
        {
            CheckedType = checkedType;
            MissedStrings = missedStrings;
        }
    }
}