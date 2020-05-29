using System;

namespace TackleBox.Extensions.Localization.Json.JsonLoader
{
    /// <inheritdoc />
    public class UnableToLoadResourceFileException: Exception
    {
        /// <summary>
        /// </summary>
        public readonly string ResourceFileName;
        /// <summary>
        /// </summary>
        public readonly string CultureName;

        /// <summary>
        /// </summary>
        public UnableToLoadResourceFileException(string resourceFileName, string cultureName, Exception innerException): base($"Unable to load {resourceFileName} resource file for {cultureName} culture", innerException)
        {
            ResourceFileName = resourceFileName;
            CultureName = cultureName;
        }
    }
}
