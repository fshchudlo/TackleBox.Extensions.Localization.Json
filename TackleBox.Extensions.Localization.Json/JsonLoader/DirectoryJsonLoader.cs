using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace TackleBox.Extensions.Localization.Json.JsonLoader
{
    /// <inheritdoc />
    internal class DirectoryJsonLoader : JsonLoader
    {
        private readonly JsonLocalizationOptions _options;
        private readonly ILogger _logger;

        /// <summary>
        /// </summary>
        public DirectoryJsonLoader(JsonLocalizationOptions options, ILogger logger)
        {
            _options = options;
            _logger = logger;
        }

        /// <inheritdoc />
        /// <exception cref="UnableToLoadResourceFileException"></exception>
        internal override JsonDocument LoadResource(string resourceFileName, CultureInfo? culture)
        {
            var cultureToUse = string.IsNullOrEmpty(culture?.Name) ? _options.DefaultCulture : culture;
            if (cultureToUse == null)
            {
                cultureToUse = new CultureInfo("");
                _logger.LogEmptyCultureWarning();
            }
            var filePath = _options.ResourcePathBuilder(_options, resourceFileName, cultureToUse);
            try
            {
                _logger.LogLoading(resourceFileName, filePath, cultureToUse);
                using FileStream fs = File.OpenRead(filePath);
                return JsonDocument.Parse(fs);
            }
            catch (Exception e)
            {
                _logger.LogLoadingError(resourceFileName, filePath, cultureToUse, e);
                throw new UnableToLoadResourceFileException(resourceFileName, cultureToUse.Name, e);
            }
        }
    }
}